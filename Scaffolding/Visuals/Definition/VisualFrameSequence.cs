namespace STS2RitsuLib.Scaffolding.Visuals.Definition
{
    /// <summary>
    ///     Immutable ordered frames for one logical cue (combat, merchant room, ancient stage, …).
    ///     一个逻辑 cue 的不可变有序帧（战斗、商人房间、远古事件舞台等）。
    /// </summary>
    /// <param name="Frames">
    ///     At least one entry for playback.
    ///     至少包含一个用于播放的条目。
    /// </param>
    /// <param name="Loop">
    ///     Whether to restart after the last frame.
    ///     到达最后一帧后是否重新开始。
    /// </param>
    public sealed record VisualFrameSequence(
        IReadOnlyList<VisualFrame> Frames,
        bool Loop = false);
}
