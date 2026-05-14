using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Content;
using STS2RitsuLib.Utils;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace STS2RitsuLib.CardPiles
{
    /// <summary>
    ///     Per-mod registration surface for custom <see cref="CardPile" />s. Mirrors the conventions used by
    ///     <c>ModKeywordRegistry</c>: ids are mod-qualified via <see cref="ModContentRegistry.GetQualifiedCardPileId" />,
    ///     <see cref="PileType" /> values are deterministically minted with
    ///     <see cref="DynamicEnumValueMinter{TEnum}" />, and registrations freeze at
    ///     <c>ModelDb.Init</c>.
    ///     自定义 <c>CardPile</c> 的 per-mod 注册入口。它对应 <c>ModKeywordRegistry</c> 使用的约定：
    ///     id 通过 <c>ModContentRegistry.GetQualifiedCardPileId</c> 加上 mod 限定；
    ///     <c>PileType</c> 值通过 <c>DynamicEnumValueMinter{TEnum}</c> 确定性生成；
    ///     注册会在 <c>ModelDb.Init</c> 时冻结。
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A single global <see cref="DynamicEnumValueMinter{TEnum}" /> reserves the high value band
    ///         (<c>[0x4000_0000, 0x7FFF_FFFF]</c>), strictly above any plausible vanilla growth. Ritsulib and
    ///         baselib use different hash families (XxHash32 vs MD5) so their minted values do not collide
    ///         numerically even when used side-by-side.
    ///         单个全局 <c>DynamicEnumValueMinter{TEnum}</c> 会保留高值区间
    ///         （<c>[0x4000_0000, 0x7FFF_FFFF]</c>），严格高于任何合理的原版增长范围。ritsulib 和 baselib
    ///         使用不同 hash 家族（XxHash32 与 MD5），因此即使并排使用，它们生成的数值也不会冲突。
    ///     </para>
    ///     <para>
    ///         Consumers reach the registry through <see cref="For" /> with their own mod id; the registry
    ///         instance is a thin per-mod façade — definitions live in a single process-wide map keyed by the
    ///         normalized id and minted value, so cross-mod lookups by id remain possible.
    ///         consumer 使用自己的 mod id 通过 <c>For</c> 取得 registry；registry 实例只是很薄的
    ///         per-mod facade。definition 存在单个进程级 map 中，按 normalized id 和 minted value 索引，
    ///         因此仍可按 id 进行跨 mod 查找。
    ///     </para>
    /// </remarks>
    public sealed class ModCardPileRegistry
    {
        private static readonly Lock SyncRoot = new();

        private static readonly Dictionary<string, ModCardPileRegistry> Registries =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, ModCardPileDefinition> Definitions =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<PileType, ModCardPileDefinition> DefinitionsByPileType = [];

        private static readonly DynamicEnumValueMinter<PileType> PileTypeMinter = new();

        private readonly Logger _logger;
        private readonly string _modId;
        private string? _freezeReason;

        private ModCardPileRegistry(string modId)
        {
            _modId = modId;
            _logger = RitsuLibFramework.CreateLogger(modId);
        }

        /// <summary>
        ///     True after the framework freezes pile registration (at <c>ModelDb.Init</c>).
        ///     framework 冻结 pile 注册后为 true（在 <c>ModelDb.Init</c> 时）。
        /// </summary>
        public static bool IsFrozen { get; private set; }

        /// <summary>
        ///     Returns the singleton registry for <paramref name="modId" />, creating it on first use.
        ///     返回 <c>modId</c> 的 singleton registry，首次使用时创建。
        /// </summary>
        /// <param name="modId">
        ///     Owning mod id.
        ///     所属 mod id。
        /// </param>
        public static ModCardPileRegistry For(string modId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);

            lock (SyncRoot)
            {
                if (Registries.TryGetValue(modId, out var existing))
                    return existing;

                var created = new ModCardPileRegistry(modId);
                Registries[modId] = created;
                return created;
            }
        }

        /// <summary>
        ///     Freezes all registries. Called from the core lifecycle patch immediately before
        ///     <c>ModelDb.Init</c> so every subsequent mint / register attempt throws.
        ///     冻结所有 registry。由 core lifecycle patch 在 <c>ModelDb.Init</c> 前立即调用，使之后每次
        ///     mint / register 尝试都会抛出异常。
        /// </summary>
        /// <param name="reason">
        ///     Human-readable context appended to log messages.
        ///     附加到日志消息中的人类可读上下文。
        /// </param>
        internal static void FreezeRegistrations(string reason)
        {
            ModCardPileRegistry[] snapshot;
            lock (SyncRoot)
            {
                if (IsFrozen)
                    return;

                IsFrozen = true;
                foreach (var registry in Registries.Values)
                    registry._freezeReason = reason;

                snapshot = [.. Registries.Values];
            }

            foreach (var registry in snapshot)
                registry._logger.Info($"[CardPiles] Pile registration is now frozen ({reason}).");
        }

        /// <summary>
        ///     Registers a card pile owned by this registry's mod. The id is mod-qualified via
        ///     <see cref="ModContentRegistry.GetQualifiedCardPileId" /> — producing the ritsulib-standard
        ///     <c>MODID_CARDPILE_LOCALSTEM</c> shape (uppercase, three segments), also used as the
        ///     <c>static_hover_tips</c> key stem. Passing the same <paramref name="localStem" /> from the same mod returns the
        ///     existing definition.
        ///     注册由此 registry 的 mod 拥有的 card pile。id 会通过
        ///     <c>ModContentRegistry.GetQualifiedCardPileId</c> 加上 mod 限定，生成 ritsulib 标准
        ///     <c>MODID_CARDPILE_LOCALSTEM</c> 形状（大写、三段），同时也作为 <c>static_hover_tips</c> key stem。
        ///     同一 mod 传入相同 <c>localStem</c> 会返回现有 definition。
        /// </summary>
        /// <param name="localStem">
        ///     Local identifier, unique within this mod.
        ///     此 mod 内唯一的本地标识符。
        /// </param>
        /// <param name="spec">
        ///     Pile metadata (scope, style, localization, icon).
        ///     pile 元数据（scope、style、localization、icon）。
        /// </param>
        public ModCardPileDefinition RegisterOwned(string localStem, ModCardPileSpec spec)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(localStem);
            ArgumentNullException.ThrowIfNull(spec);

            var id = ModContentRegistry.GetQualifiedCardPileId(_modId, localStem);
            return RegisterCore(id, spec);
        }

        /// <summary>
        ///     Registers a card pile using a raw global id. Prefer <see cref="RegisterOwned" /> to keep ids
        ///     mod-scoped.
        ///     使用 raw global id 注册 card pile。优先使用 <c>RegisterOwned</c> 以保持 id 在 mod 范围内。
        /// </summary>
        /// <param name="id">
        ///     Global id; collisions across mods are rejected.
        ///     global id；跨 mod 冲突会被拒绝。
        /// </param>
        /// <param name="spec">
        ///     Pile metadata.
        ///     pile 元数据。
        /// </param>
        public ModCardPileDefinition Register(string id, ModCardPileSpec spec)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentNullException.ThrowIfNull(spec);

            return RegisterCore(id, spec);
        }

        /// <summary>
        ///     Resolves an existing definition by id (the registry does not mint on lookup).
        ///     按 id 解析现有 definition（registry 不会在 lookup 时 mint）。
        /// </summary>
        public static bool TryGet(string id, out ModCardPileDefinition definition)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            lock (SyncRoot)
            {
                return Definitions.TryGetValue(NormalizeId(id), out definition!);
            }
        }

        /// <summary>
        ///     Returns the definition for <paramref name="id" /> or throws <see cref="KeyNotFoundException" />.
        ///     返回 <c>id</c> 的 definition，或抛出 <c>KeyNotFoundException</c>。
        /// </summary>
        public static ModCardPileDefinition Get(string id)
        {
            return TryGet(id, out var definition)
                ? definition
                : throw new KeyNotFoundException($"Card pile '{NormalizeId(id)}' is not registered.");
        }

        /// <summary>
        ///     Resolves which mod registered <paramref name="pileId" />, if any.
        ///     解析哪个 mod 注册了 <c>pileId</c>，如果存在的话。
        /// </summary>
        public static bool TryGetOwnerModId(string pileId, out string modId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(pileId);

            lock (SyncRoot)
            {
                if (Definitions.TryGetValue(NormalizeId(pileId), out var def))
                {
                    modId = def.ModId;
                    return true;
                }
            }

            modId = string.Empty;
            return false;
        }

        /// <summary>
        ///     Resolves the definition for an already minted <see cref="PileType" /> value, returning false for
        ///     vanilla enum members and for values never produced by this registry.
        ///     为已经 minted 的 <c>PileType</c> 值解析 definition；对原版 enum 成员以及此 registry
        ///     从未生成过的值返回 false。
        /// </summary>
        public static bool TryGetByPileType(PileType value, out ModCardPileDefinition definition)
        {
            lock (SyncRoot)
            {
                return DefinitionsByPileType.TryGetValue(value, out definition!);
            }
        }

        /// <summary>
        ///     Whether <paramref name="value" /> was minted by this registry.
        ///     <c>value</c> 是否由此 registry minted。
        /// </summary>
        public static bool IsModPileType(PileType value)
        {
            lock (SyncRoot)
            {
                return DefinitionsByPileType.ContainsKey(value);
            }
        }

        /// <summary>
        ///     Returns the minted <see cref="PileType" /> for <paramref name="id" /> or throws.
        ///     返回 <c>id</c> 的 minted <c>PileType</c>，或抛出异常。
        /// </summary>
        public static PileType GetPileType(string id)
        {
            return Get(id).PileType;
        }

        /// <summary>
        ///     Resolves the minted <see cref="PileType" /> for <paramref name="id" />.
        ///     解析 <c>id</c> 的 minted <c>PileType</c>。
        /// </summary>
        public static bool TryGetPileType(string id, out PileType value)
        {
            if (TryGet(id, out var definition))
            {
                value = definition.PileType;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///     Resolves either a registered mod card-pile id or a vanilla <see cref="PileType" /> enum name.
        ///     Mod ids take precedence when a string could match both.
        ///     解析已注册 mod card-pile id 或原版 <c>PileType</c> enum 名称。当字符串两者都能匹配时，
        ///     mod id 优先。
        /// </summary>
        public static bool TryResolvePileType(string idOrEnumName, out PileType value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(idOrEnumName);

            return TryGetPileType(idOrEnumName, out value) || Enum.TryParse(idOrEnumName.Trim(), true, out value);
        }

        /// <summary>
        ///     Tries to resolve the string id that minted <paramref name="value" />.
        ///     尝试解析 minted <c>value</c> 的字符串 id。
        /// </summary>
        public static bool TryGetId(PileType value, out string id)
        {
            lock (SyncRoot)
            {
                if (DefinitionsByPileType.TryGetValue(value, out var def))
                {
                    id = def.Id;
                    return true;
                }
            }

            id = string.Empty;
            return false;
        }

        /// <summary>
        ///     Convenience wrapper that mirrors <c>ModKeywordRegistry.CreateHoverTip</c>: builds a
        ///     <see cref="HoverTip" /> from the registered <see cref="ModCardPileDefinition" /> with icon and
        ///     localized title / description.
        ///     对应 <c>ModKeywordRegistry.CreateHoverTip</c> 的便捷包装：从已注册的
        ///     <c>ModCardPileDefinition</c> 构建带图标和本地化 title / description 的 <c>HoverTip</c>。
        /// </summary>
        /// <param name="id">
        ///     Normalized pile id.
        ///     normalized pile id。
        /// </param>
        public static HoverTip CreateHoverTip(string id)
        {
            return !TryGet(id, out var definition)
                ? throw new KeyNotFoundException($"Card pile '{NormalizeId(id)}' is not registered.")
                : ModCardPileHoverTipFactory.Create(definition);
        }

        /// <summary>
        ///     Snapshot of all registered definitions, stable-ordered by id.
        ///     所有已注册 definition 的快照，按 id 稳定排序。
        /// </summary>
        public static ModCardPileDefinition[] GetDefinitionsSnapshot()
        {
            lock (SyncRoot)
            {
                return Definitions.Values
                    .OrderBy(def => def.Id, StringComparer.Ordinal)
                    .ToArray();
            }
        }

        /// <summary>
        ///     Snapshot of every definition that should own a UI button / container of <paramref name="style" />.
        ///     所有应拥有 <c>style</c> UI button / container 的 definition 快照。
        /// </summary>
        internal static ModCardPileDefinition[] GetDefinitionsByStyle(ModCardPileUiStyle style)
        {
            lock (SyncRoot)
            {
                return Definitions.Values
                    .Where(def => def.Style == style)
                    .OrderBy(def => def.Id, StringComparer.Ordinal)
                    .ToArray();
            }
        }

        private ModCardPileDefinition RegisterCore(string id, ModCardPileSpec spec)
        {
            EnsureMutable("register card piles");

            var normalizedId = NormalizeId(id);
            var pileType = PileTypeMinter.Mint(normalizedId);

            var definition = new ModCardPileDefinition(
                _modId,
                normalizedId,
                pileType,
                spec.Scope,
                spec.Style,
                spec.Anchor,
                spec.IconPath,
                spec.Hotkeys,
                spec.CardShouldBeVisible,
                spec.OnOpen,
                spec.HoverTipScreenOffset,
                spec.HoverTipPlacement,
                spec.VisibleWhen,
                spec.FlightTargetPositionResolver,
                spec.FlightStartPositionResolver);

            lock (SyncRoot)
            {
                if (Definitions.TryGetValue(normalizedId, out var existing))
                {
                    if (!ReferenceEquals(existing.ModId, definition.ModId)
                        && !StringComparer.OrdinalIgnoreCase.Equals(existing.ModId, definition.ModId))
                        throw new InvalidOperationException(
                            $"Card pile '{normalizedId}' is already registered by mod '{existing.ModId}'; "
                            + $"mod '{definition.ModId}' cannot re-register it.");

                    return existing;
                }

                Definitions[normalizedId] = definition;
                DefinitionsByPileType[pileType] = definition;
            }

            _logger.Info($"[CardPiles] Registered pile: {normalizedId} (PileType=0x{(int)pileType:X8}, "
                         + $"Style={spec.Style}, Scope={spec.Scope})");
            return definition;
        }

        private void EnsureMutable(string operation)
        {
            if (!IsFrozen)
                return;

            throw new InvalidOperationException(
                $"Cannot {operation} after pile registration has been frozen ({_freezeReason ?? "unknown"}). "
                + "Register piles from your mod initializer before model initialization.");
        }

        // The registry dictionaries use StringComparer.OrdinalIgnoreCase so we do not force a case here —
        // RegisterOwned emits the canonical uppercase form (MODID_CARDPILE_LOCAL) via
        // ModContentRegistry.GetQualifiedCardPileId and Register(string, ...) preserves whatever shape
        // the caller chose. Loc keys use the same id string (vanilla `DRAW_PILE.title` style in static_hover_tips).
        private static string NormalizeId(string id)
        {
            return id.Trim();
        }
    }
}
