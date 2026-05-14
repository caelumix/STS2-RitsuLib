using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.Formatters;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Content;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Utils;

namespace STS2RitsuLib.Scaffolding.Content.Patches
{
    /// <summary>
    ///     Intercepts <c>EnergyIconsFormatter.TryEvaluateFormat</c> after it assembles the
    ///     Intercepts <c>EnergyIconsFormatter.TryEvaluateFormat</c> 之后 it assembles the
    ///     hard-coded rich-text img tag for the small energy icon used in card descriptions, and
    ///     hard-coded rich-text img tag 用于 the small energy 图标 used in 卡牌 descriptions, and
    ///     replaces it with a custom path when the owning card pool implements
    ///     replaces it 带有 a 自定义 路径 当 the owning 卡牌 pool implements
    ///     <see cref="IModTextEnergyIconPool" />.
    ///     <para />
    ///     The default game path pattern is:
    ///     The default game 路径 pattern is:
    ///     <c>[img]res://images/packed/sprite_fonts/{prefix}_energy_icon.png[/img]</c>.
    ///     Implementing <see cref="IModTextEnergyIconPool.TextEnergyIconPath" /> on the
    ///     Implementing <c>IModTextEnergyIconPool.TextEnergyIcon路径</c> on the
    ///     <see cref="MegaCrit.Sts2.Core.Models.CardPoolModel" /> lets you use any resource path.
    /// </summary>
    public class EnergyIconFormatterPatch : IPatchMethod
    {
        /// <inheritdoc cref="IPatchMethod.PatchId" />
        public static string PatchId => "energy_icon_formatter_text_icon_override";

        /// <inheritdoc cref="IPatchMethod.Description" />
        public static string Description =>
            "Allow mod card pools to override the small energy icon path in rich-text card descriptions";

        /// <inheritdoc cref="IPatchMethod.IsCritical" />
        public static bool IsCritical => false;

        /// <inheritdoc cref="IPatchMethod.GetTargets" />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(EnergyIconsFormatter), "TryEvaluateFormat")];
        }

        /// <summary>
        ///     After the formatter stores the assembled <c>text3</c> img-tag into its local variable,
        ///     之后 the 用于matter stores the assembled <c>text3</c> img-tag into its local variable,
        ///     insert a call that lets <see cref="ModTextEnergyIconHelper" /> redirect to a custom path.
        ///     insert a call that lets <c>ModTextEnergyIconHelper</c> redirect to a 自定义 路径.
        ///     Matched IL pattern (inside TryEvaluateFormat):
        ///     中文说明：Matched IL pattern (inside TryEvaluateFormat):
        ///     <code>
        ///         ldstr  "[img]res://images/packed/sprite_fonts/"
        ///         中文说明：ldstr  "[img]res://images/packed/sprite_fonts/"
        ///         ldloc  (text / prefix)
        ///         中文说明：ldloc  (text / prefix)
        ///         ldstr  "_energy_icon.png[/img]"
        ///         ldstr  "_energy_图标.png[/img]"
        ///         call   string.Concat(string, string, string)
        ///         中文说明：call   string.Concat(string, string, string)
        ///         stloc  (text3)                        ← match ends here
        ///         中文说明：stloc  (text3)                        ← match ends here
        ///     </code>
        ///     Inserted after stloc:
        ///     Inserted 之后 stloc:
        ///     <code>
        ///         ldloc  (text)
        ///         中文说明：ldloc  (text)
        ///         ldloc  (text3)
        ///         中文说明：ldloc  (text3)
        ///         call   ModTextEnergyIconHelper.OverrideTextIconTag
        ///         call   ModTextEnergyIconHelper.OverrideTextIconTag
        ///         stloc  (text3)
        ///         中文说明：stloc  (text3)
        ///     </code>
        /// </summary>
        [HarmonyAfter(Const.BaseLibHarmonyId)]
        [HarmonyPriority(Priority.Last)]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var concatMethod = AccessTools.Method(
                typeof(string), nameof(string.Concat),
                [typeof(string), typeof(string), typeof(string)]);

