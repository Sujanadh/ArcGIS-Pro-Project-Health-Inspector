using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace APHI.Utilities;

/// <summary>
/// Helper methods for interacting with the file system.
/// </summary>
public static class FileSystemHelper
{
    /// <summary>
    /// Gets the total size of a directory in megabytes.
    /// </summary>
    /// <param name="directoryPath">The path to the directory.</param>
    /// <returns>The size in MB.</returns>
    public static double GetDirectorySizeMB(string directoryPath)
    {
        if (!Directory.Exists(directoryPath)) return 0;

        try
        {
            var info = new DirectoryInfo(directoryPath);
            long size = info.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
            return size / 1024.0 / 1024.0;
        }
        catch
        {
            // Fallback for permissions issues
            return 0;
        }
    }

    /// <summary>
    /// Gets the number of files in a directory.
    /// </summary>
    public static int GetFileCount(string directoryPath, string searchPattern = "*")
    {
        if (!Directory.Exists(directoryPath)) return 0;
        
        try
        {
            return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories).Length;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Recursively finds files matching a pattern.
    /// </summary>
    public static IEnumerable<string> FindFilesRecursive(string rootPath, string searchPattern)
    {
        var pending = new Queue<string>();
        pending.Enqueue(rootPath);
        
        while (pending.Count > 0)
        {
            string currentPath = pending.Dequeue();
            
            string[] files = Array.Empty<string>();
            try
            {
                files = Directory.GetFiles(currentPath, searchPattern);
            }
            catch (UnauthorizedAccessException) { }
            catch (DirectoryNotFoundException) { }
            catch (PathTooLongException) { }

            foreach (var file in files)
            {
                yield return file;
            }

            string[] dirs = Array.Empty<string>();
            try
            {
                dirs = Directory.GetDirectories(currentPath);
            }
            catch (UnauthorizedAccessException) { }
            catch (DirectoryNotFoundException) { }
            catch (PathTooLongException) { }

            foreach (var dir in dirs)
            {
                pending.Enqueue(dir);
            }
        }
    }

    /// <summary>
    /// Checks if a path (file or directory) is accessible by the current user.
    /// </summary>
    public static bool IsPathAccessible(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                using (File.OpenRead(path)) { }
                return true;
            }
            if (Directory.Exists(path))
            {
                Directory.GetDirectories(path);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets DriveInfo for the given path.
    /// </summary>
    public static DriveInfo? GetDriveInfo(string path)
    {
        try
        {
            string root = Path.GetPathRoot(path) ?? string.Empty;
            if (string.IsNullOrEmpty(root)) return null;
            
            return new DriveInfo(root);
        }
        catch
        {
            return null;
        }
    }
}
