using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content.Patches;

namespace STS2RitsuLib.Scaffolding.Content
{
    /// <summary>
    ///     Public model-oriented helpers for runtime visual reload requests.
    ///     Public 模型-oriented helpers 用于 runtime visual re加载 requests.
    /// </summary>
    public static class RuntimeAssetReloadExtensions
    {
        /// <summary>
        ///     Requests card-node reloads for this card instance (reference or id match).
        ///     Requests 卡牌-node re加载 用于 this 卡牌 instance (reference 或 id match).
        /// </summary>
        public static void RequestVisualReload(this CardModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            RuntimeAssetRefreshCoordinator.RequestCardsWhere(candidate =>
                ReferenceEquals(candidate, model) || candidate.Id == model.Id);
        }

        /// <summary>
        ///     Requests relic-node reloads for this relic instance (reference or id match).
        ///     Requests 遗物-node re加载 用于 this 遗物 instance (reference 或 id match).
        /// </summary>
        public static void RequestVisualReload(this RelicModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            RuntimeAssetRefreshCoordinator.RequestRelicsWhere(candidate =>
                ReferenceEquals(candidate, model) || candidate.Id == model.Id);
        }

        /// <summary>
        ///     Requests potion-node reloads for this potion instance (reference or id match).
        ///     Requests potion-node re加载 用于 this potion instance (reference 或 id match).
        /// </summary>
        public static void RequestVisualReload(this PotionModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            RuntimeAssetRefreshCoordinator.RequestPotionsWhere(candidate =>
                ReferenceEquals(candidate, model) || candidate.Id == model.Id);
        }

        /// <summary>
        ///     Requests power-node reloads for this power instance (reference or id match).
        ///     Requests 能力-node re加载 用于 this 能力 instance (reference 或 id match).
        /// </summary>
        public static void RequestVisualReload(this PowerModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            RuntimeAssetRefreshCoordinator.RequestPowersWhere(candidate =>
                ReferenceEquals(candidate, model) || candidate.Id == model.Id);
        }

        /// <summary>
        ///     Requests orb-node visual updates for this orb instance (reference or id match).
        ///     Requests 充能球-node visual 更新 用于 this 充能球 instance (reference 或 id match).
        /// </summary>
        public static void RequestVisualReload(this OrbModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            RuntimeAssetRefreshCoordinator.RequestOrbsWhere(candidate =>
                ReferenceEquals(candidate, model) || candidate.Id == model.Id);
        }
    }
}
