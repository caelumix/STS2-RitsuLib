using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Content;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Utils;

namespace STS2RitsuLib.Scaffolding.Characters.Patches
{
    internal static class CharacterAssetOverridePatchHelper
    {
        internal static bool TryUseOverride(
            CharacterModel instance,
            // ReSharper disable once InconsistentNaming
            ref string __result,
            Func<IModCharacterAssetOverrides, string?> selector,
            string memberName,
            bool requireExistingResource = true)
        {
            var overrideValue = ResolveOverride(instance, selector, memberName);
            if (string.IsNullOrWhiteSpace(overrideValue))
                return true;

            if (requireExistingResource && !GodotResourcePath.ResourceExists(overrideValue))
            {
                AssetPathDiagnostics.WarnModCharacterAssetOverrideMissing(instance, memberName, overrideValue);
                return true;
            }

            __result = overrideValue;
            return false;
        }

        internal static string? ResolveCombatSpineSkeletonDataPath(CharacterModel instance)
        {
            if (TryResolveRegisteredProfile(instance, out var profile) &&
                !string.IsNullOrWhiteSpace(profile.Spine?.CombatSkeletonDataPath))
                return profile.Spine?.CombatSkeletonDataPath;

            if (instance is IModCharacterAssetOverrides overrides &&
                !string.IsNullOrWhiteSpace(overrides.CustomCombatSpineSkeletonDataPath))
                return overrides.CustomCombatSpineSkeletonDataPath;

            return null;
        }

        private static string? ResolveOverride(
            CharacterModel instance,
            Func<IModCharacterAssetOverrides, string?> selector,
            string memberName)
        {
            if (TryResolveRegisteredProfile(instance, out var profile))
            {
                var registered = memberName switch
                {
                    nameof(IModCharacterAssetOverrides.CustomVisualsPath) => profile.Scenes?.VisualsPath,
                    nameof(IModCharacterAssetOverrides.CustomEnergyCounterPath) => profile.Scenes?.EnergyCounterPath,
                    nameof(IModCharacterAssetOverrides.CustomMerchantAnimPath) => profile.Scenes?.MerchantAnimPath,
                    nameof(IModCharacterAssetOverrides.CustomRestSiteAnimPath) => profile.Scenes?.RestSiteAnimPath,
                    nameof(IModCharacterAssetOverrides.CustomIconTexturePath) => profile.Ui?.IconTexturePath,
                    nameof(IModCharacterAssetOverrides.CustomIconOutlineTexturePath) => profile.Ui
                        ?.IconOutlineTexturePath,
                    nameof(IModCharacterAssetOverrides.CustomIconPath) => profile.Ui?.IconPath,
                    nameof(IModCharacterAssetOverrides.CustomCharacterSelectBgPath) =>
                        profile.Ui?.CharacterSelectBgPath,
                    nameof(IModCharacterAssetOverrides.CustomCharacterSelectIconPath) =>
                        profile.Ui?.CharacterSelectIconPath,
                    nameof(IModCharacterAssetOverrides.CustomCharacterSelectLockedIconPath) =>
                        profile.Ui?.CharacterSelectLockedIconPath,
                    nameof(IModCharacterAssetOverrides.CustomCharacterSelectTransitionPath) =>
                        profile.Ui?.CharacterSelectTransitionPath,
                    nameof(IModCharacterAssetOverrides.CustomMapMarkerPath) => profile.Ui?.MapMarkerPath,
                    nameof(IModCharacterAssetOverrides.CustomTrailPath) => profile.Vfx?.TrailPath,
                    nameof(IModCharacterAssetOverrides.CustomAttackSfx) => profile.Audio?.AttackSfx,
                    nameof(IModCharacterAssetOverrides.CustomCastSfx) => profile.Audio?.CastSfx,
                    nameof(IModCharacterAssetOverrides.CustomDeathSfx) => profile.Audio?.DeathSfx,
                    nameof(IModCharacterAssetOverrides.CustomArmPointingTexturePath) =>
                        profile.Multiplayer?.ArmPointingTexturePath,
                    nameof(IModCharacterAssetOverrides.CustomArmRockTexturePath) =>
                        profile.Multiplayer?.ArmRockTexturePath,
                    nameof(IModCharacterAssetOverrides.CustomArmPaperTexturePath) =>
                        profile.Multiplayer?.ArmPaperTexturePath,
                    nameof(IModCharacterAssetOverrides.CustomArmScissorsTexturePath) =>
                        profile.Multiplayer?.ArmScissorsTexturePath,
                    _ => null,
                };
                if (!string.IsNullOrWhiteSpace(registered))
                    return registered;
            }

            if (instance is not IModCharacterAssetOverrides overrides) return null;
            var direct = selector(overrides);
            return !string.IsNullOrWhiteSpace(direct) ? direct : null;
        }

