using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace STS2RitsuLib.Scaffolding.Characters
{
    /// <summary>
    ///     Extension methods on <see cref="Creature" /> and <see cref="Player" /> for common combat queries
    ///     (powers, orbs, energy).
    ///     <c>Creature</c> 和 <c>Player</c> 的常用战斗查询扩展方法（能力、充能球、能量）。
    /// </summary>
    public static class CharacterCombatExtensions
    {
        /// <summary>
        ///     Returns the first active power instance of type <typeparamref name="TPower" />, if any.
        ///     返回第一个类型为 <c>TPower</c> 的激活能力实例；若不存在则返回 null。
        /// </summary>
        public static TPower? FindPower<TPower>(this Creature creature) where TPower : PowerModel
        {
            ArgumentNullException.ThrowIfNull(creature);
            return creature.Powers.OfType<TPower>().FirstOrDefault();
        }

        /// <summary>
        ///     Whether the creature currently has at least <paramref name="minimumAmount" /> stacks of
        ///     <typeparamref name="TPower" />.
        ///     此生物当前是否至少拥有 <c>minimumAmount</c> 层 <c>TPower</c>。
        /// </summary>
        public static bool HasPower<TPower>(this Creature creature, int minimumAmount = 1) where TPower : PowerModel
        {
            ArgumentNullException.ThrowIfNull(creature);
            return creature.FindPower<TPower>()?.Amount >= minimumAmount;
        }

        /// <summary>
        ///     Current stack count of <typeparamref name="TPower" />, or zero if absent.
        ///     <c>TPower</c> 的当前层数；不存在时为 0。
        /// </summary>
        public static int GetPowerAmount<TPower>(this Creature creature) where TPower : PowerModel
        {
            ArgumentNullException.ThrowIfNull(creature);
            return creature.FindPower<TPower>()?.Amount ?? 0;
        }

        /// <summary>
        ///     Whether the player’s orb queue currently contains at least one orb of type <typeparamref name="TOrb" />.
        ///     玩家充能球队列当前是否至少包含一个类型为 <c>TOrb</c> 的充能球。
        /// </summary>
        public static bool HasOrb<TOrb>(this Player player) where TOrb : OrbModel
        {
            ArgumentNullException.ThrowIfNull(player);
            return player.PlayerCombatState?.OrbQueue.Orbs.OfType<TOrb>().Any() == true;
        }

        /// <summary>
        ///     Counts orbs of type <typeparamref name="TOrb" /> in the queue.
        ///     统计队列中类型为 <c>TOrb</c> 的充能球数量。
        /// </summary>
        public static int GetOrbCount<TOrb>(this Player player) where TOrb : OrbModel
        {
            ArgumentNullException.ThrowIfNull(player);
            return player.PlayerCombatState?.OrbQueue.Orbs.OfType<TOrb>().Count() ?? 0;
        }

        /// <summary>
        ///     Current combat energy, or zero if not in a combat state.
        ///     当前战斗能量；若不在战斗状态则为 0。
        /// </summary>
        public static int GetEnergy(this Player player)
        {
            ArgumentNullException.ThrowIfNull(player);
            return player.PlayerCombatState?.Energy ?? 0;
        }

        /// <summary>
        ///     Maximum energy for the current combat state, or zero if unavailable.
        ///     当前战斗状态的最大能量；不可用时为 0。
        /// </summary>
        public static int GetMaxEnergy(this Player player)
        {
            ArgumentNullException.ThrowIfNull(player);
            return player.PlayerCombatState?.MaxEnergy ?? 0;
        }

        /// <summary>
        ///     Orb queue capacity in combat, or zero if unavailable.
        ///     战斗中的充能球队列容量；不可用时为 0。
        /// </summary>
        public static int GetOrbCapacity(this Player player)
        {
            ArgumentNullException.ThrowIfNull(player);
            return player.PlayerCombatState?.OrbQueue.Capacity ?? 0;
        }
    }
}
