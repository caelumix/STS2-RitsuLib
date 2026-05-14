using System.Reflection;
using HarmonyLib;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Interop.Patches;

namespace STS2RitsuLib.Interop
{
    /// <summary>
    ///     Extensible pipeline invoked from <see cref="ModTypeDiscoveryPatch" /> (early localization init),
    ///     mirroring BaseLib's post-mod-init scan without hard-wiring a single feature.
    ///     从 <c>ModTypeDiscoveryPatch</c>（早期 localization init）调用的可扩展管线，
    ///     对应 BaseLib 的 post-mod-init 扫描，但不硬编码到单一功能。
    /// </summary>
    public static class ModTypeDiscoveryHub
    {
        private static readonly Lock Gate = new();
        private static readonly List<IModTypeDiscoveryContributor> Contributors = [];

        private static readonly Dictionary<string, Assembly> RegisteredAssembliesByModId =
            new(StringComparer.Ordinal);

        private static bool _builtInsRegistered;

        /// <summary>
        ///     Registers a contributor. Call from your mod initializer before framework patch application
        ///     if you rely on custom discovery; otherwise built-ins are registered from <see cref="RitsuLibFramework" />.
        ///     注册 contributor。如果依赖自定义 discovery，请在你的 mod initializer 中、framework patch 应用前调用；
        ///     否则内置 contributor 会由 <c>RitsuLibFramework</c> 注册。
        /// </summary>
        public static void RegisterContributor(IModTypeDiscoveryContributor contributor)
        {
            ArgumentNullException.ThrowIfNull(contributor);
            lock (Gate)
            {
                Contributors.Add(contributor);
            }
        }

        /// <summary>
        ///     Registers a mod assembly for the one-shot discovery pipeline. Mods should call this from their initializer
        ///     before <see cref="ModTypeDiscoveryPatch" /> runs.
        ///     为一次性 discovery 管线注册 mod assembly。mod 应在 initializer 中、
        ///     <c>ModTypeDiscoveryPatch</c> 运行前调用。
        /// </summary>
        public static void RegisterModAssembly(string modId, Assembly assembly)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentNullException.ThrowIfNull(assembly);

            lock (Gate)
            {
                RegisteredAssembliesByModId[modId] = assembly;
            }
        }

        internal static void EnsureBuiltInContributorsRegistered()
        {
            lock (Gate)
            {
                if (_builtInsRegistered)
                    return;
                Contributors.Add(new ModInteropTypeDiscoveryContributor());
                Contributors.Add(new AttributeAutoRegistrationTypeDiscoveryContributor());
                _builtInsRegistered = true;
            }
        }

        internal static void RunOnce(Harmony harmony)
        {
            Dictionary<string, Assembly> map;
            IModTypeDiscoveryContributor[] snapshot;
            lock (Gate)
            {
                map = new(RegisteredAssembliesByModId, StringComparer.Ordinal);
                snapshot = Contributors.ToArray();
            }

            var orderedAssemblies = map
                .OrderBy(static kv => kv.Key, StringComparer.Ordinal)
                .Select(static kv => kv.Value)
                .Distinct()
                .ToArray();

            foreach (var assembly in orderedAssemblies)
            {
                var modTypes = AssemblyTypeScanHelper.GetLoadableTypes(assembly, RitsuLibFramework.Logger)
                    .OrderBy(static t => t.FullName ?? t.Name, StringComparer.Ordinal)
                    .ToArray();

                foreach (var modType in modTypes)
                foreach (var contributor in snapshot)
                    contributor.Contribute(harmony, map, modType);
            }
        }
    }
}
