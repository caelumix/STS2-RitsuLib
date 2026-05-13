namespace STS2RitsuLib.Scaffolding.Visuals.Definition
{
    /// <summary>
    ///     Immutable per-cue visuals: one static texture and/or a <see cref="VisualFrameSequence" /> per cue name. Used for
    ///     combat, game-over, merchant / rest-site shells, ancient foreground layers, and similar.
    ///     不可变的逐 cue 视觉定义：每个 cue 名称可对应一个静态贴图和/或一个 <see cref="VisualFrameSequence" />。
    ///     用于战斗、游戏结束、商人 / 休息点外壳、ancient 前景图层等。
    /// </summary>
    /// <param name="TexturePathByCue">
    ///     One texture per cue key.
    ///     每个 cue key 对应一个贴图。
    /// </param>
    /// <param name="FrameSequenceByCue">
    ///     Overrides <paramref name="TexturePathByCue" /> for the same cue key when present.
    ///     同一 cue key 同时存在时，会覆盖 <paramref name="TexturePathByCue" />。
    /// </param>
    public sealed record VisualCueSet(
        IReadOnlyDictionary<string, string>? TexturePathByCue = null,
        IReadOnlyDictionary<string, VisualFrameSequence>? FrameSequenceByCue = null);
}
