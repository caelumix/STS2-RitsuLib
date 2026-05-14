using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace STS2RitsuLib.Combat.HealthBars
{
    /// <summary>
    ///     Runtime context for resolving visual graft metrics on a creature health bar.
    ///     runtime context 用于 resolving visual graft metrics on a creature health bar.
    /// </summary>
    /// <param name="Creature">
    ///     Creature whose bar is being evaluated.
    ///     中文说明：Creature whose bar is being evaluated.
    /// </param>
    public readonly record struct HealthBarVisualGraftContext(Creature Creature);

    /// <summary>
    ///     Extra HP-length grafted onto the right end of the current HP fill for bar geometry and right-side forecasts.
    ///     Extra HP-length grafted onto the right end of the current HP fill 用于 bar geometry 和 right-side 用于ecasts.
    /// </summary>
    /// <param name="GraftHp">
    ///     Additional HP units drawn past the current HP edge along the bar.
    ///     中文说明：Additional HP units drawn past the current HP edge along the bar.
    /// </param>
    /// <param name="GraftSelfModulate">
    ///     Optional tint for the graft strip; null uses a default extension color.
    ///     可选 tint 用于 the graft strip; null 使用 a default extension color.
    /// </param>
    /// <param name="GraftMaterial">
    ///     Optional material for the graft strip.
    ///     可选 材质 用于 the graft strip.
    /// </param>
    public readonly record struct HealthBarVisualGraftMetrics(
        int GraftHp,
        Color? GraftSelfModulate,
        Material? GraftMaterial)
    {
        /// <summary>
        ///     Initializes metrics with no custom appearance.
        ///     初始化 metrics with no custom appearance。
        /// </summary>
        public HealthBarVisualGraftMetrics(int graftHp)
            : this(graftHp, null, null)
        {
        }
    }

    /// <summary>
    ///     Supplies visual graft metrics for a creature (temporary HP bar extension, etc.).
    ///     Supplies visual graft metrics 用于 a creature (temporary HP bar extension, etc.).
    /// </summary>
    public interface IHealthBarVisualGraftSource
    {
        /// <summary>
        ///     Returns graft metrics for <paramref name="context" />; yield zero
        ///     返回 graft metrics 用于 <c>context</c>; yield zero
        ///     <see cref="HealthBarVisualGraftMetrics.GraftHp" />
        ///     when none apply.
        ///     当 none apply.
        /// </summary>
        HealthBarVisualGraftMetrics GetHealthBarVisualGraft(HealthBarVisualGraftContext context);
    }

    /// <summary>
    ///     Aggregates graft metrics from creature powers and registered providers.
    ///     Aggregates graft metrics 从 creature 能力s 和 已注册 providers.
    /// </summary>
    public static class HealthBarVisualGraftRegistry
    {
        private static readonly Lock SyncRoot = new();
        private static readonly Dictionary<(string ModId, string SourceId), ProviderEntry> Providers = [];
        private static long _nextRegistrationOrder;

        /// <summary>
        ///     Registers or replaces a graft source implemented by <typeparamref name="TSource" />.
        ///     注册 or replaces a graft source implemented by <c>TSource</c>。
        /// </summary>
        public static void Register<TSource>(string modId, string? sourceId = null)
            where TSource : IHealthBarVisualGraftSource, new()
        {
            Register(modId, sourceId ?? typeof(TSource).FullName ?? typeof(TSource).Name, new TSource());
        }

        /// <summary>
        ///     Registers or replaces a graft source instance.
        ///     注册 or replaces a graft source instance。
        /// </summary>
        public static void Register(string modId, string sourceId, IHealthBarVisualGraftSource source)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentException.ThrowIfNullOrWhiteSpace(sourceId);
            ArgumentNullException.ThrowIfNull(source);

            lock (SyncRoot)
            {
                var key = (modId, sourceId);
                var registrationOrder = Providers.TryGetValue(key, out var existing)
                    ? existing.RegistrationOrder
                    : _nextRegistrationOrder++;

                Providers[key] = new(modId, sourceId, source, registrationOrder);
            }
        }

        /// <summary>
        ///     Removes a previously registered graft source.
        ///     Removes a previously 已注册 graft source.
        /// </summary>
        public static bool Unregister(string modId, string sourceId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentException.ThrowIfNullOrWhiteSpace(sourceId);

            lock (SyncRoot)
            {
                return Providers.Remove((modId, sourceId));
            }
        }

        /// <summary>
        ///     Sums graft HP from powers and registered providers; first non-null appearance wins for tint/material.
        ///     Sums graft HP 从 能力s 和 已注册 providers; first non-null appearance wins 用于 tint/材质.
        /// </summary>
        internal static HealthBarVisualGraftMetrics Aggregate(Creature creature)
        {
            ArgumentNullException.ThrowIfNull(creature);

            var sumHp = 0;
            Color? color = null;
            Material? material = null;
            var context = new HealthBarVisualGraftContext(creature);

            foreach (var source in creature.Powers.OfType<IHealthBarVisualGraftSource>())
                try
                {
                    var m = source.GetHealthBarVisualGraft(context);
                    sumHp += Math.Max(0, m.GraftHp);
                    color ??= m.GraftSelfModulate;
                    material ??= m.GraftMaterial;
                }
                catch (Exception ex)
                {
                    RitsuLibFramework.Logger.Warn(
                        $"[HealthBarGraft] Power '{source.GetType().FullName}' graft failed for '{creature}': {ex}");
                }

            ProviderEntry[] snapshot;
            lock (SyncRoot)
            {
                snapshot = Providers.Values.OrderBy(e => e.RegistrationOrder).ToArray();
            }

            foreach (var entry in snapshot)
                try
                {
                    var m = entry.Source.GetHealthBarVisualGraft(context);
                    sumHp += Math.Max(0, m.GraftHp);
                    color ??= m.GraftSelfModulate;
                    material ??= m.GraftMaterial;
                }
                catch (Exception ex)
                {
                    RitsuLibFramework.Logger.Warn(
                        $"[HealthBarGraft] Source '{entry.SourceId}' from mod '{entry.ModId}' failed for '{creature}': {ex}");
                }

            return new(sumHp, color, material);
        }

        private readonly record struct ProviderEntry(
            string ModId,
            string SourceId,
            IHealthBarVisualGraftSource Source,
            long RegistrationOrder);
    }
}
