using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace STS2RitsuLib.CardPiles
{
    /// <summary>
    ///     Weak per-state storage for <see cref="ModCardPile" /> instances. Piles are created lazily the first
    ///     time vanilla code asks for them via <see cref="Resolve" /> so state objects pay nothing for mods they
    ///     do not interact with.
    ///     <c>ModCardPile</c> 实例的弱 per-state 存储。pile 会在原版代码第一次通过
    ///     <c>Resolve</c> 请求时懒创建，因此 state object 不会为未交互的 mod 付出成本。
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="ModCardPileScope.CombatOnly" /> piles are keyed by <see cref="PlayerCombatState" />
    ///         and implicitly disposed with the combat (the <c>AllPiles</c> postfix adds them into the vanilla
    ///         cleanup sweep).
    ///         <c>ModCardPileScope.CombatOnly</c> pile 按 <c>PlayerCombatState</c> 索引，
    ///         并随战斗隐式释放（<c>AllPiles</c> postfix 会把它们加入原版 cleanup sweep）。
    ///     </para>
    ///     <para>
    ///         <see cref="ModCardPileScope.RunPersistent" /> piles are keyed by <see cref="Player" /> and
    ///         persist across combats for the lifetime of the player instance. Serialization is a follow-up;
    ///         for now the piles refill from empty at run load.
    ///         <c>ModCardPileScope.RunPersistent</c> pile 按 <c>Player</c> 索引，并在 player
    ///         实例生命周期内跨战斗保留。序列化是后续工作；目前 run load 时这些 pile 会从空状态重新开始。
    ///     </para>
    /// </remarks>
    internal static class ModCardPileStorage
    {
        private static readonly ConditionalWeakTable<PlayerCombatState, Dictionary<PileType, ModCardPile>>
            CombatPiles = new();

        private static readonly ConditionalWeakTable<Player, Dictionary<PileType, ModCardPile>>
            RunPiles = new();

        /// <summary>
        ///     Looks up or lazily creates the <see cref="ModCardPile" /> bound to <paramref name="player" /> for
        ///     <paramref name="type" />. Returns null when the minted type has no registered definition or when
        ///     the requested state (combat / player) is not yet available.
        ///     查找或懒创建绑定到 <c>player</c>、对应 <c>type</c> 的
        ///     <c>ModCardPile</c>。当 minted type 没有已注册 definition，或请求的状态（combat / player）
        ///     尚不可用时返回 null。
        /// </summary>
        public static ModCardPile? Resolve(PileType type, Player? player)
        {
            if (player == null)
                return null;
            if (!ModCardPileRegistry.TryGetByPileType(type, out var definition))
                return null;

            return definition.Scope switch
            {
                ModCardPileScope.CombatOnly => ResolveCombatPile(player.PlayerCombatState, definition),
                ModCardPileScope.RunPersistent => ResolveRunPile(player, definition),
                _ => null,
            };
        }

        /// <summary>
        ///     Returns the mod piles that currently belong to <paramref name="state" /> without creating new
        ///     ones. The returned collection is a snapshot and safe to enumerate while vanilla mutates piles.
        ///     返回当前属于 <c>state</c> 的 mod pile，但不创建新的 pile。返回集合是快照，
        ///     在原版变更 pile 时也可安全枚举。
        /// </summary>
        public static IReadOnlyCollection<ModCardPile> GetCombatPiles(PlayerCombatState state)
        {
            ArgumentNullException.ThrowIfNull(state);

            if (!CombatPiles.TryGetValue(state, out var piles) || piles.Count == 0)
                return [];

            lock (piles)
            {
                return [.. piles.Values];
            }
        }

        /// <summary>
        ///     Snapshot of persistent piles owned by <paramref name="player" />.
        ///     <c>player</c> 拥有的 persistent pile 快照。
        /// </summary>
        public static IReadOnlyCollection<ModCardPile> GetRunPiles(Player player)
        {
            ArgumentNullException.ThrowIfNull(player);

            if (!RunPiles.TryGetValue(player, out var piles) || piles.Count == 0)
                return [];

            lock (piles)
            {
                return [.. piles.Values];
            }
        }

        private static ModCardPile? ResolveCombatPile(PlayerCombatState? state, ModCardPileDefinition definition)
        {
            if (state == null)
                return null;

            var dict = CombatPiles.GetValue(state, static _ => []);
            lock (dict)
            {
                if (dict.TryGetValue(definition.PileType, out var existing))
                    return existing;

                var created = new ModCardPile(definition);
                dict[definition.PileType] = created;
                return created;
            }
        }

        private static ModCardPile ResolveRunPile(Player player, ModCardPileDefinition definition)
        {
            var dict = RunPiles.GetValue(player, static _ => []);
            lock (dict)
            {
                if (dict.TryGetValue(definition.PileType, out var existing))
                    return existing;

                var created = new ModCardPile(definition);
                dict[definition.PileType] = created;
                return created;
            }
        }
    }
}
