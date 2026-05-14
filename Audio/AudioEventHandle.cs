using Godot;

namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Typed handle for FMOD Studio event instances.
    ///     Typed handle 用于 FMOD Studio 事件 instances.
    /// </summary>
    public sealed class AudioEventHandle : AudioHandleBase
    {
        /// <summary>
        ///     Initializes a typed handle for an FMOD Studio event instance.
        ///     初始化 a typed handle for an FMOD Studio event instance。
        /// </summary>
        public AudioEventHandle(AudioSource source, AudioLifecycleScope scope, GodotObject? rawInstance)
            : base(source, scope, rawInstance)
        {
        }
    }
}
