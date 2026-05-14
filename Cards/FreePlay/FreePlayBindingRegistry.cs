#if !STS2_AT_LEAST_0_104_0
using CombatStateLike = MegaCrit.Sts2.Core.Combat.CombatState;
#else
using CombatStateLike = MegaCrit.Sts2.Core.Combat.ICombatState;
#endif
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Utils;

namespace STS2RitsuLib.Cards.FreePlay
{
    /// <summary>
    ///     Detailed free-play resolution result split by detection source.
    ///     Detailed free-play resolution result split 通过 detection source.
    /// </summary>
    public sealed record FreePlayResolution(
        bool IsAutoPlayNoSpend,
        bool IsCardBindingFree,
        bool IsDualResourceModelFree,
        bool IsRegisteredDetectorFree)
    {
        /// <summary>
        ///     True when any detection source marks this play as free.
        ///     当 any detection source marks this play as free 时为 true。
        /// </summary>
        public bool IsFree => IsAutoPlayNoSpend || IsCardBindingFree || IsDualResourceModelFree ||
                              IsRegisteredDetectorFree;
    }

    /// <summary>
    ///     Extensible binding registry for "this play is free" semantics.
    ///     Extensible binding 注册表 用于 "this play is free" semantics.
    /// </summary>
    public static class FreePlayBindingRegistry
    {
        private static readonly Lock Gate = new();
        private static readonly Dictionary<string, Func<CardPlay, bool>> RegisteredDetectors = [];
        private static readonly AttachedState<CardModel, CardFreeBindingState> CardStates = new(() => new());
        private static readonly AttachedState<CardPlay, PlayFreeBindingState> PlayStates = new(() => new());

        /// <summary>
        ///     Registers an additional free-play detector. The detector should return true when the specified
        ///     Registers an additional free-play detector. The detector should 返回 true 当 the specified
        ///     <see cref="CardPlay" /> is considered free by mod-defined rules.
        /// </summary>
        /// <param name="bindingId">
        ///     Stable unique identifier for replacement/debugging.
        ///     稳定的 unique identifier for replacement/debugging。
        /// </param>
        /// <param name="detector">
        ///     Predicate that evaluates whether a play is free.
        ///     中文说明：Predicate that evaluates whether a play is free.
        /// </param>
        public static void Register(string bindingId, Func<CardPlay, bool> detector)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(bindingId);
            ArgumentNullException.ThrowIfNull(detector);

            lock (Gate)
            {
                RegisteredDetectors[bindingId] = detector;
            }
        }

        /// <summary>
        ///     Marks that the given card's next play should be treated as free.
        ///     Marks that the given 卡牌's next play should be treated as free.
        /// </summary>
        /// <param name="card">
        ///     Card receiving a single-use free-play charge.
        ///     卡牌 receiving a single-使用 free-play charge.
        /// </param>
        public static void MarkCardFreeNextPlay(CardModel card)
        {
            ArgumentNullException.ThrowIfNull(card);
            CardStates.Update(card, state =>
            {
                state.NextPlayCharges++;
                return state;
            });
        }

        /// <summary>
        ///     Marks that the given card should be treated as free for the current combat.
        ///     Marks that the given 卡牌 should be treated as free 用于 the current combat.
        /// </summary>
        /// <param name="card">
        ///     Card receiving combat-duration free-play state.
        ///     卡牌 receiving combat-duration free-play state.
        /// </param>
        public static void MarkCardFreeThisCombat(CardModel card)
        {
            ArgumentNullException.ThrowIfNull(card);
            CardStates.Update(card, state =>
            {
                state.FreeThisCombatState = ResolveCombatState(card);
                return state;
            });
        }

