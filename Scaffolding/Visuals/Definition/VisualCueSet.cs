namespace STS2RitsuLib.Scaffolding.Visuals.Definition
{
    /// <summary>
    ///     Immutable per-cue visuals: one static texture and/or a <see cref="VisualFrameSequence" /> per cue name. Used for
    ///     combat, game-over, merchant / rest-site shells, ancient foreground layers, and similar.
    ///     不可变的逐 cue 视觉：每个 cue 名称对应一个静态纹理和/或一个 <see cref="VisualFrameSequence" />。用于
    ///     战斗、游戏结束、商人 / 休息处外壳、远古事件前景图层等。
    /// </summary>
    /// <param name="TexturePathByCue">
    ///     One texture per cue key.
    ///     每个 cue key 对应一个贴图。
    /// </param>
    /// <param name="FrameSequenceByCue">
    ///     Overrides <paramref name="TexturePathByCue" /> for the same cue key when present.
    ///     存在相同 cue 键时，覆盖 <paramref name="TexturePathByCue" />。
    /// </param>
    public sealed record VisualCueSet(
        IReadOnlyDictionary<string, string>? TexturePathByCue = null,
        IReadOnlyDictionary<string, VisualFrameSequence>? FrameSequenceByCue = null);
}
