using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2RitsuLib.Keywords.Patches
{
    /// <summary>
    ///     Seeds minted mod <see cref="CardKeyword" /> values onto every <see cref="ModCardTemplate" /> instance
    ///     after vanilla <c>CardModel.get_Keywords</c> materializes the underlying local keyword set. Keeps
    ///     <see cref="ModCardTemplate.RegisteredKeywordIds" /> as an independent channel from vanilla
    ///     <see cref="CardModel.CanonicalKeywords" /> so downstream mods can still override
    ///     <c>CanonicalKeywords</c> without dropping their mod keyword declarations. Seeding is tracked per
    ///     instance with a <see cref="ConditionalWeakTable{TKey,TValue}" /> marker so the postfix executes the
    ///     resolution loop exactly once per card lifetime (subsequent calls are an O(1) early-out).
    ///     在原版 <c>CardModel.get_Keywords</c> 实体化底层本地关键词集合后，将铸造的 mod <see cref="CardKeyword" /> 值种入每个
    ///     <see cref="ModCardTemplate" /> 实例。保持
    ///     <see cref="ModCardTemplate.RegisteredKeywordIds" /> 作为独立于原版
    ///     <see cref="CardModel.CanonicalKeywords" /> 的通道，使下游 mod 仍可覆盖
    ///     <c>CanonicalKeywords</c> 而不会丢失其 mod 关键词声明。种入过程使用
    ///     <see cref="ConditionalWeakTable{TKey,TValue}" /> 标记按实例跟踪，使 postfix 在每张卡牌生命周期内
    ///     只执行一次解析循环（后续调用为 O(1) 早退）。
    /// </summary>
    public sealed class CardModelKeywordsModSeedPatch : IPatchMethod
    {
        private static readonly ConditionalWeakTable<CardModel, object> SeededCards = new();
        private static readonly object SeededMarker = new();

        /// <inheritdoc />
        public static string PatchId => "ritsulib_card_model_keywords_mod_seed";

        /// <inheritdoc />
        public static string Description =>
            "Seed ModCardTemplate.RegisteredKeywordIds into CardModel.Keywords after the canonical set is built";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CardModel), "Keywords", MethodType.Getter)];
        }

        /// <summary>
        ///     Unions the minted <see cref="CardKeyword" /> values of the card's
        ///     <see cref="ModCardTemplate.RegisteredKeywordIds" /> into the vanilla keyword set the first time the
        ///     getter runs. In combat, the getter can return a temporary all-sources set rather than the private
        ///     local storage, so on APIs that expose keyword sources this writes through
        ///     <c>KeywordSources.Local</c> and mirrors the values into the current result.
        ///     第一次运行 getter 时，将卡牌的
        ///     <see cref="ModCardTemplate.RegisteredKeywordIds" /> 对应的铸造 <see cref="CardKeyword" /> 值并入原版关键词集合。
        ///     战斗中 getter 可能返回临时全来源集合而不是私有本地存储，因此在暴露 keyword source 的 API 上，
        ///     这里通过 <c>KeywordSources.Local</c> 写入，并把这些值同步到当前返回值。
        /// </summary>
        public static void Postfix(CardModel __instance, IReadOnlySet<CardKeyword> __result)
        {
            if (__instance is not ModCardTemplate template)
                return;

            if (SeededCards.TryGetValue(__instance, out _))
                return;

            if (!TryGetMutableLocalKeywordSet(__instance, __result, out var storage))
                return;

            var seeded = new List<CardKeyword>();
            foreach (var id in template.EnumerateRegisteredKeywordIds())
            {
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                if (ModKeywordRegistry.TryResolveCardKeyword(id, out var value))
                {
                    storage.Add(value);
                    seeded.Add(value);
                }
            }

            if (__result is HashSet<CardKeyword> resultStorage && !ReferenceEquals(resultStorage, storage))
            {
                foreach (var value in seeded)
                    resultStorage.Add(value);
            }

            SeededCards.Add(__instance, SeededMarker);
        }

        private static bool TryGetMutableLocalKeywordSet(
            CardModel instance,
            IReadOnlySet<CardKeyword> result,
            out HashSet<CardKeyword> storage)
        {
#if STS2_AT_LEAST_0_107_0
            if (instance.GetKeywordsWithSources(KeywordSources.Local) is HashSet<CardKeyword> localStorage)
            {
                storage = localStorage;
                return true;
            }
#else
            if (result is HashSet<CardKeyword> localStorage)
            {
                storage = localStorage;
                return true;
            }
#endif
            storage = null!;
            return false;
        }
    }
}
