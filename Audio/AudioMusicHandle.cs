using Godot;

namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Handle for long-lived music playback.
    ///     Handle 用于 long-lived music playback.
    /// </summary>
    public sealed class AudioMusicHandle(AudioSource source, AudioLifecycleScope scope, GodotObject? rawInstance)
        : AudioHandleBase(source, scope, rawInstance)
    {
        /// <summary>
        ///     Replaces this handle's playback with a new source.
        ///     Replaces this handle's playback 带有 a new source.
        /// </summary>
        public bool TrySwitchTo(AudioSource nextSource, AudioPlaybackOptions? options = null)
        {
            var next = GameFmod.Playback.Play(nextSource, options ?? new AudioPlaybackOptions { Scope = Scope });
            if (!next.Succeeded)
                return false;

            Dispose();
            return true;
        }
    }
}
