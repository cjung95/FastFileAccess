// This file is part of the Jung FastFileAccess project.
// The project is licensed under the MIT license.

using System;

namespace Jung.FastFileAccess.Models.Enums
{
    /// <summary>
    /// Specifies the access rights that can be granted for file operations.
    /// </summary>
    /// <remarks>This enumeration defines a set of flags that represent various file access permissions.  It
    /// is marked with the <see cref="FlagsAttribute"/>, allowing a bitwise combination of its values  to specify
    /// multiple access rights simultaneously. Common combinations, such as <see cref="GenericRead"/>,  <see
    /// cref="GenericWrite"/>, and <see cref="GenericExecute"/>, are provided for convenience.</remarks>
    [Flags]
    public enum FileAccessType : uint
    {
        /// <summary>
        /// The right to read the corresponding file data.
        /// </summary>
        ReadData = 0x1,

        /// <summary>
        /// The right to write data to the file.
        /// </summary>
        WriteData = 0x2,

        /// <summary>
        /// The right to append data to the file. (For local files, write operations will not overwrite existing data if this flag is specified without WriteData.)
        /// </summary>
        AppendData = 0x4,

        /// <summary>
        /// The right to read extended file attributes.
        /// </summary>
        ReadEa = 0x8,

        /// <summary>
        /// The right to write extended file attributes.
        /// </summary>
        WriteEa = 0x10,

        /// <summary>
        /// For a native code file, the right to execute the file. This access right given to scripts may cause the script to be executable, depending on the script interpreter.
        /// </summary>
        Execute = 0x20,

        /// <summary>
        /// The right to read file attributes.
        /// </summary>
        ReadAttributes = 0x80,

        /// <summary>
        /// The right to write file attributes.
        /// </summary>
        WriteAttributes = 0x100,

        /// <summary>
        /// The Right to delete an object.
        /// </summary>
        Delete = 0x00010000,

        /// <summary>
        /// Right to wait on a handle.
        /// </summary>
        Synchronize = 0x00100000,

        /// <summary>
        /// A combination of Execute | ReadAttributes | STANDARD_RIGHTS_EXECUTE | Synchronize
        /// </summary>
        GenericExecute = 0x20000000,

        /// <summary>
        /// A combination of AppendData | WriteAttributes | WriteData | WriteEa | STANDARD_RIGHTS_WRITE | Synchronize
        /// </summary>
        GenericWrite = 0x40000000,

        /// <summary>
        /// A combination of ReadAttributes | ReadData | ReadEa | STANDARD_RIGHTS_READ | Synchronize
        /// </summary>
        GenericRead = 0x80000000,

        /// <summary>
        /// All possible access rights.
        /// </summary>
        GenericAll = 0x10000000,
    }
}
