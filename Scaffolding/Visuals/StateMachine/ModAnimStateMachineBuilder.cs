using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Visuals.Definition;
using STS2RitsuLib.Scaffolding.Visuals.StateMachine.Backends;

namespace STS2RitsuLib.Scaffolding.Visuals.StateMachine
{
    /// <summary>
    ///     Fluent builder for <see cref="ModAnimStateMachine" />. Declare named states, per-state loop / next-state
    ///     / bounds metadata, per-state branches, and any-state transitions; finalise by calling one of the
    ///     <c>Build</c> overloads with an <see cref="IAnimationBackend" /> or a visuals root.
    ///     <c>ModAnimStateMachine</c> 的流式 builder。可声明命名状态、逐状态循环 / next-state /
    ///     bounds 元数据、逐状态分支和 any-state 转换；最后用 <c>IAnimationBackend</c> 或视觉根节点调用某个
    ///     <c>Build</c> 重载完成构建。
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The builder does not validate ids against backend animation availability: if a state id is unresolvable
    ///         by the chosen backend, <see cref="ModAnimStateMachine" /> will skip playback for that state and log a
    ///         warning on entry.
    ///         Builder 不会根据后端动画可用性校验 id：如果某个状态 id 无法由所选后端解析，
    ///         <c>ModAnimStateMachine</c> 会在进入该状态时跳过播放并记录 warning。
    ///     </para>
    /// </remarks>
    public sealed class ModAnimStateMachineBuilder
    {
        private readonly List<AnyBranchDraft> _anyBranches = [];
        private readonly Dictionary<string, StateDraft> _states = new(StringComparer.Ordinal);
        private string? _initialStateId;

        private ModAnimStateMachineBuilder()
        {
        }

        /// <summary>
        ///     Creates a fresh builder.
        ///     创建一个新的 builder。
        /// </summary>
        public static ModAnimStateMachineBuilder Create()
        {
            return new();
        }

        /// <summary>
        ///     Declares a state with backend animation id <paramref name="id" /> and loop hint
        ///     <paramref name="loop" />. Returns a scope object so the caller can chain
        ///     <see cref="StateScope.WithNext" />, <see cref="StateScope.WithBounds" />, and
        ///     <see cref="StateScope.AsInitial" />.
        ///     声明一个使用后端动画 id <c>id</c> 和循环提示 <c>loop</c> 的状态。
        ///     返回 scope 对象，方便调用方继续链式调用 <c>StateScope.WithNext</c>、
        ///     <c>StateScope.WithBounds</c> 和 <c>StateScope.AsInitial</c>。
        /// </summary>
        public StateScope AddState(string id, bool loop = false)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            if (_states.ContainsKey(id))
                throw new InvalidOperationException($"State '{id}' already declared.");

            var draft = new StateDraft(id, loop);
            _states[id] = draft;
            _initialStateId ??= id;
            return new(this, draft);
        }

        /// <summary>
        ///     Adds a branch from state <paramref name="fromId" /> for trigger <paramref name="trigger" /> to state
        ///     <paramref name="toId" />. Optional <paramref name="condition" /> guards activation.
        ///     为 trigger <c>trigger</c> 添加从状态 <c>fromId</c> 到状态
        ///     <c>toId</c> 的分支。可选 <c>condition</c> 用作激活 guard。
        /// </summary>
        public ModAnimStateMachineBuilder AddBranch(string fromId, string trigger, string toId,
            Func<bool>? condition = null)
        {
            if (!_states.TryGetValue(fromId, out var draft))
                throw new InvalidOperationException($"Source state '{fromId}' not declared.");

            draft.Branches.Add(new(trigger, toId, condition));
            return this;
        }

        /// <summary>
        ///     Adds an any-state branch for trigger <paramref name="trigger" /> to <paramref name="toId" />
        ///     (guarded by optional <paramref name="condition" />).
        ///     为 trigger <c>trigger</c> 添加到 <c>toId</c> 的 any-state 分支
        ///     （可由可选 <c>condition</c> 保护）。
        /// </summary>
        public ModAnimStateMachineBuilder AddAnyState(string trigger, string toId, Func<bool>? condition = null)
        {
            _anyBranches.Add(new(trigger, toId, condition));
            return this;
        }

        /// <summary>
        ///     Materialises the graph against <paramref name="backend" /> and returns a started
        ///     <see cref="ModAnimStateMachine" />.
        ///     针对 <c>backend</c> 实体化状态图，并返回已启动的 <c>ModAnimStateMachine</c>。
        /// </summary>
        public ModAnimStateMachine Build(IAnimationBackend backend)
        {
            ArgumentNullException.ThrowIfNull(backend);
            var machine = BuildCore(backend, out var initial);
            machine.Start(initial);
            return machine;
        }

        /// <summary>
        ///     Convenience overload: wraps <paramref name="controller" /> in a <see cref="SpineAnimationBackend" />
        ///     and builds the machine.
        ///     便捷重载：将 <c>controller</c> 包装为 <c>SpineAnimationBackend</c> 并构建状态机。
        /// </summary>
        public ModAnimStateMachine BuildSpine(MegaSprite controller)
        {
            return Build(new SpineAnimationBackend(controller));
        }

