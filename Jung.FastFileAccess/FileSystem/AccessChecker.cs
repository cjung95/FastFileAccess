// This file is part of the Jung FastFileAccess project.
// The project is licensed under the MIT license.

using Jung.FastFileAccess.Models;
using Jung.FastFileAccess.Models.Base;
using Jung.FastFileAccess.Native;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Jung.FastFileAccess.FileSystem
{
    /// <summary>
    /// Provides functionality to evaluate access rights for file and directory paths, including both local and network
    /// paths.
    /// </summary>
    /// <remarks>The <see cref="AccessChecker"/> class allows users to determine the accessibility of
    /// specified paths based on requested access rights. It supports evaluating both local and network paths, handling
    /// scenarios such as verifying drive existence, resolving network connectivity, and checking specific access
    /// permissions. The class also provides logging capabilities for diagnostic and operational messages.</remarks>
    public class AccessChecker
    {
        private readonly AccessCheckerConfiguration _configuration;
        private readonly ILogger<AccessChecker>? _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessChecker"/> class with the specified configuration and
        /// logger.
        /// </summary>
        /// <param name="configuration">An optional <see cref="AccessCheckerConfiguration"/> instance that defines the access checking behavior.  If
        /// null, a default configuration is used.</param>
        /// <param name="logger">An optional <see cref="ILogger{AccessChecker}"/> instance for logging diagnostic and operational messages. 
        /// If null, no logging is performed.</param>
        public AccessChecker(AccessCheckerConfiguration? configuration = null, ILogger<AccessChecker>? logger = null)
        {
            _configuration = configuration ?? new AccessCheckerConfiguration();
            _logger = logger;
        }

        /// <summary>
        /// Evaluates the access rights for a collection of access requests and determines the accessibility of the
        /// specified paths.
        /// </summary>
        /// <remarks>This method processes each access request in parallel, evaluating the accessibility
        /// of the specified paths based on their type (e.g., local or network paths) and the requested access rights.
        /// The method updates the status of each <see cref="AccessRequestItem"/>  to indicate the result of the
        /// evaluation, such as whether the path was found, the network was reachable, or the requested access was
        /// granted.  If a path is a network path, the method attempts to resolve the server's IP address and checks
        /// network connectivity. For local paths,  it verifies the existence of the drive. If the path exists, the
        /// method checks the requested access rights.  In the event of an error during evaluation, the status of the
        /// corresponding <see cref="AccessRequestItem"/> is set to indicate an unknown error.</remarks>
        /// <param name="requestItems">A collection of <see cref="AccessRequestItem"/> objects representing the paths and requested access types to
        /// evaluate.</param>
        /// <returns>An array of <see cref="AccessRequestItem"/> objects, each updated with the result of the access rights
        /// evaluation.</returns>
        public AccessRequestItem[] CheckAccessRights(IEnumerable<AccessRequestItem> requestItems)
        {
            var accessTasks = requestItems.Select(requestItem => Task.Run(() =>
            {
                try
                {
                    if (requestItem.Path.StartsWith(@"\\"))
                    {
                        string serverName = string.Empty;
                        try
                        {
                            // We have an UNC path, let's try to get the IP address of the server
                            var ping = new Ping();
                            serverName = requestItem.Path.TrimStart('\\').Split('\\')[0];
                            requestItem.HostAddresses = Dns.GetHostAddresses(serverName);

                            // We take the first address only, thats how its done in Ping.Send if we pass a hostname
                            var result = ping.Send(requestItem.HostAddresses[0], (int)_configuration.ServerRequestTimeout.TotalMilliseconds);
                            if (result.Status != IPStatus.Success)
                            {
                                _logger?.LogDebug("Ping to server {Server} failed with status {Status}.", serverName, result.Status);
                                requestItem.SetStatus(Models.Enums.AccessStatus.NetworkUnreachable);
                            }
                        }
                        catch (SocketException socketException)
                        {
                            // DNS Request failed
                            _logger?.LogDebug(socketException, "DNS resolution for server {Server} failed.", requestItem.HostName);
                            requestItem.SetStatus(Models.Enums.AccessStatus.DnsResolutionFailed);
                        }
                        catch (PingException pingException)
                        {
                            // Ping failed
                            _logger?.LogDebug(pingException, "Ping to server {Server} failed.", serverName);
                            requestItem.SetStatus(Models.Enums.AccessStatus.NetworkUnreachable);
                        }
                    }
                    else
                    {
                        // We have a local path, let's check if the drive exists
                        var drive = Path.GetPathRoot(requestItem.Path);
                        if (!Directory.Exists(drive))
                        {
                            _logger?.LogDebug("Drive {Drive} not found for path {Path}.", drive, requestItem.Path);
                            requestItem.SetStatus(Models.Enums.AccessStatus.DriveNotFound);
                        }
                    }

                    if (!requestItem.StatusIsSet)
                    {
                        // So far everything looks good, let's check the actual access rights
                        FileSystemInfo fileSystemInfo;
                        Kernel32Interop.DesiredAccess desiredAccess = 0;

                        if (requestItem is FileAccessRequest fileAccessRequest)
                        {
                            // We have a file access request
                            fileSystemInfo = new FileInfo(requestItem.Path);
                            desiredAccess = (Kernel32Interop.DesiredAccess)fileAccessRequest.AccessType;
                        }
                        else if (requestItem is DirectoryAccessRequest directoryAccessRequest)
                        {
                            // We have a directory access request
                            fileSystemInfo = new DirectoryInfo(requestItem.Path);
                            desiredAccess = (Kernel32Interop.DesiredAccess)directoryAccessRequest.AccessType;
                        }
                        else
                        {
                            throw new NotSupportedException("Unsupported AccessRequestItem type.");
                        }

                        if (fileSystemInfo.Exists)
                        {
                            // The file or directory exists, now check the access
                            var hasAccess = CheckAccess(requestItem.Path, desiredAccess);
                            if (hasAccess)
                            {
                                requestItem.SetStatus(Models.Enums.AccessStatus.AccessGranted);
                            }
                            else
                            {
                                requestItem.SetStatus(Models.Enums.AccessStatus.AccessNotGranted);
                            }
                        }
                        else
                        {
                            requestItem.SetStatus(Models.Enums.AccessStatus.PathNotFound);
                        }
                    }
                }
                catch (Exception exception)
                {
                    // In case of any unexpected error, we assume the path is not accessible
                    // and set the reason to UnknownError.
                    _logger?.LogDebug(exception, "Error checking access for path {Path}.", requestItem.Path);
                    requestItem.SetStatus(Models.Enums.AccessStatus.UnknownError);
                }
            })).ToArray();

            // Await all tasks to complete
            Task.WaitAll(accessTasks);
            return requestItems.ToArray();
        }


        /// <summary>
        /// Determines whether the specified access rights are granted for the given file or directory path.
        /// </summary>
        /// <remarks>This method attempts to open a handle to the specified file or directory with the
        /// requested access rights. If the handle cannot be opened, the method logs the failure and returns <see
        /// langword="false"/>.</remarks>
        /// <param name="path">The file or directory path to check access for. This parameter cannot be <see langword="null"/> or empty.</param>
        /// <param name="desiredAccess">The specific access rights to check, such as read, write, or execute permissions.</param>
        /// <returns><see langword="true"/> if the specified access rights are granted for the given path; otherwise, <see
        /// langword="false"/>.</returns>
        private bool CheckAccess(string path, Kernel32Interop.DesiredAccess desiredAccess)
        {
            IntPtr handle = IntPtr.Zero;
            try
            {

                // Create a handle to the file or directory
                handle = Kernel32Interop.CreateFileSafe(path,
                    dwDesiredAccess: desiredAccess,

                    // We want to allow other processes to read, write and delete the file while we have it open
                    // This is important for parallel access scenarios
                    dwShareMode: Kernel32Interop.ShareMode.FILE_SHARE_READ | 
                    Kernel32Interop.ShareMode.FILE_SHARE_WRITE | 
                    Kernel32Interop.ShareMode.FILE_SHARE_DELETE,

                    dwCreationDisposition: Kernel32Interop.CreationDisposition.OPEN_EXISTING,
                    dwFlagsAndAttributes: Kernel32Interop.FileAttributes.FILE_FLAG_BACKUP_SEMANTICS);

                if (handle == IntPtr.Zero || handle == (IntPtr)(-1))
                {
                    int error = Marshal.GetLastWin32Error();
                    _logger?.LogDebug("Failed to open handle for path {Path}. Error code: {ErrorCode}", path, error);
                    return false;
                }
            }
            finally
            {
                Kernel32Interop.CloseHandleSafe(handle);
            }
            return true;
        }
    }
}
