using Godot;
using MegaCrit.Sts2.Core.Combat;

namespace STS2RitsuLib.Combat.HealthBars
{
    /// <summary>
    ///     Convenience helpers for building health bar forecast segments.
    ///     Convenience helpers 用于 building health bar 用于ecast segments.
    /// </summary>
    public static class HealthBarForecasts
    {
        /// <summary>
        ///     Starts a general-purpose sequence builder for <paramref name="context" />.
        ///     Starts a general-purpose sequence builder 用于 <c>context</c>.
        /// </summary>
        public static HealthBarForecastSequenceBuilder For(HealthBarForecastContext context)
        {
            return new(context);
        }

        /// <summary>
        ///     Starts a right-growing forecast lane with a fixed <paramref name="color" />.
        ///     Starts a right-growing 用于ecast lane 带有 a fixed <c>color</c>.
        /// </summary>
        public static HealthBarForecastLaneBuilder FromRight(HealthBarForecastContext context, Color color)
        {
            return FromRight(context, color, null);
        }

        /// <summary>
        ///     Starts a right-growing lane with separate optional <see cref="CanvasItem.SelfModulate" /> for the nine-patch
        ///     Starts a right-growing lane 带有 separate 可选 <c>CanvasItem.SelfModulate</c> 用于 the nine-patch
        ///     overlay (e.g. white when <see cref="Godot.Material" /> carries tint).
        ///     overlay (e.g. white 当 <c>Godot.材质</c> carries tint).
        /// </summary>
        /// <param name="context">
        ///     Forecast context.
        ///     中文说明：Forecast context.
        /// </param>
        /// <param name="color">
        ///     Lethal label color and fallback overlay modulate.
        ///     Lethal label color 和 fallback overlay modulate.
        /// </param>
        /// <param name="overlaySelfModulate">
        ///     When set, used as overlay <see cref="CanvasItem.SelfModulate" /> instead of
        ///     当 设置, used as overlay <c>CanvasItem.SelfModulate</c> instead of
        ///     <paramref name="color" />.
        /// </param>
        public static HealthBarForecastLaneBuilder FromRight(
            HealthBarForecastContext context,
            Color color,
            Color? overlaySelfModulate)
        {
            return new(For(context), color, HealthBarForecastGrowthDirection.FromRight, overlaySelfModulate);
        }

        /// <summary>
        ///     Starts a left-growing forecast lane with a fixed <paramref name="color" />.
        ///     Starts a left-growing 用于ecast lane 带有 a fixed <c>color</c>.
        /// </summary>
        public static HealthBarForecastLaneBuilder FromLeft(HealthBarForecastContext context, Color color)
        {
            return FromLeft(context, color, null);
        }

        /// <inheritdoc cref="FromRight(HealthBarForecastContext, Color, Color?)" />
        public static HealthBarForecastLaneBuilder FromLeft(
            HealthBarForecastContext context,
            Color color,
            Color? overlaySelfModulate)
        {
            return new(For(context), color, HealthBarForecastGrowthDirection.FromLeft, overlaySelfModulate);
        }

        /// <summary>
        ///     Returns a single segment when <paramref name="amount" /> is positive, with optional material only.
        ///     返回 a single segment when <c>amount</c> is positive, with optional material only。
        /// </summary>
        public static IEnumerable<HealthBarForecastSegment> Single(
            int amount,
            Color color,
            HealthBarForecastGrowthDirection direction,
            int order,
            Material? overlayMaterial)
        {
            return Single(amount, color, direction, order, overlayMaterial, null);
        }

