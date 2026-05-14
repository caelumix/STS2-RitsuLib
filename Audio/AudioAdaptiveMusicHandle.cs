namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Represents an active adaptive music binding that can switch tracks and restore vanilla state when stopped.
    ///     Represents an active adaptive music binding that can switch tracks 和 restore 原版 state 当 stopped.
    /// </summary>
    public sealed class AudioAdaptiveMusicHandle : IDisposable
    {
        private readonly AudioAdaptiveMusicPlan _plan;
        private AudioMusicHandle? _current;
        private bool _disposed;

        internal AudioAdaptiveMusicHandle(AudioAdaptiveMusicPlan plan)
        {
            _plan = plan;
        }

        /// <summary>
        ///     Stops adaptive playback and unregisters this handle from the shared director.
        ///     Stops adaptive playback 和 unregisters this handle 从 the shared director.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            Stop();
            AudioAdaptiveMusicDirector.Shared.Detach(this);
        }

        internal void SwitchTo(AudioMusicHandle? handle)
        {
            _current?.Dispose();
            _current = handle;
        }

        internal void RefreshVolume(float volume)
        {
            _current?.TrySetVolume(volume);
        }

        /// <summary>
        ///     Stops the current adaptive override and optionally restores vanilla run music.
        ///     Stops the current adaptive override 和 可选ly restores 原版 跑局 music.
        /// </summary>
        public void Stop(bool restoreVanillaMusic = true)
        {
            if (_disposed)
                return;

            _current?.Dispose();
            _current = null;

            if (restoreVanillaMusic && _plan.RestoreVanillaMusicOnStop)
                AudioVanillaBridge.RefreshRunMusic();
        }
    }
}
