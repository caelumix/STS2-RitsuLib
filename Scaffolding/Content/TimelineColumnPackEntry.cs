using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Timeline;
using STS2RitsuLib.Timeline;
using STS2RitsuLib.Timeline.Scaffolding;
using STS2RitsuLib.Unlocks.Patches;

namespace STS2RitsuLib.Scaffolding.Content
{
    /// <summary>
    ///     Declarative registration for one <see cref="StoryModel" /> column: epoch order, per-epoch unlock bindings, and
    ///     <see cref="ModTimelineRegistry.RegisterStory{TStory}" /> — in a single fluent block instead of many separate
    ///     <see cref="IModContentPackEntry" /> rows.
    ///     单个 <see cref="StoryModel" /> 列的声明式注册：epoch 顺序、逐 epoch 解锁绑定，以及
    ///     <see cref="ModTimelineRegistry.RegisterStory{TStory}" />。它们可以写在一个 fluent 块中，而不是拆成许多
    ///     <see cref="IModContentPackEntry" /> 行。
    /// </summary>
    public sealed class TimelineColumnPackEntry<TStory> : IModContentPackEntry
        where TStory : StoryModel, new()
    {
        private readonly Action<TimelineColumnBuilder<TStory>> _configure;

        /// <summary>
        ///     Creates an entry that runs <paramref name="configure" /> when the content pack is applied.
        ///     创建一个在应用 content pack 时运行 <paramref name="configure" /> 的条目。
        /// </summary>
        public TimelineColumnPackEntry(Action<TimelineColumnBuilder<TStory>> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);
            _configure = configure;
        }

