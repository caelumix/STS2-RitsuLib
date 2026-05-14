using STS2RitsuLib.Scaffolding.Visuals.Definition;

namespace STS2RitsuLib.Scaffolding.Visuals
{
    /// <summary>
    ///     Entry points for building <see cref="VisualCueSet" /> and <see cref="VisualFrameSequence" /> data used across
    ///     combat, world shells, ancient stages, etc.
    ///     构建 <c>VisualCueSet</c> 和 <c>VisualFrameSequence</c> 数据的入口点；这些数据可用于战斗、
    ///     世界场景外壳、ancient 舞台等。
    /// </summary>
    public static class ModVisualCues
    {
        /// <summary>
        ///     Begins a <see cref="VisualCueSet" /> builder.
        ///     开始一个 <c>VisualCueSet</c> builder。
        /// </summary>
        public static VisualCueSetBuilder CueSet()
        {
            return VisualCueSetBuilder.Create();
        }

        /// <summary>
        ///     Begins a <see cref="VisualFrameSequence" /> builder.
        ///     开始一个 <c>VisualFrameSequence</c> builder。
        /// </summary>
        public static VisualFrameSequenceBuilder FrameSequence()
        {
            return VisualFrameSequenceBuilder.Create();
        }
    }
}
