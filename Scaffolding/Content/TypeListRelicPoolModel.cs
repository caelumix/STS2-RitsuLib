using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content.Patches;

namespace STS2RitsuLib.Scaffolding.Content
{
    /// <summary>
    ///     Relic pool base that builds relics from declared CLR types and can override energy icon paths on pools.
    ///     遗物 pool base that builds Relics 从 declared CLR types 和 can override energy 图标 路径 on pools.
    /// </summary>
    public abstract class TypeListRelicPoolModel : RelicPoolModel, IModBigEnergyIconPool, IModTextEnergyIconPool
    {
        /// <summary>
        ///     Legacy hook: enumerating relic types on the pool class. Prefer registering each relic through
        ///     Legacy hook: enumerating 遗物 types on the pool class. Prefer registering each 遗物 through
        ///     <c>ModContentRegistry.RegisterRelic&lt;TPool, TRelic&gt;()</c>,
        ///     <c>CreateContentPack.Relic&lt;TPool, TRelic&gt;()</c>,
        ///     or a manifest <c>RelicRegistrationEntry</c> so <c>ModHelper.AddModelToPool</c> injects them without
        ///     or a manifest <c>遗物RegistrationEntry</c> so <c>ModHelper.Add模型ToPool</c> injects them 带有out
        ///     duplicating the same <see cref="RelicModel" /> instances when this property also lists those types.
        ///     duplicating the same <c>RelicModel</c> instances 当 this property also lists those types.
        ///     Defaults to an empty sequence.
        ///     中文说明：Defaults to an empty sequence.
        /// </summary>
        [Obsolete(
            "Prefer ModContentRegistry / CreateContentPack .Relic<TPool, TRelic>() or manifest RelicRegistrationEntry. "
            + "Listing types here duplicates ModHelper injection. Override only for legacy mods; suppress CS0618 if required.")]
        protected virtual IEnumerable<Type> RelicTypes => [];

        /// <inheritdoc cref="IModBigEnergyIconPool.BigEnergyIconPath" />
        public virtual string? BigEnergyIconPath => null;

        /// <inheritdoc cref="IModTextEnergyIconPool.TextEnergyIconPath" />
        public virtual string? TextEnergyIconPath => null;

        /// <inheritdoc />
        protected sealed override IEnumerable<RelicModel> GenerateAllRelics()
        {
#pragma warning disable CS0618 // Intentional: base invokes legacy RelicTypes hook; suppress warning at call site only
            var types = RelicTypes;
#pragma warning restore CS0618

            return types
                .Select(type => ModelDb.GetById<RelicModel>(ModelDb.GetId(type)))
                .ToArray();
        }
    }
}
