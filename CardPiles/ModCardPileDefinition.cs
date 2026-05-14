using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;

namespace STS2RitsuLib.CardPiles
{
    /// <summary>
    ///     Immutable registry entry for a mod card pile. Produced by <see cref="ModCardPileRegistry" /> and keyed
    ///     by both the normalized id and the deterministically minted <see cref="PileType" /> value.
    ///     mod card pile 的不可变 registry entry。由 <c>ModCardPileRegistry</c> 生成，并同时按 normalized id
    ///     与确定性生成的 <c>PileType</c> 值索引。
    /// </summary>
    /// <remarks>
    ///     Localization follows the vanilla pile convention: the hover-tip title / description and
    ///     empty-pile message are always resolved against <see cref="ModCardPileSpec.HoverTipLocTable" />
    ///     (<c>static_hover_tips</c>) using the keys <c>"{Id}.title"</c>, <c>"{Id}.description"</c> and
    ///     <c>"{Id}.empty"</c> (same as the registered pile id).
    ///     本地化遵循原版 pile 约定：hover-tip title / description 与 empty-pile message 始终基于
    ///     <c>ModCardPileSpec.HoverTipLocTable</c>（<c>static_hover_tips</c>）解析，使用
    ///     <c>"{Id}.title"</c>、<c>"{Id}.description"</c> 和 <c>"{Id}.empty"</c> key
    ///     （与已注册 pile id 相同）。
    /// </remarks>
    public sealed record ModCardPileDefinition
    {
        /// <summary>
        ///     Primary constructor used by the registry; all fields are immutable once registered.
        ///     registry 使用的主构造函数；所有字段注册后不可变。
        /// </summary>
        /// <param name="modId">
        ///     Owning mod id (<c>com.example.my-mod</c>).
        ///     所属 mod id（<c>com.example.my-mod</c>）。
        /// </param>
        /// <param name="id">
        ///     Normalized global id (<c>NormalizeId</c> output from <see cref="ModCardPileRegistry" />).
        ///     normalized global id（<c>ModCardPileRegistry</c> 的 <c>NormalizeId</c> 输出）。
        /// </param>
        /// <param name="pileType">
        ///     Minted <see cref="PileType" /> value that represents this pile at runtime.
        ///     运行时代表此 pile 的 minted <c>PileType</c> 值。
        /// </param>
        /// <param name="scope">
        ///     Lifetime scope.
        ///     生命周期 scope。
        /// </param>
        /// <param name="style">
        ///     UI chrome style.
        ///     UI chrome 样式。
        /// </param>
        /// <param name="anchor">
        ///     UI slot hint.
        ///     UI slot 提示。
        /// </param>
        /// <param name="iconPath">
        ///     Optional Godot resource path for the pile icon.
        ///     pile 图标的可选 Godot ResourcePath。
        /// </param>
        /// <param name="hotkeys">
        ///     Optional hotkey ids for the pile button.
        ///     pile 按钮的可选 hotkey id。
        /// </param>
        /// <param name="cardShouldBeVisible">
        ///     Whether cards render as <c>NCard</c> nodes inside the pile container.
        ///     card 是否在 pile 容器内渲染为 <c>NCard</c> 节点。
        /// </param>
        /// <param name="onOpen">
        ///     Optional callback invoked when the pile's UI button is released (see <see cref="OnOpen" />).
        ///     pile 的 UI 按钮释放时调用的可选回调（参见 <c>OnOpen</c>）。
        /// </param>
        /// <param name="hoverTipScreenOffset">
        ///     Added to the hover tip position after automatic placement (see <see cref="HoverTipScreenOffset" />).
        ///     自动放置后添加到 hover tip 位置的偏移（参见 <c>HoverTipScreenOffset</c>）。
        /// </param>
        /// <param name="hoverTipPlacement">
        ///     How the hover tip is anchored to the pile button (see <see cref="HoverTipPlacement" />).
        ///     hover tip 锚定到 pile 按钮的方式（参见 <c>HoverTipPlacement</c>）。
        /// </param>
        /// <param name="visibleWhen">
        ///     Optional visibility predicate (see <see cref="VisibleWhen" />). Null means always visible on the
        ///     button node (subject to parent visibility).
        ///     可选可见性谓词（参见 <c>VisibleWhen</c>）。null 表示在按钮节点上始终可见
        ///     （仍受父节点可见性影响）。
        /// </param>
        /// <param name="flightTargetPositionResolver">
        ///     Optional dynamic fly-in target resolver (see <see cref="FlightTargetPositionResolver" />).
        ///     可选动态 fly-in target resolver（参见 <c>FlightTargetPositionResolver</c>）。
        /// </param>
        /// <param name="flightStartPositionResolver">
        ///     Optional dynamic fly-out source/start resolver (see <see cref="FlightStartPositionResolver" />).
        ///     可选动态 fly-out source/start resolver（参见 <c>FlightStartPositionResolver</c>）。
        /// </param>
        public ModCardPileDefinition(
            string modId,
            string id,
            PileType pileType,
            ModCardPileScope scope,
            ModCardPileUiStyle style,
            ModCardPileAnchor anchor,
            string? iconPath,
            string[]? hotkeys,
            bool cardShouldBeVisible,
            Action<ModCardPileOpenContext>? onOpen,
            Vector2 hoverTipScreenOffset,
            ModCardPileHoverTipPlacement hoverTipPlacement,
            Func<ModCardPileVisibilityContext, bool>? visibleWhen,
            Func<ModCardPileFlightTargetContext, Vector2?>? flightTargetPositionResolver,
            Func<ModCardPileFlightStartContext, Vector2?>? flightStartPositionResolver)
        {
            ModId = modId;
            Id = id;
            PileType = pileType;
            Scope = scope;
            Style = style;
            Anchor = anchor;
            IconPath = iconPath;
            Hotkeys = hotkeys;
            CardShouldBeVisible = cardShouldBeVisible;
            OnOpen = onOpen;
            HoverTipScreenOffset = hoverTipScreenOffset;
            HoverTipPlacement = hoverTipPlacement;
            VisibleWhen = visibleWhen;
            FlightTargetPositionResolver = flightTargetPositionResolver;
            FlightStartPositionResolver = flightStartPositionResolver;
        }