        private static bool TryResolveRegisteredProfile(CharacterModel instance, out CharacterAssetProfile profile)
        {
            return ModContentRegistry.TryGetEffectiveCharacterAssetReplacement(instance.Id.Entry, out profile);
        }
    }

    /// <summary>
    ///     Patches <see cref="CharacterModel.IconOutlineTexturePath" /> so <see cref="IModCharacterAssetOverrides" />
    ///     Patches <c>Character模型.图标Outline纹理路径</c> so <c>IModCharacterAssetOverrides</c>
    ///     can supply a custom outline texture path.
    ///     can supply a 自定义 outline 纹理 路径.
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterIconOutlineTexturePathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_icon_outline_texture_path";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.IconOutlineTexturePath";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.IconOutlineTexturePath), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     When the instance implements <see cref="IModCharacterAssetOverrides" /> and a valid override path exists,
        ///     当 the instance implements <c>IModCharacterAssetOverrides</c> 和 a 有效 override 路径 exists,
        ///     replaces the getter result; otherwise runs the original method.
        ///     replaces the getter result; otherwise runs the original method.
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(
                __instance,
                ref __result,
                o => o.CustomIconOutlineTexturePath,
                nameof(IModCharacterAssetOverrides.CustomIconOutlineTexturePath));
        }
    }

    /// <summary>
    ///     Patches <see cref="CharacterModel.VisualsPath" /> for custom mod character scene paths.
    ///     为 custom mod character scene paths 补丁 <c>CharacterModel.VisualsPath</c>。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterVisualsPathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_visuals_path";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.VisualsPath";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.VisualsPath), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomVisualsPath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModCharacterAssetOverrides.CustomVisualsPath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(
                __instance,
                ref __result,
                o => o.CustomVisualsPath,
                nameof(IModCharacterAssetOverrides.CustomVisualsPath));
        }
    }

    /// <summary>
    ///     Patches <see cref="CharacterModel.EnergyCounterPath" /> for mod character UI assets.
    ///     为 mod character UI assets 补丁 <c>CharacterModel.EnergyCounterPath</c>。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterEnergyCounterPathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_energy_counter_path";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.EnergyCounterPath";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.EnergyCounterPath), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomEnergyCounterPath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModCharacterAssetOverrides.CustomEnergyCounterPath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(__instance, ref __result,
                o => o.CustomEnergyCounterPath,
                nameof(IModCharacterAssetOverrides.CustomEnergyCounterPath));
        }
    }

    /// <summary>
    ///     Patches <see cref="CharacterModel.MerchantAnimPath" /> for merchant-room animations.
    ///     为 merchant-room animations 补丁 <c>CharacterModel.MerchantAnimPath</c>。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterMerchantAnimPathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_merchant_anim_path";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.MerchantAnimPath";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.MerchantAnimPath), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomMerchantAnimPath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModCharacterAssetOverrides.CustomMerchantAnimPath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(__instance, ref __result,
                o => o.CustomMerchantAnimPath,
                nameof(IModCharacterAssetOverrides.CustomMerchantAnimPath));
        }
    }

    /// <summary>
    ///     Patches <see cref="CharacterModel.RestSiteAnimPath" /> for rest-site animations.
    ///     为 rest-site animations 补丁 <c>CharacterModel.RestSiteAnimPath</c>。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterRestSiteAnimPathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_rest_site_anim_path";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.RestSiteAnimPath";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.RestSiteAnimPath), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomRestSiteAnimPath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModCharacterAssetOverrides.CustomRestSiteAnimPath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(__instance, ref __result,
                o => o.CustomRestSiteAnimPath,
                nameof(IModCharacterAssetOverrides.CustomRestSiteAnimPath));
        }
    }

    /// <summary>
    ///     Patches <see cref="CharacterModel.IconTexturePath" /> for mod character UI icon textures.
    ///     为 mod character UI icon textures 补丁 <c>CharacterModel.IconTexturePath</c>。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterIconTexturePathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_icon_texture_path";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.IconTexturePath";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.IconTexturePath), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomIconTexturePath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModCharacterAssetOverrides.CustomIconTexturePath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(__instance, ref __result,
                o => o.CustomIconTexturePath,
                nameof(IModCharacterAssetOverrides.CustomIconTexturePath));
        }
    }

    /// <summary>
    ///     Patches <see cref="CharacterModel.IconPath" /> for compact mod character icons.
    ///     为 compact mod character icons 补丁 <c>CharacterModel.IconPath</c>。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterIconPathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_icon_path";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.IconPath";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), "IconPath", MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomIconPath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModCharacterAssetOverrides.CustomIconPath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(
                __instance,
                ref __result,
                o => o.CustomIconPath,
                nameof(IModCharacterAssetOverrides.CustomIconPath));
        }
    }

    /// <summary>
    ///     Patches character-select background path so mods can replace <c>CharacterSelectBg</c>.
    ///     补丁 character-select background path so mods can replace <c>CharacterSelectBg</c>。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterSelectBgPathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_select_bg_path";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.CharacterSelectBg";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.CharacterSelectBg), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomCharacterSelectBgPath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModCharacterAssetOverrides.CustomCharacterSelectBgPath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(__instance, ref __result,
                o => o.CustomCharacterSelectBgPath,
                nameof(IModCharacterAssetOverrides.CustomCharacterSelectBgPath));
        }
    }

    /// <summary>
    ///     Patches non-public <see cref="CharacterModel.CharacterSelectIcon" /> path getter.
    ///     补丁 non-public <c>CharacterModel.CharacterSelectIcon</c> path getter。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterSelectIconPathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_select_icon_path";

        /// <inheritdoc />
        public static string Description => "Allow character-select icon path override for vanilla and mod characters";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), "CharacterSelectIconPath", null, true, MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomCharacterSelectIconPath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModCharacterAssetOverrides.CustomCharacterSelectIconPath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(
                __instance,
                ref __result,
                o => o.CustomCharacterSelectIconPath,
                nameof(IModCharacterAssetOverrides.CustomCharacterSelectIconPath));
        }
    }

    /// <summary>
    ///     Patches non-public <see cref="CharacterModel.CharacterSelectLockedIcon" /> path getter.
    ///     补丁 non-public <c>CharacterModel.CharacterSelectLockedIcon</c> path getter。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterSelectLockedIconPathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_select_locked_icon_path";

        /// <inheritdoc />
        public static string Description =>
            "Allow character-select locked icon path override for vanilla and mod characters";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), "CharacterSelectLockedIconPath", null, true, MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomCharacterSelectLockedIconPath" /> when valid.
        ///     当 valid 时提供 <c>IModCharacterAssetOverrides.CustomCharacterSelectLockedIconPath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(
                __instance,
                ref __result,
                o => o.CustomCharacterSelectLockedIconPath,
                nameof(IModCharacterAssetOverrides.CustomCharacterSelectLockedIconPath));
        }
    }

    /// <summary>
    ///     Patches non-public <see cref="CharacterModel.MapMarker" /> path getter.
    ///     补丁 non-public <c>CharacterModel.MapMarker</c> path getter。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterMapMarkerPathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_map_marker_path";

        /// <inheritdoc />
        public static string Description => "Allow character map-marker path override for vanilla and mod characters";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), "MapMarkerPath", null, true, MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomMapMarkerPath" /> when valid.
        ///     当 valid 时提供 <c>IModCharacterAssetOverrides.CustomMapMarkerPath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(
                __instance,
                ref __result,
                o => o.CustomMapMarkerPath,
                nameof(IModCharacterAssetOverrides.CustomMapMarkerPath));
        }
    }

    /// <summary>
    ///     Patches <see cref="CharacterModel.CharacterSelectTransitionPath" /> for custom select-screen transitions.
    ///     为 custom select-screen transitions 补丁 <c>CharacterModel.CharacterSelectTransitionPath</c>。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterSelectTransitionPathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_transition_path";

        /// <inheritdoc />
        public static string Description =>
            "Allow mod characters to override CharacterModel.CharacterSelectTransitionPath";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(CharacterModel), nameof(CharacterModel.CharacterSelectTransitionPath), MethodType.Getter),
            ];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomCharacterSelectTransitionPath" /> when valid.
        ///     当 valid 时提供 <c>IModCharacterAssetOverrides.CustomCharacterSelectTransitionPath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(
                __instance,
                ref __result,
                o => o.CustomCharacterSelectTransitionPath,
                nameof(IModCharacterAssetOverrides.CustomCharacterSelectTransitionPath));
        }
    }

    /// <summary>
    ///     Patches <see cref="CharacterModel.TrailPath" /> for card-trail VFX scenes.
    ///     为 card-trail VFX scenes 补丁 <c>CharacterModel.TrailPath</c>。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterTrailPathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_trail_path";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.TrailPath";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.TrailPath), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomTrailPath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModCharacterAssetOverrides.CustomTrailPath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(
                __instance,
                ref __result,
                o => o.CustomTrailPath,
                nameof(IModCharacterAssetOverrides.CustomTrailPath));
        }
    }

    /// <summary>
    ///     Patches <see cref="CharacterModel.AttackSfx" />; does not require the FMOD path to exist as a Godot resource.
    ///     补丁 <c>CharacterModel.AttackSfx</c>; does not require the FMOD path to exist as a Godot resource。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterAttackSfxPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_attack_sfx";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.AttackSfx";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.AttackSfx), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomAttackSfx" /> when non-empty.
        ///     当 non-empty 时提供 <c>IModCharacterAssetOverrides.CustomAttackSfx</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(
                __instance,
                ref __result,
                o => o.CustomAttackSfx,
                nameof(IModCharacterAssetOverrides.CustomAttackSfx),
                false);
        }
    }

    /// <summary>
    ///     Patches <see cref="CharacterModel.CastSfx" /> for custom cast audio.
    ///     为 custom cast audio 补丁 <c>CharacterModel.CastSfx</c>。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterCastSfxPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_cast_sfx";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.CastSfx";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.CastSfx), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomCastSfx" /> when non-empty.
        ///     当 non-empty 时提供 <c>IModCharacterAssetOverrides.CustomCastSfx</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(
                __instance,
                ref __result,
                o => o.CustomCastSfx,
                nameof(IModCharacterAssetOverrides.CustomCastSfx),
                false);
        }
    }

    /// <summary>
    ///     Patches <see cref="CharacterModel.DeathSfx" /> for custom death audio.
    ///     为 custom death audio 补丁 <c>CharacterModel.DeathSfx</c>。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterDeathSfxPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_death_sfx";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.DeathSfx";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.DeathSfx), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomDeathSfx" /> when non-empty.
        ///     当 non-empty 时提供 <c>IModCharacterAssetOverrides.CustomDeathSfx</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(
                __instance,
                ref __result,
                o => o.CustomDeathSfx,
                nameof(IModCharacterAssetOverrides.CustomDeathSfx),
                false);
        }
    }

    /// <summary>
    ///     Patches multiplayer arm texture path for the pointing pose.
    ///     为 the pointing pose 补丁 multiplayer arm texture path。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterArmPointingTexturePathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_arm_pointing_texture_path";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.ArmPointingTexturePath";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.ArmPointingTexturePath), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomArmPointingTexturePath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModCharacterAssetOverrides.CustomArmPointingTexturePath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(__instance, ref __result,
                o => o.CustomArmPointingTexturePath,
                nameof(IModCharacterAssetOverrides.CustomArmPointingTexturePath));
        }
    }

    /// <summary>
    ///     Patches multiplayer RPS “rock” arm texture path.
    ///     补丁 multiplayer RPS “rock” arm texture path。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterArmRockTexturePathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_arm_rock_texture_path";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.ArmRockTexturePath";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.ArmRockTexturePath), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomArmRockTexturePath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModCharacterAssetOverrides.CustomArmRockTexturePath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(__instance, ref __result,
                o => o.CustomArmRockTexturePath,
                nameof(IModCharacterAssetOverrides.CustomArmRockTexturePath));
        }
    }

    /// <summary>
    ///     Patches multiplayer RPS “paper” arm texture path.
    ///     补丁 multiplayer RPS “paper” arm texture path。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterArmPaperTexturePathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_arm_paper_texture_path";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.ArmPaperTexturePath";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.ArmPaperTexturePath), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomArmPaperTexturePath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModCharacterAssetOverrides.CustomArmPaperTexturePath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(__instance, ref __result,
                o => o.CustomArmPaperTexturePath,
                nameof(IModCharacterAssetOverrides.CustomArmPaperTexturePath));
        }
    }

    /// <summary>
    ///     Patches multiplayer RPS “scissors” arm texture path.
    ///     补丁 multiplayer RPS “scissors” arm texture path。
    /// </summary>
    [HarmonyAfter(Const.BaseLibHarmonyId)]
    [HarmonyPriority(Priority.Last)]
    public class CharacterArmScissorsTexturePathPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "character_asset_override_arm_scissors_texture_path";

        /// <inheritdoc />
        public static string Description => "Allow mod characters to override CharacterModel.ArmScissorsTexturePath";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(CharacterModel), nameof(CharacterModel.ArmScissorsTexturePath), MethodType.Getter)];
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Supplies <see cref="IModCharacterAssetOverrides.CustomArmScissorsTexturePath" /> when the resource exists.
        ///     当 the resource exists 时提供 <c>IModCharacterAssetOverrides.CustomArmScissorsTexturePath</c>。
        /// </summary>
        public static bool Prefix(CharacterModel __instance, ref string __result)
            // ReSharper restore InconsistentNaming
        {
            return CharacterAssetOverridePatchHelper.TryUseOverride(__instance, ref __result,
                o => o.CustomArmScissorsTexturePath,
                nameof(IModCharacterAssetOverrides.CustomArmScissorsTexturePath));
        }
    }
}
