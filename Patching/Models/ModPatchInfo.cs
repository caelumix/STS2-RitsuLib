using HarmonyLib;

namespace STS2RitsuLib.Patching.Models
{
    /// <summary>
    ///     Describes a static patch type targeting one vanilla method by reflection.
    ///     Describes a static patch type targeting one 原版 method 通过 reflection.
    /// </summary>
    /// <param name="id">
    ///     Stable patch identifier.
    ///     稳定的 patch identifier。
    /// </param>
    /// <param name="targetType">
    ///     Declaring type of the method to patch.
    ///     中文说明：Declaring type of the method to patch.
    /// </param>
    /// <param name="methodName">
    ///     Name of the method to patch.
    ///     中文说明：Name of the method to patch.
    /// </param>
    /// <param name="patchType">
    ///     Type containing optional Harmony <c>Prefix</c>/<c>Postfix</c>/<c>Transpiler</c>/
    ///     Type containing 可选 Harmony <c>Prefix</c>/<c>Postfix</c>/<c>Transpiler</c>/
    ///     <c>Finalizer</c>.
    /// </param>
    /// <param name="isCritical">
    ///     Whether failure should block the patcher.
    ///     表示是否 failure should block the patcher。
    /// </param>
    /// <param name="description">
    ///     Optional description; defaults to <c>Patch Type.Method</c>.
    ///     可选 description; defaults to <c>Patch Type.Method</c>.
    /// </param>
    /// <param name="parameterTypes">
    ///     Method parameter types for overload resolution; null selects by name only.
    ///     Method parameter types 用于 over加载 resolution; null selects 通过 name only.
    /// </param>
    /// <param name="ignoreIfTargetMissing">
    ///     When true, missing targets produce an ignored result instead of failure.
    ///     为 true 时，missing targets produce an ignored result instead of failure。
    /// </param>
    /// <param name="harmonyMethodType">
    ///     Harmony <see cref="MethodType" /> for target resolution (e.g. <see cref="MethodType.Async" />), matching
    ///     Harmony <c>MethodType</c> 用于 target resolution (e.g. <c>MethodType.Async</c>), matching
    ///     <c>[HarmonyPatch(..., MethodType.X)]</c>.
    /// </param>
    public class ModPatchInfo(
        string id,
        Type targetType,
        string methodName,
        Type patchType,
        bool isCritical = true,
        string description = "",
        Type[]? parameterTypes = null,
        bool ignoreIfTargetMissing = false,
        MethodType harmonyMethodType = MethodType.Normal)
    {
        /// <summary>
        ///     Legacy constructor without <see cref="HarmonyMethodType" />; forwards to
        ///     Legacy constructor 带有out <c>HarmonyMethodType</c>; 用于wards to
        ///     <see cref="MethodType.Normal" />.
        /// </summary>
        /// <param name="id">
        ///     Stable patch identifier.
        ///     稳定的 patch identifier。
        /// </param>
        /// <param name="targetType">
        ///     Declaring type of the method to patch.
        ///     中文说明：Declaring type of the method to patch.
        /// </param>
        /// <param name="methodName">
        ///     Name of the method to patch.
        ///     中文说明：Name of the method to patch.
        /// </param>
        /// <param name="patchType">
        ///     Type containing Harmony patch methods.
        ///     中文说明：Type containing Harmony patch methods.
        /// </param>
        /// <param name="isCritical">
        ///     Whether failure should block the patcher.
        ///     表示是否 failure should block the patcher。
        /// </param>
        /// <param name="description">
        ///     Human-readable description.
        ///     人类可读的 description。
        /// </param>
        /// <param name="parameterTypes">
        ///     Method parameter types for overload resolution.
        ///     Method parameter types 用于 over加载 resolution.
        /// </param>
        /// <param name="ignoreIfTargetMissing">
        ///     When true, missing targets are ignored.
        ///     为 true 时，missing targets are ignored。
        /// </param>
        public ModPatchInfo(
            string id,
            Type targetType,
            string methodName,
            Type patchType,
            bool isCritical,
            string description,
            Type[]? parameterTypes,
            bool ignoreIfTargetMissing)
            : this(
                id,
                targetType,
                methodName,
                patchType,
                isCritical,
                description,
                parameterTypes,
                ignoreIfTargetMissing,
                MethodType.Normal)
        {
        }

        /// <summary>
        ///     Unique patch id.
        ///     中文说明：Unique patch id.
        /// </summary>
        public string Id { get; } = id;

        /// <summary>
        ///     Type that declares the original method.
        ///     中文说明：Type that declares the original method.
        /// </summary>
        public Type TargetType { get; } = targetType;

        /// <summary>
        ///     Original method name.
        ///     中文说明：Original method name.
        /// </summary>
        public string MethodName { get; } = methodName;

        /// <summary>
        ///     Patch class applied via Harmony.
        ///     中文说明：Patch class applied via Harmony.
        /// </summary>
        public Type PatchType { get; } = patchType;

        /// <summary>
        ///     Whether this patch is critical.
        ///     表示是否 this patch is critical。
        /// </summary>
        public bool IsCritical { get; } = isCritical;

        /// <summary>
        ///     Parameter signature for overload resolution, when needed.
        ///     Parameter signature 用于 over加载 resolution, 当 needed.
        /// </summary>
        public Type[]? ParameterTypes { get; } = parameterTypes;

        /// <summary>
        ///     When true, a missing vanilla method yields an ignored success result.
        ///     为 true 时，a missing vanilla method yields an ignored success result。
        /// </summary>
        public bool IgnoreIfTargetMissing { get; } = ignoreIfTargetMissing;

        /// <summary>
        ///     Harmony method-type discriminator used when resolving the original <see cref="System.Reflection.MethodBase" />.
        ///     Harmony method-type discriminator used 当 resolving the original <c>System.Reflection.MethodBase</c>.
        /// </summary>
        public MethodType HarmonyMethodType { get; } = harmonyMethodType;

        /// <summary>
        ///     Human-readable description of the patch.
        ///     人类可读的 description of the patch。
        /// </summary>
        public string Description { get; } =
            string.IsNullOrEmpty(description) ? $"Patch {targetType.Name}.{methodName}" : description;

        /// <inheritdoc />
        public override string ToString()
        {
            var typeSuffix = HarmonyMethodType != MethodType.Normal ? $" [{HarmonyMethodType}]" : "";
            if (ParameterTypes == null)
                return $"{Id}: {TargetType.Name}.{MethodName}{typeSuffix} <- {PatchType.Name}";

            var paramNames = ParameterTypes.Length == 0
                ? "no parameters"
                : string.Join(", ", ParameterTypes.Select(p => p.Name));
            return $"{Id}: {TargetType.Name}.{MethodName}({paramNames}){typeSuffix} <- {PatchType.Name}";
        }
    }
}
