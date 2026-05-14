using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline.UnlockScreens;
using MegaCrit.Sts2.Core.Timeline;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2RitsuLib.Timeline.Patches
{
    /// <summary>
    ///     Before the timeline queues expansion slots, align <see cref="EpochModel.AllEpochIds" /> with
    ///     之前 the timeline queues expansion slots, align <c>Epoch模型.AllEpochIds</c> 带有
    ///     <c>EpochModel._epochTypeDictionary</c>. Otherwise <see cref="MegaCrit.Sts2.Core.Saves.ProgressState" />
    ///     <c>FilterAndSortEpochs</c> may strip mod expansion ids (IsValid false) immediately after
    ///     <see cref="MegaCrit.Sts2.Core.Saves.SaveManager.UnlockSlot" />, so the live expansion UI breaks while a cold
    ///     reload still shows slots once the cache matches the dictionary.
    ///     re加载 still shows slots once the cache matches the dictionary.
    /// </summary>
    public class QueueTimelineExpansionSyncEpochIdListPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "queue_timeline_expansion_sync_epoch_id_list";

        /// <inheritdoc />
        public static string Description =>
            "Sync EpochModel.AllEpochIds with the epoch type dictionary before QueueTimelineExpansion runs UnlockSlot";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(EpochModel), nameof(EpochModel.QueueTimelineExpansion), [typeof(EpochModel[])]),
            ];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Ensures <see cref="EpochModel.IsValid" /> sees every id present in <see cref="EpochModel.Get" />.
        ///     Ensures <c>Epoch模型.IsValid</c> sees every id present in <c>Epoch模型.Get</c>.
        /// </summary>
        public static void Prefix(EpochModel[] epochs)
        {
            ArgumentNullException.ThrowIfNull(epochs);
            ModTimelineRegistry.EnsureAllEpochIdsSyncedWithDictionary();
        }
    }

    /// <summary>
    ///     Vanilla <see cref="NUnlockTimelineScreen.SetUnlocks" /> sorts only by <see cref="EpochSlotData.EraPosition" />,
    ///     原版 <c>NUnlockTimelineScreen.设置Unlocks</c> sorts only 通过 <c>EpochSlotData.EraPosition</c>,
    ///     which collides across <see cref="EpochEra" /> values — common for mod timelines. Expansion animation then
    ///     which collides across <c>EpochEra</c> values — common 用于 mod timelines. Expansion animation then
    ///     feeds <see cref="MegaCrit.Sts2.Core.Nodes.Screens.Timeline.NTimelineScreen.AddEpochSlots" /> in the wrong era
    ///     中文说明：feeds <c>MegaCrit.Sts2.Core.Nodes.Screens.Timeline.NTimelineScreen.AddEpochSlots</c> in the wrong era
    ///     order.
    ///     中文说明：order.
    /// </summary>
    public class NUnlockTimelineScreenExpansionSlotSortPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "n_unlock_timeline_screen_expansion_slot_sort";

        /// <inheritdoc />
        public static string Description =>
            "Sort timeline expansion slots by Era then EraPosition for mod-compatible column ordering";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(NUnlockTimelineScreen), nameof(NUnlockTimelineScreen.SetUnlocks),
                    [typeof(List<EpochSlotData>)]),
            ];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Replaces the vanilla <c>_erasToUnlock</c> ordering with era-stable sorting.
        ///     Replaces the 原版 <c>_erasToUnlock</c> ordering 带有 era-stable sorting.
        /// </summary>
        public static void Postfix(NUnlockTimelineScreen __instance, List<EpochSlotData> eras)
        {
            ArgumentNullException.ThrowIfNull(eras);
            var field = AccessTools.Field(typeof(NUnlockTimelineScreen), "_erasToUnlock");
            if (field == null)
                return;

            var ordered = eras.OrderBy(a => a.Era).ThenBy(a => a.EraPosition).ToList();
            field.SetValue(__instance, ordered);
        }
    }

    /// <summary>
    ///     When <c>NeowEpoch.QueueUnlocks</c> runs (scoped by <see cref="NeowEpochQueueUnlocksCoExpansionScopePatch" />),
    ///     当 <c>NeowEpoch.QueueUnlocks</c> runs (scoped 通过 <c>NeowEpochQueueUnlocksCoExpansionScopePatch</c>),
    ///     after vanilla <see cref="EpochModel.QueueTimelineExpansion" /> unlocks the twelve base rows, also
    ///     之后 原版 <c>Epoch模型.QueueTimelineExpansion</c> unlocks the twelve base rows, also
    ///     <see cref="MegaCrit.Sts2.Core.Saves.SaveManager.UnlockSlot" /> for every <see cref="ModEpochTemplate" /> not in
    ///     that batch, and signal the
    ///     that batch, 和 signal the
    ///     animated <see cref="MegaCrit.Sts2.Core.Nodes.Screens.Timeline.NTimelineScreen.AddEpochSlots" /> prefix to merge the
    ///     中文说明：animated <c>MegaCrit.Sts2.Core.Nodes.Screens.Timeline.NTimelineScreen.AddEpochSlots</c> prefix to merge the
    ///     same mod slots in-session.
    ///     中文说明：same mod slots in-session.
    /// </summary>
    public sealed class QueueTimelineExpansionUnlockModSlotsAfterNeowPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "queue_timeline_expansion_unlock_mod_slots_after_neow";

        /// <inheritdoc />
        public static string Description =>
            "After Neow primary timeline expansion, UnlockSlot for ModEpochTemplate ids not in the vanilla batch";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(EpochModel), nameof(EpochModel.QueueTimelineExpansion), [typeof(EpochModel[])]),
            ];
        }

        /// <summary>
        ///     Runs after vanilla queues the expansion list and unlocks vanilla slots.
        ///     runs 之后 原版 queues the expansion list 和 unlocks 原版 slots.
        /// </summary>
        public static void Postfix(EpochModel[] epochs)
        {
            ArgumentNullException.ThrowIfNull(epochs);
            ModTimelineNeowCoExpansion.OnQueueTimelineExpansionPostfix(epochs);
        }
    }
}
