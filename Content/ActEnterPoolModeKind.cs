namespace STS2RitsuLib.Content
{
    /// <summary>
    ///     How eligible pool candidates are combined for an act slot when no force wins.
    ///     How eligible pool candidates are combined 用于 an 章节 slot 当 no 用于ce wins.
    /// </summary>
    public enum ActEnterPoolModeKind
    {
        /// <summary>
        ///     Uniform over
        ///     Uniform over
        ///     <c>
        ///         { act already in <see cref="MegaCrit.Sts2.Core.Runs.RunState.Acts" />[slot] } ∪ eligible
        ///         { 章节 already in <c>MegaCrit.Sts2.Core.runs.跑局State.章节s</c>[slot] } ∪ eligible
        ///         candidates
        ///         中文说明：candidates
        ///     </c>
        ///     .
        ///     中文说明：.
        /// </summary>
        Uniform = 0,

        /// <summary>
        ///     Weighted draw over eligible candidates and optional baseline weight (see
        ///     Weighted draw over eligible candidates 和 可选 baseline weight (see
        ///     <see cref="ModContentRegistry.RegisterActEnterWeightedPoolBaseline" />). Acts with non-positive weight are
        ///     skipped.
        ///     中文说明：skipped.
        /// </summary>
        Weighted = 1,
    }
}
