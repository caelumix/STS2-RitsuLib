using STS2RitsuLib.Patching.Core;

namespace STS2RitsuLib.Patching.Models
{
    /// <summary>
    ///     Interface for a class that contains patches for the mod.
    ///     Interface 用于 a class that 包含 patches 用于 the mod.
    ///     This is used to group patches together and make it easier to add them to the patcher.
    ///     This is used to group patches together 和 make it easier to add them to the patcher.
    /// </summary>
    public interface IModPatches
    {
        /// <summary>
        ///     Adds the patches to the patcher.
        ///     中文说明：Adds the patches to the patcher.
        /// </summary>
        static abstract void AddTo(ModPatcher patcher);
    }
}
