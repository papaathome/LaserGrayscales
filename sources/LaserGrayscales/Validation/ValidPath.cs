using System.IO;
using System.Runtime.CompilerServices;

namespace As.Applications.Validation
{
    internal static class ValidPath
    {
        /// <summary>
        /// Check if filename contains no invalid characters and is not empty.
        /// </summary>
        /// <param name="fileName">Name to check</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidateFileName(this string? fileName, [CallerMemberName] string name = "")
            => ValidateFileName(fileName, true, name);

        /// <summary>
        /// Check if filename contains no invalid characters, optional accept empty file names.
        /// </summary>
        /// <param name="fileName">Name to check</param>
        /// <param name="noempty">True: do not allow empty file names; False do allow empty (but no whitespace) names</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidateFileName(this string? fileName, bool noempty, [CallerMemberName] string name = "")
        {
            if (noempty && string.IsNullOrEmpty(fileName)) return "empty file name";

            if (fileName == null) return "";
            return (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
                ? ""
                : $"{name}: file name contains invalid characters";
        }

        /// <summary>
        /// Check if file exists.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidateFileExists(this string? path, [CallerMemberName] string name = "")
            => ValidateFileExists(path, true, name);

        /// <summary>
        /// Check if file exists, optionaly accept empty paths for no file specified.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <param name="noempty">True: do allow empty file paths; False empty file path is not accepted</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidateFileExists(this string? path, bool noempty, [CallerMemberName] string name = "")
        {
            return (!noempty && string.IsNullOrEmpty(path) || (path != null) && File.Exists(path))
                ? ""
                : $"{name}: File must exist";
        }

        /// <summary>
        /// Check ifdirectoryname contains no invalid characters.
        /// </summary>
        /// <param name="directoryName">Name to check</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        /// <remarks>Note: this checks the directory name with have the same rules as a file name</remarks>
        static public string ValidateDirectoryName(this string? directoryName, [CallerMemberName] string name = "")
            => ValidateDirectoryName(directoryName, true, name);

        /// <summary>
        /// Check ifdirectoryname contains no invalid characters, optional accept empty file names.
        /// </summary>
        /// <param name="directoryName">Name to check</param>
        /// <param name="noempty">True: do not allow empty name; False do allow empty (but no whitespace) name</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        /// <remarks>Note: this checks the directory name with have the same rules as a file name</remarks>
        static public string ValidateDirectoryName(this string? directoryName, bool noempty, [CallerMemberName] string name = "")
        {
            if (noempty && string.IsNullOrEmpty(directoryName)) return "empty direcotry name";

            if (directoryName == null) return "";
            return (directoryName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
                ? ""
                : $"{name}: directory name contains invalid characters";
        }

        /// <summary>
        /// Check if directory exists.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidateDirectoryExists(this string? path, [CallerMemberName] string name = "")
            => ValidateDirectoryExists(path, true, name);

        /// <summary>
        /// Check if directory exists, optionaly accept empty paths for no directory specified.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <param name="noempty">True: do allow empty direcotry paths; False empty directory path is not accepted</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidateDirectoryExists(this string? path, bool noempty, [CallerMemberName] string name = "")
        {
            return (!noempty && string.IsNullOrEmpty(path) || (path != null) && Directory.Exists(path))
                ? ""
                : $"{name}: directory must exist";
        }

        /// <summary>
        /// Check if path contains no invalid characters.
        /// </summary>
        /// <param name="path">Name to check</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidatePath(this string? path, [CallerMemberName] string name = "")
            => ValidatePath(path, true, name);

        /// <summary>
        /// Check if path contains no invalid characters, alow empty path.
        /// </summary>
        /// <param name="path">Name to check</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidatePathOrEmpty(this string? path, [CallerMemberName] string name = "")
            => ValidatePath(path, false, name);

        /// <summary>
        /// Check if path contains no invalid characters, optional accept empty path.
        /// </summary>
        /// <param name="path">Name to check</param>
        /// <param name="noempty">True: do not allow empty path; False do allow empty (but no whitespace) path</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidatePath(this string? path, bool noempty, [CallerMemberName] string name = "")
        {
            if (noempty && string.IsNullOrEmpty(path)) return "empty path";
            if (path == null) return "";
            return (path.IndexOfAny(Path.GetInvalidPathChars()) < 0) ? "" : $"{name}: path contains invalid characters";
        }

        /// <summary>
        /// Check if path exists.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidatePathExists(this string? path, [CallerMemberName] string name = "")
            => ValidatePathExists(path, true, name);

        /// <summary>
        /// Check if path exists, optionaly accept empty paths for none specified.
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <param name="noempty">True: do allow empty paths; False empty path is not accepted</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidatePathExists(this string? path, bool noempty, [CallerMemberName] string name = "")
        {
            return (!noempty && string.IsNullOrEmpty(path) || (path != null) && Directory.Exists(path))
                ? ""
                : $"{name}: path must exist";
        }
    }
}
