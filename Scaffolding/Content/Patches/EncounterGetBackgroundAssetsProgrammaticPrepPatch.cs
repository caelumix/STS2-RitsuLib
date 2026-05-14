using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.Scaffolding.Content.Patches
{
    /// <summary>
    ///     Runs before <c>EncounterModel.GetBackgroundAssets</c> so <see cref="ModEncounterTemplate" /> can fill the
    ///     runs 之前 <c>EncounterModel.GetBackgroundAssets</c> so <c>ModEncounterTemplate</c> can fill the
    ///     programmatic slot (needs <see cref="ActModel" /> + <see cref="Rng" />; vanilla
    ///     programmatic slot (needs <c>ActModel</c> + <c>Rng</c>; 原版
    ///     <c>CreateBackgroundAssetsForCustom</c> only receives <c>Rng</c>).
    /// </summary>
    public class EncounterGetBackgroundAssetsProgrammaticPrepPatch : IPatchMethod
    {
        private static readonly AccessTools.FieldRef<EncounterModel, BackgroundAssets?> CachedBackgroundAssetsField =
            AccessTools.FieldRefAccess<EncounterModel, BackgroundAssets?>("_backgroundAssets");

        /// <inheritdoc cref="IPatchMethod.PatchId" />
        public static string PatchId => "content_encounter_programmatic_background_prep";

        /// <inheritdoc cref="IPatchMethod.Description" />
        public static string Description =>
            "Prepare ModEncounterTemplate programmatic combat BackgroundAssets for CreateBackgroundAssetsForCustom";

        /// <inheritdoc cref="IPatchMethod.IsCritical" />
        public static bool IsCritical => false;

        /// <inheritdoc cref="IPatchMethod.GetTargets" />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(EncounterModel), "GetBackgroundAssets", [typeof(ActModel), typeof(Rng)])];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Invokes <see cref="ModEncounterTemplate.PrepareProgrammaticCombatBackground" /> when the encounter has no cached
        ///     Invokes <c>ModEncounterTemplate.PrepareProgrammaticCombat背景</c> 当 the encounter has no cached
        ///     background yet.
        ///     背景 yet.
        /// </summary>
        public static void Prefix(EncounterModel __instance, ActModel parentAct, Rng rng)
            // ReSharper restore InconsistentNaming
        {
            if (__instance is not ModEncounterTemplate { UsesProgrammaticCombatBackground: true } template)
                return;

            if (CachedBackgroundAssetsField(__instance) != null)
                return;

            template.PrepareProgrammaticCombatBackground(parentAct, rng);
        }
    }
}
