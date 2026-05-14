using HarmonyLib;

namespace STS2RitsuLib.Patching.Models
{
    /// <summary>
    ///     Vanilla method identity used with <see cref="IPatchMethod.GetTargets" /> to build <see cref="ModPatchInfo" />.
    ///     原版 method identity used 带有 <c>IPatchMethod.GetTargets</c> to build <c>ModPatchInfo</c>.
    /// </summary>
    /// <param name="TargetType">
    ///     Declaring type.
    ///     中文说明：Declaring type.
    /// </param>
    /// <param name="MethodName">
    ///     Method name.
    ///     中文说明：Method name.
    /// </param>
    /// <param name="ParameterTypes">
    ///     Overload parameter types, or null for name-only lookup.
    ///     Over加载 parameter types, 或 null 用于 name-only lookup.
    /// </param>
    /// <param name="IgnoreIfMissing">
    ///     Maps to <see cref="ModPatchInfo.IgnoreIfTargetMissing" />.
    ///     中文说明：Maps to <c>ModPatchInfo.IgnoreIfTargetMissing</c>.
    /// </param>
    /// <param name="HarmonyMethodType">
    ///     Harmony <see cref="MethodType" /> for resolution (e.g. <see cref="MethodType.Async" /> for async state
    ///     Harmony <c>MethodType</c> 用于 resolution (e.g. <c>MethodType.Async</c> 用于 async state
    ///     machines).
    ///     中文说明：machines).
    /// </param>
    public record ModPatchTarget(
        Type TargetType,
        string MethodName,
        Type[]? ParameterTypes,
        bool IgnoreIfMissing,
        MethodType HarmonyMethodType)
    {
        /// <summary>
        ///     Legacy four-argument constructor; sets <see cref="HarmonyMethodType" /> to <see cref="MethodType.Normal" />.
        ///     Legacy four-argument constructor; 设置 <c>HarmonyMethodType</c> to <c>MethodType.Normal</c>.
        /// </summary>
        /// <param name="targetType">
        ///     Declaring type.
        ///     中文说明：Declaring type.
        /// </param>
        /// <param name="methodName">
        ///     Method name.
        ///     中文说明：Method name.
        /// </param>
        /// <param name="parameterTypes">
        ///     Overload parameter types.
        ///     Over加载 parameter types.
        /// </param>
        /// <param name="ignoreIfMissing">
        ///     When true, missing method is non-fatal for optional patches.
        ///     为 true 时，missing method is non-fatal for optional patches。
        /// </param>
        public ModPatchTarget(Type targetType, string methodName, Type[]? parameterTypes, bool ignoreIfMissing)
            : this(targetType, methodName, parameterTypes, ignoreIfMissing, MethodType.Normal)
        {
        }

        /// <summary>
        ///     Target with optional overload signature; not ignored if missing.
        ///     目标 with optional overload signature; not ignored if missing。
        /// </summary>
        /// <param name="targetType">
        ///     Declaring type.
        ///     中文说明：Declaring type.
        /// </param>
        /// <param name="methodName">
        ///     Method name.
        ///     中文说明：Method name.
        /// </param>
        /// <param name="parameterTypes">
        ///     Overload parameter types.
        ///     Over加载 parameter types.
        /// </param>
        public ModPatchTarget(Type targetType, string methodName, Type[]? parameterTypes)
            // ReSharper disable once IntroduceOptionalParameters.Global
            : this(targetType, methodName, parameterTypes, false)
        {
        }

        /// <summary>
        ///     Target with overload signature and Harmony <see cref="MethodType" />.
        ///     目标 with overload signature and Harmony <c>MethodType</c>。
        /// </summary>
        /// <param name="targetType">
        ///     Declaring type.
        ///     中文说明：Declaring type.
        /// </param>
        /// <param name="methodName">
        ///     Method name.
        ///     中文说明：Method name.
        /// </param>
        /// <param name="parameterTypes">
        ///     Overload parameter types.
        ///     Over加载 parameter types.
        /// </param>
        /// <param name="harmonyMethodType">
        ///     Harmony method type (e.g. <see cref="MethodType.Async" />).
        ///     中文说明：Harmony method type (e.g. <c>MethodType.Async</c>).
        /// </param>
        public ModPatchTarget(Type targetType, string methodName, Type[]? parameterTypes, MethodType harmonyMethodType)
            : this(targetType, methodName, parameterTypes, false, harmonyMethodType)
        {
        }

        /// <summary>
        ///     Target without overload disambiguation; optional ignore-if-missing flag.
        ///     目标 without overload disambiguation; optional ignore-if-missing flag。
        /// </summary>
        /// <param name="targetType">
        ///     Declaring type.
        ///     中文说明：Declaring type.
        /// </param>
        /// <param name="methodName">
        ///     Method name.
        ///     中文说明：Method name.
        /// </param>
        /// <param name="ignoreIfMissing">
        ///     When true, missing method is non-fatal for optional patches.
        ///     为 true 时，missing method is non-fatal for optional patches。
        /// </param>
        public ModPatchTarget(Type targetType, string methodName, bool ignoreIfMissing)
            : this(targetType, methodName, null, ignoreIfMissing)
        {
        }

        /// <summary>
        ///     Target by name and Harmony <see cref="MethodType" /> only (no overload disambiguation).
        ///     目标 by name and Harmony <c>MethodType</c> only (no overload disambiguation)。
        /// </summary>
        /// <param name="targetType">
        ///     Declaring type.
        ///     中文说明：Declaring type.
        /// </param>
        /// <param name="methodName">
        ///     Method name.
        ///     中文说明：Method name.
        /// </param>
        /// <param name="harmonyMethodType">
        ///     Harmony method type (e.g. <see cref="MethodType.Async" />).
        ///     中文说明：Harmony method type (e.g. <c>MethodType.Async</c>).
        /// </param>
        public ModPatchTarget(Type targetType, string methodName, MethodType harmonyMethodType)
            : this(targetType, methodName, null, false, harmonyMethodType)
        {
        }

        /// <summary>
        ///     Simple target: any overload with that name, fail if missing.
        ///     Simple target: any over加载 带有 that name, fail 如果 missing.
        /// </summary>
        /// <param name="targetType">
        ///     Declaring type.
        ///     中文说明：Declaring type.
        /// </param>
        /// <param name="methodName">
        ///     Method name.
        ///     中文说明：Method name.
        /// </param>
        public ModPatchTarget(Type targetType, string methodName)
            // ReSharper disable IntroduceOptionalParameters.Global
            : this(targetType, methodName, null, false)
        // ReSharper restore IntroduceOptionalParameters.Global
        {
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var typeSuffix = HarmonyMethodType != MethodType.Normal ? $" [{HarmonyMethodType}]" : "";
            if (ParameterTypes == null) return $"{TargetType.Name}.{MethodName}{typeSuffix}";

            var paramNames = ParameterTypes.Length == 0
                ? "no parameters"
                : string.Join(", ", ParameterTypes.Select(p => p.Name));
            return $"{TargetType.Name}.{MethodName}({paramNames}){typeSuffix}";
        }
    }
}
