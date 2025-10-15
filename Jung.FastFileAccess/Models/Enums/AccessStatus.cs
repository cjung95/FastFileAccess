// This file is part of the Jung FastFileAccess project.
// The project is licensed under the MIT license.

namespace Jung.FastFileAccess.Models.Enums
{
    /// <summary>
    /// Represents the status of an access request, indicating the outcome or current state of the operation.
    /// </summary>
    public enum AccessStatus
    {
        /// <summary>
        /// Represents a state where the operation is pending and has not yet completed.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Indicates that access to the requested resource has been granted.
        /// </summary>
        AccessGranted = 1,

        /// <summary>
        /// Indicates that the specified path could not be found.
        /// </summary>
        PathNotFound = 2,

        /// <summary>
        /// Indicates that the network is unreachable.
        /// </summary>
        NetworkUnreachable = 3,

        /// <summary>
        /// Indicates that the specified drive could not be found.
        /// </summary>
        DriveNotFound = 4,

        /// <summary>
        /// Indicates that an unknown error has occurred during the access request.
        /// </summary>
        UnknownError = 5,

        /// <summary>
        /// Indicates that access to the requested resource was not granted.
        /// </summary>
        AccessNotGranted = 6,

        /// <summary>
        /// Indicates that DNS resolution for the requested resource has failed.
        /// </summary>
        DnsResolutionFailed = 7
    }
}
