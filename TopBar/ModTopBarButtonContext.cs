using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using STS2RitsuLib.CardPiles.Nodes;
using STS2RitsuLib.Screens;

namespace STS2RitsuLib.TopBar
{
    /// <summary>
    ///     Context passed to <see cref="ModTopBarButtonSpec.OnClick" /> and
    ///     中文说明：Context passed to <c>ModTopBarButtonSpec.OnClick</c> and
    ///     <see cref="ModTopBarButtonSpec.VisibleWhen" /> callbacks. Exposes the registry definition, the
    ///     local <see cref="Player" /> this button is bound to, and convenience forwarders to
    ///     local <c>Player</c> this button is bound to, 和 convenience 用于warders to
    ///     <see cref="ModScreenService" /> so handlers don't need to pull in capstone plumbing directly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A single context instance is constructed per click / visibility probe — it is not cached.
    ///         一个 single context instance is constructed per click / visibility probe — it is not cached。
    ///         <see cref="Player" /> may be null when visibility is probed before the local player has been
    ///         resolved (for example, between runs), in which case <see cref="ModTopBarButtonSpec.VisibleWhen" />
    ///         resolved (用于 example, between runs), in which case <c>ModTopBarButtonSpec.VisibleWhen</c>
    ///         handlers should be prepared to return false.
    ///         handlers should be prepared to 返回 false.
    ///     </para>
    /// </remarks>
    public sealed class ModTopBarButtonContext
    {
        internal ModTopBarButtonContext(
            ModTopBarButtonDefinition definition,
            Player? player,
            NModCardPileButton? button)
        {
            Definition = definition;
            Player = player;
            Button = button;
        }

        /// <summary>
        ///     Registry definition that produced the button.
        ///     注册表 definition that produced the button.
        /// </summary>
        public ModTopBarButtonDefinition Definition { get; }

        /// <summary>
        ///     Local player the button is currently bound to (null while the run is still booting).
        ///     Local player the button is currently bound to (null while the 跑局 is still booting).
        /// </summary>
        public Player? Player { get; }

        /// <summary>
        ///     The Godot button node, when the callback is coming from a real UI click. Shared with the
        ///     The Godot button node, 当 the callback is coming 从 a real UI click. Shared 带有 the
        ///     card-pile subsystem — action-mode buttons are instances of <see cref="NModCardPileButton" />
        ///     卡牌-pile subsystem — action-mode buttons are instances of <c>NModCardPileButton</c>
        ///     with <see cref="NModCardPileButton.ActionDefinition" /> set rather than a pile, so the UI
        ///     带有 <c>NModCardPileButton.ActionDefinition</c> 设置 rather than a pile, so the UI
        ///     layer is identical to <see cref="STS2RitsuLib.CardPiles.ModCardPileRegistry" /> buttons.
        ///     layer is identical to <c>STS2RitsuLib.CardPiles.ModCardPile注册表</c> buttons.
        /// </summary>
        public NModCardPileButton? Button { get; }

        /// <summary>
        ///     Opens <paramref name="screen" /> via <see cref="ModScreenService.Open" />.
        ///     中文说明：Opens <c>screen</c> via <c>ModScreenService.Open</c>.
        /// </summary>
        public bool OpenCapstoneScreen(ICapstoneScreen screen)
        {
            return ModScreenService.Open(screen);
        }

        /// <summary>
        ///     Toggles <paramref name="screen" /> — opens it if not currently mounted, closes it otherwise.
        ///     Toggles <c>screen</c> — opens it 如果 not currently mounted, closes it otherwise.
        /// </summary>
        public bool ToggleCapstoneScreen(ICapstoneScreen screen)
        {
            return ModScreenService.Toggle(screen);
        }

        /// <summary>
        ///     Closes the current capstone, if any.
        ///     Closes the current capstone, 如果 any.
        /// </summary>
        public bool CloseCapstoneScreen()
        {
            return ModScreenService.Close();
        }
    }
}
