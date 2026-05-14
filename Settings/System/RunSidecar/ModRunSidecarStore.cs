using System.Text.Json;
using Godot;
using STS2RitsuLib.Utils;
using STS2RitsuLib.Utils.Persistence;
using STS2RitsuLib.Utils.Persistence.Context;
using FileAccess = Godot.FileAccess;

namespace STS2RitsuLib.Settings.RunSidecar
{
    /// <summary>
    ///     Reads and writes per-mod run sidecar JSON under the framework mod’s profile-scoped storage tree (see
    ///     Reads 和 writes per-mod 跑局 sidecar JSON under the framework mod’s 档案-scoped storage tree (see
    ///     <see cref="ProfileManager" /> base paths for <see cref="SaveScope.Profile" /> and <see cref="Const.ModId" />;
    ///     Godot <c>user://</c>, resolve with <see cref="ProjectSettings.GlobalizePath(string)" />). Client-local
    ///     Godot <c>使用r://</c>, 解析 带有 <c>Project设置.Globalize路径(string)</c>). Client-local
    ///     only; not part of vanilla save sync.
    ///     only; not part of 原版 保存 sync.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Layout under the framework profile base (<see cref="Const.ModId" />):
    ///         Layout under the framework 档案 base (<c>Const.ModId</c>):
    ///         <c>{ProfileBase}/{RunSidecarSegment}/{fingerprintStem}/{sanitizedModId}.json</c>.
    ///         <c>RunSidecarSegment</c> is <c>run_sidecar/v1</c>; <c>fingerprintStem</c> is the lowercase hex SHA-256 from
    ///         <see cref="ModRunSidecarFingerprint" /> (one folder per run). Each consumer mod owns one JSON file in
    ///         that folder; the folder is removed when the run ends or when vanilla deletes the current run save file
    ///         that folder; the folder is removed 当 the 跑局 ends 或 当 原版 deletes the current 跑局 保存 file
    ///         (see <see cref="ModRunSidecarSession" />). Writes
    ///         (see <c>ModRunSidecarSession</c>). Writes
    ///         use atomic replace via <see cref="FileOperations.WriteText" />;
    ///         使用 atomic replace via <c>FileOperations.WriteText</c>;
    ///         if a write fails before any durable <c>*.json</c> exists, the empty run folder is removed best-effort.
    ///         if a write fails 之前 any durable <c>*.json</c> exists, the empty 跑局 folder is removed best-effort.
    ///     </para>
    /// </remarks>
    public static class ModRunSidecarStore
    {
        private const int EnvelopeVersion = 1;
        private const string RunSidecarSegment = "run_sidecar/v1";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };

        private static readonly Lock FileLock = new();

        /// <summary>
        ///     When a run is in progress, sets <paramref name="runDirectoryUserPath" /> to the per-run sidecar folder
        ///     当 a 跑局 is in progress, 设置 <c>RunDirectoryUserPath</c> to the per-跑局 sidecar folder
        ///     (<c>…/run_sidecar/v1/{fingerprintStem}/</c>) so callers can zip or back up every mod file for that run in
        ///     (<c>…/跑局_sidecar/v1/{fingerprintStem}/</c>) so callers can zip 或 back up every mod file 用于 that 跑局 in
        ///     one step. That directory is deleted when the run ends; copy it first if you need to keep it.
        ///     one step. That directory is deleted 当 the 跑局 ends; copy it first 如果 you need to keep it.
        /// </summary>
        /// <param name="runDirectoryUserPath">
        ///     Godot <c>user://</c> path ending at the run directory, or empty when false.
        ///     Godot <c>使用r://</c> 路径 ending at the 跑局 directory, 或 empty 当 false.
        /// </param>
        /// <returns>
        ///     False when no active run fingerprint is available.
        ///     False 当 no active 跑局 fingerprint is 可用.
        /// </returns>
        public static bool TryGetRunSidecarPackDirectoryUserPath(out string runDirectoryUserPath)
        {
            runDirectoryUserPath = string.Empty;
            if (!ModRunSidecarFingerprint.TryGetLive(out var live))
                return false;

            runDirectoryUserPath = ResolveRunDirectoryUserPath(live);
            return true;
        }

        internal static void AppendActiveRunSidecarSyncRelativePaths(int profileId, HashSet<string> sink)
        {
            if (!ModRunSidecarFingerprint.TryGetLive(out var fp) || fp.ProfileId != profileId)
                return;
            if (!TryGetRunSidecarPackDirectoryUserPath(out var runDirUser))
                return;

            var abs = ProjectSettings.GlobalizePath(runDirUser);
            if (!DirAccess.DirExistsAbsolute(abs))
                return;

            string[] names;
            using (var dir = DirAccess.Open(abs))
            {
                if (dir == null)
                    return;
                names = dir.GetFiles();
            }

            foreach (var name in names)
            {
                if (name is "." or "..")
                    continue;
                if (!name.EndsWith(".json", StringComparison.Ordinal))
                    continue;

                var fullUser = $"{runDirUser.TrimEnd('/')}/{name}";
                if (ModAccountRelativePath.TryGetRelativeAccountPath(fullUser, out var rel))
                    sink.Add(rel);
            }
        }

