namespace STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels
{
    /// <summary>
    ///     Implemented by <see cref="MegaCrit.Sts2.Core.Models.PowerModel" /> subclasses to render additional
    ///     Implemented 通过 <c>MegaCrit.Sts2.Core.Models.PowerModel</c> subclasses to render additional
    ///     numeric/text badges on <see cref="MegaCrit.Sts2.Core.Nodes.Combat.NPower" /> (separate from the vanilla
    ///     numeric/text badges on <c>MegaCrit.Sts2.Core.Nodes.Combat.NPower</c> (separate 从 the 原版
    ///     counter label).
    ///     中文说明：counter label).
    /// </summary>
    public interface IPowerExtraIconAmountLabelsProvider
    {
        /// <summary>
        ///     Each entry with non-whitespace <see cref="ExtraIconAmountLabelSlot.Text" /> becomes one badge at
        ///     Each entry 带有 non-whitespace <c>ExtraIconAmountLabelSlot.Text</c> becomes one badge at
        ///     <see cref="ExtraIconAmountLabelSlot.Corner" /> (or <see cref="ExtraIconAmountLabelCorner.Custom" /> bounds).
        ///     Order only affects z-order (later draws on top).
        ///     中文说明：Order only affects z-order (later draws on top).
        /// </summary>
        IReadOnlyList<ExtraIconAmountLabelSlot> GetPowerExtraIconAmountLabelSlots();
    }

    /// <summary>
    ///     Optional invalidation signal when only <see cref="IPowerExtraIconAmountLabelsProvider" /> slots change
    ///     可选 invalidation signal 当 only <c>IPowerExtraIconAmountLabelsProvider</c> slots change
    ///     without <see cref="MegaCrit.Sts2.Core.Models.PowerModel.DisplayAmountChanged" /> firing.
    ///     带有out <c>MegaCrit.Sts2.Core.Models.PowerModel.DisplayAmountChanged</c> firing.
    /// </summary>
    public interface IPowerExtraIconAmountLabelsChangeSource
    {
        /// <summary>
        ///     Raised when <see cref="IPowerExtraIconAmountLabelsProvider.GetPowerExtraIconAmountLabelSlots" /> may
        ///     Raised 当 <c>IPowerExtraIconAmountLabelsProvider.GetPowerExtraIconAmountLabelSlots</c> may
        ///     have changed.
        ///     中文说明：have changed.
        /// </summary>
        event Action? PowerExtraIconAmountLabelsInvalidated;
    }
}
