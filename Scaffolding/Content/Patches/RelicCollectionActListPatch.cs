using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Utils.HarmonyIl;

namespace STS2RitsuLib.Scaffolding.Content.Patches
{
    /// <summary>
    ///     Replaces the vanilla relic collection's hard-coded ancient act list with the runtime <see cref="ModelDb.Acts" />
    ///     sequence so registered mod acts can contribute ancient relic subcategories.
    /// </summary>
    public class RelicCollectionActListPatch : IPatchMethod
    {
        private static readonly MethodInfo ModelDbActsGetter =
            AccessTools.PropertyGetter(typeof(ModelDb), nameof(ModelDb.Acts));

        private static readonly MethodInfo RuntimeActListMethod =
            AccessTools.DeclaredMethod(typeof(RelicCollectionActListPatch), nameof(GetRuntimeActList));

        /// <inheritdoc />
        public static string PatchId => "relic_collection_runtime_act_list";

        /// <inheritdoc />
        public static string Description => "Use runtime ModelDb.Acts for relic collection ancient categories";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NRelicCollectionCategory), nameof(NRelicCollectionCategory.LoadRelics))];
        }

        /// <summary>
        ///     Stores the runtime act list into the local variable that vanilla later uses for ancient category generation.
        /// </summary>
        [HarmonyAfter(Const.BaseLibHarmonyId)]
        [HarmonyPriority(Priority.Last)]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var rewriter = HarmonyIlRewriter.From(instructions);
            const string operation = "[RelicCollection] Replace hard-coded ancient act list";

            if (rewriter.Contains(instruction => HarmonyIl.IsCallTo(instruction, RuntimeActListMethod)))
                return rewriter.InstructionsChecked(operation);

            if (TryFindVanillaActListStore(rewriter.Code, out var match))
            {
                rewriter.InsertBefore(match,
                [
                    HarmonyIl.Pop(),
                    HarmonyIl.Call(RuntimeActListMethod),
                ]);
                return rewriter.InstructionsChecked(operation);
            }

            RitsuLibFramework.Logger.Warn(
                "[RelicCollection] Could not find vanilla act-list validation pattern; runtime act list patch skipped.");

            return rewriter.Instructions();
        }

        private static List<ActModel> GetRuntimeActList()
        {
            return ModelDb.Acts
                .DistinctBy(static act => act.Id)
                .Select(static (act, index) => new { Act = act, Index = index })
                .OrderBy(static item => GetVanillaActOrder(item.Act))
                .ThenBy(static item => item.Index)
                .Select(static item => item.Act)
                .ToList();
        }

        private static int GetVanillaActOrder(ActModel act)
        {
            return act switch
            {
                Overgrowth => 0,
                Underdocks => 1,
                Hive => 2,
                Glory => 3,
                _ => 4,
            };
        }

        private static bool IsModelDbActsCall(CodeInstruction instruction)
        {
            return HarmonyIl.IsCallTo(instruction, ModelDbActsGetter);
        }

        private static bool IsLinqCall(CodeInstruction instruction, string methodName)
        {
            if (instruction.opcode != OpCodes.Call || instruction.operand is not MethodInfo method)
                return false;

            if (method.DeclaringType != typeof(Enumerable) || method.Name != methodName)
                return false;

            return !method.IsGenericMethod || method.GetGenericArguments().Contains(typeof(ActModel));
        }

        private static bool IsActListErrorString(CodeInstruction instruction)
        {
            return instruction.opcode == OpCodes.Ldstr && instruction.operand is string value &&
                   value.Contains("act list", StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryFindVanillaActListStore(IReadOnlyList<CodeInstruction> code, out HarmonyIlMatch match)
        {
            for (var errorIndex = 0; errorIndex < code.Count; errorIndex++)
            {
                if (!IsActListErrorString(code[errorIndex]))
                    continue;

                var anyIndex = FindPrevious(code, errorIndex,
                    instruction => IsLinqCall(instruction, nameof(Enumerable.Any)));
                if (anyIndex < 0)
                    continue;

                var exceptIndex = FindPrevious(code, anyIndex,
                    instruction => IsLinqCall(instruction, nameof(Enumerable.Except)));
                if (exceptIndex < 0)
                    continue;

                var actsIndex = FindPrevious(code, exceptIndex, IsModelDbActsCall);
                if (actsIndex < 0)
                    continue;

                var storeIndex = FindPrevious(code, actsIndex, instruction => instruction.IsStloc());
                if (storeIndex < 0)
                    continue;

                match = new(storeIndex, 1);
                return true;
            }

            match = default;
            return false;
        }

        private static int FindPrevious(
            IReadOnlyList<CodeInstruction> code,
            int beforeIndex,
            Func<CodeInstruction, bool> predicate)
        {
            for (var i = beforeIndex - 1; i >= 0; i--)
                if (predicate(code[i]))
                    return i;

            return -1;
        }
    }
}