        internal static bool IsActiveRunSidecarRelativeAccountPath(string relativeAccountPath, int activeProfileId)
        {
            if (!ModRunSidecarFingerprint.TryGetLive(out var fp) || fp.ProfileId != activeProfileId)
                return false;
            if (!TryGetRunSidecarPackDirectoryUserPath(out var runDirUser))
                return false;
            if (!ModAccountRelativePath.TryGetRelativeAccountPath(runDirUser.TrimEnd('/'), out var runDirRel))
                return false;

            var prefix = runDirRel + "/";
            if (!relativeAccountPath.StartsWith(prefix, StringComparison.Ordinal) ||
                relativeAccountPath.Length <= prefix.Length)
                return false;

            var remainder = relativeAccountPath[prefix.Length..];
            return remainder.Length > 0 && !remainder.Contains('/') &&
                   remainder.EndsWith(".json", StringComparison.Ordinal);
        }

        /// <summary>
        ///     Attempts to read the sidecar for <paramref name="modId" /> for the active run.
        ///     Attempts to read the sidecar 用于 <c>modId</c> 用于 the active 跑局.
        /// </summary>
        /// <typeparam name="TModel">
        ///     Settings DTO type; must deserialize from JSON with a parameterless constructor.
        ///     设置 DTO type; must deserialize 从 JSON 带有 a parameterless constructor.
        /// </typeparam>
        /// <param name="modId">
        ///     Owning mod id; used in the file path after sanitization.
        ///     Owning mod id; used in the file 路径 之后 sanitization.
        /// </param>
        /// <param name="model">
        ///     On success, the deserialized settings; otherwise a new instance or default.
        ///     On success, the deserialized 设置; otherwise a new instance 或 default.
        /// </param>
        /// <returns>
        ///     Status describing why read succeeded or failed.
        ///     Status describing why read succeeded 或 failed.
        /// </returns>
        public static ModRunSidecarReadStatus TryReadModel<TModel>(string modId, out TModel model)
            where TModel : class, new()
        {
            model = new();
            if (!ModRunSidecarFingerprint.TryGetLive(out var live))
                return ModRunSidecarReadStatus.NoActiveRun;

            lock (FileLock)
            {
                var path = ResolvePath(modId, live);
                if (!FileAccess.FileExists(path))
                    return ModRunSidecarReadStatus.MissingFile;

                try
                {
                    var read = FileOperations.ReadText(path, "RunSidecar");
                    if (!read.Success || read.Content == null)
                        return ModRunSidecarReadStatus.InvalidJson;

                    var envelope = JsonSerializer.Deserialize<ModRunSidecarEnvelope<TModel>>(read.Content, JsonOptions);
                    if (envelope?.Fingerprint == null || envelope.EnvelopeVersion != EnvelopeVersion)
                        return ModRunSidecarReadStatus.InvalidJson;

                    var stored = ModRunSidecarFingerprint.FromDto(envelope.Fingerprint);
                    if (!stored.EqualsFully(live))
                        return ModRunSidecarReadStatus.FingerprintMismatch;

                    model = envelope.Settings ?? new TModel();
                    return ModRunSidecarReadStatus.Ok;
                }
                catch
                {
                    return ModRunSidecarReadStatus.InvalidJson;
                }
            }
        }

        /// <summary>
        ///     Writes <paramref name="model" /> for the active run after validating the live fingerprint.
        ///     写入 <c>model</c> for the active run after validating the live fingerprint。
        /// </summary>
        /// <typeparam name="TModel">
        ///     Settings DTO type serialized into the envelope.
        ///     设置 DTO type serialized into the envelope.
        /// </typeparam>
        /// <param name="modId">
        ///     Owning mod id; used in the file path after sanitization.
        ///     Owning mod id; used in the file 路径 之后 sanitization.
        /// </param>
        /// <param name="model">
        ///     Settings payload stored under the current run fingerprint.
        ///     设置 payload stored under the current 跑局 fingerprint.
        /// </param>
        /// <returns>
        ///     True when the file was written; false when no active run or I/O failed.
        ///     当 the file was written; false when no active run or I/O failed 时为 true。
        /// </returns>
        public static bool TryWriteModel<TModel>(string modId, TModel model) where TModel : class, new()
        {
            if (!ModRunSidecarFingerprint.TryGetLive(out var live))
                return false;

            lock (FileLock)
            {
                var path = ResolvePath(modId, live);
                var runDir = ResolveRunDirectoryUserPath(live);

                var envelope = new ModRunSidecarEnvelope<TModel>
                {
                    EnvelopeVersion = EnvelopeVersion,
                    Fingerprint = live.ToDto(),
                    Settings = model,
                };

                var json = JsonSerializer.Serialize(envelope, JsonOptions);
                var write = FileOperations.WriteText(path, json, "RunSidecar");
                if (write.Success)
                {
                    ModDataCloudMirror.MirrorLocalFileAfterWriteIfEnabled(path);
                    return true;
                }

                TryRemoveRunDirectoryIfWithoutSidecarJson(runDir);
                return false;
            }
        }

