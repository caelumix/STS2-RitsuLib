using Godot;
using MegaCrit.Sts2.Core.Models;

namespace STS2RitsuLib.Scaffolding.Cards.HandOutline
{
    /// <summary>
    ///     Custom hand-card outline tint (drives <see cref="MegaCrit.Sts2.Core.Nodes.Cards.NCardHighlight" />
    ///     <c>Modulate</c> after vanilla playable / gold / red). Register with
    ///     <see cref="ModCardHandOutlineRegistry" /> or <c>ModContentRegistry.RegisterCardHandOutline&lt;TCard&gt;()</c>.
    ///     自定义手牌描边染色（在原版 playable / gold / red 之后驱动
    ///     <see cref="MegaCrit.Sts2.Core.Nodes.Cards.NCardHighlight" /> 的 <c>Modulate</c>）。请通过
    ///     <see cref="ModCardHandOutlineRegistry" /> 或
    ///     <c>ModContentRegistry.RegisterCardHandOutline&lt;TCard&gt;()</c> 注册。
    /// </summary>
    /// <param name="When">
    ///     When this returns true for the card instance, the outline color may apply.
    ///     对卡牌实例返回 true 时，描边颜色可生效。
    /// </param>
    /// <param name="Color">
    ///     Godot modulate color (alpha is respected; vanilla highlights use ~0.98).
    ///     Godot modulate 颜色（alpha 会生效；原版高亮约使用 0.98）。
    /// </param>
    /// <param name="Priority">
    ///     When several rules match, the highest <paramref name="Priority" /> wins; ties favor the most recently registered
    ///     rule.
    ///     多条规则匹配时，最高 <paramref name="Priority" /> 胜出；相同优先级时，最近注册的规则胜出。
    /// </param>
    /// <param name="VisibleWhenUnplayable">
    ///     If true, the highlight is forced visible with this color even when the card is not playable and vanilla would not
    ///     show gold/red (still only while combat is in progress).
    ///     若为 true，即使卡牌不可打出且原版不会显示 gold/red，也会强制以此颜色显示高亮（仍仅在战斗进行中）。
    /// </param>
    public readonly record struct ModCardHandOutlineRule(
        Func<CardModel, bool> When,
        Color Color,
        int Priority = 0,
        bool VisibleWhenUnplayable = false)
    {
        /// <summary>
        ///     Optional dynamic color resolver. When assigned and <see cref="When" /> passes, this is evaluated each refresh
        ///     to produce the current hand outline color.
        ///     可选动态颜色解析器。设置后且 <see cref="When" /> 通过时，每次刷新都会评估它以生成当前手牌描边颜色。
        /// </summary>
        public Func<CardModel, Color>? DynamicColor { get; init; }

        /// <summary>
        ///     Creates a rule with a dynamic color resolver.
        ///     创建带动态颜色解析器的规则。
        /// </summary>
        public static ModCardHandOutlineRule Dynamic(
            Func<CardModel, bool> when,
            Func<CardModel, Color> colorWhen,
            int priority = 0,
            bool visibleWhenUnplayable = false)
        {
            ArgumentNullException.ThrowIfNull(when);
            ArgumentNullException.ThrowIfNull(colorWhen);
            return new(when, Colors.White, priority, visibleWhenUnplayable)
            {
                DynamicColor = colorWhen,
            };
        }

        internal Color ResolveColor(CardModel card)
        {
            return DynamicColor?.Invoke(card) ?? Color;
        }
    }
}