        /// <summary>
        ///     Convenience overload: composes cue / Spine / Godot animation player / animated-sprite backends
        ///     rooted at <paramref name="visualsRoot" /> and builds the machine.
        ///     便捷重载：以 <c>visualsRoot</c> 为根组合 cue / Spine / Godot animation player /
        ///     animated-sprite 后端，并构建状态机。
        /// </summary>
        /// <param name="visualsRoot">
        ///     Visuals root (typically an <see cref="MegaCrit.Sts2.Core.Nodes.Combat.NCreatureVisuals" />).
        ///     视觉根节点（通常是 <c>MegaCrit.Sts2.Core.Nodes.Combat.NCreatureVisuals</c>）。
        /// </param>
        /// <param name="character">
        ///     Optional character model; used when <paramref name="cueSet" /> is <see langword="null" />
        ///     to pull cue data from <c>IModCharacterAssetOverrides</c>.
        ///     可选角色模型；当 <c>cueSet</c> 为 <see langword="null" /> 时，用于从
        ///     <c>IModCharacterAssetOverrides</c> 拉取 cue 数据。
        /// </param>
        /// <param name="cueSet">
        ///     Optional explicit cue set; takes priority over the character-derived one.
        ///     可选显式 cue set；优先于从角色派生的 cue set。
        /// </param>
        public ModAnimStateMachine BuildForVisualsRoot(Node visualsRoot, CharacterModel? character = null,
            VisualCueSet? cueSet = null)
        {
            var backend = CompositeBackendFactory.Build(visualsRoot, character, cueSet);
            return Build(backend);
        }

        private ModAnimStateMachine BuildCore(IAnimationBackend backend, out ModAnimState initial)
        {
            if (_initialStateId == null)
                throw new InvalidOperationException("No states declared.");

            var materialised = new Dictionary<string, ModAnimState>(StringComparer.Ordinal);

            foreach (var (id, draft) in _states)
                materialised[id] = new(draft.Id, draft.Loop) { BoundsContainer = draft.BoundsContainer };

            foreach (var (id, draft) in _states)
            {
                var state = materialised[id];
                if (draft.NextStateId != null && materialised.TryGetValue(draft.NextStateId, out var next))
                    state.NextState = next;

                foreach (var branch in draft.Branches)
                {
                    if (!materialised.TryGetValue(branch.ToId, out var target))
                        continue;

                    state.AddBranch(branch.Trigger, target, branch.Condition);
                }
            }

            initial = materialised[_initialStateId];
            var machine = new ModAnimStateMachine(backend);
            foreach (var branch in _anyBranches)
            {
                if (!materialised.TryGetValue(branch.ToId, out var target))
                    continue;

                machine.AddAnyState(branch.Trigger, target, branch.Condition);
            }

            return machine;
        }

        internal sealed class StateDraft(string id, bool loop)
        {
            public string Id { get; } = id;
            public bool Loop { get; } = loop;
            public string? NextStateId { get; set; }
            public string? BoundsContainer { get; set; }
            public List<BranchDraft> Branches { get; } = [];
        }

        internal readonly record struct BranchDraft(string Trigger, string ToId, Func<bool>? Condition);

        private readonly record struct AnyBranchDraft(string Trigger, string ToId, Func<bool>? Condition);

        /// <summary>
        ///     Fluent scope returned by <see cref="ModAnimStateMachineBuilder.AddState" /> for per-state metadata.
        ///     <c>ModAnimStateMachineBuilder.AddState</c> 返回的流式 scope，用于设置逐状态元数据。
        /// </summary>
        public sealed class StateScope
        {
            private readonly StateDraft _draft;
            private readonly ModAnimStateMachineBuilder _owner;

            internal StateScope(ModAnimStateMachineBuilder owner, StateDraft draft)
            {
                _owner = owner;
                _draft = draft;
            }

            /// <summary>
            ///     Sets <see cref="ModAnimState.NextState" /> for the current state (by target id).
            ///     为当前状态设置 <c>ModAnimState.NextState</c>（通过目标 id）。
            /// </summary>
            public StateScope WithNext(string nextStateId)
            {
                _draft.NextStateId = nextStateId;
                return this;
            }

            /// <summary>
            ///     Sets <see cref="ModAnimState.BoundsContainer" /> tag emitted via
            ///     <see cref="ModAnimStateMachine.BoundsUpdated" /> on enter.
            ///     设置进入状态时通过 <c>ModAnimStateMachine.BoundsUpdated</c> 发出的
            ///     <c>ModAnimState.BoundsContainer</c> 标签。
            /// </summary>
            public StateScope WithBounds(string boundsContainer)
            {
                _draft.BoundsContainer = boundsContainer;
                return this;
            }

            /// <summary>
            ///     Marks the current state as the initial state (overrides the auto-first-state behaviour).
            ///     将当前状态标记为初始状态（覆盖自动使用第一个状态的行为）。
            /// </summary>
            public StateScope AsInitial()
            {
                _owner._initialStateId = _draft.Id;
                return this;
            }

            /// <summary>
            ///     Returns the owning builder so chaining can continue.
            ///     返回所属 builder，以便继续链式调用。
            /// </summary>
            public ModAnimStateMachineBuilder Done()
            {
                return _owner;
            }
        }
    }
}
