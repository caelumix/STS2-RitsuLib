using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.CardPiles.Patches
{
    /// <summary>
    ///     Overrides the source/start position for shuffle fly visuals when the source pile is a mod pile and
    ///     that pile registered a <see cref="ModCardPileSpec.FlightStartPositionResolver" />.
    ///     当 source pile 是 mod pile 且该 pile 注册了
    ///     <c>ModCardPileSpec.FlightStartPositionResolver</c> 时，覆盖 shuffle fly visual 的 source/start 位置。
    /// </summary>
    public sealed class ModCardPileShuffleVfxStartPositionPatch : IPatchMethod
    {
        private static readonly FieldInfo? StartPositionField =
            AccessTools.Field(typeof(NCardFlyShuffleVfx), "_startPos");

        /// <inheritdoc />
        public static string PatchId => "ritsulib_mod_pile_shuffle_vfx_start_position";

        /// <inheritdoc />
        public static string Description =>
            "Allow mod card piles to customize shuffle-fly source positions";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NCardFlyShuffleVfx), nameof(NCardFlyShuffleVfx.Create))];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Rewrites the freshly created shuffle-vfx start position when <paramref name="startPile" /> is a
        ///     mod pile and it provided a custom start resolver.
        ///     当 <c>startPile</c> 是 mod pile 且提供了自定义 start resolver 时，
        ///     重写刚创建的 shuffle-vfx start position。
        /// </summary>
        public static void Postfix(CardPile startPile, CardPile targetPile, ref NCardFlyShuffleVfx? __result)
        {
            if (__result == null || StartPositionField == null)
                return;
            if (!ModCardPileRegistry.TryGetByPileType(startPile.Type, out var definition))
                return;

            var resolved = ModCardPileLayout.GetShuffleStartPosition(definition, startPile, targetPile);
            StartPositionField.SetValue(__result, resolved);
        }
        // ReSharper restore InconsistentNaming
    }
}
