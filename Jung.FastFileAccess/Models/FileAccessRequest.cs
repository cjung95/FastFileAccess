// This file is part of the Jung FastFileAccess project.
// The project is licensed under the MIT license.

using Jung.FastFileAccess.Models.Base;
using Jung.FastFileAccess.Models.Enums;

namespace Jung.FastFileAccess.Models
{
    /// <summary>
    /// Represents a request to access a file, including the file path and the type of access required.
    /// </summary>
    public class FileAccessRequest : AccessRequestItem
    {
        /// <summary>
        /// The type of access being requested for the file.
        /// </summary>
        public FileAccessType AccessType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAccessRequest"/> class with the specified path and access type.
        /// </summary>
        /// <param name="path">The path of the file to access.</param>
        /// <param name="accessType">The type of access being requested.</param>
        public FileAccessRequest(string path, FileAccessType accessType) : base(path)
        {
            AccessType = accessType;
        }
    }
}
