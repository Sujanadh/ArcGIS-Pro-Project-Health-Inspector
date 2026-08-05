using System;
using System.IO;

namespace APHI.Utilities;

/// <summary>
/// Provides static utility methods for analyzing and classifying file system paths
/// commonly encountered in ArcGIS Pro projects.
/// </summary>
public static class PathHelper
{
    /// <summary>
    /// Classifies the type of a file system path.
    /// </summary>
    public enum PathType
    {
        /// <summary>A local fixed drive path.</summary>
        Local,
        /// <summary>A UNC (Universal Naming Convention) network path.</summary>
        UNC,
        /// <summary>A path on an external or removable drive.</summary>
        External,
        /// <summary>A relative path.</summary>
        Relative,
        /// <summary>Path type could not be determined.</summary>
        Unknown
    }

    /// <summary>
    /// Determines whether the specified path is a UNC (Universal Naming Convention) path.
    /// </summary>
    /// <param name="path">The path to evaluate.</param>
    /// <returns><c>true</c> if the path is a UNC path; otherwise, <c>false</c>.</returns>
    public static bool IsUncPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        try
        {
            var uri = new Uri(path);
            return uri.IsUnc;
        }
        catch
        {
            return path.StartsWith(@"\\");
        }
    }

    /// <summary>
    /// Determines whether the specified path points to a network location,
    /// including both UNC paths and mapped network drives.
    /// </summary>
    /// <param name="path">The path to evaluate.</param>
    /// <returns><c>true</c> if the path is on a network; otherwise, <c>false</c>.</returns>
    public static bool IsNetworkPath(string path)
    {
        if (IsUncPath(path)) return true;

        try
        {
            string root = Path.GetPathRoot(path) ?? string.Empty;
            if (string.IsNullOrEmpty(root)) return false;

            var drive = new DriveInfo(root);
            return drive.DriveType == DriveType.Network;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether the specified path resides on an external or removable drive.
    /// </summary>
    /// <param name="path">The path to evaluate.</param>
    /// <returns><c>true</c> if the path is on a removable drive; otherwise, <c>false</c>.</returns>
    public static bool IsExternalDrive(string path)
    {
        try
        {
            string root = Path.GetPathRoot(path) ?? string.Empty;
            if (string.IsNullOrEmpty(root)) return false;

            var drive = new DriveInfo(root);
            return drive.DriveType == DriveType.Removable;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether the specified path is a relative path.
    /// </summary>
    /// <param name="path">The path to evaluate.</param>
    /// <returns><c>true</c> if the path is relative; otherwise, <c>false</c>.</returns>
    public static bool IsRelativePath(string path)
    {
        return !string.IsNullOrWhiteSpace(path) && !Path.IsPathRooted(path);
    }

    /// <summary>
    /// Normalizes a path by resolving relative segments and standardizing separators.
    /// </summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>The normalized absolute path, or the original path if normalization fails.</returns>
    public static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return string.Empty;
        try
        {
            return Path.GetFullPath(path);
        }
        catch
        {
            return path;
        }
    }

    /// <summary>
    /// Classifies a path into one of the defined <see cref="PathType"/> categories.
    /// </summary>
    /// <param name="path">The path to classify.</param>
    /// <returns>The <see cref="PathType"/> of the given path.</returns>
    public static PathType GetPathType(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return PathType.Unknown;
        if (IsRelativePath(path)) return PathType.Relative;
        if (IsUncPath(path)) return PathType.UNC;
        if (IsNetworkPath(path)) return PathType.UNC;
        if (IsExternalDrive(path)) return PathType.External;
        return PathType.Local;
    }

    /// <summary>
    /// Checks whether a file exists at the specified path.
    /// </summary>
    /// <param name="path">The file path to check.</param>
    /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
    public static bool FileExists(string path)
    {
        return File.Exists(path);
    }

    /// <summary>
    /// Checks whether a directory exists at the specified path.
    /// </summary>
    /// <param name="path">The directory path to check.</param>
    /// <returns><c>true</c> if the directory exists; otherwise, <c>false</c>.</returns>
    public static bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    /// <summary>
    /// Determines whether the specified path represents a File Geodatabase (.gdb) or
    /// an Enterprise Geodatabase connection (.sde).
    /// </summary>
    /// <param name="path">The path to evaluate.</param>
    /// <returns><c>true</c> if the path is a geodatabase; otherwise, <c>false</c>.</returns>
    public static bool IsGeodatabase(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        return path.EndsWith(".gdb", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".sde", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the specified path represents a Shapefile (.shp).
    /// </summary>
    /// <param name="path">The path to evaluate.</param>
    /// <returns><c>true</c> if the path is a shapefile; otherwise, <c>false</c>.</returns>
    public static bool IsShapefile(string path)
    {
        return path?.EndsWith(".shp", StringComparison.OrdinalIgnoreCase) == true;
    }

    /// <summary>
    /// Determines whether the specified path represents a raster dataset.
    /// Supports common raster formats: TIFF, JPEG, PNG, ERDAS Imagine, MrSID,
    /// ECW, GRID, JP2, and BMP.
    /// </summary>
    /// <param name="path">The path to evaluate.</param>
    /// <returns><c>true</c> if the path is a raster file; otherwise, <c>false</c>.</returns>
    public static bool IsRaster(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        string[] rasterExtensions = { ".tif", ".tiff", ".jpg", ".jpeg", ".png", ".img",
                                       ".sid", ".ecw", ".adf", ".jp2", ".bmp" };
        foreach (var ext in rasterExtensions)
        {
            if (path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Determines whether the specified path represents a CAD drawing file.
    /// Supports AutoCAD DWG, DXF, and MicroStation DGN formats.
    /// </summary>
    /// <param name="path">The path to evaluate.</param>
    /// <returns><c>true</c> if the path is a CAD file; otherwise, <c>false</c>.</returns>
    public static bool IsCAD(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        return path.EndsWith(".dwg", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".dxf", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".dgn", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the specified path represents a Microsoft Excel file.
    /// </summary>
    /// <param name="path">The path to evaluate.</param>
    /// <returns><c>true</c> if the path is an Excel file; otherwise, <c>false</c>.</returns>
    public static bool IsExcel(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        return path.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the specified path represents a CSV (Comma-Separated Values) file.
    /// </summary>
    /// <param name="path">The path to evaluate.</param>
    /// <returns><c>true</c> if the path is a CSV file; otherwise, <c>false</c>.</returns>
    public static bool IsCsv(string path)
    {
        return path?.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) == true;
    }

    /// <summary>
    /// Gets the size of a file in megabytes.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>The file size in megabytes, or 0 if the file does not exist.</returns>
    public static double GetFileSizeMB(string path)
    {
        if (FileExists(path))
        {
            var info = new FileInfo(path);
            return info.Length / 1024.0 / 1024.0;
        }
        return 0;
    }
}
