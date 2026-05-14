using System.Reflection;
using HarmonyLib;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.Patching.Core
{
    /// <summary>
    ///     Resolves a vanilla <see cref="MethodBase" /> from patch-target metadata, matching
    ///     <see cref="ModPatcher" /> / <see cref="ModPatchTarget" /> semantics.
    ///     根据补丁目标元数据解析原版 <c>MethodBase</c>，语义与
    ///     <c>ModPatcher</c> / <c>ModPatchTarget</c> 保持一致。
    /// </summary>
    public static class PatchTargetMethodResolver
    {
        /// <summary>
        ///     Resolves using fields from <see cref="ModPatchInfo" />.
        ///     使用 <c>ModPatchInfo</c> 中的字段进行解析。
        /// </summary>
        public static MethodBase? Resolve(ModPatchInfo modPatchInfo)
        {
            return Resolve(
                modPatchInfo.TargetType,
                modPatchInfo.MethodName,
                modPatchInfo.ParameterTypes,
                modPatchInfo.HarmonyMethodType);
        }

        /// <summary>
        ///     Resolves using <see cref="ModPatchTarget" />.
        ///     使用 <c>ModPatchTarget</c> 进行解析。
        /// </summary>
        public static MethodBase? Resolve(ModPatchTarget target)
        {
            return Resolve(target.TargetType, target.MethodName, target.ParameterTypes, target.HarmonyMethodType);
        }

        /// <summary>
        ///     Like <see cref="Resolve(ModPatchTarget)" /> but throws <see cref="MissingMethodException" /> when unresolved.
        ///     与 <c>Resolve(ModPatchTarget)</c> 类似，但在无法解析时抛出 <c>MissingMethodException</c>。
        /// </summary>
        public static MethodBase ResolveRequired(ModPatchTarget target)
        {
            return Resolve(target) ?? throw new MissingMethodException(
                target.TargetType.FullName,
                $"{target.MethodName} ({target.HarmonyMethodType})");
        }

        /// <summary>
        ///     Like <see cref="Resolve(System.Type,string,System.Type[],HarmonyLib.MethodType)" /> but throws when unresolved.
        ///     与 <c>Resolve(System.Type,string,System.Type[],HarmonyLib.MethodType)</c> 类似，但在无法解析时抛出异常。
        /// </summary>
        public static MethodBase ResolveRequired(
            Type targetType,
            string methodName,
            Type[]? parameterTypes,
            MethodType harmonyMethodType)
        {
            return Resolve(targetType, methodName, parameterTypes, harmonyMethodType) ??
                   throw new MissingMethodException(targetType.FullName, $"{methodName} ({harmonyMethodType})");
        }

        /// <summary>
        ///     Core resolution: <see cref="MethodType.Normal" /> uses reflection <c>Type.GetMethod</c> (inheritance-aware);
        ///     other <see cref="MethodType" /> values use Harmony <see cref="AccessTools" /> helpers.
        ///     核心解析逻辑：<c>MethodType.Normal</c> 使用反射 <c>Type.GetMethod</c>（支持继承）；
        ///     其他 <c>MethodType</c> 值使用 Harmony 的 <c>AccessTools</c> 辅助方法。
        /// </summary>
        public static MethodBase? Resolve(
            Type targetType,
            string methodName,
            Type[]? parameterTypes,
            MethodType harmonyMethodType)
        {
            return harmonyMethodType switch
            {
                MethodType.Normal => ResolveNormal(targetType, methodName, parameterTypes),
                MethodType.Async => GetAsyncStateMachineMoveNext(targetType, methodName, parameterTypes),
                MethodType.Getter => AccessTools.DeclaredProperty(targetType, methodName)?.GetGetMethod(true),
                MethodType.Setter => AccessTools.DeclaredProperty(targetType, methodName)?.GetSetMethod(true),
                MethodType.Constructor => AccessTools.DeclaredConstructor(targetType, parameterTypes),
                MethodType.Enumerator => GetEnumeratorMoveNext(targetType, methodName, parameterTypes),
                _ => ResolveNormal(targetType, methodName, parameterTypes),
            };
        }

        private static MethodInfo? ResolveNormal(Type targetType, string methodName, Type[]? parameterTypes)
        {
            if (parameterTypes != null)
                return targetType.GetMethod(
                    methodName,
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    parameterTypes,
                    null);

            return targetType.GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static MethodInfo? GetAsyncStateMachineMoveNext(Type targetType, string methodName,
            Type[]? parameterTypes)
        {
            var outer = ResolveNormal(targetType, methodName, parameterTypes);
            return outer is null ? null : AccessTools.AsyncMoveNext(outer);
        }

        private static MethodInfo? GetEnumeratorMoveNext(Type targetType, string methodName, Type[]? parameterTypes)
        {
            var outer = ResolveNormal(targetType, methodName, parameterTypes);
            return outer is null ? null : AccessTools.EnumeratorMoveNext(outer);
        }
    }
}