        /// <inheritdoc />
        public void Apply(ModContentPackContext context)
        {
            var builder = new TimelineColumnBuilder<TStory>(context);
            _configure(builder);
            builder.Run();
        }
    }

    /// <summary>
    ///     Fluent builder for <see cref="TimelineColumnPackEntry{TStory}" />.
    ///     <see cref="TimelineColumnPackEntry{TStory}" /> 的流式 builder。
    /// </summary>
    public sealed class TimelineColumnBuilder<TStory>
        where TStory : StoryModel, new()
    {
        private readonly ModContentPackContext _context;
        private readonly List<Action> _steps = [];

        internal TimelineColumnBuilder(ModContentPackContext context)
        {
            _context = context;
        }

        internal void Run()
        {
            foreach (var step in _steps)
                step();
        }

        /// <summary>
        ///     Registers <typeparamref name="TEpoch" /> on the story column, then optional slot configuration (pool defaults,
        ///     gated lists, etc.). For <see cref="ModEpochTemplate" /> epochs, timeline layout must be registered before freeze;
        ///     when using this builder, the typical approach is to call
        ///     <see cref="EpochSlotBuilder{TEpoch}.TimelineSlot" /> or <see cref="EpochSlotBuilder{TEpoch}.AutoTimelineSlot" />
        ///     inside <paramref name="slot" /> (or register elsewhere before freeze, e.g. ModContentPackBuilder ModEpoch*
        ///     helpers), so apply-time validation can run (conflicts with vanilla throw at apply time).
        ///     Execution order matches call order; later <c>RequireEpoch</c> for the same model overrides earlier ones.
        ///     在 story 列上注册 <typeparamref name="TEpoch" />，然后执行可选 slot 配置（池默认值、受限列表等）。
        ///     对于 <see cref="ModEpochTemplate" /> epoch，时间线布局必须在 freeze 前注册；使用此 builder 时，典型做法是在
        ///     <paramref name="slot" /> 中调用 <see cref="EpochSlotBuilder{TEpoch}.TimelineSlot" /> 或
        ///     <see cref="EpochSlotBuilder{TEpoch}.AutoTimelineSlot" />（也可以在 freeze 前通过其它位置注册，例如
        ///     ModContentPackBuilder 的 ModEpoch* helper），这样应用阶段校验可以运行（与原版冲突会在应用时抛出）。
        ///     执行顺序与调用顺序一致；同一模型后续的 <c>RequireEpoch</c> 会覆盖之前的绑定。
        /// </summary>
        public TimelineColumnBuilder<TStory> Epoch<TEpoch>(Action<EpochSlotBuilder<TEpoch>>? slot = null)
            where TEpoch : EpochModel, new()
        {
            if (slot != null)
            {
                var b = new EpochSlotBuilder<TEpoch>(_context);
                slot(b);
                foreach (var step in b.DrainSteps())
                    _steps.Add(step);
            }

            _steps.Add(() => _context.Timeline.RegisterStoryEpoch<TStory, TEpoch>());
            return this;
        }

        /// <summary>
        ///     Registers <typeparamref name="TStory" /> for vanilla story discovery (call once at the end of the column).
        ///     将 <typeparamref name="TStory" /> 注册到原版 story 发现流程（通常在列的末尾调用一次）。
        /// </summary>
        public TimelineColumnBuilder<TStory> RegisterStory()
        {
            _steps.Add(() => _context.Timeline.RegisterStory<TStory>());
            return this;
        }
    }

    /// <summary>
    ///     Per-epoch unlock hooks for the callback passed to
    ///     <see cref="TimelineColumnBuilder{TStory}" /><c>.Epoch&lt;TEpoch&gt;(...)</c>.
    ///     传给 <see cref="TimelineColumnBuilder{TStory}" /><c>.Epoch&lt;TEpoch&gt;(...)</c> 回调的逐 epoch 解锁钩子。
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         You can call these methods multiple times inside one epoch slot; they run in order. A later
    ///         <c>RequireEpoch</c> for the same model overwrites the earlier epoch binding.
    ///         可以在一个 epoch slot 内多次调用这些方法；它们会按顺序运行。同一模型后续的 <c>RequireEpoch</c>
    ///         会覆盖较早的 epoch 绑定。
    ///     </para>
    ///     <para>
    ///         Anything registered through <see cref="Unlocks.ModUnlockRegistry.RequireEpoch(Type,string)" /> is gated by
    ///         <see cref="Unlocks.ModUnlockRegistry.FilterUnlocked{TModel}" /> /
    ///         <see cref="Unlocks.ModUnlockRegistry.IsUnlocked" />.
    ///         Integrations include <see cref="CharacterUnlockFilterPatch" />, <see cref="SharedAncientUnlockFilterPatch" />,
    ///         <see cref="CardUnlockFilterPatch" />, <see cref="RelicUnlockFilterPatch" />,
    ///         <see cref="PotionUnlockFilterPatch" />,
    ///         and <see cref="GeneratedRoomEventUnlockFilterPatch" />.
    ///         任何通过 <see cref="Unlocks.ModUnlockRegistry.RequireEpoch(Type,string)" /> 注册的内容，都会受到
    ///         <see cref="Unlocks.ModUnlockRegistry.FilterUnlocked{TModel}" /> /
    ///         <see cref="Unlocks.ModUnlockRegistry.IsUnlocked" /> 限制。集成点包括
    ///         <see cref="CharacterUnlockFilterPatch" />、<see cref="SharedAncientUnlockFilterPatch" />、
    ///         <see cref="CardUnlockFilterPatch" />、<see cref="RelicUnlockFilterPatch" />、
    ///         <see cref="PotionUnlockFilterPatch" /> 和 <see cref="GeneratedRoomEventUnlockFilterPatch" />。
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             Cards · gate an entire pool behind this epoch: <c>RequireAllCardsInPool&lt;TCardPool&gt;()</c> (only
    ///             <see cref="Unlocks.ModUnlockRegistry.RequireEpoch(Type,string)" />; does not register
    ///             <see cref="ModEpochGatedContentRegistry" />).
    ///             卡牌 · 将整个池限制在此 epoch 之后：<c>RequireAllCardsInPool&lt;TCardPool&gt;()</c>（仅注册
    ///             <see cref="Unlocks.ModUnlockRegistry.RequireEpoch(Type,string)" />；不会注册
    ///             <see cref="ModEpochGatedContentRegistry" />）。
    ///         </item>
    ///         <item>
    ///             Cards · explicit list + pack-declared unlock UI: <c>Cards(types)</c>; whole pool into registry:
    ///             <c>CardsFromPool&lt;TCardPool&gt;()</c>.
    ///             卡牌 · 显式列表 + pack 声明的解锁 UI：<c>Cards(types)</c>；将整个池写入 registry：
    ///             <c>CardsFromPool&lt;TCardPool&gt;()</c>。
    ///         </item>
    ///         <item>
    ///             Relics · whole pool: <c>RequireAllRelicsInPool&lt;TRelicPool&gt;()</c>.
    ///             遗物 · 整个池：<c>RequireAllRelicsInPool&lt;TRelicPool&gt;()</c>。
    ///         </item>
    ///         <item>
    ///             Relics · explicit or pool + registry: <c>Relics(types)</c>, <c>RelicsFromPool&lt;TRelicPool&gt;()</c>.
    ///             遗物 · 显式列表或池 + registry：<c>Relics(types)</c>、<c>RelicsFromPool&lt;TRelicPool&gt;()</c>。
    ///         </item>
    ///         <item>
    ///             Potions · whole pool: <c>RequireAllPotionsInPool&lt;TPotionPool&gt;()</c> (<c>RequireEpoch</c> only; not
    ///             <see cref="ModEpochGatedContentRegistry" />).
    ///             药水 · 整个池：<c>RequireAllPotionsInPool&lt;TPotionPool&gt;()</c>（仅 <c>RequireEpoch</c>；不会写入
    ///             <see cref="ModEpochGatedContentRegistry" />）。
    ///         </item>
    ///         <item>
    ///             Potions · explicit types: <c>Potions(types)</c>. For timeline potion presentation, subclass
    ///             <see cref="PotionUnlockEpochTemplate" /> for your <see cref="EpochModel" /> and implement
    ///             <c>PotionTypes</c>;
    ///             keep those CLR types aligned with <c>Potions</c> / <c>RequireEpoch</c> (this method already applies
    ///             <c>RequireEpoch</c>).
    ///             药水 · 显式类型：<c>Potions(types)</c>。若需要时间线药水展示，请让你的 <see cref="EpochModel" />
    ///             继承 <see cref="PotionUnlockEpochTemplate" /> 并实现 <c>PotionTypes</c>；保持这些 CLR 类型与
    ///             <c>Potions</c> / <c>RequireEpoch</c> 对齐（此方法已经应用 <c>RequireEpoch</c>）。
    ///         </item>
    ///     </list>
    /// </remarks>
    public sealed class EpochSlotBuilder<TEpoch>
        where TEpoch : EpochModel, new()
    {
        private readonly ModContentPackContext _context;
        private readonly List<Action> _pending = [];
        private bool? _axisIconEnabled;
        private bool _axisIconRuleSet;
        private string? _axisIconTexturePath;
        private Action? _layoutRegistration;

        internal EpochSlotBuilder(ModContentPackContext context)
        {
            _context = context;
        }

        /// <summary>
        ///     Reserves a fixed <see cref="EpochEra" /> column and <c>EraPosition</c> for this epoch. Conflicts with vanilla
        ///     or other mods throw at registration time.
        ///     为此 epoch 保留固定的 <see cref="EpochEra" /> 列和 <c>EraPosition</c>。与原版或其它 mod 冲突时会在注册时抛出。
        /// </summary>
        public EpochSlotBuilder<TEpoch> TimelineSlot(EpochEra era, int eraPosition)
        {
            var modId = _context.ModId;
            _layoutRegistration = () =>
                ModTimelineLayoutRegistry.RegisterTimelineSlot(typeof(TEpoch), era, eraPosition, modId);
            return this;
        }

        /// <summary>
        ///     Reserves the lowest free <c>EraPosition</c> in <paramref name="era" /> after seeding vanilla occupancy.
        ///     在填入原版占位后，在 <paramref name="era" /> 中保留最低的空闲 <c>EraPosition</c>。
        /// </summary>
        public EpochSlotBuilder<TEpoch> AutoTimelineSlot(EpochEra era)
        {
            var modId = _context.ModId;
            _layoutRegistration = () =>
                ModTimelineLayoutRegistry.RegisterAutoTimelineSlot(typeof(TEpoch), era, modId);
            return this;
        }

        /// <summary>
        ///     Reserves a column strictly to the left of <paramref name="anchorEra" /> (smaller era int), preferring a new
        ///     root cell at position 0 — use for a mod story “root” before the rest of your column content.
        ///     保留一个严格位于 <paramref name="anchorEra" /> 左侧的列（更小的 era int），优先使用 position 0 的新 root cell。
        ///     可用于在列的其它内容之前放置 mod story 的“根”。
        /// </summary>
        public EpochSlotBuilder<TEpoch> AutoTimelineSlotBeforeColumn(EpochEra anchorEra)
        {
            var modId = _context.ModId;
            _layoutRegistration = () =>
                ModTimelineLayoutRegistry.RegisterAutoTimelineSlotBeforeEraColumn(typeof(TEpoch), anchorEra, modId);
            return this;
        }

        /// <inheritdoc cref="ModTimelineLayoutRegistry.RegisterAutoTimelineSlotBeforeEpochColumn" />
        public EpochSlotBuilder<TEpoch> AutoTimelineSlotBeforeEpochColumn<TReferenceEpoch>()
            where TReferenceEpoch : EpochModel, new()
        {
            var modId = _context.ModId;
            _layoutRegistration = () =>
                ModTimelineLayoutRegistry.RegisterAutoTimelineSlotBeforeEpochColumn(typeof(TEpoch),
                    typeof(TReferenceEpoch), modId);
            return this;
        }

        /// <summary>
        ///     Reserves a column strictly to the right of <paramref name="anchorEra" /> (larger era int).
        ///     保留一个严格位于 <paramref name="anchorEra" /> 右侧的列（更大的 era int）。
        /// </summary>
        public EpochSlotBuilder<TEpoch> AutoTimelineSlotAfterColumn(EpochEra anchorEra)
        {
            var modId = _context.ModId;
            _layoutRegistration = () =>
                ModTimelineLayoutRegistry.RegisterAutoTimelineSlotAfterEraColumn(typeof(TEpoch), anchorEra, modId);
            return this;
        }

        /// <inheritdoc cref="ModTimelineLayoutRegistry.RegisterAutoTimelineSlotAfterEpochColumn" />
        public EpochSlotBuilder<TEpoch> AutoTimelineSlotAfterEpochColumn<TReferenceEpoch>()
            where TReferenceEpoch : EpochModel, new()
        {
            var modId = _context.ModId;
            _layoutRegistration = () =>
                ModTimelineLayoutRegistry.RegisterAutoTimelineSlotAfterEpochColumn(typeof(TEpoch),
                    typeof(TReferenceEpoch), modId);
            return this;
        }

        /// <summary>
        ///     Reserves a slot in the same era column as <paramref name="anchorEra" />, using the first free position in
        ///     that column.
        ///     在与 <paramref name="anchorEra" /> 相同的 era 列中保留一个 slot，使用该列中第一个空闲位置。
        /// </summary>
        public EpochSlotBuilder<TEpoch> AutoTimelineSlotInColumn(EpochEra anchorEra)
        {
            var modId = _context.ModId;
            _layoutRegistration = () =>
                ModTimelineLayoutRegistry.RegisterAutoTimelineSlotInEraColumn(typeof(TEpoch), anchorEra, modId);
            return this;
        }

        /// <summary>
        ///     Reserves a slot in the same era column as <typeparamref name="TReferenceEpoch" />, using the first free
        ///     position in that column.
        ///     在与 <typeparamref name="TReferenceEpoch" /> 相同的 era 列中保留一个 slot，使用该列中第一个空闲位置。
        /// </summary>
        public EpochSlotBuilder<TEpoch> AutoTimelineSlotInEpochColumn<TReferenceEpoch>()
            where TReferenceEpoch : EpochModel, new()
        {
            var modId = _context.ModId;
            _layoutRegistration = () =>
                ModTimelineLayoutRegistry.RegisterAutoTimelineSlotInEpochColumn(typeof(TEpoch),
                    typeof(TReferenceEpoch), modId);
            return this;
        }

        /// <summary>
        ///     Hides the axis icon for this epoch's resolved era column.
        ///     隐藏此 epoch 解析出的 era 列轴图标。
        /// </summary>
        public EpochSlotBuilder<TEpoch> DisableEraAxisIcon()
        {
            _axisIconRuleSet = true;
            _axisIconEnabled = false;
            return this;
        }

        /// <summary>
        ///     Enables the axis icon for this epoch's resolved era column.
        ///     启用此 epoch 解析出的 era 列轴图标。
        /// </summary>
        public EpochSlotBuilder<TEpoch> EnableEraAxisIcon()
        {
            _axisIconRuleSet = true;
            _axisIconEnabled = true;
            return this;
        }

        /// <summary>
        ///     Overrides axis icon texture for this epoch's resolved era column.
        ///     覆盖此 epoch 解析出的 era 列轴图标贴图。
        /// </summary>
        public EpochSlotBuilder<TEpoch> EraAxisIcon(string texturePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(texturePath);
            _axisIconRuleSet = true;
            _axisIconEnabled = true;
            _axisIconTexturePath = texturePath;
            return this;
        }

        internal List<Action> DrainSteps()
        {
            var copy = new List<Action>();
            if (_layoutRegistration != null)
                copy.Add(_layoutRegistration);
            if (_axisIconRuleSet)
                copy.Add(ApplyEraIconRule);
            copy.AddRange(_pending);
            _pending.Clear();
            _layoutRegistration = null;
            _axisIconRuleSet = false;
            _axisIconEnabled = null;
            _axisIconTexturePath = null;
            return copy;
        }

        private void ApplyEraIconRule()
        {
            var era = ModTimelineLayoutRegistry.ResolveEra(typeof(TEpoch));
            ModTimelineEraIconRegistry.Configure(era, _axisIconEnabled, _axisIconTexturePath);
        }

        /// <summary>
        ///     Every <see cref="CardModel" /> in <typeparamref name="TPool" /> for this mod requires
        ///     <typeparamref name="TEpoch" />
        ///     (no <see cref="ModEpochGatedContentRegistry" /> row — for default “whole pool until character” style gates).
        ///     此 mod 中 <typeparamref name="TPool" /> 内的每个 <see cref="CardModel" /> 都需要
        ///     <typeparamref name="TEpoch" />（不写入 <see cref="ModEpochGatedContentRegistry" /> 行，适用于默认的
        ///     “整个池直到角色解锁” 风格 gate）。
        /// </summary>
        public EpochSlotBuilder<TEpoch> RequireAllCardsInPool<TPool>()
            where TPool : CardPoolModel
        {
            _pending.Add(() => ModEpochGatedContentPackHelper.ApplyRequireAllPoolCards<TEpoch, TPool>(_context));
            return this;
        }

        /// <summary>
        ///     Every <see cref="RelicModel" /> in <typeparamref name="TPool" /> requires <typeparamref name="TEpoch" />.
        ///     <typeparamref name="TPool" /> 内的每个 <see cref="RelicModel" /> 都需要 <typeparamref name="TEpoch" />。
        /// </summary>
        public EpochSlotBuilder<TEpoch> RequireAllRelicsInPool<TPool>()
            where TPool : RelicPoolModel
        {
            _pending.Add(() => ModEpochGatedContentPackHelper.ApplyRequireAllPoolRelics<TEpoch, TPool>(_context));
            return this;
        }

        /// <summary>
        ///     Every <see cref="PotionModel" /> in <typeparamref name="TPool" /> requires <typeparamref name="TEpoch" />.
        ///     <typeparamref name="TPool" /> 内的每个 <see cref="PotionModel" /> 都需要 <typeparamref name="TEpoch" />。
        /// </summary>
        public EpochSlotBuilder<TEpoch> RequireAllPotionsInPool<TPool>()
            where TPool : PotionPoolModel
        {
            _pending.Add(() => ModEpochGatedContentPackHelper.ApplyRequireAllPoolPotions<TEpoch, TPool>(_context));
            return this;
        }

        /// <summary>
        ///     Explicit card types for unlock UI + <c>RequireEpoch</c> (see <see cref="PackDeclaredCardUnlockEpochTemplate" />).
        ///     用于解锁 UI + <c>RequireEpoch</c> 的显式卡牌类型列表（见 <see cref="PackDeclaredCardUnlockEpochTemplate" />）。
        /// </summary>
        public EpochSlotBuilder<TEpoch> Cards(IReadOnlyList<Type> types)
        {
            ArgumentNullException.ThrowIfNull(types);
            _pending.Add(() =>
                ModEpochGatedContentPackHelper.ApplyExplicitTypes<TEpoch>(_context, types, []));
            return this;
        }

        /// <summary>
        ///     Explicit relic types for unlock UI + <c>RequireEpoch</c>.
        ///     用于解锁 UI + <c>RequireEpoch</c> 的显式遗物类型列表。
        /// </summary>
        public EpochSlotBuilder<TEpoch> Relics(IReadOnlyList<Type> types)
        {
            ArgumentNullException.ThrowIfNull(types);
            _pending.Add(() =>
                ModEpochGatedContentPackHelper.ApplyExplicitTypes<TEpoch>(_context, [], types));
            return this;
        }

        /// <summary>
        ///     Explicit potion types — <c>RequireEpoch</c> only (no <see cref="ModEpochGatedContentRegistry" /> row).
        ///     Pair with <see cref="PotionUnlockEpochTemplate" /> on the epoch if you need timeline potion unlock presentation.
        ///     显式药水类型列表，仅应用 <c>RequireEpoch</c>（不写入 <see cref="ModEpochGatedContentRegistry" /> 行）。
        ///     如果需要时间线药水解锁展示，请在 epoch 上配合 <see cref="PotionUnlockEpochTemplate" />。
        /// </summary>
        public EpochSlotBuilder<TEpoch> Potions(IReadOnlyList<Type> types)
        {
            ArgumentNullException.ThrowIfNull(types);
            _pending.Add(() => ModEpochGatedContentPackHelper.ApplyExplicitPotions<TEpoch>(_context, types));
            return this;
        }

        /// <summary>
        ///     All relics registered in <typeparamref name="TRelicPool" /> for this mod — registry + <c>RequireEpoch</c>.
        ///     此 mod 中注册在 <typeparamref name="TRelicPool" /> 内的所有遗物：写入 registry + <c>RequireEpoch</c>。
        /// </summary>
        public EpochSlotBuilder<TEpoch> RelicsFromPool<TRelicPool>()
            where TRelicPool : RelicPoolModel
        {
            _pending.Add(() => ModEpochGatedContentPackHelper.ApplyRelicsFromPool<TEpoch, TRelicPool>(_context));
            return this;
        }

        /// <summary>
        ///     All cards registered in <typeparamref name="TCardPool" /> for this mod — registry + <c>RequireEpoch</c>.
        ///     此 mod 中注册在 <typeparamref name="TCardPool" /> 内的所有卡牌：写入 registry + <c>RequireEpoch</c>。
        /// </summary>
        public EpochSlotBuilder<TEpoch> CardsFromPool<TCardPool>()
            where TCardPool : CardPoolModel
        {
            _pending.Add(() => ModEpochGatedContentPackHelper.ApplyCardsFromPool<TEpoch, TCardPool>(_context));
            return this;
        }
    }
}
