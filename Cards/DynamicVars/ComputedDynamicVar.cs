using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace STS2RitsuLib.Cards.DynamicVars
{
    /// <summary>
    ///     <see cref="DynamicVar" /> whose displayed value is produced by delegates instead of a fixed base amount.
    ///     显示值由委托生成、而不是使用固定基础数值的 <c>DynamicVar</c>。
    /// </summary>
    public sealed class ComputedDynamicVar : DynamicVar
    {
        private readonly Func<CardModel?, Creature?, decimal> _currentValueFactory;
        private readonly Func<CardModel?, CardPreviewMode, Creature?, bool, decimal>? _previewValueFactory;

        /// <summary>
        ///     Creates a computed variable with optional preview-specific logic.
        ///     创建 a computed variable with optional preview-specific logic。
        /// </summary>
        /// <param name="name">
        ///     Dynamic var key.
        ///     中文说明：Dynamic var key.
        /// </param>
        /// <param name="baseValue">
        ///     Fallback numeric base when no preview override applies.
        ///     Fallback numeric base 当 no preview override applies.
        /// </param>
        /// <param name="currentValueFactory">
        ///     Resolves the live value from the owning <see cref="CardModel" /> (may be null outside card context).
        ///     解析 the live value from the owning <c>CardModel</c> (may be null outside card context)。
        /// </param>
        /// <param name="previewValueFactory">
        ///     Optional override used during card preview; when null, <paramref name="currentValueFactory" /> is used.
        ///     可选 override used 期间 卡牌 preview; 当 null, <c>currentValueFactory</c> is used.
        /// </param>
        public ComputedDynamicVar(
            string name,
            decimal baseValue,
            Func<CardModel?, decimal> currentValueFactory,
            Func<CardModel?, CardPreviewMode, Creature?, bool, decimal>? previewValueFactory = null)
            : base(name, baseValue)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(currentValueFactory);

            _currentValueFactory = (card, _) => currentValueFactory(card);
            _previewValueFactory = previewValueFactory;
        }

        /// <summary>
        ///     Creates a computed variable with target-aware evaluation.
        ///     创建 a computed variable with target-aware evaluation。
        /// </summary>
        /// <param name="name">
        ///     Dynamic var key.
        ///     中文说明：Dynamic var key.
        /// </param>
        /// <param name="baseValue">
        ///     Fallback numeric base when no preview override applies.
        ///     Fallback numeric base 当 no preview override applies.
        /// </param>
        /// <param name="currentValueFactory">
        ///     Resolves the live value from the owning <see cref="CardModel" /> and current target.
        ///     解析 the live value from the owning <c>CardModel</c> and current target。
        /// </param>
        /// <param name="previewValueFactory">
        ///     Optional override used during card preview; when null, <paramref name="currentValueFactory" /> is used.
        ///     可选 override used 期间 卡牌 preview; 当 null, <c>currentValueFactory</c> is used.
        /// </param>
        public ComputedDynamicVar(
            string name,
            decimal baseValue,
            Func<CardModel?, Creature?, decimal> currentValueFactory,
            Func<CardModel?, CardPreviewMode, Creature?, bool, decimal>? previewValueFactory = null)
            : base(name, baseValue)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(currentValueFactory);

            _currentValueFactory = currentValueFactory;
            _previewValueFactory = previewValueFactory;
        }

        /// <summary>
        ///     Computes the dynamic value for the current owner and target.
        ///     Computes the dynamic value 用于 the current owner 和 target.
        /// </summary>
        public decimal Calculate(Creature? target)
        {
            return _currentValueFactory(_owner as CardModel, target);
        }

        /// <summary>
        ///     Computes the dynamic value for the current owner.
        ///     Computes the dynamic value 用于 the current owner.
        /// </summary>
        public decimal Calculate()
        {
            return Calculate(null);
        }

        /// <inheritdoc />
        public override void UpdateCardPreview(
            CardModel card,
            CardPreviewMode previewMode,
            Creature? target,
            bool runGlobalHooks)
        {
            PreviewValue = _previewValueFactory?.Invoke(card, previewMode, target, runGlobalHooks)
                           ?? _currentValueFactory(card, target);
        }

        /// <inheritdoc />
        protected override decimal GetBaseValueForIConvertible()
        {
            return Calculate(null);
        }

        /// <summary>
        ///     Returns the computed value as a string.
        ///     返回 the computed value as a string。
        /// </summary>
        public override string ToString()
        {
            return Calculate(null).ToString();
        }
    }
}
