using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;

namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     SFX aligned with <see cref="SfxCmd" /> guards. Use <see cref="GameFmod.Studio" /> for music, mixer, or unguarded
    ///     SFX aligned 带有 <c>SfxCmd</c> guards. 使用 <c>GameFmod.Studio</c> 用于 music, mixer, 或 unguarded
    ///     playback.
    ///     中文说明：playback.
    /// </summary>
    public static class Sts2SfxAlignedFmod
    {
        /// <summary>
        ///     One-shot through <see cref="SfxCmd.Play(string, float)" />.
        ///     中文说明：One-shot through <c>SfxCmd.Play(string, float)</c>.
        /// </summary>
        public static void PlayOneShot(string eventPath, float volume = 1f)
        {
            SfxCmd.Play(eventPath, volume);
        }

        /// <summary>
        ///     One-shot with a single named parameter via <see cref="SfxCmd" />.
        ///     One-shot 带有 a single named parameter via <c>SfxCmd</c>.
        /// </summary>
        public static void PlayOneShot(string eventPath, string parameterName, float parameterValue, float volume = 1f)
        {
            SfxCmd.Play(eventPath, parameterName, parameterValue, volume);
        }

        /// <summary>
        ///     One-shot with parameters; skips when non-interactive or combat is ending, otherwise uses
        ///     One-shot 带有 parameters; skips 当 non-interactive 或 combat is ending, otherwise 使用
        ///     <see cref="GameFmod.Studio" />.
        /// </summary>
        public static void PlayOneShot(string eventPath, IReadOnlyDictionary<string, float> parameters,
            float volume = 1f)
        {
            if (NonInteractiveMode.IsActive || CombatManager.Instance.IsEnding)
                return;

            GameFmod.Studio.PlayOneShot(eventPath, parameters, volume);
        }

        /// <summary>
        ///     Starts a guarded loop via
        ///     中文说明：Starts a guarded loop via
        ///     <see>
        ///         <cref>SfxCmd.PlayLoop</cref>
        ///     </see>
        ///     .
        ///     中文说明：.
        /// </summary>
        public static void PlayLoop(string eventPath, bool usesLoopParam = true)
        {
            SfxCmd.PlayLoop(eventPath, usesLoopParam);
        }

        /// <summary>
        ///     Stops a loop via
        ///     中文说明：Stops a loop via
        ///     <see>
        ///         <cref>SfxCmd.StopLoop</cref>
        ///     </see>
        ///     .
        ///     中文说明：.
        /// </summary>
        public static void StopLoop(string eventPath)
        {
            SfxCmd.StopLoop(eventPath);
        }

        /// <summary>
        ///     Sets a loop parameter via <see cref="SfxCmd.SetParam" />.
        ///     设置 a loop parameter via <c>SfxCmd.SetParam</c>.
        /// </summary>
        public static void SetLoopParameter(string eventPath, string parameterName, float value)
        {
            SfxCmd.SetParam(eventPath, parameterName, value);
        }
    }
}
