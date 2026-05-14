using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using STS2RitsuLib.CardPiles.Nodes;
using STS2RitsuLib.Screens;

namespace STS2RitsuLib.CardPiles
{
    /// <summary>
    ///     Context passed to <see cref="ModCardPileSpec.OnOpen" /> when a mod pile's UI button is released.
    ///     Exposes the backing pile plus convenience helpers so handlers can swap in a custom
    ///     <see cref="ICapstoneScreen" /> (or invoke the default <see cref="NCardPileScreen" />) without
    ///     hand-wiring <see cref="NCapstoneContainer" />.
    ///     mod pile 的 UI 按钮释放时传给 <c>ModCardPileSpec.OnOpen</c> 的 context。它暴露 backing pile
    ///     与便捷 helper，使 handler 可以切换到自定义 <c>ICapstoneScreen</c>（或调用默认
    ///     <c>NCardPileScreen</c>），无需手动连接 <c>NCapstoneContainer</c>。
    /// </summary>
    /// <remarks>
    ///     Handlers may:
    ///     <list type="bullet">
    ///         <item>Call <see cref="ShowDefaultPileScreen" /> to reuse the vanilla <see cref="NCardPileScreen" />.</item>
    ///         <item>
    ///             Call <see cref="OpenCapstoneScreen(ICapstoneScreen)" /> to mount a custom
    ///             <see cref="ICapstoneScreen" /> (for example a Godot scene script).
    ///         </item>
    ///         <item>Do nothing — the button returns to its idle state after the tween.</item>
    ///     </list>
    ///     Handlers are invoked from the button's release handler after the click tween starts and after
    ///     ritsulib already ensured the pile is non-empty (empty piles trigger
    ///     <see cref="ModCardPileDefinition.EmptyPileMessage" /> via a thought bubble and skip the callback).
    ///     handler 可以调用 <c>ShowDefaultPileScreen</c> 复用原版 <c>NCardPileScreen</c>；
    ///     调用 <c>OpenCapstoneScreen(ICapstoneScreen)</c> 挂载自定义 <c>ICapstoneScreen</c>
    ///     （例如 Godot scene script）；或什么都不做，让按钮在 tween 后回到 idle 状态。handler 会在按钮 release
    ///     处理器中、click tween 开始后调用，并且 ritsulib 已确认 pile 非空（空 pile 会通过 thought bubble 触发
    ///     <c>ModCardPileDefinition.EmptyPileMessage</c> 并跳过回调）。
    /// </remarks>
    public sealed class ModCardPileOpenContext
    {
        internal ModCardPileOpenContext(
            ModCardPileDefinition definition,
            ModCardPile pile,
            Player player,
            NModCardPileButton? button)
        {
            Definition = definition;
            Pile = pile;
            Player = player;
            Button = button;
        }

        /// <summary>
        ///     Definition of the pile whose button was pressed.
        ///     被按下按钮对应 pile 的 definition。
        /// </summary>
        public ModCardPileDefinition Definition { get; }

        /// <summary>
        ///     Live <see cref="ModCardPile" /> resolved for <see cref="Player" />.
        ///     为 <c>Player</c> 解析出的 live <c>ModCardPile</c>。
        /// </summary>
        public ModCardPile Pile { get; }

        /// <summary>
        ///     The local player this button is bound to.
        ///     此按钮绑定到的本地玩家。
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     The clicked button, when the open was triggered from a
        ///     <see cref="ModCardPileUiStyle.TopBarDeck" /> / <see cref="ModCardPileUiStyle.BottomLeft" /> /
        ///     <see cref="ModCardPileUiStyle.BottomRight" /> UI node. Null for programmatic invocations.
        ///     触发打开操作的点击按钮；当 open 来自 <c>ModCardPileUiStyle.TopBarDeck</c> /
        ///     <c>ModCardPileUiStyle.BottomLeft</c> / <c>ModCardPileUiStyle.BottomRight</c> UI 节点时存在。
        ///     程序化调用时为 null。
        /// </summary>
        public NModCardPileButton? Button { get; }

        /// <summary>
        ///     Launches the vanilla <see cref="NCardPileScreen" /> for the current pile, re-using
        ///     <see cref="ModCardPileDefinition.Hotkeys" /> when set. This is exactly what the default open
        ///     handler does when <see cref="ModCardPileSpec.OnOpen" /> is null.
        ///     为当前 pile 启动原版 <c>NCardPileScreen</c>，并在设置时复用
        ///     <c>ModCardPileDefinition.Hotkeys</c>。当 <c>ModCardPileSpec.OnOpen</c> 为 null 时，
        ///     默认 open handler 做的正是这件事。
        /// </summary>
        public void ShowDefaultPileScreen()
        {
            NCardPileScreen.ShowScreen(Pile, Definition.Hotkeys ?? []);
        }

        /// <summary>
        ///     Opens <paramref name="screen" /> through <see cref="ModScreenService.Open" />. If a capstone
        ///     is already showing, it is closed first so the new screen can take the stage.
        ///     通过 <c>ModScreenService.Open</c> 打开 <c>screen</c>。如果已有 capstone 显示，
        ///     会先关闭它，让新 screen 接管显示。
        /// </summary>
        /// <remarks>
        ///     Thin convenience forwarder — the actual capstone plumbing lives in
        ///     <see cref="ModScreenService" /> so any mod code can open custom screens without pulling in
        ///     the card-pile subsystem.
        ///     轻量便捷转发器；实际 capstone plumbing 位于 <c>ModScreenService</c>，因此任何 mod 代码都可以
        ///     打开自定义 screen，而不必引入 card-pile 子系统。
        /// </remarks>
        /// <param name="screen">
        ///     Custom screen implementing <see cref="ICapstoneScreen" />.
        ///     实现 <c>ICapstoneScreen</c> 的自定义 screen。
        /// </param>
        public void OpenCapstoneScreen(ICapstoneScreen screen)
        {
            ModScreenService.Open(screen);
        }
    }
}
