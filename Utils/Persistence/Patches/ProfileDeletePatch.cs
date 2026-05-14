using MegaCrit.Sts2.Core.Saves;
using STS2RitsuLib.Data;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.Utils.Persistence.Patches
{
    /// <summary>
    ///     Cleans up mod persistence when the game deletes a save profile.
    ///     Cleans up mod persistence 当 the game deletes a 保存 档案.
    /// </summary>
    public class ProfileDeletePatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "profile_delete";

        /// <inheritdoc />
        public static string Description => "Delete mod data when game profile is deleted";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(SaveManager), "DeleteProfile", [typeof(int)])];
        }

        /// <summary>
        ///     Deletes mod data, notifies listeners, and invalidates the data-ready lifecycle for the profile.
        ///     Deletes mod data, notifies listeners, 和 invalidates the data-ready lifecycle 用于 the 档案.
        /// </summary>
        public static void Prefix(int profileId)
        {
            try
            {
                ModDataCloudMirror.ScheduleDeleteCloudModDataForProfile(profileId);
                ModDataStore.DeleteAllProfileData(profileId);
                ProfileManager.Instance.OnProfileDeleted(profileId);
                DataReadyLifecycle.NotifyProfileInvalidated(profileId, "SaveManager.DeleteProfile");
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Warn(
                    $"[Persistence] Failed to delete mod data for profile {profileId}: {ex.Message}");
            }
        }
    }
}
