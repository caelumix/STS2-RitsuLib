using Godot;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content.Patches;

namespace STS2RitsuLib.Scaffolding.Content
{
    /// <summary>
    ///     Card pool base that can enumerate legacy CLR card types, expose optional pool frame material overrides,
    ///     卡牌 pool base that can enumerate legacy CLR 卡牌 types, expose 可选 pool frame 材质 overrides,
    ///     and map energy icon paths for UI patches.
    ///     and map energy 图标 路径 用于 UI patches.
    /// </summary>
    public abstract class TypeListCardPoolModel : CardPoolModel, IModBigEnergyIconPool, IModTextEnergyIconPool,
        IModCardPoolFrameMaterial
    {
        /// <summary>
        ///     Legacy hook: enumerating card types on the pool class. Prefer registering each card through
        ///     Legacy hook: enumerating 卡牌 types on the pool class. Prefer registering each 卡牌 through
        ///     <c>ModContentRegistry.RegisterCard&lt;TPool, TCard&gt;()</c>, <c>CreateContentPack.Card&lt;TPool, TCard&gt;()</c>,
        ///     or a manifest <c>CardRegistrationEntry</c> so <c>ModHelper.AddModelToPool</c> injects them without
        ///     or a manifest <c>卡牌RegistrationEntry</c> so <c>ModHelper.Add模型ToPool</c> injects them 带有out
        ///     duplicating the same <see cref="CardModel" /> instances when this property also lists those types.
        ///     duplicating the same <c>CardModel</c> instances 当 this property also lists those types.
        ///     Defaults to an empty sequence.
        ///     中文说明：Defaults to an empty sequence.
        /// </summary>
        [Obsolete(
            "Prefer ModContentRegistry / CreateContentPack .Card<TPool, TCard>() or manifest CardRegistrationEntry. "
            + "Listing types here duplicates ModHelper injection. Override only for legacy mods; suppress CS0618 if required.")]
        protected virtual IEnumerable<Type> CardTypes => [];

        /// <summary>
        ///     Path-based fallback for the card frame material.
        ///     路径-based fallback 用于 the 卡牌 frame 材质.
        ///     Only used when <see cref="PoolFrameMaterial" /> is null.
        ///     Only used 当 <c>PoolFrame材质</c> is null.
        ///     Override this if you want to reference a pre-existing <c>.tres</c> material file.
        ///     Override this 如果 you want to reference a pre-existing <c>.tres</c> 材质 file.
        /// </summary>
        public override string CardFrameMaterialPath => "card_frame_colorless";

        /// <inheritdoc cref="IModBigEnergyIconPool.BigEnergyIconPath" />
        public virtual string? BigEnergyIconPath => null;

        /// <summary>
        ///     Directly supply a <see cref="Material" /> for all card frames in this pool.
        ///     Directly supply a <c>材质</c> 用于 all 卡牌 frames in this pool.
        ///     When non-null, <see cref="CardFrameMaterialPath" /> is ignored.
        ///     当 non-null, <c>CardFrame材质路径</c> is ignored.
        /// </summary>
        public virtual Material? PoolFrameMaterial => null;

        /// <inheritdoc cref="IModTextEnergyIconPool.TextEnergyIconPath" />
        public virtual string? TextEnergyIconPath => null;

        /// <inheritdoc />
        protected sealed override CardModel[] GenerateAllCards()
        {
#pragma warning disable CS0618 // Intentional: base invokes legacy CardTypes hook; suppress warning at call site only
            var types = CardTypes;
#pragma warning restore CS0618

            return types
                .Select(type => ModelDb.GetById<CardModel>(ModelDb.GetId(type)))
                .ToArray();
        }
    }
}
