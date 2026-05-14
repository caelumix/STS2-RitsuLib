using System.Reflection;
using HarmonyLib;
using STS2RitsuLib.Interop.Internal;

namespace STS2RitsuLib.Interop
{
    /// <summary>
    ///     Built-in contributor: processes <see cref="ModInteropAttribute" /> stubs.
    ///     内置 contributor：处理 <c>ModInteropAttribute</c> stub。
    /// </summary>
    public sealed class ModInteropTypeDiscoveryContributor : IModTypeDiscoveryContributor
    {
        /// <inheritdoc />
        public void Contribute(
            Harmony harmony,
            IReadOnlyDictionary<string, Assembly> modAssembliesByManifestId,
            Type modType)
        {
            ModInteropEmitter.TryProcessType(harmony, modAssembliesByManifestId, modType);
        }
    }
}
