// This file is part of the Jung FastFileAccess project.
// The project is licensed under the MIT license.

using System;

namespace Jung.FastFileAccess.Models.Enums
{
    /// <summary>
    /// Specifies the access rights that can be assigned to a directory.  This enumeration supports bitwise combination
    /// of its member values.
    /// </summary>
    /// <remarks>The <see cref="DirectoryAccessType"/> enumeration defines various access rights  that can be
    /// granted for directories, such as the ability to list contents, create files,  or modify attributes. It is marked
    /// with the <see cref="FlagsAttribute"/>, allowing  multiple values to be combined using a bitwise OR operation.</remarks>
    [Flags]
    public enum DirectoryAccessType : uint
    {
        /// <summary>
        /// The right to list the contents of the directory.
        /// </summary>
        ListDirectory = 0x1,

        /// <summary>
        /// The right to create a file in the directory.
        /// </summary>
        AddFile = 0x2,

        /// <summary>
        /// The right to create a subdirectory.
        /// </summary>
        AddSubdirectory = 0x4,

        /// <summary>
        /// The right to read extended file attributes.
        /// </summary>
        ReadEa = 0x8,

        /// <summary>
        /// The right to write extended file attributes.
        /// </summary>
        WriteEa = 0x10,

        /// <summary>
        /// The right to traverse the directory. By default, users are assigned the BYPASS_TRAVERSE_CHECKING <see href="https://learn.microsoft.com/en-us/windows/win32/secauthz/privileges">privilege</see>, which ignores the FILE_TRAVERSE <see href="https://learn.microsoft.com/en-us/windows/win32/secauthz/access-rights-and-access-masks">access right</see>. See the remarks in <see href="https://learn.microsoft.com/en-us/windows/win32/fileio/file-security-and-access-rights">File Security and Access Rights</see> for more information.
        /// </summary>
        Traverse = 0x20,

        /// <summary>
        /// The right to delete a directory and all the files it contains, including read-only files.
        /// </summary>
        DeleteChild = 0x40,

        /// <summary>
        /// The right to read file attributes.
        /// </summary>
        ReadAttributes = 0x80,

        /// <summary>
        /// The right to write file attributes.
        /// </summary>
        WriteAttributes = 0x100,

        /// <summary>
        /// Right to delete an object.
        /// </summary>
        Delete = 0x00010000,

        /// <summary>
        /// Right to wait on a handle.
        /// </summary>
        Synchronize = 0x00100000,

        /// <summary>
        /// ListDirectory | ReadEa | ReadAttributes | STANDARD_RIGHTS_READ | Synchronize
        /// </summary>
        GenericRead = 0x80000000,

        /// <summary>
        /// AddFile | AddSubdirectory | WriteEa | WriteAttributes | STANDARD_RIGHTS_WRITE | Synchronize
        /// </summary>
        GenericWrite = 0x40000000,

        /// <summary>
        /// Traverse | ReadAttributes | Synchronize
        /// </summary>
        GenericExecute = 0x20000000,

        /// <summary>
        /// All possible access rights.
        /// </summary>
        GenericAll = 0x10000000,
    }
}
