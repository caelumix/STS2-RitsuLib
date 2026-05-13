using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Models;

namespace STS2RitsuLib.Scaffolding.Cards.HandGlow
{
    /// <summary>
    ///     Reusable condition functions matching common vanilla card patterns (Evil Eye, Fetch, Osty attacks). Prefer
    ///     <see cref="CardModelHandGlowExtensions" /> on <see cref="CardModel" /> for override bodies.
    ///     匹配常见原版卡牌模式（Evil Eye、Fetch、Osty 攻击）的可复用条件函数。重写方法体中优先使用
    ///     <see cref="CardModelHandGlowExtensions" /> 的 <see cref="CardModel" /> 扩展。
    /// </summary>
    public static class ModCardHandGlowPredicates
    {
        /// <summary>
        ///     Same idea as vanilla Osty attack cards: red when the owner’s companion is not present.
        ///     与原版 Osty 攻击牌同思路：拥有者的同伴不在场时显示红色。
        /// </summary>
        public static bool OwnerCompanionOstyMissing(CardModel card)
        {
            return card.Owner?.IsOstyMissing == true;
        }

        /// <summary>
        ///     Same history shape as <see cref="MegaCrit.Sts2.Core.Models.Cards.EvilEye" />: any of the owner’s cards was
        ///     exhausted this turn (often drives gold while a stronger effect line is active).
        ///     与 <see cref="MegaCrit.Sts2.Core.Models.Cards.EvilEye" /> 相同的历史记录形态：拥有者任意卡牌在本回合被消耗
        ///     （通常用于更强效果行激活时驱动金色）。
        /// </summary>
        public static bool AnyOfOwnersCardsExhaustedThisTurn(CardModel card)
        {
            var owner = card.Owner;
            var combat = card.CombatState;
            var history = CombatManager.Instance?.History;
            if (owner is null || combat is null || history is null)
                return false;

            return history.Entries.OfType<CardExhaustedEntry>()
                .Any(e => e.HappenedThisTurn(combat) && e.Card.Owner == owner);
        }

        /// <summary>
        ///     Same history shape as <see cref="MegaCrit.Sts2.Core.Models.Cards.Fetch" /> gold: this card has not finished a
        ///     play this turn.
        ///     与 <see cref="MegaCrit.Sts2.Core.Models.Cards.Fetch" /> 金色逻辑相同的历史记录形态：此卡本回合尚未完成打出。
        /// </summary>
        public static bool ThisCardNotFinishedPlayThisTurn(CardModel card)
        {
            var combat = card.CombatState;
            var history = CombatManager.Instance?.History;
            if (combat is null || history is null)
                return false;

            return !history.CardPlaysFinished.Any(e =>
                e.CardPlay.Card == card && e.HappenedThisTurn(combat));
        }
    }
}