        /// <summary>
        ///     Compatibility overload that omitted <see cref="FlightTargetPositionResolver" />; forwards with null.
        ///     省略 <c>FlightTargetPositionResolver</c> 的兼容重载；以 null 转发。
        /// </summary>
        public ModCardPileDefinition(
            string modId,
            string id,
            PileType pileType,
            ModCardPileScope scope,
            ModCardPileUiStyle style,
            ModCardPileAnchor anchor,
            string? iconPath,
            string[]? hotkeys,
            bool cardShouldBeVisible,
            Action<ModCardPileOpenContext>? onOpen,
            Vector2 hoverTipScreenOffset,
            ModCardPileHoverTipPlacement hoverTipPlacement,
            Func<ModCardPileVisibilityContext, bool>? visibleWhen)
            : this(modId, id, pileType, scope, style, anchor, iconPath, hotkeys, cardShouldBeVisible, onOpen,
                hoverTipScreenOffset, hoverTipPlacement, visibleWhen, null, null)
        {
        }

        /// <summary>
        ///     Compatibility overload that omitted <see cref="FlightStartPositionResolver" />; forwards with null.
        ///     省略 <c>FlightStartPositionResolver</c> 的兼容重载；以 null 转发。
        /// </summary>
        public ModCardPileDefinition(
            string modId,
            string id,
            PileType pileType,
            ModCardPileScope scope,
            ModCardPileUiStyle style,
            ModCardPileAnchor anchor,
            string? iconPath,
            string[]? hotkeys,
            bool cardShouldBeVisible,
            Action<ModCardPileOpenContext>? onOpen,
            Vector2 hoverTipScreenOffset,
            ModCardPileHoverTipPlacement hoverTipPlacement,
            Func<ModCardPileVisibilityContext, bool>? visibleWhen,
            Func<ModCardPileFlightTargetContext, Vector2?>? flightTargetPositionResolver)
            : this(modId, id, pileType, scope, style, anchor, iconPath, hotkeys, cardShouldBeVisible, onOpen,
                hoverTipScreenOffset, hoverTipPlacement, visibleWhen, flightTargetPositionResolver, null)
        {
        }

        /// <summary>
        ///     Compatibility overload that omitted <see cref="VisibleWhen" />; forwards with null.
        ///     省略 <c>VisibleWhen</c> 的兼容重载；以 null 转发。
        /// </summary>
        public ModCardPileDefinition(
            string modId,
            string id,
            PileType pileType,
            ModCardPileScope scope,
            ModCardPileUiStyle style,
            ModCardPileAnchor anchor,
            string? iconPath,
            string[]? hotkeys,
            bool cardShouldBeVisible,
            Action<ModCardPileOpenContext>? onOpen,
            Vector2 hoverTipScreenOffset,
            ModCardPileHoverTipPlacement hoverTipPlacement)
            : this(modId, id, pileType, scope, style, anchor, iconPath, hotkeys, cardShouldBeVisible, onOpen,
                hoverTipScreenOffset, hoverTipPlacement, null, null, null)
        {
        }

