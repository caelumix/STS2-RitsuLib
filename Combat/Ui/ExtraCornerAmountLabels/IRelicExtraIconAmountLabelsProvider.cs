namespace STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels
{
    /// <summary>
    ///     Implemented by <see cref="MegaCrit.Sts2.Core.Models.RelicModel" /> subclasses to render additional badges
    ///     Implemented 通过 <c>MegaCrit.Sts2.Core.Models.RelicModel</c> subclasses to render additional badges
    ///     on <see cref="MegaCrit.Sts2.Core.Nodes.Relics.NRelicInventoryHolder" />.
    ///     on <c>MegaCrit.Sts2.Core.Nodes.Relics.NRelicInventoryHolder</c>.
    /// </summary>
    public interface IRelicExtraIconAmountLabelsProvider
    {
        /// <summary>
        ///     Each entry with non-whitespace <see cref="ExtraIconAmountLabelSlot.Text" /> becomes one badge at
        ///     Each entry 带有 non-whitespace <c>ExtraIconAmountLabelSlot.Text</c> becomes one badge at
        ///     <see cref="ExtraIconAmountLabelSlot.Corner" /> (or <see cref="ExtraIconAmountLabelCorner.Custom" /> bounds).
        /// </summary>
        IReadOnlyList<ExtraIconAmountLabelSlot> GetRelicExtraIconAmountLabelSlots();
    }

    /// <summary>
    ///     Optional invalidation when only relic extra slots change without
    ///     可选 invalidation 当 only 遗物 extra slots change 带有out
    ///     <see cref="MegaCrit.Sts2.Core.Models.RelicModel.DisplayAmountChanged" />.
    /// </summary>
    public interface IRelicExtraIconAmountLabelsChangeSource
    {
        /// <summary>
        ///     Raised when <see cref="IRelicExtraIconAmountLabelsProvider.GetRelicExtraIconAmountLabelSlots" /> may
        ///     Raised 当 <c>IRelicExtraIconAmountLabelsProvider.GetRelicExtraIconAmountLabelSlots</c> may
        ///     have changed.
        ///     中文说明：have changed.
        /// </summary>
        event Action? RelicExtraIconAmountLabelsInvalidated;
    }
}
