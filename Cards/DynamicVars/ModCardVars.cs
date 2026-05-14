using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace STS2RitsuLib.Cards.DynamicVars
{
    /// <summary>
    ///     Factory helpers for common mod card <see cref="DynamicVar" /> shapes.
    ///     Factory helpers 用于 common mod 卡牌 <c>DynamicVar</c> shapes.
    /// </summary>
    public static class ModCardVars
    {
        /// <summary>
        ///     Creates an integer-backed dynamic var named <paramref name="name" /> with amount
        ///     创建 an integer-backed dynamic var named <c>name</c> 带有 amount
        ///     <paramref name="amount" />.
        /// </summary>
        public static IntVar Int(string name, decimal amount)
        {
            return new(name, amount);
        }

        /// <summary>
        ///     Creates a string dynamic var named <paramref name="name" />.
        ///     创建 a string dynamic var named <c>name</c>。
        /// </summary>
        public static StringVar String(string name, string value = "")
        {
            return new(name, value);
        }

        /// <summary>
        ///     Creates a <see cref="ComputedDynamicVar" /> with optional preview-specific computation.
        ///     创建 a <c>ComputedDynamicVar</c> with optional preview-specific computation。
        /// </summary>
        public static ComputedDynamicVar Computed(
            string name,
            decimal baseValue,
            Func<CardModel?, decimal> currentValueFactory,
            Func<CardModel?, CardPreviewMode, Creature?, bool, decimal>? previewValueFactory = null)
        {
            return new(name, baseValue, currentValueFactory, previewValueFactory);
        }

        /// <summary>
        ///     Creates a <see cref="ComputedDynamicVar" /> with target-aware computation.
        ///     创建 a <c>ComputedDynamicVar</c> with target-aware computation。
        /// </summary>
        public static ComputedDynamicVar Computed(
            string name,
            decimal baseValue,
            Func<CardModel?, Creature?, decimal> currentValueFactory,
            Func<CardModel?, CardPreviewMode, Creature?, bool, decimal>? previewValueFactory = null)
        {
            return new(name, baseValue, currentValueFactory, previewValueFactory);
        }
    }
}
