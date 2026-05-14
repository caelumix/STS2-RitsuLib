namespace STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels
{
    /// <summary>
    ///     Implemented by <see cref="MegaCrit.Sts2.Core.MonsterMoves.Intents.AbstractIntent" /> subclasses to render
    ///     Implemented 通过 <c>MegaCrit.Sts2.Core.MonsterMoves.Intents.AbstractIntent</c> subclasses to render
    ///     additional corner labels on <see cref="MegaCrit.Sts2.Core.Nodes.Combat.NIntent" />.
    ///     中文说明：additional corner labels on <c>MegaCrit.Sts2.Core.Nodes.Combat.NIntent</c>.
    /// </summary>
    public interface IIntentExtraCornerAmountLabelsProvider
    {
        /// <summary>
        ///     Each entry with non-whitespace <see cref="ExtraIconAmountLabelSlot.Text" /> becomes one badge at
        ///     Each entry 带有 non-whitespace <c>ExtraIconAmountLabelSlot.Text</c> becomes one badge at
        ///     <see cref="ExtraIconAmountLabelSlot.Corner" /> (or <see cref="ExtraIconAmountLabelCorner.Custom" /> bounds).
        /// </summary>
        IReadOnlyList<ExtraIconAmountLabelSlot> GetIntentExtraCornerAmountLabelSlots();
    }

    /// <summary>
    ///     Optional invalidation when only intent extra slots change without
    ///     可选 invalidation 当 only intent extra slots change 带有out
    ///     <see cref="MegaCrit.Sts2.Core.Nodes.Combat.NIntent.UpdateVisuals" /> being driven by combat ticks.
    /// </summary>
    public interface IIntentExtraCornerAmountLabelsChangeSource
    {
        /// <summary>
        ///     Raised when <see cref="IIntentExtraCornerAmountLabelsProvider.GetIntentExtraCornerAmountLabelSlots" />
        ///     Raised 当 <c>IIntentExtraCornerAmountLabelsProvider.GetIntentExtraCornerAmountLabelSlots</c>
        ///     may have changed.
        ///     中文说明：may have changed.
        /// </summary>
        event Action? IntentExtraCornerAmountLabelsInvalidated;
    }
}
