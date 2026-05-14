using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace STS2RitsuLib.CardPiles
{
    /// <summary>
    ///     Context passed to <see cref="ModCardPileSpec.FlightStartPositionResolver" /> when shuffle-style fly
    ///     visuals need a source/start position for a mod pile.
    ///     当 shuffle 风格 fly visual 需要 mod pile 的 source/start 位置时，传给
    ///     <c>ModCardPileSpec.FlightStartPositionResolver</c> 的 context。
    /// </summary>
    public sealed class ModCardPileFlightStartContext : IModCardPileFlightContext
    {
        internal ModCardPileFlightStartContext(
            ModCardPileDefinition definition,
            CardPile startPile,
            CardPile targetPile,
            Vector2 defaultStartPosition,
            NCard? cardNode = null)
        {
            Definition = definition;
            StartPile = startPile;
            TargetPile = targetPile;
            DefaultStartPosition = defaultStartPosition;
            CardNode = cardNode;
        }

        /// <summary>
        ///     Ritsulib's default start position for this request.
        ///     ritsulib 为此请求计算的默认 start 位置。
        /// </summary>
        public Vector2 DefaultStartPosition { get; }

        /// <summary>
        ///     Source pile for this shuffle fly visual.
        ///     此 shuffle fly visual 的 source pile。
        /// </summary>
        public CardPile StartPile { get; }

        /// <summary>
        ///     Destination pile for this shuffle fly visual.
        ///     此 shuffle fly visual 的 destination pile。
        /// </summary>
        public CardPile TargetPile { get; }

        /// <summary>
        ///     Definition of the source pile.
        ///     source pile 的 definition。
        /// </summary>
        public ModCardPileDefinition Definition { get; }

        /// <inheritdoc />
        public Vector2 DefaultPosition => DefaultStartPosition;

        /// <inheritdoc />
        public NCard? CardNode { get; }

        /// <inheritdoc />
        public CardModel? CardModel => CardNode?.Model;
    }
}
