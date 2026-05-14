using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Content;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.Lifecycle.Patches
{
    /// <summary>
    ///     After the map UI finishes applying <see cref="NMapScreen.SetMap" />, optionally bumps
    ///     <see cref="MegaCrit.Sts2.Core.Multiplayer.Game.MapSelectionSynchronizer.MapGenerationCount" /> when act-enter logic
    ///     replaced the <see cref="ActModel" />, so votes and relic-driven layout changes stay
    ///     consistent (avoids patching <see cref="RunManager.SetActInternal" />, whose Harmony postfix can run before
    ///     <see cref="RunManager.GenerateMap" /> completes).
    ///     在地图 UI 完成应用 <c>NMapScreen.SetMap</c> 后，如果进入章节逻辑替换了 <c>ActModel</c>，
    ///     则可选择递增 <c>MegaCrit.Sts2.Core.Multiplayer.Game.MapSelectionSynchronizer.MapGenerationCount</c>，
    ///     让投票和遗物驱动的布局变更保持一致（避免补丁 <c>RunManager.SetActInternal</c>，其 Harmony postfix
    ///     可能早于 <c>RunManager.GenerateMap</c> 完成）。
    /// </summary>
    public sealed class ActEnterMapSelectionSyncPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "act_enter_map_selection_sync";

        /// <inheritdoc />
        public static string Description =>
            "After NMapScreen.SetMap, call MapSelectionSynchronizer.BeforeMapGenerated when EnterAct replaced the act model";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(NMapScreen), nameof(NMapScreen.SetMap), [typeof(ActMap), typeof(uint), typeof(bool)]),
            ];
        }

        /// <summary>
        ///     Harmony postfix: synchronizer bump after the visible map matches <see cref="RunManager.GenerateMap" /> output.
        ///     Harmony postfix：在可见地图与 <c>RunManager.GenerateMap</c> 输出一致后递增同步器。
        /// </summary>
        public static void Postfix()
        {
            if (!ModContentRegistry.TryConsumeActEnterPostMapUiMapSyncBump())
                return;

            RunManager.Instance?.MapSelectionSynchronizer?.BeforeMapGenerated();
        }
    }
}
