using MegaCrit.Sts2.Core.Entities.Players;
using STS2RitsuLib.CardPiles.Nodes;

namespace STS2RitsuLib.CardPiles
{
    /// <summary>
    ///     Context passed to <see cref="ModCardPileSpec.VisibleWhen" />. Exposes the pile definition, the bound
    ///     <see cref="Player" />, the live button node, and the resolved <see cref="ModCardPile" /> when available.
    ///     传给 <c>ModCardPileSpec.VisibleWhen</c> 的 context。暴露 pile definition、绑定的
    ///     <c>Player</c>、live button node，以及可用时解析出的 <c>ModCardPile</c>。
    /// </summary>
    /// <remarks>
    ///     <see cref="Player" /> or <see cref="Pile" /> may be null while the run or combat UI is still wiring up;
    ///     predicates should return false when required state is missing unless the pile should show anyway.
    ///     当 run 或 combat UI 仍在接线时，<c>Player</c> 或 <c>Pile</c> 可能为 null；
    ///     除非 pile 即使缺少状态也应显示，否则谓词应在所需状态缺失时返回 false。
    /// </remarks>
    public sealed class ModCardPileVisibilityContext
    {
        internal ModCardPileVisibilityContext(
            ModCardPileDefinition definition,
            Player? player,
            NModCardPileButton? button,
            ModCardPile? pile)
        {
            Definition = definition;
            Player = player;
            Button = button;
            Pile = pile;
        }

        /// <summary>
        ///     Registry definition for this pile.
        ///     此 pile 的 registry definition。
        /// </summary>
        public ModCardPileDefinition Definition { get; }

        /// <summary>
        ///     Local player the button is bound to, when known.
        ///     按钮绑定的本地玩家（已知时）。
        /// </summary>
        public Player? Player { get; }

        /// <summary>
        ///     The pile UI button instance.
        ///     pile UI 按钮实例。
        /// </summary>
        public NModCardPileButton? Button { get; }

        /// <summary>
        ///     Runtime pile instance, when <see cref="NModCardPileButton.Initialize" /> has attached it.
        ///     <c>NModCardPileButton.Initialize</c> 已附加时的运行时 pile 实例。
        /// </summary>
        public ModCardPile? Pile { get; }
    }
}
