using Godot;

namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Typed handle for loose file and streaming-backed audio instances.
    ///     Typed handle 用于 loose file 和 streaming-backed audio instances.
    /// </summary>
    public sealed class AudioFileHandle : AudioHandleBase
    {
        /// <summary>
        ///     Initializes a typed handle for a loose file or streaming audio instance.
        ///     初始化 a typed handle for a loose file or streaming audio instance。
        /// </summary>
        public AudioFileHandle(AudioSource source, AudioLifecycleScope scope, GodotObject? rawInstance)
            : base(source, scope, rawInstance)
        {
        }
    }
}
