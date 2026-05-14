using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Managers;
using MegaCrit.Sts2.Core.Timeline;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2RitsuLib.Unlocks.Patches
{
    /// <summary>
    ///     Vanilla <see cref="ProgressSaveManager.GetRevealableEpochs" /> only marks epochs reachable from Neow via
    ///     <see cref="EpochModel.GetTimelineExpansion" /> BFS. Mod story roots are not in that graph, so obtained mod slots
    ///     never count for <see cref="SaveManager.GetDiscoveredEpochCount" />, main-menu timeline cues, or related UX even
    ///     when the UI slot is already in the obtained (click-to-reveal) state.
    ///     原版 <c>ProgressSaveManager.GetRevealableEpochs</c> 只会标记可从 Neow 通过
    ///     <c>EpochModel.GetTimelineExpansion</c> BFS 到达的 epoch。mod 故事根不在该图中，因此已取得的 mod 槽位即使
    ///     UI 已处于取得（点击揭示）状态，也不会计入 <c>SaveManager.GetDiscoveredEpochCount</c>、主菜单时间线提示或相关体验。
    /// </summary>
    public sealed class ProgressSaveManagerGetRevealableEpochsModTemplatePatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "progress_save_manager_get_revealable_epochs_mod_template";

        /// <inheritdoc />
        public static string Description =>
            "Union ModEpochTemplate rows in Obtained/ObtainedNoSlot into GetRevealableEpochs when vanilla omits them";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ProgressSaveManager), nameof(ProgressSaveManager.GetRevealableEpochs), Type.EmptyTypes)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Appends mod template epochs that satisfy vanilla's satisfied-state filter but were dropped for lack of
        ///     reachability from Neow.
        ///     追加满足原版已满足状态过滤、但因无法从 Neow 到达而被丢弃的 mod 模板 epoch。
        /// </summary>
        public static IEnumerable<SerializableEpoch> Postfix(
                IEnumerable<SerializableEpoch> __result,
                ProgressSaveManager __instance)
            // ReSharper restore InconsistentNaming
        {
            var list = __result.ToList();
            var seen = new HashSet<string>(list.Select(e => e.Id));

            foreach (var epoch in __instance.Progress.Epochs)
            {
                var st = epoch.State;
                if (st != EpochState.Obtained && st != EpochState.ObtainedNoSlot)
                    continue;
                if (!seen.Add(epoch.Id))
                    continue;

                EpochModel model;
                try
                {
                    model = EpochModel.Get(epoch.Id);
                }
                catch
                {
                    seen.Remove(epoch.Id);
                    continue;
                }

                if (model is not ModEpochTemplate)
                {
                    seen.Remove(epoch.Id);
                    continue;
                }

                list.Add(epoch);
            }

            return list;
        }
    }
}
