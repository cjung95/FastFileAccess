// This file is part of the Jung FastFileAccess project.
// The project is licensed under the MIT license.

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Jung.FastFileAccess.Native
{
    /// <summary>
    /// Provides managed wrappers for selected Windows Kernel32 API functions related to file and handle operations.
    /// </summary>
    /// <remarks>The Kernel32 Interoperability class contains methods that allow managed code to interact with the unmanaged kernel32.dll.</remarks>
    public class Kernel32Interop
    {
        #region structures

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WIN32_FIND_DATA
        {
            public uint dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
            public uint dwFileType;
            public uint dwCreatorType;
            public uint wFinderFlags;
        }

        #endregion

        #region enums

        /// <summary>
        /// Specifies the access rights that can be requested or granted for file and directory objects.
        /// </summary>
        /// <remarks>This enumeration defines a set of flags that represent specific access rights for
        /// file and directory objects. These rights include permissions to read, write, execute, and manage files and
        /// directories, as well as generic rights that combine multiple specific rights. The enumeration is marked with
        /// the <see cref="FlagsAttribute"/>, allowing bitwise combinations of its member values to specify multiple
        /// access rights simultaneously. 
        /// <para> For more information about file and directory access rights, see <see
        /// href="https://learn.microsoft.com/en-us/windows/win32/fileio/file-access-rights-constants">File Access Rights Constants</see>. 
        /// </para>
        /// </remarks>
        [Flags]
        public enum DesiredAccess : uint
        {
            /// <summary>
            /// For a directory, the right to list the contents of the directory.
            /// </summary>
            FILE_LIST_DIRECTORY = 0x1,

            /// <summary>
            /// For a file object, the right to read the corresponding file data. For a directory object, the right to read the corresponding directory data.
            /// </summary>
            FILE_READ_DATA = 0x1,

            /// <summary>
            /// For a directory, the right to create a file in the directory.
            /// </summary>
            FILE_ADD_FILE = 0x2,

            /// <summary>
            /// For a file object, the right to write data to the file.
            /// </summary>
            FILE_WRITE_DATA = 0x2,

            /// <summary>
            /// For a directory, the right to create a subdirectory.
            /// </summary>
            FILE_ADD_SUBDIRECTORY = 0x4,

            /// <summary>
            /// For a file object, the right to append data to the file. (For local files, write operations will not overwrite existing data if this flag is specified without FILE_WRITE_DATA.)
            /// </summary>
            FILE_APPEND_DATA = 0x4,

            /// <summary>
            /// For a named pipe, the right to create a pipe.
            /// </summary>
            FILE_CREATE_PIPE_INSTANCE = 0x4,

            /// <summary>
            /// The right to read extended file attributes.
            /// </summary>
            FILE_READ_EA = 0x8,

            /// <summary>
            /// The right to write extended file attributes.
            /// </summary>
            FILE_WRITE_EA = 0x10,

            /// <summary>
            /// For a native code file, the right to execute the file. This access right given to scripts may cause the script to be executable, depending on the script interpreter.
            /// </summary>
            FILE_EXECUTE = 0x20,

            /// <summary>
            /// For a directory, the right to traverse the directory. By default, users are assigned the BYPASS_TRAVERSE_CHECKING <see href="https://learn.microsoft.com/en-us/windows/win32/secauthz/privileges">privilege</see>, which ignores the FILE_TRAVERSE <see href="https://learn.microsoft.com/en-us/windows/win32/secauthz/access-rights-and-access-masks">access right</see>. See the remarks in <see href="https://learn.microsoft.com/en-us/windows/win32/fileio/file-security-and-access-rights">File Security and Access Rights</see> for more information.
            /// </summary>
            FILE_TRAVERSE = 0x20,

            /// <summary>
            /// For a directory, the right to delete a directory and all the files it contains, including read-only files.
            /// </summary>
            FILE_DELETE_CHILD = 0x40,

            /// <summary>
            /// The right to read file attributes.
            /// </summary>
            FILE_READ_ATTRIBUTES = 0x80,

            /// <summary>
            /// The right to write file attributes.
            /// </summary>
            FILE_WRITE_ATTRIBUTES = 0x100,

            /// <summary>
            /// Right to delete an object.
            /// </summary>
            DELETE = 0x00010000,

            /// <summary>
            /// Right to wait on a handle.
            /// </summary>
            SYNCHRONIZE = 0x00100000,

            // The following generic rights map to standard and specific rights.
            // The information about the mapping comes from https://learn.microsoft.com/en-us/windows/win32/fileio/file-security-and-access-rights

            /// <summary>
            /// FILE_READ_ATTRIBUTES | FILE_READ_DATA | FILE_READ_EA | STANDARD_RIGHTS_READ | SYNCHRONIZE
            /// </summary>
            FILE_GENERIC_READ = 0x80000000,

            /// <summary>
            /// FILE_APPEND_DATA | FILE_WRITE_ATTRIBUTES | FILE_WRITE_DATA | FILE_WRITE_EA | STANDARD_RIGHTS_WRITE | SYNCHRONIZE
            /// </summary>
            GenericWrite = 0x40000000,

            /// <summary>
            /// FILE_EXECUTE | FILE_READ_ATTRIBUTES | STANDARD_RIGHTS_EXECUTE | SYNCHRONIZE
            /// </summary>
            GenericExecute = 0x20000000,

            /// <summary>
            /// All possible access rights.
            /// </summary>
            GenericAll = 0x10000000,
        }

        /// <summary>
        /// Specifies the sharing mode of a file or device when it is opened or created.
        /// </summary>
        /// <remarks>
        /// This enumeration defines constants that represent different sharing modes for files and devices. 
        /// These sharing modes determine how the file or device can be accessed by other processes while it is open.
        /// The enumeration is marked with the <see cref="FlagsAttribute"/>, allowing bitwise combinations of its member values to specify multiple
        /// access rights simultaneously. 
        /// </remarks>
        [Flags]
        public enum ShareMode : uint
        {
            /// <summary>
            /// Prevents subsequent open operations on a file or device if they request delete, read, or write access.
            /// </summary>
            FILE_SHARE_NONE = 0x00000000,

            /// <summary>
            /// Enables subsequent open operations on a file or device to request read access. Otherwise, no process can open the file or device if it requests read access.
            /// </summary>
            /// <remarks>
            /// If this flag is not specified, but the file or device has been opened for read access, the function fails.
            /// </remarks>
            FILE_SHARE_READ = 0x00000001,

            /// <summary>
            /// Enables subsequent open operations on a file or device to request write access. Otherwise, no process can open the file or device if it requests write access.
            /// </summary>
            /// <remarks>
            /// If this flag is not specified, but the file or device has been opened for write access or has a file mapping with write access, the function fails.
            /// </remarks>
            FILE_SHARE_WRITE = 0x00000002,

            /// <summary>
            /// Enables subsequent open operations on a file or device to request delete access. Otherwise, no process can open the file or device if it requests delete access.
            /// </summary>
            /// <remarks>
            /// If this flag is not specified, but the file or device has been opened for delete access, the function fails.
            /// </remarks>
            FILE_SHARE_DELETE = 0x00000004,
        }

        /// <summary>
        /// Specifies the action to take on a file or device when calling file creation or opening functions.
        /// </summary>
        /// <remarks>This enumeration is used to define the behavior of file creation or opening
        /// operations, such as whether to create a new file, open an existing file, or truncate an existing file.  Each
        /// value corresponds to a specific action and may result in different outcomes depending on the existence of
        /// the file and the access permissions.</remarks>
        public enum CreationDisposition : uint
        {
            /// <summary>
            /// Creates a new file, only if it does not already exist. If the specified file exists, the function fails and the last-error code is set to ERROR_FILE_EXISTS (80).
            /// </summary>
            /// <remarks>
            /// If the specified file does not exist and is a valid path to a writable location, a new file is created.
            /// </remarks>
            CREATE_NEW = 1,

            /// <summary>
            /// Creates a new file, always. If the specified file exists and is writable, the function truncates the file, the function succeeds, and last-error code is set to ERROR_ALREADY_EXISTS (183).
            /// </summary>
            /// <remarks>
            /// If the specified file does not exist and is a valid path, a new file is created, the function succeeds, and the last-error code is set to zero.
            /// </remarks>
            CREATE_ALWAYS = 2,

            /// <summary>
            /// Opens a file or device, only if it exists. If the specified file or device does not exist, the function fails and the last-error code is set to ERROR_FILE_NOT_FOUND (2).
            /// </summary>
            OPEN_EXISTING = 3,

            /// <summary>
            /// Opens a file, always. If the specified file exists, the function succeeds and the last-error code is set to ERROR_ALREADY_EXISTS (183).
            /// </summary>
            OPEN_ALWAYS = 4,

            /// <summary>
            /// Opens a file and truncates it so that its size is zero bytes, only if it exists. If the specified file does not exist, the function fails and the last-error code is set to ERROR_FILE_NOT_FOUND (2).
            /// </summary>
            /// <remarks>
            /// The calling process must open the file with the GENERIC_WRITE bit set as part of the DesiredAccess parameter.
            /// </remarks>
            TRUNCATE_EXISTING = 5,
        }

        /// <summary>
        /// Specifies the attributes of files and directories. This enumeration defines constants that represent various
        /// file and directory attributes, such as read-only, hidden, system, and others. These attributes can be
        /// combined using a bitwise OR operation due to the <see cref="FlagsAttribute"/> applied to the enumeration.
        /// </summary>
        /// <remarks>The <see cref="FileAttributes"/> enumeration is used to describe the characteristics
        /// of files and directories in the file system. These attributes can be used to query or modify file and
        /// directory metadata.</remarks>
        [Flags]
        public enum FileAttributes : uint
        {
            /// <summary>
            /// A file that is read-only. Applications can read the file, but cannot write to it or delete it. This attribute is not honored on directories.
            /// </summary>
            FILE_ATTRIBUTE_READONLY = 0x00000001,

            /// <summary>
            /// The file or directory is hidden. It is not included in an ordinary directory listing.
            /// </summary>
            FILE_ATTRIBUTE_HIDDEN = 0x00000002,

            /// <summary>
            /// A file or directory that the operating system uses a part of, or uses exclusively.
            /// </summary>
            FILE_ATTRIBUTE_SYSTEM = 0x00000004,

            /// <summary>
            /// The handle that identifies a directory.
            /// </summary>
            FILE_ATTRIBUTE_DIRECTORY = 0x00000010,

            /// <summary>
            /// A file or directory that is an archive file or directory. Applications typically use this attribute to mark files for backup or removal.
            /// </summary>
            FILE_ATTRIBUTE_ARCHIVE = 0x00000020,

            /// <summary>
            /// This value is reserved for system use.
            /// </summary>
            FILE_ATTRIBUTE_DEVICE = 0x00000040,

            /// <summary>
            /// A file that does not have other attributes set. This attribute is valid only when used alone.
            /// </summary>
            FILE_ATTRIBUTE_NORMAL = 0x00000080,

            /// <summary>
            /// A file that is being used for temporary storage. File systems avoid writing data back to mass storage if sufficient cache memory is available, because typically, an application deletes a temporary file after the handle is closed. In that scenario, the system can entirely avoid writing the data. Otherwise, the data is written after the handle is closed.
            /// </summary>
            FILE_ATTRIBUTE_TEMPORARY = 0x00000100,

            /// <summary>
            /// A file that is a sparse file.
            /// </summary>
            FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,

            /// <summary>
            /// A file or directory that has an associated reparse point, or a file that is a symbolic link.
            /// </summary>
            FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,

            /// <summary>
            /// A file or directory that is compressed. For a file, all of the data in the file is compressed. For a directory, compression is the default for newly created files and subdirectories.
            /// </summary>
            FILE_ATTRIBUTE_COMPRESSED = 0x00000800,

            /// <summary>
            /// The data of a file is not available immediately. This attribute indicates that the file data is physically moved to offline storage. This attribute is used by Remote Storage, which is the hierarchical storage management software. Applications should not arbitrarily change this attribute.
            /// </summary>
            FILE_ATTRIBUTE_OFFLINE = 0x00001000,

            /// <summary>
            /// The file or directory is not to be indexed by the content indexing service.
            /// </summary>
            FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000,

            /// <summary>
            /// A file or directory that is encrypted. For a file, all data streams in the file are encrypted. For a directory, encryption is the default for newly created files and subdirectories.
            /// </summary>
            FILE_ATTRIBUTE_ENCRYPTED = 0x00004000,

            /// <summary>
            /// The directory or user data stream is configured with integrity (only supported on ReFS volumes). It is not included in an ordinary directory listing. The integrity setting persists with the file if it's renamed. If a file is copied the destination file will have integrity set if either the source file or destination directory have integrity set.
            /// Windows Server 2008 R2, Windows 7, Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP: This flag is not supported until Windows Server 2012.
            /// </summary>
            FILE_ATTRIBUTE_INTEGRITY_STREAM = 0x00008000,

            /// <summary>
            /// This value is reserved for system use.
            /// </summary>
            FILE_ATTRIBUTE_VIRTUAL = 0x00010000,

            /// <summary>
            /// The user data stream not to be read by the background data integrity scanner (AKA scrubber). When set on a directory it only provides inheritance. This flag is only supported on Storage Spaces and ReFS volumes. It is not included in an ordinary directory listing.
            /// Windows Server 2008 R2, Windows 7, Windows Server 2008, Windows Vista, Windows Server 2003 and Windows XP: This flag is not supported until Windows 8 and Windows Server 2012.
            /// </summary>
            FILE_ATTRIBUTE_NO_SCRUB_DATA = 0x00020000,

            /// <summary>
            /// A file or directory with extended attributes.
            /// IMPORTANT: This constant is for internal use only.
            /// </summary>
            FILE_ATTRIBUTE_EA = 0x00040000,

            /// <summary>
            /// This attribute indicates user intent that the file or directory should be kept fully present locally even when not being actively accessed. This attribute is for use with hierarchical storage management software.
            /// </summary>
            FILE_ATTRIBUTE_PINNED = 0x00080000,

            /// <summary>
            /// This attribute indicates that the file or directory should not be kept fully present locally except when being actively accessed. This attribute is for use with hierarchical storage management software.
            /// </summary>
            FILE_ATTRIBUTE_UNPINNED = 0x00100000,

            /// <summary>
            /// This attribute only appears in directory enumeration classes (FILE_DIRECTORY_INFORMATION, FILE_BOTH_DIR_INFORMATION, etc.). When this attribute is set, it means that the file or directory has no physical representation on the local system; the item is virtual. Opening the item will be more expensive than normal, e.g. it will cause at least some of it to be fetched from a remote store.
            /// </summary>
            FILE_ATTRIBUTE_RECALL_ON_OPEN = 0x00040000,

            /// <summary>
            /// When this attribute is set, it means that the file or directory is not fully present locally. For a file that means that not all of its data is on local storage (e.g. it may be sparse with some data still in remote storage). For a directory it means that some of the directory contents are being virtualized from another location. Reading the file / enumerating the directory will be more expensive than normal, e.g. it will cause at least some of the file/directory content to be fetched from a remote store. Only kernel-mode callers can set this bit.
            /// File system mini filters below the 180000 – 189999 altitude range (FSFilter HSM Load Order Group) must not issue targeted cached reads or writes to files that have this attribute set. This could lead to cache pollution and potential file corruption. For more information, see Handling placeholders.
            /// </summary>
            FILE_ATTRIBUTE_RECALL_ON_DATA_ACCESS = 0x00400000,

            /// <summary>
            /// The file data is requested, but it should continue to be located in remote storage. It should not be transported back to local storage. This flag is for use by remote storage systems.
            /// </summary>
            FILE_FLAG_OPEN_NO_RECALL = 0x00100000,

            /// <summary>
            /// Normal reparse point processing will not occur; CreateFile will attempt to open the reparse point. When a file is opened, a file handle is returned, whether or not the filter that controls the reparse point is operational.
            /// This flag cannot be used with the CREATE_ALWAYS flag.
            /// If the file is not a reparse point, then this flag is ignored.
            /// </summary>
            FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000,

            /// <summary>
            /// The file or device is being opened with session awareness. If this flag is not specified, then per-session devices (such as a device using RemoteFX USB Redirection) cannot be opened by processes running in session 0. This flag has no effect for callers not in session 0. This flag is supported only on server editions of Windows.
            /// Windows Server 2008 R2 and Windows Server 2008: This flag is not supported before Windows Server 2012.
            /// </summary>
            FILE_FLAG_SESSION_AWARE = 0x00800000,

            /// <summary>
            /// The file is being opened or created for a backup or restore operation. The system ensures that the calling process overrides file security checks when the process has SE_BACKUP_NAME and SE_RESTORE_NAME privileges. For more information, see Changing Privileges in a Token.
            /// You must set this flag to obtain a handle to a directory. A directory handle can be passed to some functions instead of a file handle. For more information, see the Remarks section.
            /// </summary>
            FILE_FLAG_BACKUP_SEMANTICS = 0x02000000,

            /// <summary>
            /// Access is intended to be sequential from beginning to end. The system can use this as a hint to optimize file caching.
            /// This flag should not be used if read-behind (that is, reverse scans) will be used.
            /// This flag has no effect if the file system does not support cached I/O and FILE_FLAG_NO_BUFFERING.
            /// For more information, see the Caching Behavior section of this topic.
            /// </summary>
            FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000,

            /// <summary>
            /// Access is intended to be random. The system can use this as a hint to optimize file caching.
            /// This flag has no effect if the file system does not support cached I/O and FILE_FLAG_NO_BUFFERING.
            /// For more information, see the Caching Behavior section of this topic.
            /// </summary>
            FILE_FLAG_RANDOM_ACCESS = 0x10000000,

            /// <summary>
            /// The file or device is being opened with no system caching for data reads and writes. This flag does not affect hard disk caching or memory mapped files.
            /// There are strict requirements for successfully working with files opened with CreateFile using the FILE_FLAG_NO_BUFFERING flag, for details see File Buffering.
            /// </summary>
            FILE_FLAG_NO_BUFFERING = 0x20000000,

            /// <summary>
            /// The file or device is being opened or created for asynchronous I/O.
            /// When subsequent I/O operations are completed on this handle, the event specified in the OVERLAPPED structure will be set to the signaled state.
            /// If this flag is specified, the file can be used for simultaneous read and write operations.
            /// If this flag is not specified, then I/O operations are serialized, even if the calls to the read and write functions specify an OVERLAPPED structure.
            /// For information about considerations when using a file handle created with this flag, see the Synchronous and Asynchronous I/O Handles section of this topic.
            /// </summary>
            FILE_FLAG_OVERLAPPED = 0x40000000,

            /// <summary>
            /// Write operations will not go through any intermediate cache, they will go directly to disk.
            /// For additional information, see the Caching Behavior section of this topic.
            /// </summary>
            FILE_FLAG_WRITE_THROUGH = 0x80000000,

            /// <summary>
            /// Access will occur according to POSIX rules. This includes allowing multiple files with names, differing only in case, for file systems that support that naming. Use care when using this option, because files created with this flag may not be accessible by applications that are written for MS-DOS or 16-bit Windows.
            /// </summary>
            FILE_FLAG_POSIX_SEMANTICS = 0x01000000,

            /// <summary>
            /// The file is to be deleted immediately after all of its handles are closed, which includes the specified handle and any other open or duplicated handles.
            /// If there are existing open handles to a file, the call fails unless they were all opened with the FILE_SHARE_DELETE share mode.
            /// Subsequent open requests for the file fail, unless the FILE_SHARE_DELETE share mode is specified.
            /// </summary>
            FILE_FLAG_DELETE_ON_CLOSE = 0x04000000
        }

        #endregion

        #region external methods

        // When adding new external methods, ensure setting SetLastError = true in the DllImport attribute
        // to allow proper error handling using Marshal.GetLastWin32Error().

        // https://stackoverflow.com/questions/57020034/how-to-specify-to-createfilew-function-get-uncached-result
        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateFile(
        string lpFileName,
        int dwDesiredAccess,
        int dwShareMode,
        IntPtr lpSecurityAttributes,
        int dwCreationDisposition,
        int dwFlagsAndAttributes,
        IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FindClose(IntPtr hFindFile);

        #endregion


        /// <summary>
        /// Creates or opens a file and returns a handle to the file as an <see cref="IntPtr"/>.
        /// </summary>
        /// <remarks>This method is a wrapper for a platform-specific file creation/opening function. Ensure that
        /// the parameters provided are valid and consistent with the underlying platform's requirements.</remarks>
        /// <param name="lpFileName">The name of the file to be created or opened. This can include a path.</param>
        /// <param name="dwDesiredAccess">The access mode for the file, such as read, write, or both. Use constants defined in the Windows API, such
        /// as <c>GENERIC_READ</c> or <c>GENERIC_WRITE</c>.</param>
        /// <param name="dwShareMode">The sharing mode for the file. Use values from the <see cref="FileShare"/> enumeration to specify how the
        /// file can be shared.</param>
        /// <param name="dwCreationDisposition">Specifies how the operating system should create or open the file. Use values from the <see
        /// cref="FileMode"/> enumeration, such as <see cref="FileMode.Create"/> or <see cref="FileMode.Open"/>.</param>
        /// <param name="dwFlagsAndAttributes">The file attributes and flags. Use constants defined in the Windows API, such as
        /// <c>FILE_ATTRIBUTE_NORMAL</c> or <c>FILE_FLAG_OVERLAPPED</c>.</param>
        /// <param name="ignoreMaxPathLength">If set to <c>true</c>, the method will handle file paths longer than the traditional MAX_PATH limit by
        /// the Windows API. This is done by prefixing the path with <c>\\?\</c> or <c>\\?\UNC\</c> for network paths.</param>
        /// <returns>An <see cref="IntPtr"/> representing the handle to the file. If the operation fails, the returned handle
        /// will be <see cref="IntPtr.Zero"/>.</returns>
        public static IntPtr CreateFileSafe(
            string lpFileName,
            DesiredAccess dwDesiredAccess, // int
            ShareMode dwShareMode,
            CreationDisposition dwCreationDisposition,
            FileAttributes dwFlagsAndAttributes,
            bool ignoreMaxPathLength = false)
        {
            if (ignoreMaxPathLength)
            {
                if (!lpFileName.StartsWith(@"\\?\"))
                {
                    if (lpFileName.StartsWith(@"\\"))
                    {
                        // Network path
                        lpFileName = @"\\?\UNC\" + lpFileName[2..];
                    }
                    else
                    {
                        lpFileName = @"\\?\" + lpFileName;
                    }
                }
            }

            return CreateFile(
                lpFileName,
                (int)dwDesiredAccess,
                (int)dwShareMode,
                IntPtr.Zero,
                (int)dwCreationDisposition,
                (int)dwFlagsAndAttributes,
                IntPtr.Zero);
        }

        /// <summary>
        /// Safely closes a handle if it is valid.
        /// </summary>
        /// <remarks>This method ensures that only valid handles are passed to the underlying
        /// handle-closing operation. If the handle is invalid (e.g., <see cref="IntPtr.Zero"/> or -1), the method does
        /// nothing.</remarks>
        /// <param name="handle">The handle to be closed. Must not be <see cref="IntPtr.Zero"/> or <see cref="IntPtr"/> value -1.</param>
        public static void CloseHandleSafe(IntPtr handle)
        {
            if (handle != IntPtr.Zero && handle != (IntPtr)(-1))
            {
                CloseHandle(handle);
            }
        }

        internal static IntPtr FindFirstFileSafe(string fileName, out WIN32_FIND_DATA findData, bool handleError = false)
        {
            var handle = FindFirstFile(fileName, out findData);

            if (handleError)
            {
                if (handle == IntPtr.Zero || handle == new IntPtr(-1))
                {
                    // The error codes can be found here: https://learn.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-
                    var error = Marshal.GetLastWin32Error();
                    switch (error)
                    {
                        case 0:
                            // NO_ERROR 0(0x0)
                            // The operation completed successfully.
                            // This should not happen, as the handle is invalid.
                            throw new System.ComponentModel.Win32Exception("An unknown error occurred while trying to find the first file.");

                        case 2:
                            // ERROR_FILE_NOT_FOUND 2(0x2)
                            // The system cannot find the file specified.
                            throw new FileNotFoundException($"File not found: {fileName}");

                        case 3:
                            // ERROR_PATH_NOT_FOUND 3(0x3)
                            // The system cannot find the path specified.
                            throw new FileNotFoundException($"The specified path was not found: {fileName}");

                        case 123:
                            // ERROR_INVALID_NAME 123(0x7B)
                            // The filename, directory name, or volume label syntax is incorrect.

                            // This error code indicates that the user provided an invalid name for the file.
                            // In most cases the file name ends with an backslash, which indicates a directory.
                            throw new ArgumentException($"No search pattern was provided, or it is invalid: {fileName}", nameof(fileName));

                        case 87:
                            // ERROR_INVALID_PARAMETER 87(0x57)
                            // The parameter is incorrect.
                            throw new ArgumentException($"The file name is too long or contains invalid characters: {fileName}", nameof(fileName));
                        default:
                            throw new System.ComponentModel.Win32Exception(error, $"Failed to find first file: {fileName}");
                    }
                }
            }
            return handle;
        }

        internal static bool FindNextFileSafe(IntPtr handle, out WIN32_FIND_DATA findData)
        {
            var result = FindNextFile(handle, out findData);
            if (!result)
            {
                var error = Marshal.GetLastWin32Error();
                if (error != 18) // ERROR_NO_MORE_FILES
                {
                    throw new System.ComponentModel.Win32Exception(error, "Failed to find next file.");
                }
            }
            return result;
        }

        internal static void FindCloseSafe(IntPtr handle)
        {
            if (handle != IntPtr.Zero && handle != (IntPtr)(-1))
            {
                FindClose(handle);
            }
        }
    }
}
