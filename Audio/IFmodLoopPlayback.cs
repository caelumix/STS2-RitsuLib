namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Looping events keyed by path (vanilla loop dictionary semantics).
    ///     Looping 事件s keyed 通过 路径 (原版 loop dictionary semantics).
    /// </summary>
    public interface IFmodLoopPlayback
    {
        /// <summary>
        ///     Starts or continues a loop; <paramref name="usesLoopParam" /> matches vanilla loop-parameter convention.
        ///     Starts 或 continues a loop; <c>使用LoopParam</c> matches 原版 loop-parameter convention.
        /// </summary>
        void PlayLoop(string eventPath, bool usesLoopParam = true);

        /// <summary>
        ///     Stops a previously started loop for <paramref name="eventPath" />.
        ///     Stops a previously started loop 用于 <c>事件路径</c>.
        /// </summary>
        void StopLoop(string eventPath);

        /// <summary>
        ///     Sets a parameter on the active loop instance for <paramref name="eventPath" />.
        ///     设置 a parameter on the active loop instance 用于 <c>事件路径</c>.
        /// </summary>
        void SetLoopParameter(string eventPath, string parameterName, float value);

        /// <summary>
        ///     Stops every managed loop.
        ///     中文说明：Stops every managed loop.
        /// </summary>
        void StopAllLoops();
    }
}