        /// <summary>
        ///     Returns a single segment when <paramref name="amount" /> is positive, with optional material and overlay
        ///     返回 a single segment 当 <c>amount</c> is positive, 带有 可选 材质 和 overlay
        ///     <see cref="CanvasItem.SelfModulate" />.
        /// </summary>
        /// <param name="amount">
        ///     HP chunk size.
        ///     中文说明：HP chunk size.
        /// </param>
        /// <param name="color">
        ///     Lethal label color and fallback modulate.
        ///     Lethal label color 和 fallback modulate.
        /// </param>
        /// <param name="direction">
        ///     Growth direction.
        ///     中文说明：Growth direction.
        /// </param>
        /// <param name="order">
        ///     Sort order among segments.
        ///     中文说明：Sort order among segments.
        /// </param>
        /// <param name="overlayMaterial">
        ///     Optional segment material.
        ///     可选 segment 材质.
        /// </param>
        /// <param name="overlaySelfModulate">
        ///     When set, stored on <see cref="HealthBarForecastSegment.OverlaySelfModulate" />.
        ///     当 设置, stored on <c>HealthBarForecastSegment.OverlaySelfModulate</c>.
        /// </param>
        public static IEnumerable<HealthBarForecastSegment> Single(
            int amount,
            Color color,
            HealthBarForecastGrowthDirection direction,
            int order,
            Material? overlayMaterial,
            Color? overlaySelfModulate)
        {
            if (amount <= 0)
                return [];

            return [new(amount, color, direction, order, overlayMaterial, overlaySelfModulate)];
        }

        /// <summary>
        ///     Returns a single segment when <paramref name="amount" /> is positive, without a custom material.
        ///     返回 a single segment when <c>amount</c> is positive, without a custom material。
        /// </summary>
        public static IEnumerable<HealthBarForecastSegment> Single(
            int amount,
            Color color,
            HealthBarForecastGrowthDirection direction,
            int order = 0)
        {
            return Single(amount, color, direction, order, null, null);
        }
    }

    /// <summary>
    ///     Mutable builder for one forecast source's ordered segment sequence.
    ///     Mutable builder 用于 one 用于ecast source's ordered segment sequence.
    /// </summary>
    public sealed class HealthBarForecastSequenceBuilder(HealthBarForecastContext context)
    {
        private readonly List<HealthBarForecastSegment> _segments = [];

        /// <summary>
        ///     Forecast context associated with this sequence.
        ///     Forecast context associated 带有 this sequence.
        /// </summary>
        public HealthBarForecastContext Context { get; } = context;

        /// <summary>
        ///     Appends a segment when <paramref name="amount" /> is positive.
        ///     Appends a segment 当 <c>amount</c> is positive.
        ///     Consecutive segments with identical color, direction, order, material reference, and overlay modulate are merged.
        ///     Consecutive segments 带有 identical color, direction, order, 材质 reference, 和 overlay modulate are merged.
        /// </summary>
        public HealthBarForecastSequenceBuilder Add(
            int amount,
            Color color,
            HealthBarForecastGrowthDirection direction,
            int order,
            Material? overlayMaterial)
        {
            return Add(amount, color, direction, order, overlayMaterial, null);
        }

        /// <summary>
        ///     Appends a segment when <paramref name="amount" /> is positive, with explicit overlay modulate.
        ///     Appends a segment 当 <c>amount</c> is positive, 带有 explicit overlay modulate.
        /// </summary>
        /// <param name="amount">
        ///     HP chunk size.
        ///     中文说明：HP chunk size.
        /// </param>
        /// <param name="color">
        ///     Lethal label color and fallback modulate.
        ///     Lethal label color 和 fallback modulate.
        /// </param>
        /// <param name="direction">
        ///     Growth direction.
        ///     中文说明：Growth direction.
        /// </param>
        /// <param name="order">
        ///     Sort order among segments.
        ///     中文说明：Sort order among segments.
        /// </param>
        /// <param name="overlayMaterial">
        ///     Optional segment material.
        ///     可选 segment 材质.
        /// </param>
        /// <param name="overlaySelfModulate">
        ///     Optional overlay <see cref="CanvasItem.SelfModulate" />; null uses <paramref name="color" />.
        ///     可选 overlay <c>CanvasItem.SelfModulate</c>; null 使用 <c>color</c>.
        /// </param>
        public HealthBarForecastSequenceBuilder Add(
            int amount,
            Color color,
            HealthBarForecastGrowthDirection direction,
            int order,
            Material? overlayMaterial,
            Color? overlaySelfModulate)
        {
            if (amount <= 0)
                return this;

            var segment =
                new HealthBarForecastSegment(amount, color, direction, order, overlayMaterial, overlaySelfModulate);
            if (_segments.Count > 0)
            {
                var last = _segments[^1];
                if (CanMerge(last, segment))
                {
                    _segments[^1] = last with { Amount = last.Amount + segment.Amount };
                    return this;
                }
            }

            _segments.Add(segment);
            return this;
        }

