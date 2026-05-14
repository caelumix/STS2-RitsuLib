using MegaCrit.Sts2.Core.Modding;

namespace STS2RitsuLib.Compat
{
    /// <summary>
    ///     Central entry for enumerating the host <see cref="ModManager" />'s mod lists.
    ///     Central entry 用于 enumerating the host <c>ModManager</c>'s mod lists.
    /// </summary>
    internal static class Sts2ModManagerCompat
    {
        internal static IEnumerable<Mod> EnumerateLoadedModsWithAssembly()
        {
            return ModManager.GetLoadedMods();
        }

        /// <summary>
        ///     All registered mods (including disabled / not loaded), for manifest name/description lookup.
        ///     All 已注册 mods (including 禁用 / not loaded), 用于 manifest name/description lookup.
        /// </summary>
        internal static IEnumerable<Mod> EnumerateModsForManifestLookup()
        {
            return ModManager.Mods;
        }
    }
}
