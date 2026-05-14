using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.Keywords.Patches
{
    /// <summary>
    ///     Strips hover tips for mod keywords whose <see cref="ModKeywordDefinition.IncludeInCardHoverTip" /> is
    ///     <c>false</c> from the vanilla <see cref="CardModel.HoverTips" /> enumeration. Mod keywords now live
    ///     inside vanilla <c>CardModel.Keywords</c> as minted <c>CardKeyword</c> values, so vanilla already
    ///     iterates them and calls <see cref="HoverTipFactory.FromKeyword" /> on each; the Registry routing
    ///     patch (<see cref="HoverTipFactoryFromKeywordPatch" />) returns a real hover tip for every mod
    ///     keyword. This postfix is only required to honor the opt-out flag.
    ///     从原版 <c>CardModel.HoverTips</c> 枚举中移除
    ///     <c>ModKeywordDefinition.IncludeInCardHoverTip</c> 为 <c>false</c> 的 mod keyword hover tip。
    ///     mod keyword 现在作为 minted <c>CardKeyword</c> 值存在于原版 <c>CardModel.Keywords</c> 中，
    ///     因此原版已经会枚举它们并对每个值调用 <c>HoverTipFactory.FromKeyword</c>；
    ///     registry routing patch（<c>HoverTipFactoryFromKeywordPatch</c>）会为每个 mod keyword 返回真实
    ///     hover tip。此 postfix 仅用于遵守 opt-out flag。
    /// </summary>
    public sealed class CardModelHoverTipsModKeywordPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "ritsulib_card_model_hover_tips_mod_keyword_exclude";

        /// <inheritdoc />
        public static string Description =>
            "Remove mod keyword hover tips from CardModel.HoverTips when IncludeInCardHoverTip is false";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CardModel), "HoverTips", MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Removes any mod-keyword hover tip that vanilla produced (via
        ///     <see cref="HoverTipFactory.FromKeyword" />) but is marked non-hoverable in the registry.
        ///     移除任何由原版（通过 <c>HoverTipFactory.FromKeyword</c>）产生、但在 registry 中标记为
        ///     non-hoverable 的 mod-keyword hover tip。
        /// </summary>
        public static void Postfix(CardModel __instance, ref IEnumerable<IHoverTip> __result)
        {
            HashSet<IHoverTip>? toRemove = null;
            foreach (var keyword in __instance.Keywords)
            {
                if (!ModKeywordRegistry.TryGetByCardKeyword(keyword, out var definition))
                    continue;

                if (definition.IncludeInCardHoverTip)
                    continue;

                toRemove ??= [];
                toRemove.Add(HoverTipFactory.FromKeyword(keyword));
            }

            if (toRemove is null)
                return;

            __result = __result.Where(tip => !toRemove.Contains(tip)).ToArray();
        }
        // ReSharper restore InconsistentNaming
    }
}
