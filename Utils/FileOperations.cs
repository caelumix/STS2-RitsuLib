using System.Text.Json;
using Godot;
using FileAccess = Godot.FileAccess;

namespace STS2RitsuLib.Utils
{
    /// <summary>
    ///     Unified file operations wrapper for Godot's FileAccess with consistent error handling and logging.
    ///     Unified file operations wrapper 用于 Godot's FileAccess 带有 consistent error handling 和 logging.
    ///     Supports atomic writes with backup rotation (mirrors STS2's GodotFileIo pattern).
    ///     Supports atomic writes 带有 backup rotation (mirrors STS2's GodotFileIo pattern).
    /// </summary>
    public static class FileOperations
    {
        private const string TempSuffix = ".tmp";
        private const string BackupSuffix = ".backup";

        /// <summary>
        ///     Reads text content from a file with detailed error handling.
        ///     Reads text content 从 a file 带有 detailed error handling.
        /// </summary>
        public static ReadResult ReadText(string filePath, string? logContext = null)
        {
            var context = logContext ?? "FileOperations";

            try
            {
                if (!FileAccess.FileExists(filePath))
                {
                    RitsuLibFramework.Logger.Debug($"[{context}] File not found at '{filePath}'");
                    return new()
                    {
                        Success = false,
                        ErrorMessage = "File not found",
                    };
                }

                using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
                if (file == null)
                {
                    var error = FileAccess.GetOpenError();
                    RitsuLibFramework.Logger.Error($"[{context}] Failed to open file '{filePath}' (Error: {error})");
                    return new()
                    {
                        Success = false,
                        ErrorCode = error,
                        ErrorMessage = $"Failed to open file (Error: {error})",
                    };
                }

                var content = file.GetAsText();

                if (string.IsNullOrWhiteSpace(content))
                {
                    RitsuLibFramework.Logger.Warn($"[{context}] File '{filePath}' is empty");
                    return new()
                    {
                        Success = false,
                        Content = content,
                        ErrorMessage = "File is empty",
                    };
                }

                RitsuLibFramework.Logger.Debug(
                    $"[{context}] Successfully read file '{filePath}' ({content.Length} characters)");
                return new()
                {
                    Success = true,
                    Content = content,
                };
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Error($"[{context}] Unexpected error reading file '{filePath}': {ex.Message}");
                return new()
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error: {ex.Message}",
                };
            }
        }

