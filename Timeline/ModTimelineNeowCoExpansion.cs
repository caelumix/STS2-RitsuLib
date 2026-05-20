using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2RitsuLib.Timeline
{
    internal static class ModTimelineNeowCoExpansion
    {
        private static int _neowQueueUnlocksDepth;

        private static int _pendingNeowAnimatedSlotMerge;

        private static bool IsInsideNeowQueueUnlocks => Volatile.Read(ref _neowQueueUnlocksDepth) > 0;

        internal static void EnterNeowQueueUnlocks()
        {
            Interlocked.Increment(ref _neowQueueUnlocksDepth);
        }

        internal static void ExitNeowQueueUnlocks()
        {
            Interlocked.Decrement(ref _neowQueueUnlocksDepth);
        }

        internal static bool TryConsumePendingNeowAnimatedSlotMerge()
        {
            return Interlocked.Exchange(ref _pendingNeowAnimatedSlotMerge, 0) != 0;
        }

        /// <summary>
        ///     True once vanilla Neow&apos;s primary expansion has written at least one of its slots into progress (first slot is
        ///     <see cref="Colorless1Epoch" />). Avoids painting every mod story column on cold open before that event.
        ///     当原版 Neow&apos;s 主扩展已将至少一个槽位写入进度后为 true（第一个槽位是 <see cref="Colorless1Epoch" />）。避免在该事件前冷打开时绘制每个 mod story 列。
        /// </summary>
        internal static bool HasVanillaNeowTimelineExpansionStarted(ProgressState? progress)
        {
            return progress != null && progress.HasEpoch(EpochModel.GetId<Colorless1Epoch>());
        }

        internal static bool IsNeowPrimaryTimelineExpansion(EpochModel[] epochs)
        {
            if (epochs is not { Length: >= 8 })
                return false;

            var ids = epochs.Select(e => e.Id).ToHashSet();
            return ids.Contains(EpochModel.GetId<Colorless1Epoch>())
                   && ids.Contains(EpochModel.GetId<Silent1Epoch>());
        }

        internal static bool IsNeowPrimaryTimelineExpansionSlots(IReadOnlyList<EpochSlotData> slots)
        {
            if (slots is not { Count: >= 8 })
                return false;

            var ids = slots.Select(s => s.Model.Id).ToHashSet();
            return ids.Contains(EpochModel.GetId<Colorless1Epoch>())
                   && ids.Contains(EpochModel.GetId<Silent1Epoch>());
        }

        internal static void MergeModEpochTemplateSlotsInto(List<EpochSlotData> slotsToAdd, ProgressState? progress)
        {
            RemoveUnattachedUnobtainedModEpochTemplateSlots(slotsToAdd, progress);

            var existing = new HashSet<string>(slotsToAdd.Count);
            foreach (var s in slotsToAdd)
                existing.Add(s.Model.Id);

            foreach (var id in EpochModel.AllEpochIds)
            {
                if (existing.Contains(id))
                    continue;

                EpochModel model;
                try
                {
                    model = EpochModel.Get(id);
                }
                catch
                {
                    continue;
                }

                if (model is not ModEpochTemplate)
                    continue;

                var state = ResolveMergedModSlotState(id, progress);
                if (state == EpochSlotState.NotObtained && !ShouldShowUnobtainedModSlot(id, progress))
                    continue;
                slotsToAdd.Add(new(model, state));
                existing.Add(id);
            }

            SortEpochSlotsByEraThenPosition(slotsToAdd);
        }

        /// <summary>
        ///     Called from <see cref="EpochModel.QueueTimelineExpansion" /> postfix only; runs mod
        ///     <see cref="SaveManager.UnlockSlot" /> for already-obtained mod epochs when the expansion was triggered from
        ///     <see cref="NeowEpoch.QueueUnlocks" />.
        ///     仅从 <see cref="EpochModel.QueueTimelineExpansion" /> postfix 调用；当扩展由 <see cref="NeowEpoch.QueueUnlocks" />
        ///     触发时为已获得的 mod 纪元运行
        ///     <see cref="SaveManager.UnlockSlot" />。
        /// </summary>
        internal static void OnQueueTimelineExpansionPostfix(EpochModel[] vanillaEpochs)
        {
            if (!IsInsideNeowQueueUnlocks)
                return;
            if (!IsNeowPrimaryTimelineExpansion(vanillaEpochs))
                return;

            UnlockModEpochSlotsCore(vanillaEpochs);
            Interlocked.Exchange(ref _pendingNeowAnimatedSlotMerge, 1);
        }

        private static void UnlockModEpochSlotsCore(EpochModel[] vanillaEpochs)
        {
            var progress = SaveManager.Instance.Progress;
            var vanillaIds = vanillaEpochs.Select(e => e.Id).ToHashSet();
            foreach (var id in EpochModel.AllEpochIds)
            {
                if (vanillaIds.Contains(id))
                    continue;

                EpochModel model;
                try
                {
                    model = EpochModel.Get(id);
                }
                catch
                {
                    continue;
                }

                if (model is not ModEpochTemplate)
                    continue;

                var row = progress.Epochs.FirstOrDefault(e => e.Id == id);
                if (row?.State != EpochState.ObtainedNoSlot && (row != null || !IsModTimelineRootSlot(id)))
                    continue;

                SaveManager.Instance.UnlockSlot(id);
            }
        }

        private static void RemoveUnattachedUnobtainedModEpochTemplateSlots(List<EpochSlotData> slotsToAdd,
            ProgressState? progress)
        {
            for (var i = slotsToAdd.Count - 1; i >= 0; i--)
            {
                var slot = slotsToAdd[i];
                if (slot.Model is not ModEpochTemplate)
                    continue;

                if (ResolveMergedModSlotState(slot.Model.Id, progress) != EpochSlotState.NotObtained)
                    continue;

                if (ShouldShowUnobtainedModSlot(slot.Model.Id, progress))
                    continue;

                slotsToAdd.RemoveAt(i);
            }
        }

        private static bool ShouldShowUnobtainedModSlot(string id, ProgressState? progress)
        {
            return IsModTimelineRootSlot(id) || IsParentVisibleUnobtainedModSlot(id, progress);
        }

        private static bool IsModTimelineRootSlot(string id)
        {
            foreach (var parentId in EpochModel.AllEpochIds)
            {
                if (parentId == id)
                    continue;

                EpochModel parent;
                try
                {
                    parent = EpochModel.Get(parentId);
                }
                catch
                {
                    continue;
                }

                if (parent.GetTimelineExpansion().Any(child => child.Id == id))
                    return false;
            }

            return true;
        }

        private static bool IsParentVisibleUnobtainedModSlot(string id, ProgressState? progress)
        {
            if (progress == null)
                return false;

            var row = progress.Epochs.FirstOrDefault(e => e.Id == id);
            if (row?.State != EpochState.NotObtained)
                return false;

            foreach (var epoch in progress.Epochs)
            {
                if (epoch.State < EpochState.Revealed)
                    continue;

                EpochModel parent;
                try
                {
                    parent = EpochModel.Get(epoch.Id);
                }
                catch
                {
                    continue;
                }

                if (parent.GetTimelineExpansion().Any(child => child.Id == id))
                    return true;
            }

            return false;
        }

        private static EpochSlotState ResolveMergedModSlotState(string id, ProgressState? progress)
        {
            if (progress == null || !progress.HasEpoch(id))
                return EpochSlotState.NotObtained;

            var row = progress.Epochs.FirstOrDefault(e => e.Id == id);
            if (row == null)
                return EpochSlotState.NotObtained;

            return row.State switch
            {
                EpochState.Revealed => EpochSlotState.Complete,
                EpochState.Obtained => EpochSlotState.Obtained,
                EpochState.ObtainedNoSlot => EpochSlotState.Obtained,
                _ => EpochSlotState.NotObtained,
            };
        }

        private static void SortEpochSlotsByEraThenPosition(List<EpochSlotData> slots)
        {
            slots.Sort((a, b) =>
            {
                var c = a.Era.CompareTo(b.Era);
                return c != 0 ? c : a.EraPosition.CompareTo(b.EraPosition);
            });
        }
    }
}
