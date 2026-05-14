namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     One-shots through <see cref="MegaCrit.Sts2.Core.Nodes.Audio.NAudioManager" />.
    ///     中文说明：One-shots through <c>MegaCrit.Sts2.Core.Nodes.Audio.NAudioManager</c>.
    /// </summary>
    public interface IFmodOneShotPlayback
    {
        /// <summary>
        ///     Plays a one-shot at <paramref name="volume" /> linear scale.
        ///     中文说明：Plays a one-shot at <c>volume</c> linear scale.
        /// </summary>
        void PlayOneShot(string eventPath, float volume = 1f);

        /// <summary>
        ///     Plays a one-shot with initial parameter values and linear <paramref name="volume" />.
        ///     Plays a one-shot 带有 initial parameter values 和 linear <c>volume</c>.
        /// </summary>
        void PlayOneShot(string eventPath, IReadOnlyDictionary<string, float> parameters, float volume = 1f);
    }
}
