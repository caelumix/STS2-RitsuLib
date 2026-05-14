#if !STS2_AT_LEAST_0_104_0
using CombatStateLike = MegaCrit.Sts2.Core.Combat.CombatState;
#else
using CombatStateLike = MegaCrit.Sts2.Core.Combat.ICombatState;
#endif
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace STS2RitsuLib.Combat.HealthBars
{
    /// <summary>
    ///     Runtime context passed to health bar forecast sources.
    ///     runtime context passed to health bar 用于ecast sources.
    /// </summary>
    /// <param name="Creature">
    ///     Creature whose health bar is being rendered.
    ///     中文说明：Creature whose health bar is being rendered.
    /// </param>
    public readonly record struct HealthBarForecastContext(Creature Creature)
    {
        /// <summary>
        ///     Current combat state, when the creature is in combat.
        ///     当前 combat state, when the creature is in combat。
        /// </summary>
        public CombatStateLike? CombatState => Creature.CombatState;

        /// <summary>
        ///     Side whose turn is currently active, when available.
        ///     Side whose turn is currently active, 当 可用.
        /// </summary>
        public CombatSide? CurrentSide => Creature.CombatState?.CurrentSide;
    }

    /// <summary>
    ///     Produces one or more health bar forecast segments for a creature.
    ///     Produces one 或 more health bar 用于ecast segments 用于 a creature.
    /// </summary>
    /// <remarks>
    ///     Power models can implement this directly and will be discovered automatically from
    ///     能力 Models can implement this directly 和 will be discovered automatically 从
    ///     <see cref="Creature.Powers" /> without any extra registration.
    ///     Non-power forecast sources can be registered through the content-pack workflow.
    ///     Non-能力 用于ecast sources can be 已注册 through the content-pack workflow.
    /// </remarks>
    public interface IHealthBarForecastSource
    {
        /// <summary>
        ///     Returns forecast segments to render for <paramref name="context" />.
        ///     返回 forecast segments to render for <c>context</c>。
        /// </summary>
        IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context);
    }
}
