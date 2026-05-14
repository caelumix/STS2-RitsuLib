using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace STS2RitsuLib.CardPiles
{
    /// <summary>
    ///     Common subset of data exposed by mod card pile flight contexts.
    ///     mod card pile flight context 暴露的通用数据子集。
    /// </summary>
    public interface IModCardPileFlightContext
    {
        /// <summary>
        ///     Definition associated with the flight request.
        ///     与 flight 请求关联的 definition。
        /// </summary>
        ModCardPileDefinition Definition { get; }

        /// <summary>
        ///     Ritsulib's default position for this request.
        ///     ritsulib 为此请求计算的默认位置。
        /// </summary>
        Vector2 DefaultPosition { get; }

        /// <summary>
        ///     Source pile for this request, when applicable.
        ///     此请求的 source pile（适用时）。
        /// </summary>
        CardPile? StartPile { get; }

        /// <summary>
        ///     Destination pile for this request, when applicable.
        ///     此请求的 destination pile（适用时）。
        /// </summary>
        CardPile? TargetPile { get; }

        /// <summary>
        ///     Live card node involved in the flight, when available.
        ///     flight 涉及的 live card node（可用时）。
        /// </summary>
        NCard? CardNode { get; }

        /// <summary>
        ///     Card model involved in the flight, when available.
        ///     flight 涉及的 card model（可用时）。
        /// </summary>
        CardModel? CardModel { get; }
    }
}
