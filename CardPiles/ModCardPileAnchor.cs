using Godot;

namespace STS2RitsuLib.CardPiles
{
    /// <summary>
    ///     Location hint for a mod card pile's UI node or fly-in target. Explicit anchors take precedence over
    ///     style defaults; when no anchor is provided, ritsulib auto-stacks same-style piles in registration
    ///     order ("explicit anchor + auto-stack fallback").
    ///     mod card pile 的 UI 节点或 fly-in target 的位置提示。显式 anchor 优先于 style 默认值；
    ///     未提供 anchor 时，ritsulib 会按注册顺序自动堆叠同样式 pile
    ///     （“显式 anchor + auto-stack fallback”）。
    /// </summary>
    public enum ModCardPileAnchorKind
    {
        /// <summary>
        ///     Let the style's default slot decide; multiple entries auto-stack along the style axis.
        ///     由样式的默认 slot 决定；多个 entry 会沿样式轴自动堆叠。
        /// </summary>
        StyleDefault = 0,

        /// <summary>
        ///     Near the bottom-left draw pile button (auto-stacks rightwards toward the discard row).
        ///     靠近左下 draw pile 按钮（向右朝 discard 行自动堆叠）。
        /// </summary>
        BottomLeftPrimary = 1,

        /// <summary>
        ///     Near the bottom-left discard button (auto-stacks rightwards on overflow).
        ///     靠近左下 discard 按钮（溢出时向右自动堆叠）。
        /// </summary>
        BottomLeftSecondary = 2,

        /// <summary>
        ///     Near the bottom-right exhaust button (auto-stacks leftwards on overflow).
        ///     靠近右下 exhaust 按钮（溢出时向左自动堆叠）。
        /// </summary>
        BottomRightPrimary = 3,

        /// <summary>
        ///     Reserved for a future second bottom-right slot; stacks left of the primary.
        ///     为未来第二个右下 slot 保留；堆叠在 primary 左侧。
        /// </summary>
        BottomRightSecondary = 4,

        /// <summary>
        ///     Slot in the top bar immediately after the vanilla deck button.
        ///     top bar 中紧跟原版 deck 按钮之后的 slot。
        /// </summary>
        TopBarAfterDeck = 5,

        /// <summary>
        ///     Slot in the top bar before the right-most modifier cluster.
        ///     top bar 中位于最右侧 modifier cluster 之前的 slot。
        /// </summary>
        TopBarBeforeModifiers = 6,

        /// <summary>
        ///     Centered above the vanilla hand (used by <see cref="ModCardPileUiStyle.ExtraHand" />).
        ///     位于原版 hand 上方居中（由 <c>ModCardPileUiStyle.ExtraHand</c> 使用）。
        /// </summary>
        ExtraHandAbove = 7,

        /// <summary>
        ///     Centered below the vanilla hand (used by <see cref="ModCardPileUiStyle.ExtraHand" />).
        ///     位于原版 hand 下方居中（由 <c>ModCardPileUiStyle.ExtraHand</c> 使用）。
        /// </summary>
        ExtraHandBelow = 8,

        /// <summary>
        ///     User-specified mount position; pairing of <see cref="ModCardPileAnchor.CustomPosition" /> /
        ///     <see cref="ModCardPileAnchor.CustomAuthoringPivot" /> is described under <see cref="ModCardPileAnchor" />.
        ///     用户指定的 mount 位置；<c>ModCardPileAnchor.CustomPosition</c> /
        ///     <c>ModCardPileAnchor.CustomAuthoringPivot</c> 的配对规则见 <c>ModCardPileAnchor</c>。
        /// </summary>
        Custom = 9,
    }

