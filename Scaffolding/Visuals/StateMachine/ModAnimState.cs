namespace STS2RitsuLib.Scaffolding.Visuals.StateMachine
{
    /// <summary>
    ///     Backend-agnostic animation state, semantically equivalent to
    ///     <see cref="MegaCrit.Sts2.Core.Animation.AnimState" /> but usable from any
    ///     <see cref="IAnimationBackend" /> (Spine, Godot animation player, animated sprite, cue frame sequences).
    ///     与后端无关的动画状态，语义上等价于 <c>MegaCrit.Sts2.Core.Animation.AnimState</c>，但可用于任何
    ///     <c>IAnimationBackend</c>（Spine、Godot animation player、animated sprite、cue 帧序列）。
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Transitions follow the vanilla pattern:
    ///         转换遵循原版模式：
    ///     </para>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <see cref="NextState" /> is consumed only when the current animation completes (non-looping) or
    ///                 when the backend signals completion; if <see langword="null" />, the state is preserved.
    ///                 <c>NextState</c> 只会在当前动画完成（非循环）或后端发出完成信号时被消费；
    ///                 若为 <see langword="null" />，状态会保持不变。
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see cref="CallTrigger" /> resolves branches added via <see cref="AddBranch" />; branches may
    ///                 declare an optional guard <see cref="System.Func{TResult}" />.
    ///                 <c>CallTrigger</c> 会解析通过 <c>AddBranch</c> 添加的分支；分支可以声明可选 guard
    ///                 <see cref="System.Func{TResult}" />。
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public sealed class ModAnimState
    {
        private readonly Dictionary<string, List<Branch>> _branches = new(StringComparer.Ordinal);

        /// <summary>
        ///     Creates a new state bound to backend animation <paramref name="id" />.
        ///     创建绑定到后端动画 <c>id</c> 的新状态。
        /// </summary>
        /// <param name="id">
        ///     Animation id resolved by <see cref="IAnimationBackend.HasAnimation" />.
        ///     由 <c>IAnimationBackend.HasAnimation</c> 解析的动画 id。
        /// </param>
        /// <param name="isLooping">
        ///     When <see langword="true" />, the backend is asked to loop playback.
        ///     为 <see langword="true" /> 时，请求后端循环播放。
        /// </param>
        public ModAnimState(string id, bool isLooping = false)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            Id = id;
            IsLooping = isLooping;
        }

        /// <summary>
        ///     Backend animation id (Spine track, Godot animation name, cue key, or sprite-frames animation name).
        ///     后端动画 id（Spine track、Godot 动画名、cue key 或 sprite-frames 动画名）。
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Whether the state loops while active.
        ///     此状态激活时是否循环。
        /// </summary>
        public bool IsLooping { get; }

        /// <summary>
        ///     Optional follow-up state used by <see cref="ModAnimStateMachine" /> when this state completes.
        ///     此状态完成时供 <c>ModAnimStateMachine</c> 使用的可选后续状态。
        /// </summary>
        /// <remarks>
        ///     Keep <see langword="null" /> for terminal states (e.g. <c>die</c>) so completion does not advance.
        ///     对终止状态（例如 <c>die</c>）保持 <see langword="null" />，这样完成后不会继续推进。
        /// </remarks>
        public ModAnimState? NextState { get; set; }

        /// <summary>
        ///     Optional bounds-container tag forwarded through
        ///     <see cref="ModAnimStateMachine.BoundsUpdated" /> on start and completion.
        ///     可选 bounds-container 标签，会在开始和完成时通过 <c>ModAnimStateMachine.BoundsUpdated</c> 转发。
        /// </summary>
        public string? BoundsContainer { get; init; }

        /// <summary>
        ///     <see langword="true" /> once a looping state has completed at least one full cycle.
        ///     循环状态至少完成一个完整周期后为 <see langword="true" />。
        /// </summary>
        public bool HasLooped { get; private set; }

        /// <summary>
        ///     Adds a conditional branch to <paramref name="target" /> for trigger <paramref name="trigger" />.
        ///     为 trigger <c>trigger</c> 添加到 <c>target</c> 的条件分支。
        /// </summary>
        /// <param name="trigger">
        ///     Trigger name compared verbatim during <see cref="CallTrigger" />.
        ///     在 <c>CallTrigger</c> 中逐字比较的 trigger 名称。
        /// </param>
        /// <param name="target">
        ///     State to transition to when the trigger fires and <paramref name="condition" /> passes.
        ///     trigger 触发且 <c>condition</c> 通过时要转入的状态。
        /// </param>
        /// <param name="condition">
        ///     Optional guard evaluated at trigger time; <see langword="null" /> means always.
        ///     在 trigger 时评估的可选 guard；<see langword="null" /> 表示总是通过。
        /// </param>
        public void AddBranch(string trigger, ModAnimState target, Func<bool>? condition = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(trigger);
            ArgumentNullException.ThrowIfNull(target);

            if (!_branches.TryGetValue(trigger, out var list))
            {
                list = [];
                _branches[trigger] = list;
            }

            list.Add(new(target, condition));
        }

        /// <summary>
        ///     Resolves the first matching branch for <paramref name="trigger" /> whose guard passes,
        ///     or <see langword="null" /> when no branch is eligible.
        ///     解析 <c>trigger</c> 第一个 guard 通过的匹配分支；没有合格分支时返回 <see langword="null" />。
        /// </summary>
        public ModAnimState? CallTrigger(string trigger)
        {
            return !_branches.TryGetValue(trigger, out var list)
                ? null
                : (from branch in list where branch.Condition == null || branch.Condition() select branch.Target)
                .FirstOrDefault();
        }

        /// <summary>
        ///     <see langword="true" /> when at least one branch is registered for <paramref name="trigger" />.
        ///     当至少有一个分支注册到 <c>trigger</c> 时返回 <see langword="true" />。
        /// </summary>
        public bool HasTrigger(string trigger)
        {
            return _branches.ContainsKey(trigger);
        }

        /// <summary>
        ///     Marks the state as having completed one loop iteration (for bounds / debug logic).
        ///     标记此状态已完成一次循环迭代（用于 bounds / debug 逻辑）。
        /// </summary>
        public void MarkHasLooped()
        {
            HasLooped = true;
        }

        private readonly record struct Branch(ModAnimState Target, Func<bool>? Condition);
    }
}
