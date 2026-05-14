using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content.Patches;

namespace STS2RitsuLib.Scaffolding.Content
{
    /// <summary>
    ///     Potion pool base that builds potions from declared CLR types and can override energy icon paths on pools.
    ///     Potion pool base that builds potions 从 declared CLR types 和 can override energy 图标 路径 on pools.
    /// </summary>
    public abstract class TypeListPotionPoolModel : PotionPoolModel, IModBigEnergyIconPool, IModTextEnergyIconPool
    {
        /// <summary>
        ///     Legacy hook: enumerating potion types on the pool class. Prefer registering each potion through
        ///     中文说明：Legacy hook: enumerating potion types on the pool class. Prefer registering each potion through
        ///     <c>ModContentRegistry.RegisterPotion&lt;TPool, TPotion&gt;()</c>,
        ///     <c>CreateContentPack.Potion&lt;TPool, TPotion&gt;()</c>,
        ///     or a manifest <c>PotionRegistrationEntry</c> so <c>ModHelper.AddModelToPool</c> injects them without
        ///     or a manifest <c>PotionRegistrationEntry</c> so <c>ModHelper.Add模型ToPool</c> injects them 带有out
        ///     duplicating the same <see cref="PotionModel" /> instances when this property also lists those types.
        ///     duplicating the same <c>PotionModel</c> instances 当 this property also lists those types.
        ///     Defaults to an empty sequence.
        ///     中文说明：Defaults to an empty sequence.
        /// </summary>
        [Obsolete(
            "Prefer ModContentRegistry / CreateContentPack .Potion<TPool, TPotion>() or manifest PotionRegistrationEntry. "
            + "Listing types here duplicates ModHelper injection. Override only for legacy mods; suppress CS0618 if required.")]
        protected virtual IEnumerable<Type> PotionTypes => [];

        /// <inheritdoc cref="IModBigEnergyIconPool.BigEnergyIconPath" />
        public virtual string? BigEnergyIconPath => null;

        /// <inheritdoc cref="IModTextEnergyIconPool.TextEnergyIconPath" />
        public virtual string? TextEnergyIconPath => null;

        /// <inheritdoc />
        protected sealed override IEnumerable<PotionModel> GenerateAllPotions()
        {
#pragma warning disable CS0618 // Intentional: base invokes legacy PotionTypes hook; suppress warning at call site only
            var types = PotionTypes;
#pragma warning restore CS0618

            return types
                .Select(type => ModelDb.GetById<PotionModel>(ModelDb.GetId(type)))
                .ToArray();
        }
    }
}