    /// <summary>
    ///     UI anchoring descriptor paired with <see cref="ModCardPileUiStyle" />. Combines a discrete slot kind
    ///     with an optional pixel offset (and an authoring point for <see cref="ModCardPileAnchorKind.Custom" />).
    ///     Preserved construction shapes: primary (<see cref="Kind" />, <see cref="Offset" />, optional custom
    ///     fields); two-argument (<c>kind</c>, <c>offset</c>); three-argument custom (<c>kind</c>,
    ///     <c>offset</c>, <c>customPosition</c>); pivot as either <see cref="Vector2" /> or separate floats.
    ///     与 <c>ModCardPileUiStyle</c> 配对的 UI anchoring 描述。组合离散 slot kind 与可选像素 offset
    ///     （以及 <c>ModCardPileAnchorKind.Custom</c> 使用的 authoring point）。保留的构造形状包括：
    ///     primary（<c>Kind</c>、<c>Offset</c>、可选 custom 字段）；
    ///     双参数（<c>kind</c>、<c>offset</c>）；三参数 custom（<c>kind</c>、<c>offset</c>、
    ///     <c>customPosition</c>）；pivot 可用 <c>Vector2</c> 或拆分 float 表示。
    /// </summary>
    /// <param name="Kind">
    ///     Discrete slot the pile wants to attach to.
    ///     pile 想附着到的离散 slot。
    /// </param>
    /// <param name="Offset">
    ///     Additional pixel offset in the mount parent's local space, applied together with resolving
    ///     <paramref name="CustomPosition" /> for <see cref="ModCardPileAnchorKind.Custom" />.
    ///     mount parent 本地空间中的额外像素 offset；对 <c>ModCardPileAnchorKind.Custom</c> 会与解析后的
    ///     <c>CustomPosition</c> 一起应用。
    /// </param>
    /// <param name="CustomPosition">
    ///     Point in the mount parent's local space lying on nominal pile chrome after pivot resolution when
    ///     <paramref name="Kind" /> is <see cref="ModCardPileAnchorKind.Custom" />; ignored otherwise.
    ///     当 <c>Kind</c> 为 <c>ModCardPileAnchorKind.Custom</c> 时，pivot 解析后位于名义
    ///     pile chrome 上、以 mount parent 本地空间表示的点；其它情况忽略。
    /// </param>
    /// <param name="CustomAuthoringPivot">
    ///     For <see cref="ModCardPileAnchorKind.Custom" />: component-wise fractions (typically 0..1) mapping
    ///     <paramref name="CustomPosition" /> to a landmark on nominal chrome —
    ///     <c>(0,0)</c> top-left, <c>(0.5,0.5)</c> center, <c>(1,1)</c> bottom-right. Injected upper-left corner is
    ///     <c>CustomPosition + Offset − nominalChromeSize * CustomAuthoringPivot</c>; ignored unless
    ///     <paramref name="Kind" /> is <see cref="ModCardPileAnchorKind.Custom" />.
    ///     对 <c>ModCardPileAnchorKind.Custom</c>：逐分量比例（通常每轴 0..1），用于将
    ///     <c>CustomPosition</c> 映射到名义 chrome 上的 landmark：
    ///     <c>(0,0)</c> 左上，<c>(0.5,0.5)</c> 中心，<c>(1,1)</c> 右下。注入的左上角为
    ///     <c>CustomPosition + Offset − nominalChromeSize * CustomAuthoringPivot</c>；除非
    ///     <c>Kind</c> 为 <c>ModCardPileAnchorKind.Custom</c>，否则忽略。
    /// </param>
    public readonly record struct ModCardPileAnchor(
        ModCardPileAnchorKind Kind,
        Vector2 Offset = default,
        Vector2 CustomPosition = default,
        Vector2 CustomAuthoringPivot = default)
    {
        /// <summary>
        ///     Historical two-argument anchor shape preserved for call sites (
        ///     <c>
        ///         <see cref="Offset" />
        ///     </c>
        ///     plus
        ///     default custom fields).
        ///     为调用点保留的历史双参数 anchor 形状（
        ///     <c>
        ///         <see cref="Offset" />
        ///     </c>
        ///     加默认 custom 字段）。
        /// </summary>
        public ModCardPileAnchor(ModCardPileAnchorKind kind, Vector2 offset)
            : this(kind, offset, default, default)
        {
        }

        /// <summary>
        ///     Three-argument custom anchor shape preserving <c>kind + offset + customPosition</c> with pivot
        ///     defaulting to <see cref="PivotUpperLeft" />.
        ///     三参数 custom anchor 形状，保留 <c>kind + offset + customPosition</c>，pivot 默认
        ///     <see cref="PivotUpperLeft" />。
        /// </summary>
        public ModCardPileAnchor(ModCardPileAnchorKind kind, Vector2 offset, Vector2 customPosition)
            : this(kind, offset, customPosition, default)
        {
        }

        /// <summary>
        ///     Custom anchor with authoring pivot expressed as scalar fractions (typically 0..1 per axis).
        ///     使用标量比例表示 authoring pivot 的 custom anchor（通常每轴 0..1）。
        /// </summary>
        public ModCardPileAnchor(
            ModCardPileAnchorKind kind,
            Vector2 offset,
            Vector2 customPosition,
            float customAuthoringPivotX,
            float customAuthoringPivotY)
            : this(kind, offset, customPosition, new(customAuthoringPivotX, customAuthoringPivotY))
        {
        }

        /// <summary>
        ///     Pivot that places <see cref="CustomPosition" /> on nominal chrome upper-left (<c>(0,0)</c>,
        ///     default <see cref="ModCardPileAnchorKind.Custom" /> behaviour).
        ///     将 <c>CustomPosition</c> 放到名义 chrome 左上角的 pivot（<c>(0,0)</c>，
        ///     默认 <c>ModCardPileAnchorKind.Custom</c> 行为）。
        /// </summary>
        public static Vector2 PivotUpperLeft => Vector2.Zero;

        /// <summary>
        ///     Pivot that places <see cref="CustomPosition" /> on nominal chrome geometric center.
        ///     将 <c>CustomPosition</c> 放到名义 chrome 几何中心的 pivot。
        /// </summary>
        public static Vector2 PivotCenter => Vector2.One * 0.5f;

        /// <summary>
        ///     Convenience anchor that falls back to the style's default slot.
        ///     回退到样式默认 slot 的便捷 anchor。
        /// </summary>
        public static ModCardPileAnchor Default { get; } = new(ModCardPileAnchorKind.StyleDefault);

        /// <summary>
        ///     Builds a <see cref="ModCardPileAnchorKind.Custom" /> anchor at authored chrome upper-left
        ///     <paramref name="upperLeftPosition" /> (<see cref="PivotUpperLeft" /> semantics).
        ///     在 authored chrome 左上角 <c>upperLeftPosition</c> 构建
        ///     <c>ModCardPileAnchorKind.Custom</c> anchor（<c>PivotUpperLeft</c> 语义）。
        /// </summary>
        public static ModCardPileAnchor AtPosition(Vector2 upperLeftPosition)
        {
            return new(ModCardPileAnchorKind.Custom, Vector2.Zero, upperLeftPosition);
        }

        /// <summary>
        ///     Builds a <see cref="ModCardPileAnchorKind.Custom" /> anchor at <paramref name="authoringPoint" />
        ///     interpreted as landmark <paramref name="chromePivotFraction" /> on nominal chrome (<c>X,Y</c> typically
        ///     between 0 and 1 inclusive).
        ///     在 <c>authoringPoint</c> 构建 <c>ModCardPileAnchorKind.Custom</c> anchor，
        ///     并将其解释为名义 chrome 上的 landmark <c>chromePivotFraction</c>
        ///     （<c>X,Y</c> 通常位于 0 到 1 之间，含端点）。
        /// </summary>
        public static ModCardPileAnchor AtPivot(Vector2 authoringPoint, Vector2 chromePivotFraction)
        {
            return new(ModCardPileAnchorKind.Custom, Vector2.Zero, authoringPoint, chromePivotFraction);
        }

        /// <summary>
        ///     Builds a <see cref="ModCardPileAnchorKind.Custom" /> anchor placing nominal chrome geometric center at
        ///     <paramref name="centerPosition" /> (<see cref="PivotCenter" /> semantics).
        ///     构建 <c>ModCardPileAnchorKind.Custom</c> anchor，将名义 chrome 的几何中心放在
        ///     <c>centerPosition</c>（<c>PivotCenter</c> 语义）。
        /// </summary>
        public static ModCardPileAnchor AtCenter(Vector2 centerPosition)
        {
            return new(ModCardPileAnchorKind.Custom, Vector2.Zero, centerPosition, PivotCenter);
        }
    }
}
