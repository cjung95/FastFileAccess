// This file is part of the Jung FastFileAccess project.
// The project is licensed under the MIT license.

using System;

namespace Jung.FastFileAccess.Models
{
    /// <summary>
    /// Represents the configuration settings for an access checker.
    /// </summary>
    public class AccessCheckerConfiguration
    {
        /// <summary>
        /// Gets or sets the timeout duration for server requests. The default value is 1 second.
        /// </summary>
        /// <remarks>Adjust this value based on the expected response time of the server. Setting a very
        /// low value may result in frequent timeouts, while a very high value may delay error handling in case of
        /// server issues.</remarks>
        public TimeSpan ServerRequestTimeout { get; set; } = TimeSpan.FromSeconds(1);
    }
}