        /// <summary>
        ///     Appends a segment without a custom material.
        ///     Appends a segment 带有out a 自定义 材质.
        /// </summary>
        public HealthBarForecastSequenceBuilder Add(
            int amount,
            Color color,
            HealthBarForecastGrowthDirection direction,
            int order = 0)
        {
            return Add(amount, color, direction, order, null, null);
        }

        /// <summary>
        ///     Appends all positive amounts as consecutive segments.
        ///     中文说明：Appends all positive amounts as consecutive segments.
        /// </summary>
        public HealthBarForecastSequenceBuilder AddRange(
            IEnumerable<int> amounts,
            Color color,
            HealthBarForecastGrowthDirection direction,
            int order,
            Material? overlayMaterial)
        {
            return AddRange(amounts, color, direction, order, overlayMaterial, null);
        }

        /// <summary>
        ///     Appends all positive amounts as consecutive segments with explicit overlay modulate.
        ///     Appends all positive amounts as consecutive segments 带有 explicit overlay modulate.
        /// </summary>
        /// <param name="amounts">
        ///     HP chunk sizes.
        ///     中文说明：HP chunk sizes.
        /// </param>
        /// <param name="color">
        ///     Lethal label color and fallback modulate.
        ///     Lethal label color 和 fallback modulate.
        /// </param>
        /// <param name="direction">
        ///     Growth direction.
        ///     中文说明：Growth direction.
        /// </param>
        /// <param name="order">
        ///     Sort order among segments.
        ///     中文说明：Sort order among segments.
        /// </param>
        /// <param name="overlayMaterial">
        ///     Optional segment material.
        ///     可选 segment 材质.
        /// </param>
        /// <param name="overlaySelfModulate">
        ///     Optional overlay <see cref="CanvasItem.SelfModulate" /> shared by chunks.
        ///     可选 overlay <c>CanvasItem.SelfModulate</c> shared 通过 chunks.
        /// </param>
        public HealthBarForecastSequenceBuilder AddRange(
            IEnumerable<int> amounts,
            Color color,
            HealthBarForecastGrowthDirection direction,
            int order,
            Material? overlayMaterial,
            Color? overlaySelfModulate)
        {
            ArgumentNullException.ThrowIfNull(amounts);

            foreach (var amount in amounts)
                Add(amount, color, direction, order, overlayMaterial, overlaySelfModulate);

            return this;
        }

        /// <summary>
        ///     Appends all positive amounts as consecutive segments without a custom material.
        ///     Appends all positive amounts as consecutive segments 带有out a 自定义 材质.
        /// </summary>
        public HealthBarForecastSequenceBuilder AddRange(
            IEnumerable<int> amounts,
            Color color,
            HealthBarForecastGrowthDirection direction,
            int order = 0)
        {
            return AddRange(amounts, color, direction, order, null, null);
        }

        /// <summary>
        ///     Appends segments that trigger at the start of <paramref name="triggerSide" />'s turn.
        ///     中文说明：Appends segments that trigger at the start of <c>triggerSide</c>'s turn.
        /// </summary>
        public HealthBarForecastSequenceBuilder AddSideTurnStart(
            CombatSide triggerSide,
            Color color,
            HealthBarForecastGrowthDirection direction,
            params int[] amounts)
        {
            return AddRange(
                amounts,
                color,
                direction,
                HealthBarForecastOrder.ForSideTurnStart(Context.Creature, triggerSide));
        }

