using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Timeline;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2RitsuLib.Timeline.Scaffolding
{
    /// <summary>
    ///     <see cref="EpochModel" /> base that unlocks potions from declared CLR types and optional timeline expansions.
    /// </summary>
    /// <remarks>
    ///     Pool visibility still depends on <see cref="Unlocks.ModUnlockRegistry.RequireEpoch(Type,string)" /> for each
    ///     Pool visibility still depends on <c>Unlocks.ModUnlockRegistry.RequireEpoch(Type,string)</c> 用于 each
    ///     potion type. Register those bindings from a pack using <see cref="TimelineColumnPackEntry{TStory}" /> with
    ///     potion type. Register those bindings 从 a pack using <c>TimelineColumnPackEntry{TStory}</c> 带有
    ///     <c>EpochSlotBuilder&lt;TEpoch&gt;</c> callbacks (<c>RequireAllPotionsInPool&lt;TPool&gt;()</c>,
    ///     <c>Potions(IReadOnlyList&lt;Type&gt;)</c>), or enqueue equivalent <c>RequireEpoch</c> steps from
    ///     <see cref="ModContentPackBuilder" />.
    /// </remarks>
    public abstract class PotionUnlockEpochTemplate : ModEpochTemplate
    {
        /// <summary>
        ///     Resolved <see cref="PotionModel" /> instances for <see cref="PotionTypes" />.
        ///     resolved <c>PotionModel</c> instances 用于 <c>PotionTypes</c>.
        /// </summary>
        public IReadOnlyList<PotionModel> Potions => PotionTypes
            .Select(type => ModelDb.GetById<PotionModel>(ModelDb.GetId(type)))
            .ToArray();

        /// <inheritdoc />
        public override string UnlockText => CreatePotionUnlockText(Potions.ToList());

        /// <summary>
        ///     CLR types of potions to unlock; each must be registered in <see cref="ModelDb" />.
        ///     CLR types of potions to unlock; each must be 已注册 in <c>ModelDb</c>.
        /// </summary>
        protected abstract IEnumerable<Type> PotionTypes { get; }

        /// <summary>
        ///     Additional epoch types to append when this epoch unlocks; default none.
        ///     Additional epoch types to append 当 this epoch unlocks; default none.
        /// </summary>
        protected virtual IEnumerable<Type> ExpansionEpochTypes => [];

        /// <inheritdoc />
        public override EpochModel[] GetTimelineExpansion()
        {
            return ExpansionEpochTypes.Select(type => Get(GetId(type))).ToArray();
        }

        /// <inheritdoc />
        public override void QueueUnlocks()
        {
            NTimelineScreen.Instance.QueuePotionUnlock(Potions.ToList());

            var expansion = GetTimelineExpansion();
            if (expansion.Length > 0)
                QueueTimelineExpansion(expansion);
        }
    }
}
