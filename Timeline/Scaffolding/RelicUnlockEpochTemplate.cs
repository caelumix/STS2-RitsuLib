using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Timeline;

namespace STS2RitsuLib.Timeline.Scaffolding
{
    /// <summary>
    ///     <see cref="EpochModel" /> base that unlocks relics from declared CLR types and optional timeline expansions.
    ///     基于声明的 CLR 类型和可选时间线扩展来解锁遗物的 <c>EpochModel</c> 基类。
    /// </summary>
    public abstract class RelicUnlockEpochTemplate : ModEpochTemplate
    {
        /// <summary>
        ///     Resolved <see cref="RelicModel" /> instances for <see cref="RelicTypes" />.
        ///     resolved <c>RelicModel</c> instances 用于 <c>RelicTypes</c>.
        /// </summary>
        public IReadOnlyList<RelicModel> Relics => RelicTypes
            .Select(type => ModelDb.GetById<RelicModel>(ModelDb.GetId(type)))
            .ToArray();

        /// <inheritdoc />
        public override string UnlockText => CreateRelicUnlockText(Relics.ToList());

        /// <summary>
        ///     CLR types of relics to unlock; each must be registered in <see cref="ModelDb" />.
        ///     CLR types of Relics to unlock; each must be 已注册 in <c>ModelDb</c>.
        /// </summary>
        protected abstract IEnumerable<Type> RelicTypes { get; }

        /// <summary>
        ///     Additional epoch types to append when this epoch unlocks; default none.
        ///     Additional epoch types to append 当 this epoch unlocks; default none.
        /// </summary>
        protected virtual IEnumerable<Type> ExpansionEpochTypes => [];

        /// <summary>
        ///     Same as <see cref="RelicTypes" /> for batch <see cref="Unlocks.ModUnlockRegistry.RequireEpoch(Type,string)" />
        ///     Same as <c>RelicTypes</c> 用于 batch <c>Unlocks.ModUnlockRegistry.RequireEpoch(Type,string)</c>
        ///     registration from a content-pack manifest.
        ///     注册 从 a content-pack manifest.
        /// </summary>
        public IEnumerable<Type> EnumerateUnlockRelicTypes()
        {
            return RelicTypes;
        }

        /// <inheritdoc />
        public override EpochModel[] GetTimelineExpansion()
        {
            return ExpansionEpochTypes.Select(type => Get(GetId(type))).ToArray();
        }

        /// <inheritdoc />
        public override void QueueUnlocks()
        {
            NTimelineScreen.Instance.QueueRelicUnlock(Relics.ToList());

            var expansion = GetTimelineExpansion();
            if (expansion.Length > 0)
                QueueTimelineExpansion(expansion);
        }
    }
}
