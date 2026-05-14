using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.Content.Patches
{
    /// <summary>
    ///     Applies dynamic Harmony postfixes so <see cref="ModContentRegistry" /> character-starter registrations merge
    ///     Applies dynamic Harmony 后置补丁es so <c>ModContentRegistry</c> character-starter 注册s merge
    ///     into every concrete <see cref="CharacterModel" /> (vanilla and mod) before <see cref="ModelDb.Init" /> caches
    ///     into every concrete <c>Character模型</c> (原版 和 mod) 之前 <c>ModelDb.Init</c> caches
    ///     content.
    ///     中文说明：content.
    /// </summary>
    public sealed class DynamicCharacterStarterContentPatchBootstrap : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "dynamic_character_starter_content_patch_bootstrap";

        /// <inheritdoc />
        public static string Description =>
            "Patch all CharacterModel starter property getters to merge registry character-starter content";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), nameof(ModelDb.Init))];
        }

        /// <summary>
        ///     Ensures starter merge patches are applied for every loaded character type before ModelDb initialization.
        ///     Ensures starter merge patches are applied 用于 every loaded character type 之前 ModelDb initialization.
        /// </summary>
        public static void Prefix()
        {
            DynamicCharacterStarterContentPatcher.EnsurePatched();
        }
    }
}
