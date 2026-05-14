namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Mixer volumes using the same linear curve as vanilla settings.
    ///     Mixer volumes using the same linear curve as 原版 设置.
    /// </summary>
    public interface IFmodMixerVolumes
    {
        /// <summary>
        ///     Master bus volume in linear 0–1 space.
        ///     中文说明：Master bus volume in linear 0–1 space.
        /// </summary>
        void SetMasterVolume(float linear01);

        /// <summary>
        ///     SFX bus volume in linear 0–1 space.
        ///     中文说明：SFX bus volume in linear 0–1 space.
        /// </summary>
        void SetSfxVolume(float linear01);

        /// <summary>
        ///     Ambience bus volume in linear 0–1 space.
        ///     中文说明：Ambience bus volume in linear 0–1 space.
        /// </summary>
        void SetAmbienceVolume(float linear01);

        /// <summary>
        ///     Music / BGM bus volume in linear 0–1 space.
        ///     中文说明：Music / BGM bus volume in linear 0–1 space.
        /// </summary>
        void SetBgmVolume(float linear01);
    }
}
