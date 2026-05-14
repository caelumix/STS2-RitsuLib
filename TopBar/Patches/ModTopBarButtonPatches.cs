using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.CardPiles.Nodes;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.TopBar.Patches
{
    /// <summary>
    ///     Mounts every registered <see cref="ModTopBarButtonDefinition" /> onto <see cref="NTopBar" /> after
    ///     Mounts every 已注册 <c>ModTopBarButtonDefinition</c> onto <c>NTopBar</c> 之后
    ///     its vanilla <c>_Ready</c> has populated <c>%Deck</c>. Buttons are <see cref="NModCardPileButton" />
    ///     its 原版 <c>_Ready</c> has populated <c>%Deck</c>. Buttons are <c>NModCardPileButton</c>
    ///     instances in "action mode" — that is, they look and animate exactly like <b>pile-backed</b>
    ///     instances in "action mode" — that is, they look 和 animate exactly like <b>pile-backed</b>
    ///     <see cref="STS2RitsuLib.CardPiles.ModCardPileRegistry" /> top-bar buttons, but dispatch clicks
    ///     through <see cref="ModTopBarButtonSpec.OnClick" /> and draw their count label from
    ///     through <c>ModTopBarButtonSpec.OnClick</c> 和 draw their count label 从
    ///     <see cref="ModTopBarButtonSpec.CountProvider" />. The two registries share one placement
    ///     algorithm (see <see cref="ModTopBarLayout" />) so the player-side cluster next to <c>%Deck</c>
    ///     中文说明：algorithm (see <c>ModTopBarLayout</c>) so the player-side cluster next to <c>%Deck</c>
    ///     never splits into "pile row" vs "action row".
    ///     never splits into "pile row" vs "action row".
    /// </summary>
    public sealed class ModTopBarActionButtonReadyPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "ritsulib_top_bar_ready_action_inject";

        /// <inheritdoc />
        public static string Description => "Inject mod action buttons into NTopBar";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NTopBar), nameof(NTopBar._Ready))];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Wires generic mod top-bar buttons alongside the vanilla deck/map/pause nodes and any
        ///     Wires generic mod top-bar buttons alongside the 原版 deck/map/pause nodes 和 any
        ///     pile-backed <see cref="STS2RitsuLib.CardPiles.ModCardPileUiStyle.TopBarDeck" /> buttons.
        ///     pile-backed <c>STS2RitsuLib.CardPiles.ModCardPileUiStyle.TopBarDeck</c> buttons.
        /// </summary>
        public static void Postfix(NTopBar __instance)
        {
            var definitions = ModTopBarButtonRegistry.GetDefinitionsSnapshot();
            if (definitions.Length == 0)
                return;

            // The right-side cluster (%Deck / %Map / %Pause / Options …) lives inside the
            // `RightAlignedStuff` container, not directly on NTopBar. `ModTopBarLayout.Place` handles
            // that re-parenting for us so buttons end up as siblings of %Deck.
            foreach (var definition in definitions)
            {
                var button = NModCardPileButton.CreateAction(definition);
                __instance.AddChildSafely(button);
                ModTopBarLayout.Place(__instance, button, definition.Offset);
            }
        }
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    ///     Binds every injected action-mode <see cref="NModCardPileButton" /> to the local
    ///     中文说明：Binds every injected action-mode <c>NModCardPileButton</c> to the local
    ///     Binds every injected action-mode <c>NModCardPileButton</c> to the local
    ///     中文说明：Binds every injected action-mode <c>NModCardPileButton</c> to the local
    ///     <see cref="Player" /> on <see cref="NTopBar.Initialize" />, mirroring
    ///     <c>ModCardPileTopBarInitializePatch</c>.
    /// </summary>
    public sealed class ModTopBarActionButtonInitializePatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "ritsulib_top_bar_initialize_action_bind";

        /// <inheritdoc />
        public static string Description =>
            "Bind mod action buttons to the local player on NTopBar.Initialize";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(NTopBar), nameof(NTopBar.Initialize), [typeof(IRunState)]),
            ];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Binds each injected action button to the local <see cref="Player" />.
        ///     中文说明：Binds each injected action button to the local <c>Player</c>.
        ///     Binds each injected action button to the local <c>Player</c>.
        ///     中文说明：Binds each injected action button to the local <c>Player</c>.
        /// </summary>
        public static void Postfix(NTopBar __instance, IRunState runState)
        {
            var player = LocalContext.GetMe(runState);
            if (player == null)
                return;
            // Action buttons live inside `RightAlignedStuff` (Deck's parent), not on NTopBar directly.
            var container = ModTopBarLayout.GetRightAlignedContainer(__instance);
            if (container == null)
                return;
            foreach (var button in container.GetChildren().OfType<NModCardPileButton>())
                if (button.IsActionMode)
                    button.Initialize(player);
        }
        // ReSharper restore InconsistentNaming
    }
}