        /// <summary>
        ///     Compatibility overload that omitted <see cref="HoverTipPlacement" />; forwards with
        ///     <see cref="ModCardPileHoverTipPlacement.Auto" />.
        ///     省略 <c>HoverTipPlacement</c> 的兼容重载；以
        ///     <c>ModCardPileHoverTipPlacement.Auto</c> 转发。
        /// </summary>
        public ModCardPileDefinition(
            string modId,
            string id,
            PileType pileType,
            ModCardPileScope scope,
            ModCardPileUiStyle style,
            ModCardPileAnchor anchor,
            string? iconPath,
            string[]? hotkeys,
            bool cardShouldBeVisible,
            Action<ModCardPileOpenContext>? onOpen,
            Vector2 hoverTipScreenOffset)
            : this(modId, id, pileType, scope, style, anchor, iconPath, hotkeys, cardShouldBeVisible, onOpen,
                hoverTipScreenOffset, ModCardPileHoverTipPlacement.Auto)
        {
        }

        /// <summary>
        ///     Compatibility overload matching the historical call shape that omitted
        ///     <see cref="OnOpen" />; forwards with a null <see cref="OnOpen" />,
        ///     <see cref="Vector2.Zero" /> for <see cref="HoverTipScreenOffset" />, and
        ///     <see cref="ModCardPileHoverTipPlacement.Auto" /> for <see cref="HoverTipPlacement" />.
        ///     匹配历史调用形状的兼容重载，该形状省略 <c>OnOpen</c>；以 null 的 <c>OnOpen</c>、
        ///     <c>HoverTipScreenOffset</c> 的 <c>Vector2.Zero</c>，以及
        ///     <c>HoverTipPlacement</c> 的 <c>ModCardPileHoverTipPlacement.Auto</c> 转发。
        /// </summary>
        public ModCardPileDefinition(
            string modId,
            string id,
            PileType pileType,
            ModCardPileScope scope,
            ModCardPileUiStyle style,
            ModCardPileAnchor anchor,
            string? iconPath,
            string[]? hotkeys,
            bool cardShouldBeVisible)
            : this(modId, id, pileType, scope, style, anchor, iconPath, hotkeys, cardShouldBeVisible, null,
                default, ModCardPileHoverTipPlacement.Auto)
        {
        }

        /// <summary>
        ///     Compatibility overload for the historical shape before
        ///     <see cref="HoverTipScreenOffset" /> and <see cref="HoverTipPlacement" />; forwards with
        ///     <see cref="Vector2.Zero" /> and <see cref="ModCardPileHoverTipPlacement.Auto" />.
        ///     针对引入 <c>HoverTipScreenOffset</c> 和 <c>HoverTipPlacement</c> 之前历史形状的
        ///     兼容重载；以 <c>Vector2.Zero</c> 和 <c>ModCardPileHoverTipPlacement.Auto</c> 转发。
        /// </summary>
        public ModCardPileDefinition(
            string modId,
            string id,
            PileType pileType,
            ModCardPileScope scope,
            ModCardPileUiStyle style,
            ModCardPileAnchor anchor,
            string? iconPath,
            string[]? hotkeys,
            bool cardShouldBeVisible,
            Action<ModCardPileOpenContext>? onOpen)
            : this(modId, id, pileType, scope, style, anchor, iconPath, hotkeys, cardShouldBeVisible, onOpen,
                default, ModCardPileHoverTipPlacement.Auto)
        {
        }

        /// <summary>
        ///     Owning mod id.
        ///     所属 mod id。
        /// </summary>
        public string ModId { get; }

        /// <summary>
        ///     Normalized global id (trimmed).
        ///     normalized global id（已 trim）。
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Deterministically minted <see cref="PileType" /> value.
        ///     确定性 minted 的 <c>PileType</c> 值。
        /// </summary>
        public PileType PileType { get; }

        /// <summary>
        ///     Lifetime scope declared at registration.
        ///     注册时声明的生命周期 scope。
        /// </summary>
        public ModCardPileScope Scope { get; }

        /// <summary>
        ///     UI chrome style.
        ///     UI chrome 样式。
        /// </summary>
        public ModCardPileUiStyle Style { get; }

        /// <summary>
        ///     UI slot hint.
        ///     UI slot 提示。
        /// </summary>
        public ModCardPileAnchor Anchor { get; }

        /// <summary>
        ///     Icon resource path (<c>res://...</c>); null falls back to a placeholder icon.
        ///     图标ResourcePath（<c>res://...</c>）；null 时回退到占位图标。
        /// </summary>
        public string? IconPath { get; }

