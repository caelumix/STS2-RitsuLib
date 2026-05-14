using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.Scaffolding.Content.Patches
{
    /// <summary>
    ///     Optional creature visuals scene path (vanilla <c>MonsterModel.VisualsPath</c>); use
    ///     可选 creature visuals 场景 路径 (原版 <c>Monster模型.Visuals路径</c>); 使用
    ///     <see cref="ModMonsterTemplate" /> or implement on a mod <see cref="MonsterModel" />.
    /// </summary>
    public interface IModMonsterAssetOverrides
    {
        /// <summary>
        ///     Path bundle; <c>Custom*</c> properties mirror these fields unless overridden.
        ///     路径包；<c>Custom*</c> properties mirror these fields unless overridden。
        /// </summary>
        MonsterAssetProfile AssetProfile => MonsterAssetProfile.Empty;

        /// <summary>
        ///     Override packed scene path for combat creature visuals.
        ///     combat creature visuals 的 PackedScene 路径覆盖。
        /// </summary>
        string? CustomVisualsPath => AssetProfile.VisualsScenePath;
    }

    /// <summary>
    ///     Patches <see cref="MonsterModel.VisualsPath" /> for <see cref="IModMonsterAssetOverrides" />.
    ///     为 <c>IModMonsterAssetOverrides</c> 补丁 <c>MonsterModel.VisualsPath</c>。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class MonsterVisualsPathPatch : IPatchMethod
    {
        /// <inheritdoc cref="IPatchMethod.PatchId" />
        public static string PatchId => "content_asset_override_monster_visuals_path";

        /// <inheritdoc cref="IPatchMethod.Description" />
        public static string Description => "Allow mod monsters to override VisualsPath";

        /// <inheritdoc cref="IPatchMethod.IsCritical" />
        public static bool IsCritical => false;

        /// <inheritdoc cref="IPatchMethod.GetTargets" />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(MonsterModel), "VisualsPath", MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModMonsterAssetOverrides.CustomVisualsPath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModMonsterAssetOverrides.CustomVisualsPath</c>。
        /// </summary>
        public static bool Prefix(MonsterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return ContentAssetOverridePatchHelper.TryUseStringOverride<IModMonsterAssetOverrides>(
                __instance,
                ref __result,
                o => o.CustomVisualsPath,
                nameof(IModMonsterAssetOverrides.CustomVisualsPath));
        }
    }
}