        /// <summary>
        ///     Marks the current <see cref="CardPlay" /> as free immediately.
        ///     Marks the current <c>CardPlay</c> as free immediately.
        /// </summary>
        /// <param name="play">
        ///     Play instance to mark.
        ///     中文说明：Play instance to mark.
        /// </param>
        public static void MarkCurrentPlayFree(CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play);
            PlayStates.Set(play, new()
            {
                IsResolved = true,
                Resolution = new(false, true, false, false),
            });
        }

        /// <summary>
        ///     Resolves detailed free-play sources for this <see cref="CardPlay" />.
        ///     解析 detailed free-play sources for this <c>CardPlay</c>。
        /// </summary>
        /// <param name="play">
        ///     Play instance to evaluate.
        ///     中文说明：Play instance to evaluate.
        /// </param>
        /// <returns>
        ///     A split resolution indicating which source marked the play as free.
        ///     一个 split resolution indicating which source marked the play as free。
        /// </returns>
        public static FreePlayResolution Resolve(CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play);

            var cached = PlayStates.GetOrCreate(play);
            if (cached.IsResolved)
                return cached.Resolution;

            var resolution = BuildResolution(play);
            PlayStates.Set(play, new()
            {
                IsResolved = true,
                Resolution = resolution,
            });
            return resolution;
        }

        /// <summary>
        ///     Convenience helper returning whether the play is free by any source.
        ///     Convenience helper 返回ing whether the play is free 通过 any source.
        /// </summary>
        /// <param name="play">
        ///     Play instance to evaluate.
        ///     中文说明：Play instance to evaluate.
        /// </param>
        /// <returns>
        ///     True when any free-play source applies.
        ///     当 any free-play source applies 时为 true。
        /// </returns>
        public static bool IsFreeForPlay(CardPlay play)
        {
            return Resolve(play).IsFree;
        }

        private static FreePlayResolution BuildResolution(CardPlay play)
        {
            if (play.IsAutoPlay)
                return new(true, false, false, false);

            var isCardBindingFree = EvaluateAndConsumeCardBindings(play);
            var isDualResourceModelFree = IsFreeByDualResourceModel(play);
            var isRegisteredDetectorFree = EvaluateRegisteredDetectors(play);
            return new(false, isCardBindingFree, isDualResourceModelFree, isRegisteredDetectorFree);
        }

        private static bool EvaluateAndConsumeCardBindings(CardPlay play)
        {
            var card = play.Card;
            var state = CardStates.GetOrCreate(card);
            var combatState = ResolveCombatState(card);

            if (state.FreeThisCombatState != null && ReferenceEquals(state.FreeThisCombatState, combatState))
                return true;

            if (state.NextPlayCharges <= 0)
                return false;

            CardStates.Update(card, current =>
            {
                current.NextPlayCharges = Math.Max(0, current.NextPlayCharges - 1);
                return current;
            });
            return true;
        }

        private static bool EvaluateRegisteredDetectors(CardPlay play)
        {
            Func<CardPlay, bool>[] detectors;
            lock (Gate)
            {
                detectors = RegisteredDetectors.Values.ToArray();
            }

            return detectors.Any(detector => detector(play));
        }

        private static bool IsFreeByDualResourceModel(CardPlay play)
        {
            var card = play.Card;
            var owner = card.Owner;
            if (owner?.Creature == null)
                return false;

            if (play.IsAutoPlay)
                return false;

            var models = owner.Creature.Powers
                .Cast<AbstractModel>()
                .Concat(owner.Relics);

            return models.Any(model => IsDualResourceZeroedByModel(model, card));
        }

        private static bool IsDualResourceZeroedByModel(AbstractModel model, CardModel card)
        {
            var energyOriginal = (decimal)card.EnergyCost.GetWithModifiers(CostModifiers.Local);
            var starOriginal = card.CurrentStarCost;

            var changedEnergy = model.TryModifyEnergyCostInCombat(card, energyOriginal, out var energyModified);
            if (!changedEnergy || energyModified > 0m)
                return false;

            var changedStar = model.TryModifyStarCost(card, starOriginal, out var starModified);
            return changedStar && starModified <= 0m;
        }

        private static CombatStateLike? ResolveCombatState(CardModel card)
        {
            return card.CombatState ?? card.Owner?.Creature?.CombatState;
        }

        private sealed class CardFreeBindingState
        {
            public int NextPlayCharges { get; set; }
            public CombatStateLike? FreeThisCombatState { get; set; }
        }

        private sealed class PlayFreeBindingState
        {
            public bool IsResolved { get; set; }
            public FreePlayResolution Resolution { get; set; } = new(false, false, false, false);
        }
    }
}
