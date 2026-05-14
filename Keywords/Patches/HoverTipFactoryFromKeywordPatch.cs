using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.Keywords.Patches
{
    /// <summary>
    ///     Routes <see cref="HoverTipFactory.FromKeyword" /> calls for minted mod <see cref="CardKeyword" />
    ///     values to <see cref="ModKeywordRegistry.CreateHoverTip" /> so the hover tip uses the registered
    ///     title / description / icon instead of the slugified numeric fallback produced by
    ///     <c>CardKeywordExtensions.GetLocKeyPrefix</c> for unknown enum values. Vanilla keywords skip the
    ///     prefix entirely and fall through to the original factory.
    ///     将 minted mod <c>CardKeyword</c> 值的 <c>HoverTipFactory.FromKeyword</c> 调用路由到
    ///     <c>ModKeywordRegistry.CreateHoverTip</c>，使 hover tip 使用已注册 title / description / icon，
    ///     而不是对未知 enum 值由 <c>CardKeywordExtensions.GetLocKeyPrefix</c> 生成的 slugified numeric fallback。
    ///     原版 keyword 会完全跳过 prefix，并落回原始 factory。
    /// </summary>
    [HarmonyBefore(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.First)]
    public sealed class HoverTipFactoryFromKeywordPatch : IPatchMethod
    {
        private static readonly Dictionary<CardKeyword, IHoverTip> ModKeywordTipCache = [];
        private static readonly Lock SyncRoot = new();

        /// <inheritdoc />
        public static string PatchId => "ritsulib_hover_tip_factory_from_keyword_mod_route";

        /// <inheritdoc />
        public static string Description =>
            "Route HoverTipFactory.FromKeyword to ModKeywordRegistry for minted mod CardKeyword values";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(HoverTipFactory), nameof(HoverTipFactory.FromKeyword))];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Short-circuits mod keyword lookups before vanilla's slug-based <see cref="HoverTip" /> construction
        ///     runs, returning a cached registry-built tip. Non-mod values return <c>true</c> so vanilla executes.
        ///     在原版基于 slug 的 <c>HoverTip</c> 构造运行前短路 mod keyword lookup，返回缓存的
        ///     registry-built tip。非 mod 值返回 <c>true</c>，让原版继续执行。
        /// </summary>
        public static bool Prefix(CardKeyword keyword, ref IHoverTip __result)
        {
            if (!ModKeywordRegistry.TryGetByCardKeyword(keyword, out var definition))
                return true;

            lock (SyncRoot)
            {
                if (!ModKeywordTipCache.TryGetValue(keyword, out var cached))
                {
                    cached = ModKeywordRegistry.CreateHoverTip(definition.Id);
                    ModKeywordTipCache[keyword] = cached;
                }

                __result = cached;
            }

            return false;
        }
        // ReSharper restore InconsistentNaming
    }
}