        /// <summary>
        ///     Deletes the on-disk per-run sidecar directory for <paramref name="fingerprint" /> (best-effort).
        ///     Deletes the on-disk per-跑局 sidecar directory 用于 <c>fingerprint</c> (best-effort).
        /// </summary>
        /// <remarks>
        ///     Called when a run ends so local sidecar data does not outlive the run instance. Uses the same path
        ///     Called 当 a 跑局 ends so local sidecar data does not outlive the 跑局 instance. 使用 the same 路径
        ///     layout as reads/writes for that fingerprint.
        ///     layout as reads/writes 用于 that fingerprint.
        /// </remarks>
        internal static void TryDeleteRunDirectoryForFingerprint(ModRunSidecarFingerprint fingerprint)
        {
            lock (FileLock)
            {
                var runDir = ResolveRunDirectoryUserPath(fingerprint);
                var abs = ProjectSettings.GlobalizePath(runDir);
                if (DirAccess.DirExistsAbsolute(abs))
                    _ = FileOperations.DeleteDirectoryRecursive(abs, "RunSidecarRunEnded");
            }
        }

        /// <summary>
        ///     Deletes all sidecar files for a profile directory (used when a profile is deleted).
        ///     Deletes all sidecar files 用于 a 档案 directory (used 当 a 档案 is deleted).
        /// </summary>
        internal static void TryDeleteAllForProfile(int profileId)
        {
            if (profileId < 0)
                return;

            lock (FileLock)
            {
                var dir = $"{ProfileManager.GetBasePath(SaveScope.Profile, profileId)}/{RunSidecarSegment}";
                var abs = ProjectSettings.GlobalizePath(dir);
                if (DirAccess.DirExistsAbsolute(abs))
                    FileOperations.DeleteDirectoryRecursive(abs, "RunSidecar");
            }
        }

        private static string ResolveRunDirectoryUserPath(ModRunSidecarFingerprint live)
        {
            var stem = live.ComputeFileStem();
            if (string.IsNullOrEmpty(stem))
                throw new InvalidOperationException("Run sidecar fingerprint stem must not be empty.");

            return StoragePathResolver.ResolveBasePathUser(Const.ModId, SaveScope.RunSidecar,
                StorageContext.Empty
                    .With(StorageContextKeys.ProfileId, live.ProfileId)
                    .With(StorageContextKeys.RunFingerprintStem, stem));
        }

        private static string ResolvePath(string modId, ModRunSidecarFingerprint live)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            var safeMod = SanitizePathSegment(modId);
            var runDir = ResolveRunDirectoryUserPath(live);
            return $"{runDir}/{safeMod}.json";
        }

        /// <summary>
        ///     Removes the per-run folder when it contains no committed <c>*.json</c> sidecar (e.g. only <c>.tmp</c> /
        ///     Removes the per-跑局 folder 当 it 包含 no committed <c>*.json</c> sidecar (e.g. only <c>.tmp</c> /
        ///     <c>.backup</c> left after a failed atomic write).
        /// </summary>
        private static void TryRemoveRunDirectoryIfWithoutSidecarJson(string runDirectoryUserPath)
        {
            try
            {
                var abs = ProjectSettings.GlobalizePath(runDirectoryUserPath);
                if (!DirAccess.DirExistsAbsolute(abs))
                    return;

                string[] names;
                using (var dir = DirAccess.Open(abs))
                {
                    if (dir == null)
                        return;
                    names = dir.GetFiles();
                }

                if (names.Where(name => name is not ("." or "..")).Any(name =>
                        name.EndsWith(".json", StringComparison.Ordinal) &&
                        !name.EndsWith(".tmp", StringComparison.Ordinal)))
                    return;

                _ = FileOperations.DeleteDirectoryRecursive(abs, "RunSidecarPrune");
            }
            catch
            {
                // best-effort: avoid leaving empty or junk-only run folders
            }
        }

        private static string SanitizePathSegment(string modId)
        {
            var chars = modId
                .Select(ch => char.IsLetterOrDigit(ch) || ch is '_' or '-' ? ch : '_')
                .ToArray();
            var s = new string(chars).Trim('_');
            return string.IsNullOrEmpty(s) ? "mod" : s;
        }
    }
}
