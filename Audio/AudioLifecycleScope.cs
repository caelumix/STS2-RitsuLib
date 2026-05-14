namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Built-in lifecycle buckets for automatic audio cleanup.
    ///     Built-in lifecycle buckets 用于 automatic audio cleanup.
    /// </summary>
    public enum AudioLifecycleScope
    {
        /// <summary>
        ///     Caller-managed only.
        ///     中文说明：Caller-managed only.
        /// </summary>
        Manual = 0,

        /// <summary>
        ///     Stops when combat ends.
        ///     Stops 当 combat ends.
        /// </summary>
        Combat = 1,

        /// <summary>
        ///     Stops when the current room is exited.
        ///     Stops 当 the current room is exited.
        /// </summary>
        Room = 2,

        /// <summary>
        ///     Stops when the run ends.
        ///     Stops 当 the 跑局 ends.
        /// </summary>
        Run = 3,

        /// <summary>
        ///     Reserved for screen-scoped flows.
        ///     Reserved 用于 screen-scoped flows.
        /// </summary>
        Screen = 4,
    }
}