            var overrideMethod = AccessTools.Method(
                typeof(ModTextEnergyIconHelper),
                nameof(ModTextEnergyIconHelper.OverrideTextIconTag));

            var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            var matcher = new CodeMatcher(codeInstructions)
                .MatchStartForward(
                    new CodeMatch(OpCodes.Ldstr, "[img]res://images/packed/sprite_fonts/"),
                    new CodeMatch(i => i.IsLdloc()),
                    new CodeMatch(OpCodes.Ldstr),
                    new CodeMatch(OpCodes.Call, concatMethod));

            if (matcher.IsInvalid)
                return codeInstructions;

            var ldlocText = matcher.InstructionAt(1).Clone();

            matcher.Advance(4);

            if (!matcher.Instruction.IsStloc())
                return codeInstructions;

            var stlocText3 = matcher.Instruction;
            var ldlocText3 = StlocToLdloc(stlocText3);

            matcher.Advance().Insert(ldlocText, ldlocText3, new CodeInstruction(OpCodes.Call, overrideMethod),
                stlocText3.Clone());

            return matcher.InstructionEnumeration();
        }

        private static CodeInstruction StlocToLdloc(CodeInstruction stloc)
        {
            if (stloc.opcode == OpCodes.Stloc_0) return new(OpCodes.Ldloc_0);
            if (stloc.opcode == OpCodes.Stloc_1) return new(OpCodes.Ldloc_1);
            if (stloc.opcode == OpCodes.Stloc_2) return new(OpCodes.Ldloc_2);
            if (stloc.opcode == OpCodes.Stloc_3) return new(OpCodes.Ldloc_3);
            if (stloc.opcode == OpCodes.Stloc_S) return new(OpCodes.Ldloc_S, stloc.operand);
            return new(OpCodes.Ldloc, stloc.operand);
        }
    }

    /// <summary>
    ///     Runtime helper called by the patched formatter.
    ///     runtime helper called 通过 the patched 用于matter.
    ///     On first use it builds a lookup table from all registered mod characters' card pools
    ///     On first 使用 it builds a lookup table 从 all 已注册 mod characters' 卡牌 pools
    ///     that implement <see cref="IModTextEnergyIconPool" />.
    ///     that implement <c>IModTextEnergyIconPool</c>.
    /// </summary>
    internal static class ModTextEnergyIconHelper
    {
        private static Dictionary<string, string>? _cache;

        public static string OverrideTextIconTag(string prefix, string defaultTag)
        {
            _cache ??= BuildCache();
            return _cache.TryGetValue(prefix, out var path)
                ? $"[img]{path}[/img]"
                : defaultTag;
        }

        private static Dictionary<string, string> BuildCache()
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var character in ModContentRegistry.GetModCharacters())
                AddPoolIfMapped(dict, character.CardPool);

            foreach (var pool in ModelDb.AllCards.Select(c => c.Pool).Distinct())
                AddPoolIfMapped(dict, pool);

            foreach (var pool in ModelDb.AllRelics.Select(r => r.Pool).Distinct())
                AddPoolIfMapped(dict, pool);

            foreach (var pool in ModelDb.AllPotions.Select(p => p.Pool).Distinct())
                AddPoolIfMapped(dict, pool);

            return dict;
        }

        private static void AddPoolIfMapped(Dictionary<string, string> dict, IPoolModel pool)
        {
            if (pool is not IModTextEnergyIconPool mapped)
                return;

            if (string.IsNullOrWhiteSpace(mapped.TextEnergyIconPath))
                return;

            if (!AssetPathDiagnostics.Exists(mapped.TextEnergyIconPath!, pool,
                    nameof(IModTextEnergyIconPool.TextEnergyIconPath)))
                return;

            dict.TryAdd(pool.EnergyColorName, mapped.TextEnergyIconPath!);
        }
    }
}
