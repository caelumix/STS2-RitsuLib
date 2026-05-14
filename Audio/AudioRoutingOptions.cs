namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Higher-level playback routing options such as singleton channels and tagged groups.
    ///     Higher-level playback routing options such as singleton channels 和 tagged groups.
    /// </summary>
    public sealed class AudioRoutingOptions
    {
        /// <summary>
        ///     Optional singleton channel name. New playback can keep or replace the current channel owner.
        ///     可选 singleton channel name. New playback can keep 或 replace the current channel owner.
        /// </summary>
        public string? Channel { get; init; }

        /// <summary>
        ///     Optional group tag for bulk stop or replacement patterns.
        ///     可选 group tag 用于 bulk stop 或 replacement patterns.
        /// </summary>
        public string? Tag { get; init; }

        /// <summary>
        ///     Channel collision behavior when <see cref="Channel" /> is already occupied.
        ///     Channel collision behavior 当 <c>Channel</c> is already occupied.
        /// </summary>
        public AudioChannelMode ChannelMode { get; init; } = AudioChannelMode.ReplaceExisting;

        /// <summary>
        ///     Whether replacement should allow fade-out for the previous owner.
        ///     表示是否 replacement should allow fade-out for the previous owner。
        /// </summary>
        public bool AllowFadeOutOnReplace { get; init; } = true;

        /// <summary>
        ///     When true and <see cref="Tag" /> is set, existing handles in that tag stop before the new handle is attached.
        ///     当 true 和 <c>Tag</c> is 设置, existing handles in that tag stop 之前 the new handle is attached.
        /// </summary>
        public bool ReplaceTaggedGroup { get; init; }
    }
}
