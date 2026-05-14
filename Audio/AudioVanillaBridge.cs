using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Runs;

namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Small helper for restoring or refreshing the game's native run music controller state.
    ///     Small helper 用于 restoring 或 refreshing the game's native 跑局 music controller state.
    /// </summary>
    public static class AudioVanillaBridge
    {
        /// <summary>
        ///     Rebuilds vanilla run music, track state, and ambience.
        ///     Rebuilds 原版 跑局 music, track state, 和 ambience.
        /// </summary>
        public static void RefreshRunMusic()
        {
            var controller = NRunMusicController.Instance;
            if (controller is null || !RunManager.Instance.IsInProgress)
                return;

            controller.UpdateMusic();
            controller.UpdateTrack();
            controller.UpdateAmbience();
        }

        /// <summary>
        ///     Refreshes vanilla track progression and ambience without rebuilding the act music selection.
        ///     Refreshes 原版 track progression 和 ambience 带有out rebuilding the 章节 music selection.
        /// </summary>
        public static void RefreshTrackAndAmbience()
        {
            var controller = NRunMusicController.Instance;
            if (controller is null || !RunManager.Instance.IsInProgress)
                return;

            controller.UpdateTrack();
            controller.UpdateAmbience();
        }
    }
}
