// This file is part of the Jung FastFileAccess project.
// The project is licensed under the MIT license.

using Jung.FastFileAccess.Models.Enums;
using System.Net;

namespace Jung.FastFileAccess.Models.Base
{
    /// <summary>
    /// Represents an abstract base class for access requests, providing properties and methods to evaluate and retrieve
    /// information about a resource's accessibility.
    /// </summary>
    /// <remarks>This class is designed to encapsulate the details of an access request for a specific
    /// resource, identified by its path. It provides properties to determine the type of path (e.g., UNC or local),
    /// retrieve relevant path components (e.g., host name or drive letter), and store the access status and associated
    /// metadata such as host IP addresses.</remarks>
    public abstract class AccessRequestItem
    {
        /// <summary>
        /// The path of the resource to check access for.
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// The reason why the resource is not accessible.
        /// </summary>
        public AccessStatus Status { get; private set; } = AccessStatus.Pending;

        internal bool StatusIsSet => Status != AccessStatus.Pending;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessRequestItem"/> class with the specified path.
        /// </summary>
        /// <param name="path"></param>
        public AccessRequestItem(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Gets a value indicating whether the path is a Universal Naming Convention (UNC) path.
        /// </summary>
        /// <remarks>A UNC path typically refers to a network resource, such as \\server\share. This
        /// property checks if the path conforms to that format.</remarks>
        public bool IsUncPath => Path.StartsWith("\\\\");

        /// <summary>
        /// Gets the host name from the UNC path if the path is a valid UNC path; otherwise, returns <see
        /// langword="null"/>.
        /// </summary>
        /// <remarks>A UNC path typically follows the format "\\server\share". The host name corresponds
        /// to the "server" part of the path.</remarks>
        public string? HostName
        {
            get
            {
                if (!IsUncPath)
                    return null;
                var parts = Path.Split(new[] { '\\' }, 4);
                return parts.Length >= 3 ? parts[2] : null;
            }
        }

        /// <summary>
        /// Gets the drive letter of the path, if applicable; otherwise, returns <see langword="null"/>.
        /// </summary>
        public string? DriveLetter
        {
            get
            {
                if (IsUncPath)
                    return null;
                var parts = Path.Split(new[] { ':' }, 2);
                return parts.Length >= 1 ? parts[0] : null;
            }
        }

        /// <summary>
        /// Gets the collection of IP addresses associated with the host. This property is populated on successful access checks.
        /// </summary>
        public IPAddress[]? HostAddresses { get; internal set; }

        internal void SetStatus(AccessStatus status)
        {
            Status = status;
        }
    }
}