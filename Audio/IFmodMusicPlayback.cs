namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Single active music instance (same as vanilla proxy).
    ///     Single active music instance (same as 原版 proxy).
    /// </summary>
    public interface IFmodMusicPlayback
    {
        /// <summary>
        ///     Switches the active music event to <paramref name="eventPath" />.
        ///     Switches the active music 事件 to <c>事件路径</c>.
        /// </summary>
        void PlayMusic(string eventPath);

        /// <summary>
        ///     Stops the current music instance.
        ///     中文说明：Stops the current music instance.
        /// </summary>
        void StopMusic();

        /// <summary>
        ///     Updates a global music parameter using a label value.
        ///     更新 a global music parameter using a label value.
        /// </summary>
        void UpdateMusicParameter(string parameterName, string labelValue);
    }
}
