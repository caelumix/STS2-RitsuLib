using MegaCrit.Sts2.Core.Entities.Cards;

namespace STS2RitsuLib.CardPiles
{
    /// <summary>
    ///     Extension helpers for working with minted mod <see cref="PileType" /> values.
    /// </summary>
    public static class ModCardPileExtensions
    {
        /// <summary>
        ///     Convenience: minted <see cref="PileType" /> value for <paramref name="pileId" />, intended for
        ///     vanilla-style call sites such as <c>player.GetPile(id.GetModCardPileType())</c>.
        /// </summary>
        public static PileType GetModCardPileType(this string pileId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(pileId);
            return ModCardPileRegistry.GetPileType(pileId);
        }
    }
}
