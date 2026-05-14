using MegaCrit.Sts2.Core.Timeline;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.Content.Patches;

namespace STS2RitsuLib.Timeline.Scaffolding
{
    /// <summary>
    ///     Base <see cref="EpochModel" /> for mods with optional <see cref="IModEpochAssetOverrides" /> timeline portrait
    ///     Base <c>Epoch模型</c> 用于 mods 带有 可选 <c>IModEpochAssetOverrides</c> timeline 肖像
    ///     paths. Unlock-epoch templates in this namespace inherit from this type.
    ///     路径. Unlock-epoch templates in this namespace inherit 从 this type.
    /// </summary>
    public abstract class ModEpochTemplate : EpochModel, IModEpochAssetOverrides
    {
        /// <inheritdoc />
        public sealed override EpochEra Era => ModTimelineLayoutRegistry.ResolveEra(GetType());

        /// <inheritdoc />
        public sealed override int EraPosition => ModTimelineLayoutRegistry.ResolveEraPosition(GetType());

        /// <inheritdoc />
        public virtual EpochAssetProfile AssetProfile => EpochAssetProfile.Empty;

        /// <inheritdoc />
        public virtual string? CustomPackedPortraitPath => AssetProfile.PackedPortraitPath;

        /// <inheritdoc />
        public virtual string? CustomBigPortraitPath => AssetProfile.BigPortraitPath;
    }
}