        /// <summary>
        ///     Writes text content to a file with atomic write pattern:
        ///     Writes text content to a file 带有 atomic write pattern:
        ///     1. Rotate existing file to .backup
        ///     中文说明：1. Rotate existing file to .backup
        ///     2. Write to .tmp file
        ///     中文说明：2. Write to .tmp file
        ///     3. Rename .tmp to target path
        ///     3. Rename .tmp to target 路径
        /// </summary>
        public static WriteResult WriteText(string filePath, string content, string? logContext = null,
            bool atomic = true)
        {
            var context = logContext ?? "FileOperations";

            if (!atomic)
                return WriteTextDirect(filePath, content, context);

            try
            {
                EnsureDirectoryExists(filePath);

                var tempPath = filePath + TempSuffix;
                var backupPath = filePath + BackupSuffix;

                RotateBackup(filePath, backupPath, context);

                var writeResult = WriteTextDirect(tempPath, content, context);
                if (!writeResult.Success)
                {
                    RestoreFromBackup(filePath, backupPath, context);
                    return writeResult;
                }

                var renameResult = RenameFile(tempPath, filePath, context);
                if (!renameResult.Success)
                {
                    DeleteFileSilent(tempPath);
                    RestoreFromBackup(filePath, backupPath, context);
                    return new()
                    {
                        Success = false,
                        ErrorMessage = $"Failed to rename temp file: {renameResult.ErrorMessage}",
                    };
                }

                RitsuLibFramework.Logger.Debug($"[{context}] Atomic write completed for '{filePath}'");
                return new() { Success = true };
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Error(
                    $"[{context}] Unexpected error during atomic write to '{filePath}': {ex.Message}");
                return new()
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error: {ex.Message}",
                };
            }
        }

        /// <summary>
        ///     Direct write without atomic pattern (internal use)
        ///     Direct write 带有out atomic pattern (internal 使用)
        /// </summary>
        private static WriteResult WriteTextDirect(string filePath, string content, string context)
        {
            try
            {
                EnsureDirectoryExists(filePath);

                using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
                if (file == null)
                {
                    var error = FileAccess.GetOpenError();
                    RitsuLibFramework.Logger.Error(
                        $"[{context}] Failed to open file '{filePath}' for writing (Error: {error})");
                    return new()
                    {
                        Success = false,
                        ErrorCode = error,
                        ErrorMessage = $"Failed to open file for writing (Error: {error})",
                    };
                }

                file.StoreString(content);
                RitsuLibFramework.Logger.Debug(
                    $"[{context}] Successfully wrote to file '{filePath}' ({content.Length} characters)");
                return new() { Success = true };
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Error(
                    $"[{context}] Unexpected error writing to file '{filePath}': {ex.Message}");
                return new()
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error: {ex.Message}",
                };
            }
        }

        /// <summary>
        ///     Rotate backup: delete old .backup, rename current file to .backup
        ///     中文说明：Rotate backup: delete old .backup, rename current file to .backup
        /// </summary>
        private static void RotateBackup(string filePath, string backupPath, string context)
        {
            try
            {
                if (FileAccess.FileExists(backupPath))
                    DeleteFileSilent(backupPath);

                if (!FileAccess.FileExists(filePath)) return;
                var result = RenameFile(filePath, backupPath, context);
                if (result.Success)
                    RitsuLibFramework.Logger.Debug($"[{context}] Rotated '{filePath}' to backup");
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Warn($"[{context}] Failed to rotate backup: {ex.Message}");
            }
        }

        /// <summary>
        ///     Restore file from backup
        ///     Restore file 从 backup
        /// </summary>
        private static void RestoreFromBackup(string filePath, string backupPath, string context)
        {
            try
            {
                if (!FileAccess.FileExists(backupPath)) return;

                var result = RenameFile(backupPath, filePath, context);
                if (result.Success)
                    RitsuLibFramework.Logger.Info($"[{context}] Restored '{filePath}' from backup");
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Error($"[{context}] Failed to restore from backup: {ex.Message}");
            }
        }

        /// <summary>
        ///     Rename a file
        ///     中文说明：Rename a file
        /// </summary>
        public static WriteResult RenameFile(string fromPath, string toPath, string? logContext = null)
        {
            try
            {
                var dir = GetDirectoryFromPath(fromPath);
                using var dirAccess = DirAccess.Open(dir);

                if (dirAccess == null)
                    return new()
                    {
                        Success = false,
                        ErrorMessage = $"Failed to access directory '{dir}'",
                    };

                var error = dirAccess.Rename(fromPath, toPath);
                if (error != Error.Ok)
                    return new()
                    {
                        Success = false,
                        ErrorCode = error,
                        ErrorMessage = $"Rename failed (Error: {error})",
                    };

                return new() { Success = true };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error: {ex.Message}",
                };
            }
        }

        /// <summary>
        ///     Try to load from backup file if main file fails
        ///     Try to 加载 从 backup file 如果 main file fails
        /// </summary>
        public static ReadResult ReadTextWithBackupFallback(string filePath, string? logContext = null)
        {
            var context = logContext ?? "FileOperations";
            var result = ReadText(filePath, context);

            if (result.Success)
                return result;

            var backupPath = filePath + BackupSuffix;
            if (!FileAccess.FileExists(backupPath))
                return result;

            RitsuLibFramework.Logger.Info($"[{context}] Attempting to load from backup '{backupPath}'");
            var backupResult = ReadText(backupPath, context);

            if (!backupResult.Success) return backupResult;
            backupResult = backupResult with { LoadedFromBackup = true };
            RitsuLibFramework.Logger.Info($"[{context}] Successfully loaded from backup");

            return backupResult;
        }

        private static void DeleteFileSilent(string filePath)
        {
            try
            {
                if (!FileAccess.FileExists(filePath)) return;
                var dir = GetDirectoryFromPath(filePath);
                using var dirAccess = DirAccess.Open(dir);
                dirAccess?.Remove(filePath);
            }
            catch
            {
                // Ignore errors in silent delete
            }
        }

        private static string GetDirectoryFromPath(string filePath)
        {
            var lastSlash = filePath.LastIndexOf('/');
            return lastSlash > 0 ? filePath[..lastSlash] : "user://";
        }

        /// <summary>
        ///     Ensures the directory for a file path exists.
        ///     Ensures the directory 用于 a file 路径 exists.
        /// </summary>
        private static void EnsureDirectoryExists(string filePath)
        {
            var lastSlash = filePath.LastIndexOf('/');
            if (lastSlash <= 0) return;

            var directory = filePath[..lastSlash];
            if (string.IsNullOrEmpty(directory)) return;
            if (DirAccess.DirExistsAbsolute(directory)) return;

            var error = DirAccess.MakeDirRecursiveAbsolute(directory);
            if (error != Error.Ok)
                RitsuLibFramework.Logger.Warn($"Failed to create directory '{directory}' (Error: {error})");
        }

        /// <summary>
        ///     Reads and deserializes JSON content from a file.
        ///     Reads 和 deserializes JSON content 从 a file.
        /// </summary>
        public static JsonResult<T> ReadJson<T>(string filePath, JsonSerializerOptions? options = null,
            string? logContext = null)
        {
            var context = logContext ?? "FileOperations";
            var readResult = ReadText(filePath, context);

            if (!readResult.Success || readResult.Content == null)
                return new()
                {
                    Success = false,
                    ErrorMessage = readResult.ErrorMessage,
                };

            try
            {
                var data = JsonSerializer.Deserialize<T>(readResult.Content, options);

                if (data == null)
                {
                    RitsuLibFramework.Logger.Error(
                        $"[{context}] Deserialization resulted in null object for file '{filePath}'");
                    return new()
                    {
                        Success = false,
                        ErrorMessage = "Deserialization resulted in null object",
                    };
                }

                RitsuLibFramework.Logger.Debug($"[{context}] Successfully deserialized JSON from '{filePath}'");
                return new()
                {
                    Success = true,
                    Data = data,
                };
            }
            catch (JsonException ex)
            {
                RitsuLibFramework.Logger.Error($"[{context}] JSON parsing error in file '{filePath}': {ex.Message}");
                return new()
                {
                    Success = false,
                    ErrorMessage = $"JSON parsing error: {ex.Message}",
                };
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Error(
                    $"[{context}] Unexpected error deserializing file '{filePath}': {ex.Message}");
                return new()
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error: {ex.Message}",
                };
            }
        }

        /// <summary>
        ///     Serializes and writes JSON content to a file.
        ///     Serializes 和 writes JSON content to a file.
        /// </summary>
        public static WriteResult WriteJson<T>(string filePath, T data, JsonSerializerOptions? options = null,
            string? logContext = null)
        {
            var context = logContext ?? "FileOperations";

            try
            {
                var jsonContent = JsonSerializer.Serialize(data, options);
                return WriteText(filePath, jsonContent, context);
            }
            catch (JsonException ex)
            {
                RitsuLibFramework.Logger.Error($"[{context}] JSON serialization error: {ex.Message}");
                return new()
                {
                    Success = false,
                    ErrorMessage = $"JSON serialization error: {ex.Message}",
                };
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Error($"[{context}] Unexpected error serializing data: {ex.Message}");
                return new()
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error: {ex.Message}",
                };
            }
        }

        /// <summary>
        ///     Checks if a file exists.
        ///     Checks 如果 a file exists.
        /// </summary>
        public static bool FileExists(string filePath)
        {
            return FileAccess.FileExists(filePath);
        }

        /// <summary>
        ///     Deletes a file with detailed error handling.
        ///     Deletes a file 带有 detailed error handling.
        /// </summary>
        public static WriteResult DeleteFile(string filePath, string? logContext = null)
        {
            var context = logContext ?? "FileOperations";

            try
            {
                if (!FileAccess.FileExists(filePath))
                {
                    RitsuLibFramework.Logger.Debug($"[{context}] File '{filePath}' does not exist, nothing to delete");
                    return new() { Success = true };
                }

                var pathParts = filePath.Split('/');
                var directory = pathParts.Length > 1 ? string.Join("/", pathParts[..^1]) : "user://";

                var dirAccess = DirAccess.Open(directory);
                if (dirAccess == null)
                {
                    RitsuLibFramework.Logger.Error(
                        $"[{context}] Failed to access directory '{directory}' for file deletion");
                    return new()
                    {
                        Success = false,
                        ErrorMessage = $"Failed to access directory '{directory}'",
                    };
                }

                var error = dirAccess.Remove(filePath);
                if (error != Error.Ok)
                {
                    RitsuLibFramework.Logger.Error($"[{context}] Failed to delete file '{filePath}' (Error: {error})");
                    return new()
                    {
                        Success = false,
                        ErrorCode = error,
                        ErrorMessage = $"Failed to delete file (Error: {error})",
                    };
                }

                RitsuLibFramework.Logger.Info($"[{context}] Successfully deleted file '{filePath}'");
                return new() { Success = true };
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Error(
                    $"[{context}] Unexpected error deleting file '{filePath}': {ex.Message}");
                return new()
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error: {ex.Message}",
                };
            }
        }

        /// <summary>
        ///     Recursively deletes a directory and all its contents.
        ///     Recursively deletes a directory 和 all its contents.
        /// </summary>
        public static WriteResult DeleteDirectoryRecursive(string directoryPath, string? logContext = null)
        {
            var context = logContext ?? "FileOperations";

            try
            {
                if (!DirAccess.DirExistsAbsolute(directoryPath))
                {
                    RitsuLibFramework.Logger.Debug(
                        $"[{context}] Directory '{directoryPath}' does not exist, nothing to delete");
                    return new() { Success = true };
                }

                using var dirAccess = DirAccess.Open(directoryPath);
                if (dirAccess == null)
                {
                    RitsuLibFramework.Logger.Error($"[{context}] Failed to open directory '{directoryPath}'");
                    return new()
                    {
                        Success = false,
                        ErrorMessage = $"Failed to open directory '{directoryPath}'",
                    };
                }

                foreach (var file in dirAccess.GetFiles())
                {
                    var filePath = $"{directoryPath}/{file}";
                    var result = DeleteFile(filePath, context);
                    if (!result.Success)
                        RitsuLibFramework.Logger.Warn(
                            $"[{context}] Failed to delete file '{filePath}': {result.ErrorMessage}");
                }

                foreach (var subDir in dirAccess.GetDirectories())
                {
                    var subDirPath = $"{directoryPath}/{subDir}";
                    DeleteDirectoryRecursive(subDirPath, context);
                }

                var parentPath = GetDirectoryFromPath(directoryPath);
                using var parentAccess = DirAccess.Open(parentPath);
                if (parentAccess != null)
                {
                    var error = parentAccess.Remove(directoryPath);
                    if (error != Error.Ok)
                    {
                        RitsuLibFramework.Logger.Warn(
                            $"[{context}] Failed to remove directory '{directoryPath}' (Error: {error})");
                        return new()
                        {
                            Success = false,
                            ErrorCode = error,
                            ErrorMessage = $"Failed to remove directory (Error: {error})",
                        };
                    }
                }

                RitsuLibFramework.Logger.Info($"[{context}] Successfully deleted directory '{directoryPath}'");
                return new() { Success = true };
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Error(
                    $"[{context}] Unexpected error deleting directory '{directoryPath}': {ex.Message}");
                return new()
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error: {ex.Message}",
                };
            }
        }

        /// <summary>
        ///     Result of a file read operation.
        ///     中文说明：Result of a file read operation.
        /// </summary>
        public record ReadResult
        {
            /// <summary>
            ///     True when the file was read successfully with non-empty content.
            ///     当 the file was read successfully with non-empty content 时为 true。
            /// </summary>
            public bool Success { get; init; }

            /// <summary>
            ///     File text when <see cref="Success" /> is true.
            ///     File text 当 <c>Success</c> is true.
            /// </summary>
            public string? Content { get; init; }

            /// <summary>
            ///     Godot error code when a low-level open/read failure occurred.
            ///     Godot error code 当 a low-level open/read failure occurred.
            /// </summary>
            public Error? ErrorCode { get; init; }

            /// <summary>
            ///     Human-readable failure reason when <see cref="Success" /> is false.
            ///     人类可读的 failure reason when <c>Success</c> is false。
            /// </summary>
            public string? ErrorMessage { get; init; }

            /// <summary>
            ///     True when content was recovered from the <c>.backup</c> sibling file.
            ///     当 content was recovered from the <c>.backup</c> sibling file 时为 true。
            /// </summary>
            public bool LoadedFromBackup { get; init; }
        }

        /// <summary>
        ///     Result of a file write operation.
        ///     中文说明：Result of a file write operation.
        /// </summary>
        public class WriteResult
        {
            /// <summary>
            ///     True when the write (or no-op delete) completed successfully.
            ///     当 the write (or no-op delete) completed successfully 时为 true。
            /// </summary>
            public bool Success { get; init; }

            /// <summary>
            ///     Godot error code when a low-level operation failed.
            ///     Godot error code 当 a low-level operation failed.
            /// </summary>
            public Error? ErrorCode { get; init; }

            /// <summary>
            ///     Human-readable failure reason when <see cref="Success" /> is false.
            ///     人类可读的 failure reason when <c>Success</c> is false。
            /// </summary>
            public string? ErrorMessage { get; init; }
        }

        /// <summary>
        ///     Result of a JSON deserialization operation.
        ///     中文说明：Result of a JSON deserialization operation.
        /// </summary>
        public class JsonResult<T>
        {
            /// <summary>
            ///     True when JSON was parsed into a non-null instance.
            ///     当 JSON was parsed into a non-null instance 时为 true。
            /// </summary>
            public bool Success { get; init; }

            /// <summary>
            ///     Deserialized object when <see cref="Success" /> is true.
            ///     Deserialized object 当 <c>Success</c> is true.
            /// </summary>
            public T? Data { get; init; }

            /// <summary>
            ///     Human-readable failure reason when <see cref="Success" /> is false.
            ///     人类可读的 failure reason when <c>Success</c> is false。
            /// </summary>
            public string? ErrorMessage { get; init; }
        }
    }
}
