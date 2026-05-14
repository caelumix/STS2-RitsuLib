using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.Scaffolding.Cards.HandOutline.Patches
{
    /// <summary>
    ///     After vanilla hand highlight color (playable / gold / red), applies <see cref="ModCardHandOutlineRegistry" /> when
    ///     之后 原版 hand highlight color (playable / gold / red), applies <c>ModCardHandOutline注册表</c> 当
    ///     a matching rule exists.
    ///     一个 matching rule exists。
    /// </summary>
    internal sealed class NHandCardHolderUpdateCardHandOutlinePatch : IPatchMethod
    {
        public static string PatchId => "n_hand_card_holder_update_card_hand_outline";

        public static string Description => "Apply ModCardHandOutlineRegistry colors to NHandCardHolder.UpdateCard";

        public static bool IsCritical => false;

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NHandCardHolder), nameof(NHandCardHolder.UpdateCard), true)];
        }

        // ReSharper disable once InconsistentNaming
        public static void Postfix(NHandCardHolder __instance)
        {
            if (!ModCardHandOutlinePatchHelper.TryGetRule(__instance, out var model, out var rule))
                return;

            ModCardHandOutlinePatchHelper.ApplyHighlight(__instance, model, rule);
        }
    }
}
