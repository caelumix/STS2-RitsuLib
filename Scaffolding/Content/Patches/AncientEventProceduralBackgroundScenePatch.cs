using Godot;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Scaffolding.Content.Visuals;

namespace STS2RitsuLib.Scaffolding.Content.Patches
{
    /// <summary>
    ///     When an ancient uses <see cref="AncientEventPresentationAssetProfile.StageProcedural" />, supplies a tiny packed
    ///     当 an ancient 使用 <c>AncientEventPresentationAssetProfile.StageProcedural</c>, supplies a tiny packed
    ///     placeholder so <see cref="EventModel.CreateBackgroundScene" /> does not require a real background <c>tscn</c> path.
    ///     placeholder so <c>EventModel.CreateBackground场景</c> does not require a real 背景 <c>tscn</c> 路径.
    /// </summary>
    public class AncientEventProceduralBackgroundScenePatch : IPatchMethod
    {
        /// <inheritdoc cref="IPatchMethod.PatchId" />
        public static string PatchId => "ancient_event_procedural_background_scene";

        /// <inheritdoc cref="IPatchMethod.Description" />
        public static string Description =>
            "Return placeholder PackedScene for CreateBackgroundScene when ancient StageProcedural is defined";

        /// <inheritdoc cref="IPatchMethod.IsCritical" />
        public static bool IsCritical => false;

        /// <inheritdoc cref="IPatchMethod.GetTargets" />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(EventModel), nameof(EventModel.CreateBackgroundScene))];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Short-circuits background scene load when the ancient uses procedural stage layers.
        ///     Short-circuits 背景 场景 加载 当 the ancient 使用 procedural stage layers.
        /// </summary>
        public static bool Prefix(EventModel __instance, ref PackedScene __result)
            // ReSharper restore InconsistentNaming
        {
            if (__instance is not AncientEventModel)
                return true;

            if (__instance is not IModAncientEventAssetOverrides mod)
                return true;

            if (mod.AncientPresentationAssetProfile.StageProcedural == null)
                return true;

            __result = AncientStageProceduralRootFactory.PlaceholderBackgroundPackedScene;
            return false;
        }
    }
}
