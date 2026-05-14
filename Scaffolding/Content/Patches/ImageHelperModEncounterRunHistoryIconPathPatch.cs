using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Utils;

namespace STS2RitsuLib.Scaffolding.Content.Patches
{
    /// <summary>
    ///     Run history and top bar call <see cref="ImageHelper.GetRoomIconPath" /> with a <see cref="ModelId" /> that may be
    ///     跑局 history 和 top bar call <c>ImageHelper.GetRoom图标路径</c> 带有 a <c>ModelId</c> that may be
    ///     an
    ///     中文说明：an
    ///     encounter, ancient, event, etc. Only when the id resolves to an <see cref="EncounterModel" /> with mod overrides do
    ///     encounter, ancient, 事件, etc. Only 当 the id 解析 to an <c>EncounterModel</c> 带有 mod overrides do
    ///     we
    ///     中文说明：we
    ///     remap paths (otherwise vanilla resolution runs). Mod encounters without this would hit missing
    ///     remap 路径 (otherwise 原版 resolution runs). Mod encounters 带有out this would hit missing
    ///     <c>ui/run_history/&lt;mod_entry&gt;.png</c>. This prefix returns
    ///     <see cref="IModEncounterAssetOverrides.CustomRunHistoryIconPath" /> /
    ///     <see cref="IModEncounterAssetOverrides.CustomRunHistoryIconOutlinePath" />
    ///     when those paths exist (same pattern as <see cref="ImageHelperAncientModRunHistoryIconPathPatch" /> for ancients).
    ///     当 those 路径 exist (same pattern as <c>ImageHelperAncientModRunHistoryIcon路径Patch</c> 用于 ancients).
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public sealed class ImageHelperModEncounterRunHistoryIconPathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "image_helper_mod_encounter_run_history_icon_path";

        /// <inheritdoc />
        public static string Description =>
            "Route encounter run-history icon paths through IModEncounterAssetOverrides custom texture paths";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(ImageHelper), nameof(ImageHelper.GetRoomIconPath)),
                new(typeof(ImageHelper), nameof(ImageHelper.GetRoomIconOutlinePath)),
            ];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Harmony prefix: return the configured <c>res://images/…</c> path when present on disk / in the resource loader.
        ///     Harmony 前置补丁: 返回 the configured <c>res://images/…</c> 路径 当 present on disk / in the 资源 加载er.
        /// </summary>
        public static bool Prefix(
                MethodBase __originalMethod,
                MapPointType mapPointType,
                RoomType roomType,
                ModelId? modelId,
                ref string? __result)
            // ReSharper restore InconsistentNaming
        {
            if (modelId is null)
                return true;

            if (ModelDb.GetByIdOrNull<AbstractModel>(modelId) is not EncounterModel encounter)
                return true;

            var path = __originalMethod.Name switch
            {
                nameof(ImageHelper.GetRoomIconPath) => ResolveMainIconPath(encounter),
                nameof(ImageHelper.GetRoomIconOutlinePath) => ResolveOutlineIconPath(encounter),
                _ => null,
            };

            var memberLabel = __originalMethod.Name == nameof(ImageHelper.GetRoomIconPath)
                ? nameof(IModEncounterAssetOverrides.CustomRunHistoryIconPath)
                : nameof(IModEncounterAssetOverrides.CustomRunHistoryIconOutlinePath);

            if (string.IsNullOrWhiteSpace(path) ||
                !AssetPathDiagnostics.Exists(path, encounter, memberLabel))
                return true;

            __result = path;
            return false;
        }

        private static string? ResolveMainIconPath(EncounterModel encounter)
        {
            return ExternalAssetOverrideRegistry.TryGetEncounterRunHistoryIconPath(encounter, out var externalPath)
                ? externalPath
                : (encounter as IModEncounterAssetOverrides)?.CustomRunHistoryIconPath;
        }

        private static string? ResolveOutlineIconPath(EncounterModel encounter)
        {
            return ExternalAssetOverrideRegistry.TryGetEncounterRunHistoryIconOutlinePath(encounter,
                out var externalPath)
                ? externalPath
                : (encounter as IModEncounterAssetOverrides)?.CustomRunHistoryIconOutlinePath;
        }
    }
}
