// This file is part of the Jung FastFileAccess project.
// The project is licensed under the MIT license.

using System;

namespace Jung.FastFileAccess.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs during file search operations.
    /// </summary>

    [Serializable]
    public class FileSearchException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSearchException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public FileSearchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}