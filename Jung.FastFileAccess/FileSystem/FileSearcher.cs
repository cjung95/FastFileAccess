// This file is part of the Jung FastFileAccess project.
// The project is licensed under the MIT license.

using Jung.FastFileAccess.Exceptions;
using Jung.FastFileAccess.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Jung.FastFileAccess.FileSystem
{
    /// <summary>
    /// Provides methods for searching files in a directory based on specified criteria.
    /// </summary>
    /// <remarks>The <see cref="FileSearcher"/> class includes static methods for finding files in a
    /// directory, such as identifying the newest file or retrieving files sorted by creation time. These methods
    /// support search patterns to filter files by name and are designed to work with the Windows file system.</remarks>
    public class FileSearcher
    {
        /// <summary>
        /// Finds the newest file in the specified directory that matches the search pattern. 
        /// </summary>
        /// <param name="directoryPath">The directory to search in.</param>
        /// <param name="searchPattern">The search pattern to match against the names of files in the directory. The file name can include wildcard characters, for example, an asterisk (*) or a question mark (?).</param>
        /// <returns>The path of the newest file that matches the search pattern, or null if no matching file was found.</returns>
        public string? FindNewestFile(string directoryPath, string searchPattern)
        {
            ThrowForInvalidDirectoryPath(directoryPath);
            ThrowForInvalidSearchPattern(searchPattern);

            string pathPattern = Path.Combine(directoryPath, searchPattern);
            string? newestFile = null;
            var handle = new IntPtr(-1);
            try
            {
                // Start the file search
                handle = Kernel32Interop.FindFirstFileSafe(pathPattern, out Kernel32Interop.WIN32_FIND_DATA findData, handleError: true);
                DateTime newest = DateTime.MinValue;

                do
                {
                    DateTime lastWriteTime = ExtractLastWriteTime(findData);
                    if (lastWriteTime > newest)
                    {
                        // This file is newer than the current newest file
                        newest = lastWriteTime;
                        newestFile = Path.Combine(directoryPath, findData.cFileName);
                    }
                } while (Kernel32Interop.FindNextFileSafe(handle, out findData));
            }
            catch (System.ComponentModel.Win32Exception win32Exception)
            {
                throw new FileSearchException($"An error occurred while searching for files in '{directoryPath}' with pattern '{searchPattern}'.", win32Exception);
            }
            finally
            {
                // Close the find handle
                Kernel32Interop.FindCloseSafe(handle);
            }

            return newestFile;
        }

        /// <summary>
        /// Finds all files in the specified directory that match the search pattern and returns them in descending order of creation time.
        /// </summary>
        /// <param name="directoryPath">The directory to search in.</param>
        /// <param name="searchPattern">The search pattern to match against the names of files in the directory.</param>
        /// <returns>An array of file paths in descending order of creation time.</returns>
        public string[] FindFilesWithDescendingCreationTime(string directoryPath, string searchPattern)
        {
            ThrowForInvalidDirectoryPath(directoryPath);
            ThrowForInvalidSearchPattern(searchPattern);
            string pathPattern = Path.Combine(directoryPath, searchPattern);
            var fileList = new Dictionary<DateTime, string>();
            var handle = new IntPtr(-1);
            try
            {
                handle = Kernel32Interop.FindFirstFileSafe(pathPattern, out Kernel32Interop.WIN32_FIND_DATA findData);
                if (handle == new IntPtr(-1))
                {
                    // No files found
                    return Array.Empty<string>();
                }

                // Iterate through all found files and add them to the list
                do
                {
                    var lastWriteTime = ExtractLastWriteTime(findData);
                    fileList.Add(lastWriteTime, Path.Combine(directoryPath, findData.cFileName));
                } while (Kernel32Interop.FindNextFileSafe(handle, out findData));
            }
            catch (System.ComponentModel.Win32Exception win32Exception)
            {
                throw new FileSearchException($"An error occurred while searching for files in '{directoryPath}' with pattern '{searchPattern}'.", win32Exception);
            }
            catch (Exception exception)
            {
                throw new FileSearchException($"An unexpected error occurred while searching for files in '{directoryPath}' with pattern '{searchPattern}'.", exception);
            }
            finally
            {
                // Close the find handle
                Kernel32Interop.FindCloseSafe(handle);
            }

            // Sort the list by creation time in descending order and return the file paths
            return fileList.OrderByDescending(fileList => fileList.Key).Select(fileList => fileList.Value).ToArray();
        }

        /// <summary>
        /// Extracts the last write time from the specified <see cref="Kernel32Interop.WIN32_FIND_DATA"/> structure.
        /// </summary>
        /// <remarks>This method converts the <c>ftLastWriteTime</c> field of the <see
        /// cref="Kernel32Interop.WIN32_FIND_DATA"/> structure, which is represented as a <c>FILETIME</c>, into a <see
        /// cref="DateTime"/> object.</remarks>
        /// <param name="findData">A <see cref="Kernel32Interop.WIN32_FIND_DATA"/> structure containing file metadata, including the last write
        /// time.</param>
        /// <returns>A <see cref="DateTime"/> representing the last write time of the file, expressed in Coordinated Universal
        /// Time (UTC).</returns>
        private DateTime ExtractLastWriteTime(Kernel32Interop.WIN32_FIND_DATA findData)
        {
            // Convert the FILETIME structure to a DateTime object
            long fileTime = ((long)findData.ftLastWriteTime.dwHighDateTime << 32) | (uint)findData.ftLastWriteTime.dwLowDateTime;
            DateTime lastWriteTime = DateTime.FromFileTimeUtc(fileTime);
            return lastWriteTime;
        }


        private void ThrowForInvalidSearchPattern(string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("Search pattern cannot be null or empty.", nameof(searchPattern));
            }
            if (searchPattern.TrimEnd().EndsWith('\\'))
            {
                throw new ArgumentException("Search pattern cannot end with a backslash ('\\').", nameof(searchPattern));
            }
        }

        private void ThrowForInvalidDirectoryPath(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));
            }
        }
    }
}
