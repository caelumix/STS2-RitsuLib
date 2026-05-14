using Godot;

namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Handle for loop-oriented playback.
    ///     Handle 用于 loop-oriented playback.
    /// </summary>
    public sealed class AudioLoopHandle(AudioSource source, AudioLifecycleScope scope, GodotObject? rawInstance)
        : AudioHandleBase(source, scope, rawInstance)
    {
        /// <summary>
        ///     Restarts the loop by stopping and playing it again.
        ///     Restarts the loop 通过 stopping 和 playing it again.
        /// </summary>
        public bool TryRestart()
        {
            return TryStop() && TryPlay();
        }
    }
}
