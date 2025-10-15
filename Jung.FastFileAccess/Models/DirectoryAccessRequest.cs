// This file is part of the Jung FastFileAccess project.
// The project is licensed under the MIT license.

using Jung.FastFileAccess.Models.Base;
using Jung.FastFileAccess.Models.Enums;

namespace Jung.FastFileAccess.Models
{
    /// <summary>
    /// Represents a request for access to a directory, including the type of access being requested.
    /// </summary>
    public class DirectoryAccessRequest : AccessRequestItem
    {
        /// <summary>
        /// Gets or sets the type of access allowed for the directory.
        /// </summary>
        public DirectoryAccessType AccessType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryAccessRequest"/> class with the specified path and access type.
        /// </summary>
        /// <param name="path">The path of the directory to check access for.</param>
        /// <param name="accessType">The type of access being requested for the directory.</param>
        public DirectoryAccessRequest(string path, DirectoryAccessType accessType) : base(path)
        {
            AccessType = accessType;
        }
    }
}