        /// <summary>
        ///     Hover-tip title resolved against <see cref="ModCardPileSpec.HoverTipLocTable" /> with key
        ///     <c>"{Id}.title"</c>.
        ///     基于 <c>ModCardPileSpec.HoverTipLocTable</c>、使用 <c>"{Id}.title"</c> key 解析的
        ///     hover-tip 标题。
        /// </summary>
        public LocString Title => new(ModCardPileSpec.HoverTipLocTable, $"{Id}.title");

        /// <summary>
        ///     Hover-tip description resolved against <see cref="ModCardPileSpec.HoverTipLocTable" /> with
        ///     key <c>"{Id}.description"</c>.
        ///     基于 <c>ModCardPileSpec.HoverTipLocTable</c>、使用 <c>"{Id}.description"</c> key 解析的
        ///     hover-tip 描述。
        /// </summary>
        public LocString Description => new(ModCardPileSpec.HoverTipLocTable, $"{Id}.description");

        /// <summary>
        ///     Message displayed when the pile is opened while empty; resolved against
        ///     <see cref="ModCardPileSpec.HoverTipLocTable" /> with key <c>"{Id}.empty"</c>.
        ///     pile 为空时打开所显示的消息；基于 <c>ModCardPileSpec.HoverTipLocTable</c>、
        ///     使用 <c>"{Id}.empty"</c> key 解析。
        /// </summary>
        public LocString EmptyPileMessage => new(ModCardPileSpec.HoverTipLocTable, $"{Id}.empty");

        /// <summary>
        ///     Hotkey ids (see <c>MegaInput</c>) forwarded to <c>NCardPileScreen.ShowScreen</c>.
        ///     转发给 <c>NCardPileScreen.ShowScreen</c> 的 hotkey id（参见 <c>MegaInput</c>）。
        /// </summary>
        public string[]? Hotkeys { get; }

        /// <summary>
        ///     When true, the pile renders cards as <c>NCard</c> nodes (only meaningful for
        ///     <see cref="ModCardPileUiStyle.ExtraHand" />).
        ///     为 true 时，pile 将 card 渲染为 <c>NCard</c> 节点（仅对
        ///     <c>ModCardPileUiStyle.ExtraHand</c> 有意义）。
        /// </summary>
        public bool CardShouldBeVisible { get; }

        /// <summary>
        ///     Handler invoked when the pile's UI button is released. Null means "use the default
        ///     <c>NCardPileScreen</c>". See <see cref="ModCardPileSpec.OnOpen" /> for the full contract.
        ///     pile 的 UI 按钮释放时调用的 handler。null 表示“使用默认 <c>NCardPileScreen</c>”。
        ///     完整契约参见 <c>ModCardPileSpec.OnOpen</c>。
        /// </summary>
        public Action<ModCardPileOpenContext>? OnOpen { get; }

        /// <summary>
        ///     Extra pixels added to the hover tip position (see <see cref="ModCardPileSpec.HoverTipScreenOffset" />).
        ///     添加到 hover tip 位置的额外像素（参见 <c>ModCardPileSpec.HoverTipScreenOffset</c>）。
        /// </summary>
        public Vector2 HoverTipScreenOffset { get; }

        /// <summary>
        ///     How the hover tip is anchored to the pile button (see <see cref="ModCardPileSpec.HoverTipPlacement" />).
        ///     hover tip 锚定到 pile 按钮的方式（参见 <c>ModCardPileSpec.HoverTipPlacement</c>）。
        /// </summary>
        public ModCardPileHoverTipPlacement HoverTipPlacement { get; }

        /// <summary>
        ///     When non-null, the pile button evaluates this on <c>_Process</c> and hides itself when the
        ///     predicate returns false (see <see cref="ModCardPileSpec.VisibleWhen" />).
        ///     非 null 时，pile 按钮会在 <c>_Process</c> 中求值；谓词返回 false 时隐藏自身
        ///     （参见 <c>ModCardPileSpec.VisibleWhen</c>）。
        /// </summary>
        public Func<ModCardPileVisibilityContext, bool>? VisibleWhen { get; }

        /// <summary>
        ///     Optional resolver invoked for each fly-in targeting request to this pile.
        ///     每次以此 pile 为目标的 fly-in 请求都会调用的可选 resolver。
        /// </summary>
        public Func<ModCardPileFlightTargetContext, Vector2?>? FlightTargetPositionResolver { get; }

        /// <summary>
        ///     Optional resolver invoked when a shuffle-style fly visual starts from this pile.
        ///     shuffle 风格 fly visual 从此 pile 开始时调用的可选 resolver。
        /// </summary>
        public Func<ModCardPileFlightStartContext, Vector2?>? FlightStartPositionResolver { get; }
    }
}
