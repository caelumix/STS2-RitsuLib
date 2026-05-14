using MegaCrit.Sts2.Core.Models;

namespace STS2RitsuLib.Scaffolding.Cards.HandGlow
{
    /// <summary>
    ///     Declarative hand-highlight rules that mirror vanilla <see cref="CardModel.ShouldGlowGold" /> (gold border when a
    ///     bonus / stronger line is active while the card is playable) and <see cref="CardModel.ShouldGlowRed" /> (red border
    ///     for warning states such as companion missing). Applied either by overriding the protected
    ///     <c>ShouldGlow*Internal</c>
    ///     members, or by registering with <see cref="ModCardHandGlowRegistry" /> /
    ///     <see cref="STS2RitsuLib.Content.ModContentRegistry" /> (see
    ///     <c>RegisterCardHandGlow&lt;TCard&gt;</c> on the content registry).
    ///     声明式手牌高亮规则，镜像原版 <c>CardModel.ShouldGlowGold</c>（卡牌可打出且奖励 /
    ///     强化文本行激活时的金色边框）和 <c>CardModel.ShouldGlowRed</c>（同伴缺失等警告状态的红色边框）。
    ///     可通过重写 protected <c>ShouldGlow*Internal</c> 成员应用，也可通过
    ///     <see cref="ModCardHandGlowRegistry" /> / <see cref="STS2RitsuLib.Content.ModContentRegistry" />
    ///     注册（见 content registry 上的 <c>RegisterCardHandGlow&lt;TCard&gt;</c>）。
    /// </summary>
    public readonly record struct ModCardHandGlowRules
    {
        /// <summary>
        ///     When this returns true for a card instance, the hand UI may show the gold highlight (same channel as
        ///     <see cref="CardModel.ShouldGlowGoldInternal" />).
        ///     对卡牌实例返回 true 时，手牌 UI 可显示金色高亮（与 <c>CardModel.ShouldGlowGoldInternal</c> 同通道）。
        /// </summary>
        public Func<CardModel, bool>? GoldWhenBonusActive { get; init; }

        /// <summary>
        ///     When this returns true, the hand UI may show the red highlight (same channel as
        ///     <see cref="CardModel.ShouldGlowRedInternal" />).
        ///     返回 true 时，手牌 UI 可显示红色高亮（与 <c>CardModel.ShouldGlowRedInternal</c> 同通道）。
        /// </summary>
        public Func<CardModel, bool>? RedWhenHandWarning { get; init; }

        /// <summary>
        ///     Gold only (e.g. Evil Eye–style “stronger effect active”).
        ///     仅金色（例如 Evil Eye 风格的“更强效果已激活”）。
        /// </summary>
        public static ModCardHandGlowRules Gold(Func<CardModel, bool> whenBonusActive)
        {
            return new() { GoldWhenBonusActive = whenBonusActive };
        }

        /// <summary>
        ///     Red only (e.g. Osty-missing attack cards).
        ///     仅红色（例如 Osty 缺失攻击牌）。
        /// </summary>
        public static ModCardHandGlowRules Red(Func<CardModel, bool> whenHandWarning)
        {
            return new() { RedWhenHandWarning = whenHandWarning };
        }

        /// <summary>
        ///     Both channels in one rule set.
        ///     在一个规则集中同时包含两个通道。
        /// </summary>
        public static ModCardHandGlowRules GoldAndRed(
            Func<CardModel, bool>? goldWhenBonusActive,
            Func<CardModel, bool>? redWhenHandWarning)
        {
            return new()
            {
                GoldWhenBonusActive = goldWhenBonusActive,
                RedWhenHandWarning = redWhenHandWarning,
            };
        }

        /// <summary>
        ///     Merges with <paramref name="other" /> by OR-ing each channel (useful when multiple mods register the same
        ///     card type, or you split rules across calls).
        ///     通过对每个通道做 OR 与 <c>other</c> 合并（适用于多个 mod 注册同一卡牌类型，或将规则拆到多次调用中）。
        /// </summary>
        public ModCardHandGlowRules Or(ModCardHandGlowRules other)
        {
            return new()
            {
                GoldWhenBonusActive = CombineOr(GoldWhenBonusActive, other.GoldWhenBonusActive),
                RedWhenHandWarning = CombineOr(RedWhenHandWarning, other.RedWhenHandWarning),
            };
        }

        private static Func<CardModel, bool>? CombineOr(Func<CardModel, bool>? a, Func<CardModel, bool>? b)
        {
            if (a == null)
                return b;
            return b == null ? a : c => a(c) || b(c);
        }
    }
}