        /// <summary>
        ///     Appends segments that trigger at the end of <paramref name="triggerSide" />'s turn.
        ///     中文说明：Appends segments that trigger at the end of <c>triggerSide</c>'s turn.
        /// </summary>
        public HealthBarForecastSequenceBuilder AddSideTurnEnd(
            CombatSide triggerSide,
            Color color,
            HealthBarForecastGrowthDirection direction,
            params int[] amounts)
        {
            return AddRange(
                amounts,
                color,
                direction,
                HealthBarForecastOrder.ForSideTurnEnd(Context.Creature, triggerSide));
        }

        /// <summary>
        ///     Creates a fixed-color right-growing lane on this sequence.
        ///     创建 a fixed-color right-growing lane on this sequence。
        /// </summary>
        public HealthBarForecastLaneBuilder FromRight(Color color)
        {
            return FromRight(color, null);
        }

        /// <inheritdoc cref="HealthBarForecasts.FromRight(HealthBarForecastContext, Color, Color?)" />
        public HealthBarForecastLaneBuilder FromRight(Color color, Color? overlaySelfModulate)
        {
            return new(this, color, HealthBarForecastGrowthDirection.FromRight, overlaySelfModulate);
        }

        /// <summary>
        ///     Creates a fixed-color left-growing lane on this sequence.
        ///     创建 a fixed-color left-growing lane on this sequence。
        /// </summary>
        public HealthBarForecastLaneBuilder FromLeft(Color color)
        {
            return FromLeft(color, null);
        }

        /// <inheritdoc cref="FromRight(Color, Color?)" />
        public HealthBarForecastLaneBuilder FromLeft(Color color, Color? overlaySelfModulate)
        {
            return new(this, color, HealthBarForecastGrowthDirection.FromLeft, overlaySelfModulate);
        }

        /// <summary>
        ///     Returns the built sequence snapshot.
        ///     返回 the built sequence snapshot。
        /// </summary>
        public IReadOnlyList<HealthBarForecastSegment> Build()
        {
            return _segments.Count == 0 ? [] : _segments.ToArray();
        }

        private static bool CanMerge(HealthBarForecastSegment left, HealthBarForecastSegment right)
        {
            return left.Color == right.Color &&
                   left.Direction == right.Direction &&
                   left.Order == right.Order &&
                   left.OverlaySelfModulate == right.OverlaySelfModulate &&
                   left.LeftOriginLayout == right.LeftOriginLayout &&
                   left.LeftExclusiveZGroup == right.LeftExclusiveZGroup &&
                   ReferenceEquals(left.OverlayMaterial, right.OverlayMaterial);
        }
    }

