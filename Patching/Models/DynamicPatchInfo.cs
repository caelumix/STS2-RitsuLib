using System.Reflection;
using HarmonyLib;
using STS2RitsuLib.Patching.Core;

namespace STS2RitsuLib.Patching.Models
{
    /// <summary>
    ///     Describes a runtime-discovered patch target and the Harmony methods to apply to it.
    ///     Describes a runtime-discovered patch target 和 the Harmony methods to apply to it.
    /// </summary>
    /// <param name="id">
    ///     Stable patch identifier for logging and unpatch.
    ///     稳定的 patch identifier for logging and unpatch。
    /// </param>
    /// <param name="originalMethod">
    ///     Vanilla method to patch.
    ///     原版 method to patch.
    /// </param>
    /// <param name="prefix">
    ///     Optional Harmony prefix.
    ///     可选 Harmony 前置补丁.
    /// </param>
    /// <param name="postfix">
    ///     Optional Harmony postfix.
    ///     可选 Harmony 后置补丁.
    /// </param>
    /// <param name="transpiler">
    ///     Optional transpiler.
    ///     可选 transpiler.
    /// </param>
    /// <param name="finalizer">
    ///     Optional finalizer.
    ///     可选 finalizer.
    /// </param>
    /// <param name="isCritical">
    ///     When false, failures may be treated as optional by the patcher.
    ///     为 false 时，failures may be treated as optional by the patcher。
    /// </param>
    /// <param name="description">
    ///     Human-readable description; defaults to type.method.
    ///     人类可读的 description; defaults to type.method。
    /// </param>
    public sealed class DynamicPatchInfo(
        string id,
        MethodBase originalMethod,
        HarmonyMethod? prefix = null,
        HarmonyMethod? postfix = null,
        HarmonyMethod? transpiler = null,
        HarmonyMethod? finalizer = null,
        bool isCritical = true,
        string? description = null)
    {
        /// <summary>
        ///     Unique patch id within the owning patcher.
        ///     Unique patch id 带有in the owning patcher.
        /// </summary>
        public string Id { get; } = id;

        /// <summary>
        ///     Target method being patched.
        ///     目标 method being patched。
        /// </summary>
        public MethodBase OriginalMethod { get; } = originalMethod;

        /// <summary>
        ///     Harmony prefix delegate, if any.
        ///     Harmony 前置补丁 delegate, 如果 any.
        /// </summary>
        public HarmonyMethod? Prefix { get; } = prefix;

        /// <summary>
        ///     Harmony postfix delegate, if any.
        ///     Harmony 后置补丁 delegate, 如果 any.
        /// </summary>
        public HarmonyMethod? Postfix { get; } = postfix;

        /// <summary>
        ///     Harmony transpiler, if any.
        ///     Harmony transpiler, 如果 any.
        /// </summary>
        public HarmonyMethod? Transpiler { get; } = transpiler;

        /// <summary>
        ///     Harmony finalizer, if any.
        ///     Harmony finalizer, 如果 any.
        /// </summary>
        public HarmonyMethod? Finalizer { get; } = finalizer;

        /// <summary>
        ///     Whether this patch is considered critical for mod correctness.
        ///     表示是否 this patch is considered critical for mod correctness。
        /// </summary>
        public bool IsCritical { get; } = isCritical;

        /// <summary>
        ///     Log-friendly description of the patch purpose.
        ///     中文说明：Log-friendly description of the patch purpose.
        /// </summary>
        public string Description { get; } = string.IsNullOrWhiteSpace(description)
            ? $"Patch {originalMethod.DeclaringType?.Name}.{originalMethod.Name}"
            : description;

        /// <summary>
        ///     True when at least one Harmony hook is non-null.
        ///     当 at least one Harmony hook is non-null 时为 true。
        /// </summary>
        public bool HasPatchMethods => Prefix != null || Postfix != null || Transpiler != null || Finalizer != null;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Id}: {OriginalMethod.DeclaringType?.Name}.{OriginalMethod.Name}";
        }

        /// <summary>
        ///     Builds a dynamic patch by resolving <paramref name="target" /> the same way as <see cref="ModPatcher" />
        ///     Builds a dynamic patch 通过 resolving <c>target</c> the same way as <c>ModPatcher</c>
        ///     resolves <see cref="ModPatchInfo" />.
        ///     解析 <c>ModPatchInfo</c>。
        /// </summary>
        public static DynamicPatchInfo FromModPatchTarget(
            string id,
            ModPatchTarget target,
            HarmonyMethod? prefix = null,
            HarmonyMethod? postfix = null,
            HarmonyMethod? transpiler = null,
            HarmonyMethod? finalizer = null,
            bool isCritical = true,
            string? description = null)
        {
            ArgumentNullException.ThrowIfNull(target);

            var originalMethod = PatchTargetMethodResolver.ResolveRequired(target);
            return new(
                id,
                originalMethod,
                prefix,
                postfix,
                transpiler,
                finalizer,
                isCritical,
                description);
        }
    }
}
