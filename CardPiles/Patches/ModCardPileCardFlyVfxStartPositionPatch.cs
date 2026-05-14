using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.CardPiles.Patches
{
    /// <summary>
    ///     When a card fly visual is spawned after the card has already been reparented, the node may no
    ///     longer carry enough "old pile" context to choose a correct start position. This patch consults
    ///     <see cref="ModCardPileFlightHistory" /> (fed by <see cref="CardPile.CardRemoved" />) to recover the
    ///     source pile and applies <see cref="ModCardPileSpec.FlightStartPositionResolver" /> when the source
    ///     pile is a mod pile.
    ///     当 card 已经 reparent 后才生成 card fly visual 时，节点可能不再携带足够的“old pile”上下文来选择正确
    ///     start position。此 patch 查询 <c>ModCardPileFlightHistory</c>（由
    ///     <c>CardPile.CardRemoved</c> 填充）来恢复 source pile，并在 source pile 是 mod pile 时应用
    ///     <see cref="ModCardPileSpec.FlightStartPositionResolver" />。
    /// </summary>
    public sealed class ModCardPileCardFlyVfxStartPositionPatch : IPatchMethod
    {
        private static readonly FieldInfo? StartPositionField =
            AccessTools.Field(typeof(NCardFlyVfx), "_startPos");

        /// <inheritdoc />
        public static string PatchId => "ritsulib_mod_pile_card_fly_vfx_start_position";

        /// <inheritdoc />
        public static string Description => "Allow mod piles to customize NCardFlyVfx start positions";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NCardFlyVfx), nameof(NCardFlyVfx.Create))];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Post-processes the created fly vfx and overwrites its start position when the recovered
        ///     source pile is a mod pile with a start-position resolver.
        ///     后处理已创建的 fly vfx；当恢复出的 source pile 是带 start-position resolver 的 mod pile 时，
        ///     覆盖其 start position。
        /// </summary>
        public static void Postfix(NCard card, Vector2 end, bool isAddingToPile, string trailPath,
            ref NCardFlyVfx? __result)
        {
            if (__result == null || StartPositionField == null)
                return;

            var model = card.Model;
            if (model == null)
                return;

            var oldPile = ModCardPileFlightHistory.TryGetLastRemovedPile(model);
            if (oldPile == null)
                return;
            if (!ModCardPileRegistry.TryGetByPileType(oldPile.Type, out var definition))
                return;

            var resolver = definition.FlightStartPositionResolver;
            if (resolver == null)
                return;

            var targetPile = model.Pile ?? oldPile;
            var defaultStartPosition = ModCardPileLayout.GetTargetPosition(definition, card);
            var context =
                new ModCardPileFlightStartContext(definition, oldPile, targetPile, defaultStartPosition, card);
            var resolved = resolver(context) ?? defaultStartPosition;

            StartPositionField.SetValue(__result, resolved);
            card.GlobalPosition = resolved;
        }
        // ReSharper restore InconsistentNaming
    }
}
