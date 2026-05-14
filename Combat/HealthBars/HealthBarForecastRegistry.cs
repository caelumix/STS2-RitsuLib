using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace STS2RitsuLib.Combat.HealthBars
{
    /// <summary>
    ///     Which side of the health bar a forecast segment should grow from.
    ///     Which side of the health bar a 用于ecast segment should grow 从.
    /// </summary>
    public enum HealthBarForecastGrowthDirection
    {
        /// <summary>
        ///     Grows inward from the current HP edge, like poison.
        ///     Grows inward 从 the current HP edge, like poison.
        /// </summary>
        FromRight = 0,

        /// <summary>
        ///     Grows outward from the empty side, like doom.
        ///     Grows outward 从 the empty side, like doom.
        /// </summary>
        FromLeft = 1,
    }

    /// <summary>
    ///     How <see cref="HealthBarForecastGrowthDirection.FromLeft" /> segments share the empty-edge origin.
    ///     中文说明：How <c>HealthBarForecastGrowthDirection.FromLeft</c> segments share the empty-edge origin.
    /// </summary>
    public enum HealthBarForecastLeftOriginLayout
    {
        /// <summary>
        ///     Segments connect end-to-end from the empty edge (legacy layout).
        ///     Segments connect end-to-end 从 the empty edge (legacy layout).
        /// </summary>
        Chained = 0,

        /// <summary>
        ///     Each segment spans from the empty edge by its own <c>Amount</c> (capped to remaining HP). Larger
        ///     Each segment spans 从 the empty edge 通过 its own <c>Amount</c> (capped to remaining HP). Larger
        ///     <c>Amount</c> is drawn behind; smaller in front. Equal widths rotate front/back on a timer.
        /// </summary>
        OverlapFromOrigin = 1,
    }

    /// <summary>
    ///     One forecast overlay segment for a creature health bar.
    ///     One 用于ecast overlay segment 用于 a creature health bar.
    /// </summary>
    /// <param name="Amount">
    ///     HP amount represented by this segment.
    ///     HP amount represented 通过 this segment.
    /// </param>
    /// <param name="Color">
    ///     Lethal HP label theming; also used as the forecast nine-patch <see cref="CanvasItem.SelfModulate" /> when
    ///     Lethal HP label theming; also used as the 用于ecast nine-patch <c>CanvasItem.SelfModulate</c> 当
    ///     <see cref="OverlaySelfModulate" /> is null.
    /// </param>
    /// <param name="Direction">
    ///     Which edge the segment grows from.
    ///     Which edge the segment grows 从.
    /// </param>
    /// <param name="Order">
    ///     Lower values are rendered earlier in the chain.
    ///     中文说明：Lower values are rendered earlier in the chain.
    ///     For <see cref="HealthBarForecastGrowthDirection.FromRight" />, earlier segments stay closer to the current HP
    ///     中文说明：For <c>HealthBarForecastGrowthDirection.FromRight</c>, earlier segments stay closer to the current HP
    ///     edge; for <see cref="HealthBarForecastGrowthDirection.FromLeft" />, earlier segments stay closer to the empty
    ///     edge; 用于 <c>HealthBarForecastGrowthDirection.FromLeft</c>, earlier segments stay closer to the empty
    ///     edge.
    ///     中文说明：edge.
    /// </param>
    /// <param name="OverlayMaterial">
    ///     Optional Godot material (e.g. shader like vanilla doom). When null, only <see cref="Color" /> tint applies.
    ///     可选 Godot 材质 (e.g. shader like 原版 doom). 当 null, only <c>Color</c> tint applies.
    /// </param>
    /// <param name="OverlaySelfModulate">
    ///     Optional <see cref="CanvasItem.SelfModulate" /> for the forecast nine-patch. When null, <see cref="Color" /> is
    ///     可选 <c>CanvasItem.SelfModulate</c> 用于 the 用于ecast nine-patch. 当 null, <c>Color</c> is
    ///     used
    ///     used
    ///     for both overlay tint and lethal HP label; when set, <see cref="Color" /> is still used for lethal label theming.
    ///     用于 both overlay tint 和 lethal HP label; 当 设置, <c>Color</c> is still used 用于 lethal label theming.
    /// </param>
    /// <param name="LeftOriginLayout">
    ///     For <see cref="HealthBarForecastGrowthDirection.FromLeft" /> only:
    ///     中文说明：For <c>HealthBarForecastGrowthDirection.FromLeft</c> only:
    ///     <see cref="HealthBarForecastLeftOriginLayout.Chained" />
    ///     or <see cref="HealthBarForecastLeftOriginLayout.OverlapFromOrigin" />. Ignored for
    ///     or <c>HealthBarForecastLeftOriginLayout.OverlapFromOrigin</c>. Ignored 用于
    ///     <see cref="HealthBarForecastGrowthDirection.FromRight" />.
    /// </param>
    /// <param name="LeftExclusiveZGroup">
    ///     For <see cref="HealthBarForecastLeftOriginLayout.OverlapFromOrigin" />: larger values draw above smaller values.
    ///     中文说明：For <c>HealthBarForecastLeftOriginLayout.OverlapFromOrigin</c>: larger values draw above smaller values.
    ///     Within the same group, longer strips sit behind shorter strips; equal widths rotate.
    ///     中文说明：Within the same group, longer strips sit behind shorter strips; equal widths rotate.
    /// </param>
    public readonly record struct HealthBarForecastSegment(
        int Amount,
        Color Color,
        HealthBarForecastGrowthDirection Direction,
        int Order,
        Material? OverlayMaterial,
        Color? OverlaySelfModulate = null,
        HealthBarForecastLeftOriginLayout LeftOriginLayout = HealthBarForecastLeftOriginLayout.Chained,
        int LeftExclusiveZGroup = 0)
    {
        /// <summary>
        ///     Initializes a segment without overlay material or separate overlay modulate.
        ///     初始化 a segment without overlay material or separate overlay modulate。
        /// </summary>
        public HealthBarForecastSegment(int amount, Color color, HealthBarForecastGrowthDirection direction,
            int order = 0)
            : this(amount, color, direction, order, null, null)
        {
        }

        /// <summary>
        ///     Initializes a segment with an optional <see cref="OverlayMaterial" /> and default overlay modulate.
        ///     初始化 a segment with an optional <c>OverlayMaterial</c> and default overlay modulate。
        /// </summary>
        // ReSharper disable once RedundantOverload.Global
        public HealthBarForecastSegment(
            int amount,
            Color color,
            HealthBarForecastGrowthDirection direction,
            int order,
            Material? overlayMaterial)
            : this(amount, color, direction, order, overlayMaterial, null)
        {
        }
    }

    /// <summary>
    ///     Helpers for common turn-relative ordering of forecast segments.
    ///     Helpers 用于 common turn-relative ordering of 用于ecast segments.
    /// </summary>
    public static class HealthBarForecastOrder
    {
        /// <summary>
        ///     Returns an order key for effects that trigger at the start of <paramref name="triggerSide" />'s turn.
        ///     返回 an order key for effects that trigger at the start of <c>triggerSide</c>'s turn。
        /// </summary>
        public static int ForSideTurnStart(Creature creature, CombatSide triggerSide)
        {
            ArgumentNullException.ThrowIfNull(creature);
            return creature.CombatState?.CurrentSide == triggerSide ? 1 : 0;
        }

        /// <summary>
        ///     Returns an order key for effects that trigger at the end of <paramref name="triggerSide" />'s turn.
        ///     返回 an order key for effects that trigger at the end of <c>triggerSide</c>'s turn。
        /// </summary>
        public static int ForSideTurnEnd(Creature creature, CombatSide triggerSide)
        {
            ArgumentNullException.ThrowIfNull(creature);
            return creature.CombatState?.CurrentSide == triggerSide ? 0 : 1;
        }
    }

    /// <summary>
    ///     Global registry of health bar forecast providers contributed by mods.
    ///     Global 注册表 of health bar 用于ecast providers contributed 通过 mods.
    /// </summary>
    public static class HealthBarForecastRegistry
    {
        private static readonly Lock SyncRoot = new();
        private static readonly Dictionary<(string ModId, string ProviderId), ProviderEntry> Providers = [];
        private static long _nextRegistrationOrder;

        /// <summary>
        ///     Registers or replaces a forecast provider for <paramref name="modId" />.
        ///     注册 or replaces a forecast provider for <c>modId</c>。
        /// </summary>
        /// <typeparam name="TSource">
        ///     Concrete <see cref="IHealthBarForecastSource" /> with a parameterless constructor.
        ///     Concrete <c>IHealthBarForecastSource</c> 带有 a parameterless constructor.
        /// </typeparam>
        /// <param name="modId">
        ///     Owning mod identifier.
        ///     中文说明：Owning mod identifier.
        /// </param>
        /// <param name="sourceId">
        ///     Optional unique id; defaults to the type full name.
        ///     可选 unique id; defaults to the type full name.
        /// </param>
        public static void Register<TSource>(string modId, string? sourceId = null)
            where TSource : IHealthBarForecastSource, new()
        {
            Register(modId, sourceId ?? typeof(TSource).FullName ?? typeof(TSource).Name, new TSource());
        }

        /// <summary>
        ///     Registers or replaces a forecast source instance for <paramref name="modId" />.
        ///     注册 or replaces a forecast source instance for <c>modId</c>。
        /// </summary>
        /// <param name="modId">
        ///     Owning mod identifier.
        ///     中文说明：Owning mod identifier.
        /// </param>
        /// <param name="sourceId">
        ///     Unique id for this source within the mod.
        ///     Unique id 用于 this source 带有in the mod.
        /// </param>
        /// <param name="source">
        ///     Provider instance.
        ///     中文说明：Provider instance.
        /// </param>
        public static void Register(
            string modId,
            string sourceId,
            IHealthBarForecastSource source)
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
        ///     Removes a previously registered provider.
        ///     Removes a previously 已注册 provider.
        /// </summary>
        /// <param name="modId">
        ///     Mod identifier used at registration.
        ///     Mod identifier used at 注册.
        /// </param>
        /// <param name="sourceId">
        ///     Source id used at registration.
        ///     来源 id used at registration。
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if an entry was removed.
        ///     <see langword="true" /> 如果 an entry was removed.
        /// </returns>
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
        ///     Collects segments from powers implementing <see cref="IHealthBarForecastSource" /> and registered providers.
        ///     Collects segments 从 能力s implementing <c>IHealthBarForecastSource</c> 和 已注册 providers.
        /// </summary>
        /// <param name="creature">
        ///     Creature whose bar is being evaluated.
        ///     中文说明：Creature whose bar is being evaluated.
        /// </param>
        internal static IReadOnlyList<RegisteredHealthBarForecastSegment> GetSegments(Creature creature)
        {
            ArgumentNullException.ThrowIfNull(creature);

            var context = new HealthBarForecastContext(creature);
            List<RegisteredHealthBarForecastSegment> segments = [];

            var powerSequenceOrder = 0L;
            // ReSharper disable once SuspiciousTypeConversion.Global
            foreach (var source in creature.Powers.OfType<IHealthBarForecastSource>())
                AppendSegments(
                    source,
                    source.GetType().FullName ?? source.GetType().Name,
                    context,
                    powerSequenceOrder++,
                    segments);

            ProviderEntry[] snapshot;
            lock (SyncRoot)
            {
                snapshot = Providers.Values.OrderBy(entry => entry.RegistrationOrder).ToArray();
            }

            const long externalSourceOrderOffset = 1_000_000L;
            foreach (var entry in snapshot)
                AppendSegments(
                    entry.Source,
                    entry.SourceId,
                    context,
                    externalSourceOrderOffset + entry.RegistrationOrder,
                    segments,
                    entry.ModId);

            return segments;
        }

        private static void AppendSegments(
            IHealthBarForecastSource source,
            string sourceId,
            HealthBarForecastContext context,
            long sequenceOrder,
            List<RegisteredHealthBarForecastSegment> segments,
            string? modId = null)
        {
            try
            {
                var providedSegments = source.GetHealthBarForecastSegments(context);
                segments.AddRange(from segment in providedSegments
                    where segment.Amount > 0
                    select new RegisteredHealthBarForecastSegment(segment, sequenceOrder));
            }
            catch (Exception ex)
            {
                var ownerText = modId == null ? "runtime source" : $"mod '{modId}'";
                RitsuLibFramework.Logger.Warn(
                    $"[HealthBarForecast] Source '{sourceId}' from {ownerText} failed for creature '{context.Creature}': {ex}");
            }
        }

        /// <summary>
        ///     Segment plus a sequence key for stable ordering when <see cref="HealthBarForecastSegment.Order" /> ties.
        ///     Segment plus a sequence key 用于 stable ordering 当 <c>HealthBarForecastSegment.Order</c> ties.
        /// </summary>
        /// <param name="Segment">
        ///     Forecast data.
        ///     中文说明：Forecast data.
        /// </param>
        /// <param name="SequenceOrder">
        ///     Monotonic key (powers first, then registered sources).
        ///     Monotonic key (能力s first, then 已注册 sources).
        /// </param>
        internal readonly record struct RegisteredHealthBarForecastSegment(
            HealthBarForecastSegment Segment,
            long SequenceOrder);

        private readonly record struct ProviderEntry(
            string ModId,
            string SourceId,
            IHealthBarForecastSource Source,
            long RegistrationOrder);
    }
}
