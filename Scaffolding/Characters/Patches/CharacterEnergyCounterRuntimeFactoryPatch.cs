using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Scaffolding.Godot;

namespace STS2RitsuLib.Scaffolding.Characters.Patches
{
    /// <summary>
    ///     Allows mod characters to convert legacy energy-counter scenes into <see cref="NEnergyCounter" /> before
    ///     Allows mod characters to convert legacy energy-counter 场景s into <c>NEnergyCounter</c> 之前
    ///     vanilla tries to instantiate the scene as the final type directly.
    ///     原版 tries to instantiate the 场景 as the final type directly.
    /// </summary>
    public class CharacterEnergyCounterRuntimeFactoryPatch : IPatchMethod
    {
        private static readonly FieldInfo PlayerField = AccessTools.Field(typeof(NEnergyCounter), "_player")!;

        /// <inheritdoc />
        public static string PatchId => "character_energy_counter_runtime_factory";

        /// <inheritdoc />
        public static string Description =>
            "Allow mod characters to supply NEnergyCounter via Ritsu scene conversion before direct scene instantiate";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NEnergyCounter), nameof(NEnergyCounter.Create))];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Converts a mod energy-counter scene into <see cref="NEnergyCounter" /> and injects the owning player
        ///     Converts a mod energy-counter 场景 into <c>NEnergyCounter</c> 和 injects the owning player
        ///     before vanilla performs direct scene instantiation.
        ///     之前 原版 performs direct 场景 instantiation.
        /// </summary>
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(Player player, ref NEnergyCounter? __result)
        {
            if (player.Character is not IModCharacterAssetOverrides { CustomEnergyCounterPath: { } energyCounterPath })
                return true;

            if (!ResourceLoader.Exists(energyCounterPath))
                return true;

            var created = RitsuGodotNodeFactories.CreateFromScenePath<NEnergyCounter>(
                energyCounterPath,
                PackedScene.GenEditState.Disabled);
            PlayerField.SetValue(created, player);
            __result = created;
            return false;
        }
        // ReSharper restore InconsistentNaming
    }
}
