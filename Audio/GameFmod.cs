namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Game-routed FMOD Studio API (vanilla <see cref="MegaCrit.Sts2.Core.Nodes.Audio.NAudioManager" />).
    ///     Game-routed FMOD Studio API (原版 <c>MegaCrit.Sts2.Core.Nodes.Audio.NAudioManager</c>).
    /// </summary>
    public static class GameFmod
    {
        /// <summary>
        ///     Vanilla-routed FMOD API (singleton <see cref="GameFmodAudioService" />).
        ///     原版-routed FMOD API (singleton <c>GameFmodAudioService</c>).
        /// </summary>
        public static IGameFmodAudio Studio => GameFmodAudioService.Shared;

        /// <summary>
        ///     Higher-level playback API with typed handles and lifecycle scoping.
        ///     Higher-level playback API 带有 typed handles 和 lifecycle scoping.
        /// </summary>
        public static IGameAudio Playback => GameAudioService.Shared;
    }
}