    /// <summary>
    ///     Convenience wrapper for the common case of one fixed-color forecast lane.
    ///     Convenience wrapper 用于 the common case of one fixed-color 用于ecast lane.
    /// </summary>
    /// <param name="sequence">
    ///     Parent sequence builder.
    ///     中文说明：Parent sequence builder.
    /// </param>
    /// <param name="color">
    ///     Lane label / fallback modulate color.
    ///     中文说明：Lane label / fallback modulate color.
    /// </param>
    /// <param name="direction">
    ///     Growth edge for this lane.
    ///     Growth edge 用于 this lane.
    /// </param>
    /// <param name="overlaySelfModulate">
    ///     When set, used as <see cref="CanvasItem.SelfModulate" /> for segments in this lane.
    ///     当 设置, used as <c>CanvasItem.SelfModulate</c> 用于 segments in this lane.
    /// </param>
    public sealed class HealthBarForecastLaneBuilder(
        HealthBarForecastSequenceBuilder sequence,
        Color color,
        HealthBarForecastGrowthDirection direction,
        Color? overlaySelfModulate = null)
    {
        /// <summary>
        ///     Parent sequence builder.
        ///     中文说明：Parent sequence builder.
        /// </summary>
        public HealthBarForecastSequenceBuilder Sequence { get; } = sequence;

        /// <summary>
        ///     Appends a segment with explicit <paramref name="order" /> and optional <paramref name="overlayMaterial" />.
        ///     Appends a segment 带有 explicit <c>order</c> 和 可选 <c>overlay材质</c>.
        /// </summary>
        public HealthBarForecastLaneBuilder Add(int amount, int order, Material? overlayMaterial)
        {
            Sequence.Add(amount, color, direction, order, overlayMaterial, overlaySelfModulate);
            return this;
        }

        /// <summary>
        ///     Appends a segment without a custom material.
        ///     Appends a segment 带有out a 自定义 材质.
        /// </summary>
        public HealthBarForecastLaneBuilder Add(int amount, int order = 0)
        {
            return Add(amount, order, null);
        }

        /// <summary>
        ///     Appends multiple segments with the same <paramref name="order" /> and optional <paramref name="overlayMaterial" />.
        ///     Appends multiple segments 带有 the same <c>order</c> 和 可选 <c>overlay材质</c>.
        /// </summary>
        public HealthBarForecastLaneBuilder AddRange(IEnumerable<int> amounts, int order, Material? overlayMaterial)
        {
            Sequence.AddRange(amounts, color, direction, order, overlayMaterial, overlaySelfModulate);
            return this;
        }

        /// <summary>
        ///     Appends multiple segments without a custom material.
        ///     Appends multiple segments 带有out a 自定义 材质.
        /// </summary>
        public HealthBarForecastLaneBuilder AddRange(IEnumerable<int> amounts, int order = 0)
        {
            return AddRange(amounts, order, null);
        }

        /// <summary>
        ///     Appends segments that trigger at the start of <paramref name="triggerSide" />'s turn.
        ///     中文说明：Appends segments that trigger at the start of <c>triggerSide</c>'s turn.
        /// </summary>
        public HealthBarForecastLaneBuilder AtSideTurnStart(CombatSide triggerSide, params int[] amounts)
        {
            var order = HealthBarForecastOrder.ForSideTurnStart(Sequence.Context.Creature, triggerSide);
            Sequence.AddRange(amounts, color, direction, order, null, overlaySelfModulate);
            return this;
        }

        /// <summary>
        ///     Appends segments that trigger at the end of <paramref name="triggerSide" />'s turn.
        ///     中文说明：Appends segments that trigger at the end of <c>triggerSide</c>'s turn.
        /// </summary>
        public HealthBarForecastLaneBuilder AtSideTurnEnd(CombatSide triggerSide, params int[] amounts)
        {
            var order = HealthBarForecastOrder.ForSideTurnEnd(Sequence.Context.Creature, triggerSide);
            Sequence.AddRange(amounts, color, direction, order, null, overlaySelfModulate);
            return this;
        }

        /// <summary>
        ///     Starts another right-growing lane on the same parent sequence.
        ///     中文说明：Starts another right-growing lane on the same parent sequence.
        /// </summary>
        public HealthBarForecastLaneBuilder ThenFromRight(Color nextColor)
        {
            return Sequence.FromRight(nextColor, null);
        }

        /// <summary>
        ///     Starts another left-growing lane on the same parent sequence.
        ///     中文说明：Starts another left-growing lane on the same parent sequence.
        /// </summary>
        public HealthBarForecastLaneBuilder ThenFromLeft(Color nextColor)
        {
            return Sequence.FromLeft(nextColor, null);
        }

        /// <summary>
        ///     Returns the built segment snapshot.
        ///     返回 the built segment snapshot。
        /// </summary>
        public IReadOnlyList<HealthBarForecastSegment> Build()
        {
            return Sequence.Build();
        }
    }
}
