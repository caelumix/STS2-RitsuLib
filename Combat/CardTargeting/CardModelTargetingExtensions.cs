using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace STS2RitsuLib.Combat.CardTargeting
{
    /// <summary>
    ///     Extension helpers for resolving card targets by target type.
    ///     用于按目标类型解析卡牌目标集合的扩展 helper。
    /// </summary>
    public static class CardModelTargetingExtensions
    {
        /// <summary>
        ///     Returns targets resolved from the card's current <see cref="TargetType" />.
        ///     返回根据当前卡牌 <c>TargetType</c> 解析得到的目标列表。
        /// </summary>
        public static List<Creature> GetTargets(this CardModel card)
        {
            ArgumentNullException.ThrowIfNull(card);

            var state = card.CombatState;
            switch (card.TargetType)
            {
                case TargetType.AllAllies:
                    return state?.PlayerCreatures.Where(c => c.IsAlive).ToList() ?? [];
                case TargetType.AllEnemies:
                    return state?.HittableEnemies.ToList() ?? [];
                case TargetType.RandomEnemy:
                {
                    var allTargets = state?.HittableEnemies.ToList();
                    if (allTargets == null || allTargets.Count == 0)
                        return [];
                    var target = card.Owner.RunState.Rng.CombatTargets.NextItem(allTargets);
                    return target == null ? [] : [target];
                }
                case TargetType.None:
                    return [];
                case TargetType.Self:
                    return [card.Owner.Creature];
                default:
                {
                    if (!CustomTargetTypeRegistry.IsCustomMultiTargetType(card.TargetType))
                        return [];

                    return state?.Creatures
                               .Where(c =>
                                   CustomTargetTypeRegistry.TryShouldIncludeMultiTarget(card.TargetType, c,
                                       out var include) && include)
                               .ToList() ??
                           [];
                }
            }
        }
    }
}
