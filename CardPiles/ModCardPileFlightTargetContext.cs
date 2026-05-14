using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace STS2RitsuLib.CardPiles
{
    /// <summary>
    ///     Context passed to <see cref="ModCardPileSpec.FlightTargetPositionResolver" /> each time a card
    ///     requests a fly-in target position for a mod pile.
    ///     每当 card 请求飞入 mod pile 的目标位置时，传给
    ///     <c>ModCardPileSpec.FlightTargetPositionResolver</c> 的 context。
    /// </summary>
    public sealed class ModCardPileFlightTargetContext : IModCardPileFlightContext
    {
        internal ModCardPileFlightTargetContext(
            ModCardPileDefinition definition,
            NCard? cardNode,
            Vector2 defaultTargetPosition)
        {
            Definition = definition;
            CardNode = cardNode;
            DefaultTargetPosition = defaultTargetPosition;
        }

        /// <summary>
        ///     Ritsulib's default target position for this request.
        ///     ritsulib 为此请求计算的默认 target 位置。
        /// </summary>
        public Vector2 DefaultTargetPosition { get; }

        /// <summary>
        ///     Definition of the target pile.
        ///     target pile 的 definition。
        /// </summary>
        public ModCardPileDefinition Definition { get; }

        /// <summary>
        ///     Live card node that is flying into the pile, when available.
        ///     正在飞入 pile 的 live card node（可用时）。
        /// </summary>
        public NCard? CardNode { get; }

        /// <inheritdoc />
        public Vector2 DefaultPosition => DefaultTargetPosition;

        /// <inheritdoc />
        public CardPile? StartPile => null;

        /// <inheritdoc />
        public CardPile? TargetPile => null;

        /// <inheritdoc />
        public CardModel? CardModel => CardNode?.Model;
    }
}
