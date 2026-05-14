using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.Content.Patches
{
    /// <summary>
    ///     Appends RitsuLib-registered characters to <see cref="ModelDb.AllCharacters" />.
    ///     Appends RitsuLib-已注册 characters to <c>ModelDb.AllCharacters</c>.
    /// </summary>
    public class AllCharactersPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_all_characters";

        /// <inheritdoc />
        public static string Description => "Append registered characters to ModelDb.AllCharacters";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "AllCharacters", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Concatenates mod-registered characters onto the vanilla sequence.
        ///     Concatenates mod-已注册 characters onto the 原版 sequence.
        /// </summary>
        [HarmonyAfter(Const.BaseLibHarmonyId)]
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(ref IEnumerable<CharacterModel> __result)
        {
            __result = ModContentRegistry.AppendCharacters(__result);
        }
    }

    /// <summary>
    ///     Merges RitsuLib-registered monster types into <see cref="ModelDb.Monsters" /> by <see cref="AbstractModel.Id" />.
    ///     Merges RitsuLib-已注册 monster types into <c>ModelDb.Monsters</c> 通过 <c>AbstrActModel.Id</c>.
    /// </summary>
    public class AllMonstersPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_all_monsters";

        /// <inheritdoc />
        public static string Description => "Merge registered monsters into ModelDb.Monsters";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "Monsters", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Ensures standalone-registered monsters appear in global monster enumeration even before every act lists them.
        ///     Ensures standalone-已注册 monsters appear in global monster enumeration even 之前 every 章节 lists them.
        /// </summary>
        public static void Postfix(ref IEnumerable<MonsterModel> __result)
            // ReSharper restore InconsistentNaming
        {
            __result = ModContentRegistry.AppendRegisteredMonsters(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered acts to <see cref="ModelDb.Acts" />.
    ///     Appends RitsuLib-已注册 章节s to <c>ModelDb.章节s</c>.
    /// </summary>
    public class ActsPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_acts";

        /// <inheritdoc />
        public static string Description => "Append registered acts to ModelDb.Acts";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "Acts", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Concatenates mod-registered acts onto the vanilla sequence.
        ///     Concatenates mod-已注册 章节s onto the 原版 sequence.
        /// </summary>
        [HarmonyAfter(Const.BaseLibHarmonyId)]
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(ref IEnumerable<ActModel> __result)
        {
            __result = ModContentRegistry.AppendActs(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered shared events to <see cref="ModelDb.AllSharedEvents" />.
    ///     Appends RitsuLib-已注册 shared 事件s to <c>ModelDb.AllSharedEvents</c>.
    /// </summary>
    public class AllSharedEventsPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_all_shared_events";

        /// <inheritdoc />
        public static string Description => "Append registered shared events to ModelDb.AllSharedEvents";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "AllSharedEvents", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Concatenates mod shared events onto the vanilla sequence.
        ///     Concatenates mod shared 事件s onto the 原版 sequence.
        /// </summary>
        [HarmonyAfter(Const.BaseLibHarmonyId)]
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(ref IEnumerable<EventModel> __result)
        {
            __result = ModContentRegistry.AppendSharedEvents(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered powers to <see cref="ModelDb.AllPowers" />.
    ///     Appends RitsuLib-已注册 能力s to <c>ModelDb.AllPowers</c>.
    /// </summary>
    public class AllPowersPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_all_powers";

        /// <inheritdoc />
        public static string Description => "Append registered powers to ModelDb.AllPowers";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "AllPowers", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Concatenates mod powers onto the vanilla sequence.
        ///     Concatenates mod 能力s onto the 原版 sequence.
        /// </summary>
        public static void Postfix(ref IEnumerable<PowerModel> __result)
        {
            __result = ModContentRegistry.AppendPowers(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered orbs to <see cref="ModelDb.Orbs" />.
    ///     Appends RitsuLib-已注册 充能球s to <c>ModelDb.充能球s</c>.
    /// </summary>
    public class AllOrbsPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_orbs";

        /// <inheritdoc />
        public static string Description => "Append registered orbs to ModelDb.Orbs";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "Orbs", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Concatenates mod orbs onto the vanilla sequence.
        ///     Concatenates mod 充能球s onto the 原版 sequence.
        /// </summary>
        public static void Postfix(ref IEnumerable<OrbModel> __result)
        {
            __result = ModContentRegistry.AppendOrbs(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered shared card pools to <see cref="ModelDb.AllSharedCardPools" />.
    ///     Appends RitsuLib-已注册 shared 卡牌 pools to <c>ModelDb.AllSharedCardPools</c>.
    /// </summary>
    public class AllSharedCardPoolsPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_all_shared_card_pools";

        /// <inheritdoc />
        public static string Description => "Append registered shared card pools to ModelDb.AllSharedCardPools";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "AllSharedCardPools", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Concatenates mod shared card pools onto the vanilla sequence.
        ///     Concatenates mod shared 卡牌 pools onto the 原版 sequence.
        /// </summary>
        [HarmonyAfter(Const.BaseLibHarmonyId)]
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(ref IEnumerable<CardPoolModel> __result)
        {
            __result = ModContentRegistry.AppendSharedCardPools(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered shared events to <see cref="ModelDb.AllEvents" />.
    ///     Appends RitsuLib-已注册 shared 事件s to <c>ModelDb.AllEvents</c>.
    /// </summary>
    public class AllEventsPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_all_events";

        /// <inheritdoc />
        public static string Description => "Append registered shared events to ModelDb.AllEvents";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "AllEvents", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Concatenates mod shared events onto the <see cref="ModelDb.AllEvents" /> sequence.
        ///     Concatenates mod shared 事件s onto the <c>ModelDb.AllEvents</c> sequence.
        /// </summary>
        public static void Postfix(ref IEnumerable<EventModel> __result)
        {
            __result = ModContentRegistry.AppendSharedEvents(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered shared ancients to <see cref="ModelDb.AllSharedAncients" />.
    ///     Appends RitsuLib-已注册 shared ancients to <c>ModelDb.AllSharedAncients</c>.
    /// </summary>
    public class AllSharedAncientsPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_all_shared_ancients";

        /// <inheritdoc />
        public static string Description => "Append registered shared ancients to ModelDb.AllSharedAncients";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "AllSharedAncients", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Concatenates mod shared ancients onto the vanilla sequence.
        ///     Concatenates mod shared ancients onto the 原版 sequence.
        /// </summary>
        [HarmonyAfter(Const.BaseLibHarmonyId)]
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(ref IEnumerable<AncientEventModel> __result)
        {
            __result = ModContentRegistry.AppendSharedAncients(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered shared ancients to <see cref="ModelDb.AllAncients" />.
    ///     Appends RitsuLib-已注册 shared ancients to <c>ModelDb.AllAncients</c>.
    /// </summary>
    public class AllAncientsPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_all_ancients";

        /// <inheritdoc />
        public static string Description => "Append registered shared ancients to ModelDb.AllAncients";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "AllAncients", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Concatenates mod shared ancients onto the <see cref="ModelDb.AllAncients" /> sequence.
        ///     中文说明：Concatenates mod shared ancients onto the <c>ModelDb.AllAncients</c> sequence.
        ///     Concatenates mod shared ancients onto the <c>ModelDb.AllAncients</c> sequence.
        ///     中文说明：Concatenates mod shared ancients onto the <c>ModelDb.AllAncients</c> sequence.
        /// </summary>
        public static void Postfix(ref IEnumerable<AncientEventModel> __result)
        {
            __result = ModContentRegistry.AppendSharedAncients(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered enchantments to <see cref="ModelDb.DebugEnchantments" /> (covers dynamic types not in
    ///     Appends RitsuLib-已注册 enchantments to <c>ModelDb.DebugEnchantments</c> (covers dynamic types not in
    ///     subtype scan).
    ///     中文说明：subtype scan).
    /// </summary>
    public class DebugEnchantmentsPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_debug_enchantments";

        /// <inheritdoc />
        public static string Description => "Append registered enchantments to ModelDb.DebugEnchantments";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "DebugEnchantments", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Concatenates mod enchantments onto the vanilla sequence.
        ///     Concatenates mod enchantments onto the 原版 sequence.
        /// </summary>
        public static void Postfix(ref IEnumerable<EnchantmentModel> __result)
        {
            __result = ModContentRegistry.AppendEnchantments(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered afflictions to <see cref="ModelDb.DebugAfflictions" />.
    ///     Appends RitsuLib-已注册 afflictions to <c>ModelDb.DebugAfflictions</c>.
    /// </summary>
    public class DebugAfflictionsPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_debug_afflictions";

        /// <inheritdoc />
        public static string Description => "Append registered afflictions to ModelDb.DebugAfflictions";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "DebugAfflictions", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Concatenates mod afflictions onto the vanilla sequence.
        ///     Concatenates mod afflictions onto the 原版 sequence.
        /// </summary>
        public static void Postfix(ref IEnumerable<AfflictionModel> __result)
        {
            __result = ModContentRegistry.AppendAfflictions(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered achievements to <see cref="ModelDb.Achievements" />.
    ///     Appends RitsuLib-已注册 achievements to <c>ModelDb.Achievements</c>.
    /// </summary>
    public class AchievementsPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_achievements";

        /// <inheritdoc />
        public static string Description => "Append registered achievements to ModelDb.Achievements";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "Achievements", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Merges mod achievements into the vanilla list by <see cref="AbstractModel.Id" />.
        ///     Merges mod achievements into the 原版 list 通过 <c>AbstrActModel.Id</c>.
        /// </summary>
        public static void Postfix(ref IReadOnlyList<AchievementModel> __result)
        {
            __result = ModContentRegistry.AppendAchievements(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered modifiers to <see cref="ModelDb.GoodModifiers" />.
    ///     Appends RitsuLib-已注册 modifiers to <c>ModelDb.GoodModifiers</c>.
    /// </summary>
    public class GoodModifiersPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_good_modifiers";

        /// <inheritdoc />
        public static string Description => "Append registered good modifiers to ModelDb.GoodModifiers";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "GoodModifiers", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Merges mod good modifiers into the vanilla list by <see cref="AbstractModel.Id" />.
        ///     Merges mod good modifiers into the 原版 list 通过 <c>AbstrActModel.Id</c>.
        /// </summary>
        public static void Postfix(ref IReadOnlyList<ModifierModel> __result)
        {
            __result = ModContentRegistry.AppendGoodModifiers(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered modifiers to <see cref="ModelDb.BadModifiers" />.
    ///     Appends RitsuLib-已注册 modifiers to <c>ModelDb.BadModifiers</c>.
    /// </summary>
    public class BadModifiersPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_bad_modifiers";

        /// <inheritdoc />
        public static string Description => "Append registered bad modifiers to ModelDb.BadModifiers";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "BadModifiers", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Merges mod bad modifiers into the vanilla list by <see cref="AbstractModel.Id" />.
        ///     Merges mod bad modifiers into the 原版 list 通过 <c>AbstrActModel.Id</c>.
        /// </summary>
        public static void Postfix(ref IReadOnlyList<ModifierModel> __result)
        {
            __result = ModContentRegistry.AppendBadModifiers(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered shared relic pools to <see cref="ModelDb.AllRelicPools" />.
    ///     Appends RitsuLib-已注册 shared 遗物 pools to <c>ModelDb.AllRelicPools</c>.
    /// </summary>
    public class AllRelicPoolsPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_all_relic_pools";

        /// <inheritdoc />
        public static string Description => "Append registered shared relic pools to ModelDb.AllRelicPools";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "AllRelicPools", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Concatenates mod shared relic pools onto the vanilla sequence.
        ///     Concatenates mod shared 遗物 pools onto the 原版 sequence.
        /// </summary>
        public static void Postfix(ref IEnumerable<RelicPoolModel> __result)
        {
            __result = ModContentRegistry.AppendSharedRelicPools(__result);
        }
    }

    /// <summary>
    ///     Appends RitsuLib-registered shared potion pools to <see cref="ModelDb.AllPotionPools" />.
    ///     Appends RitsuLib-已注册 shared potion pools to <c>ModelDb.AllPotionPools</c>.
    /// </summary>
    public class AllPotionPoolsPatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "modeldb_all_potion_pools";

        /// <inheritdoc />
        public static string Description => "Append registered shared potion pools to ModelDb.AllPotionPools";

        /// <inheritdoc />
        public static bool IsCritical => true;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(ModelDb), "AllPotionPools", MethodType.Getter)];
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Concatenates mod shared potion pools onto the vanilla sequence.
        ///     Concatenates mod shared potion pools onto the 原版 sequence.
        /// </summary>
        public static void Postfix(ref IEnumerable<PotionPoolModel> __result)
        {
            __result = ModContentRegistry.AppendSharedPotionPools(__result);
        }
    }
}
