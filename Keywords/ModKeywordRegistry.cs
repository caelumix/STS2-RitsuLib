using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Content;
using STS2RitsuLib.Utils;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace STS2RitsuLib.Keywords
{
    /// <summary>
    ///     Per-mod registration surface for hover-tip keywords. Definitions are stored in a single global map keyed by
    ///     normalized id; prefer <c>RegisterOwned</c> / <c>RegisterCardKeywordOwnedByLocNamespace</c> so ids stay mod-scoped
    ///     like fixed
    ///     model public entries.
    ///     hover-tip keyword 的 per-mod 注册入口。definition 存储在按 normalized id 索引的单一全局 map 中；
    ///     优先使用 <c>RegisterOwned</c> / <c>RegisterCardKeywordOwnedByLocNamespace</c>，使 id 像固定
    ///     model public entry 一样保持在 mod 范围内。
    /// </summary>
    public sealed class ModKeywordRegistry
    {
        private static readonly Lock SyncRoot = new();

        private static readonly Dictionary<string, ModKeywordRegistry> Registries =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, ModKeywordDefinition> Definitions =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<CardKeyword, ModKeywordDefinition> DefinitionsByCardKeyword = [];

        private static readonly DynamicEnumValueMinter<CardKeyword> CardKeywordMinter = new();

        private readonly Logger _logger;

        private readonly string _modId;
        private string? _freezeReason;

        private ModKeywordRegistry(string modId)
        {
            _modId = modId;
            _logger = RitsuLibFramework.CreateLogger(modId);
        }

        /// <summary>
        ///     True after the framework freezes keyword registration (with content/timeline at model init).
        ///     framework 在 model init 时与 content/timeline 一起冻结 keyword 注册后为 true。
        /// </summary>
        public static bool IsFrozen { get; private set; }

        /// <summary>
        ///     Convenience view of <see cref="IsFrozen" /> as <see cref="KeywordRegistrationState" />.
        ///     将 <c>IsFrozen</c> 作为 <c>KeywordRegistrationState</c> 查看时的便捷视图。
        /// </summary>
        public static KeywordRegistrationState State => IsFrozen
            ? KeywordRegistrationState.Frozen
            : KeywordRegistrationState.Open;

        /// <summary>
        ///     Returns the singleton registry for <paramref name="modId" />, creating it on first use.
        ///     返回 <c>modId</c> 的 singleton registry，首次使用时创建。
        /// </summary>
        public static ModKeywordRegistry For(string modId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);

            lock (SyncRoot)
            {
                if (Registries.TryGetValue(modId, out var existing))
                    return existing;

                var created = new ModKeywordRegistry(modId);
                Registries[modId] = created;
                return created;
            }
        }

        internal static void FreezeRegistrations(string reason)
        {
            ModKeywordRegistry[] registriesSnapshot;
            lock (SyncRoot)
            {
                if (IsFrozen)
                    return;

                IsFrozen = true;
                foreach (var registry in Registries.Values)
                    registry._freezeReason = reason;

                registriesSnapshot = [.. Registries.Values];
            }

            foreach (var registry in registriesSnapshot)
                registry._logger.Info($"[Keywords] Keyword registration is now frozen ({reason}).");
        }

        /// <summary>
        ///     Resolves which mod registered <paramref name="keywordId" />, if any.
        ///     解析哪个 mod 注册了 <c>keywordId</c>，如果存在的话。
        /// </summary>
        public static bool TryGetOwnerModId(string keywordId, out string modId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(keywordId);

            lock (SyncRoot)
            {
                if (Definitions.TryGetValue(NormalizeId(keywordId), out var def))
                {
                    modId = def.ModId;
                    return true;
                }
            }

            modId = string.Empty;
            return false;
        }

        /// <summary>
        ///     Registers a keyword with an id derived from <see cref="ModContentRegistry.GetQualifiedKeywordId" /> using
        ///     this registry’s mod id and <paramref name="localKeywordStem" />.
        ///     使用此 registry 的 mod id 与 <c>localKeywordStem</c>，通过
        ///     <c>ModContentRegistry.GetQualifiedKeywordId</c> 派生出的 id 注册 keyword。
        /// </summary>
        public ModKeywordDefinition RegisterOwned(
            string localKeywordStem,
            string titleTable,
            string? titleKey,
            string? descriptionTable,
            string? descriptionKey,
            string? iconPath,
            ModKeywordCardDescriptionPlacement cardDescriptionPlacement,
            bool includeInCardHoverTip)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(localKeywordStem);
            var id = ModContentRegistry.GetQualifiedKeywordId(_modId, localKeywordStem);
            return RegisterCore(
                id,
                titleTable,
                titleKey,
                descriptionTable,
                descriptionKey,
                iconPath,
                cardDescriptionPlacement,
                includeInCardHoverTip);
        }

        /// <summary>
        ///     <c>RegisterOwned</c> with default title/description key rules (same as legacy
        ///     <c>Register(string, titleTable, ...)</c>).
        ///     使用默认 title / description key 规则的 <c>RegisterOwned</c>（与 legacy
        ///     <c>Register(string, titleTable, ...)</c> 相同）。
        /// </summary>
        public ModKeywordDefinition RegisterOwned(
            string localKeywordStem,
            string titleTable = "card_keywords",
            string? titleKey = null,
            string? descriptionTable = null,
            string? descriptionKey = null,
            string? iconPath = null)
        {
            return RegisterOwned(
                localKeywordStem,
                titleTable,
                titleKey,
                descriptionTable,
                descriptionKey,
                iconPath,
                ModKeywordCardDescriptionPlacement.None,
                true);
        }

        /// <summary>
        ///     Registers a <c>card_keywords</c> entry whose id and loc stem both come from
        ///     <see cref="ModContentRegistry.GetQualifiedKeywordId" />(<paramref name="localKeywordStem" />): keys are
        ///     <c>{id}.title</c> and <c>{id}.description</c> on <c>card_keywords</c> (uppercase id).
        ///     注册一个 <c>card_keywords</c> entry，其 id 和 loc stem 都来自
        ///     <see cref="ModContentRegistry.GetQualifiedKeywordId" />(<paramref name="localKeywordStem" />)：
        ///     key 是 <c>card_keywords</c> 上的 <c>{id}.title</c> 和 <c>{id}.description</c>（大写 id）。
        /// </summary>
        public ModKeywordDefinition RegisterCardKeywordOwnedByLocNamespace(
            string localKeywordStem,
            string? iconPath,
            ModKeywordCardDescriptionPlacement cardDescriptionPlacement,
            bool includeInCardHoverTip)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(localKeywordStem);

            var id = ModContentRegistry.GetQualifiedKeywordId(_modId, localKeywordStem);

            return RegisterCore(
                id,
                "card_keywords",
                $"{id}.title",
                "card_keywords",
                $"{id}.description",
                iconPath,
                cardDescriptionPlacement,
                includeInCardHoverTip);
        }

        /// <summary>
        ///     <c>RegisterCardKeywordOwnedByLocNamespace</c> with legacy hover defaults.
        ///     使用 legacy hover 默认值的 <c>RegisterCardKeywordOwnedByLocNamespace</c>。
        /// </summary>
        public ModKeywordDefinition RegisterCardKeywordOwnedByLocNamespace(
            string localKeywordStem,
            string? iconPath = null)
        {
            return RegisterCardKeywordOwnedByLocNamespace(
                localKeywordStem,
                iconPath,
                ModKeywordCardDescriptionPlacement.None,
                true);
        }

        /// <summary>
        ///     Registers a keyword with a raw global id. Prefer <c>RegisterOwned</c> to avoid cross-mod collisions.
        ///     使用 raw global id 注册 keyword。优先使用 <c>RegisterOwned</c> 以避免跨 mod 冲突。
        /// </summary>
        [Obsolete(
            "Flat keyword ids are global: they collide across mods and do not follow fixed public entry naming. Use RegisterOwned / RegisterCardKeywordOwnedByLocNamespace, or ModContentRegistry.GetQualifiedKeywordId for cross-mod references.")]
        public ModKeywordDefinition Register(
            string id,
            string titleTable,
            string? titleKey,
            string? descriptionTable,
            string? descriptionKey,
            string? iconPath,
            ModKeywordCardDescriptionPlacement cardDescriptionPlacement,
            bool includeInCardHoverTip)
        {
            return RegisterCore(
                id,
                titleTable,
                titleKey,
                descriptionTable,
                descriptionKey,
                iconPath,
                cardDescriptionPlacement,
                includeInCardHoverTip);
        }

        /// <summary>
        ///     Legacy <c>Register</c> signature preserved for older mods; forwards with prior hover-tip behavior.
        ///     为旧 mod 保留的 legacy <c>Register</c> 签名；以旧 hover-tip 行为转发。
        /// </summary>
        [Obsolete(
            "Flat keyword ids are global: they collide across mods and do not follow fixed public entry naming. Use RegisterOwned / RegisterCardKeywordOwnedByLocNamespace, or ModContentRegistry.GetQualifiedKeywordId for cross-mod references.")]
        public ModKeywordDefinition Register(
            string id,
            string titleTable = "card_keywords",
            string? titleKey = null,
            string? descriptionTable = null,
            string? descriptionKey = null,
            string? iconPath = null)
        {
            return RegisterCore(
                id,
                titleTable,
                titleKey,
                descriptionTable,
                descriptionKey,
                iconPath,
                ModKeywordCardDescriptionPlacement.None,
                true);
        }

        /// <summary>
        ///     Registers a card keyword with a raw global id. Prefer <c>RegisterCardKeywordOwnedByLocNamespace</c>.
        ///     使用 raw global id 注册 card keyword。优先使用 <c>RegisterCardKeywordOwnedByLocNamespace</c>。
        /// </summary>
        [Obsolete(
            "Flat keyword ids are global: they collide across mods and do not follow fixed public entry naming. Use RegisterCardKeywordOwnedByLocNamespace, or ModContentRegistry.GetQualifiedKeywordId for cross-mod references.")]
        public ModKeywordDefinition RegisterCardKeyword(
            string id,
            string? entryStem,
            string? iconPath,
            ModKeywordCardDescriptionPlacement cardDescriptionPlacement,
            bool includeInCardHoverTip)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            var prefix = string.IsNullOrWhiteSpace(entryStem)
                ? StringHelper.Slugify(id)
                : entryStem.Trim();

            return RegisterCore(
                id,
                "card_keywords",
                $"{prefix}.title",
                "card_keywords",
                $"{prefix}.description",
                iconPath,
                cardDescriptionPlacement,
                includeInCardHoverTip);
        }

        /// <summary>
        ///     Legacy <c>RegisterCardKeyword</c> signature preserved for older mods; forwards with prior hover-tip behavior.
        ///     为旧 mod 保留的 legacy <c>RegisterCardKeyword</c> 签名；以旧 hover-tip 行为转发。
        /// </summary>
        [Obsolete(
            "Flat keyword ids are global: they collide across mods and do not follow fixed public entry naming. Use RegisterCardKeywordOwnedByLocNamespace, or ModContentRegistry.GetQualifiedKeywordId for cross-mod references.")]
        public ModKeywordDefinition RegisterCardKeyword(string id, string? entryStem = null, string? iconPath = null)
        {
            return RegisterCardKeyword(
                id,
                entryStem,
                iconPath,
                ModKeywordCardDescriptionPlacement.None,
                true);
        }

        /// <summary>
        ///     Same as obsolete <c>Register</c> (full signature) without triggering obsolete warnings; for in-library
        ///     forwarding from manifests.
        ///     与 obsolete <c>Register</c>（完整签名）相同，但不会触发 obsolete warning；用于库内从 manifest 转发。
        /// </summary>
        internal ModKeywordDefinition RegisterCore(
            string id,
            string titleTable,
            string? titleKey,
            string? descriptionTable,
            string? descriptionKey,
            string? iconPath,
            ModKeywordCardDescriptionPlacement cardDescriptionPlacement,
            bool includeInCardHoverTip)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentException.ThrowIfNullOrWhiteSpace(titleTable);

            EnsureMutable("register keywords");

            var normalizedId = NormalizeId(id);
            var cardKeywordValue = CardKeywordMinter.Mint(normalizedId);
            var definition = new ModKeywordDefinition(
                _modId,
                normalizedId,
                titleTable,
                titleKey ?? $"{normalizedId}.title",
                descriptionTable ?? titleTable,
                descriptionKey ?? $"{normalizedId}.description",
                iconPath,
                cardDescriptionPlacement,
                includeInCardHoverTip)
            {
                CardKeywordValue = cardKeywordValue,
            };

            lock (SyncRoot)
            {
                if (Definitions.TryGetValue(normalizedId, out var existing))
                {
                    if (existing != definition)
                        throw new InvalidOperationException(
                            $"Keyword '{normalizedId}' is already registered by mod '{existing.ModId}' with different data; ids are global and must not be reused with conflicting definitions.");

                    return existing;
                }

                Definitions[normalizedId] = definition;
                DefinitionsByCardKeyword[cardKeywordValue] = definition;
            }

            _logger.Info(
                $"[Keywords] Registered keyword: {normalizedId} (CardKeyword=0x{(int)cardKeywordValue:X8})");
            return definition;
        }

        /// <summary>
        ///     Tries to resolve a global definition by keyword id.
        ///     尝试按 keyword id 解析全局 definition。
        /// </summary>
        public static bool TryGet(string id, out ModKeywordDefinition definition)
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
        public static ModKeywordDefinition Get(string id)
        {
            return TryGet(id, out var definition)
                ? definition
                : throw new KeyNotFoundException($"Keyword '{NormalizeId(id)}' is not registered.");
        }

        /// <summary>
        ///     Reverse lookup: resolves the mod keyword <see cref="ModKeywordDefinition" /> that minted
        ///     <paramref name="value" />. Returns <c>false</c> for vanilla <see cref="CardKeyword" /> literals and
        ///     for any value that was never registered.
        ///     反向查找：解析 minted <c>value</c> 的 mod keyword
        ///     <c>ModKeywordDefinition</c>。对原版 <c>CardKeyword</c> literal 和任何从未注册的值返回
        ///     <c>false</c>。
        /// </summary>
        public static bool TryGetByCardKeyword(CardKeyword value, out ModKeywordDefinition definition)
        {
            lock (SyncRoot)
            {
                return DefinitionsByCardKeyword.TryGetValue(value, out definition!);
            }
        }

        /// <summary>
        ///     Whether <paramref name="value" /> is a registered mod keyword (as opposed to a vanilla
        ///     <see cref="CardKeyword" /> literal or an unknown integer cast).
        ///     <c>value</c> 是否为已注册 mod keyword（而不是原版 <c>CardKeyword</c> literal
        ///     或未知整数 cast）。
        /// </summary>
        public static bool IsModCardKeyword(CardKeyword value)
        {
            lock (SyncRoot)
            {
                return DefinitionsByCardKeyword.ContainsKey(value);
            }
        }

        /// <summary>
        ///     Resolves the <see cref="CardKeyword" /> value minted for <paramref name="id" />. Prefer this over
        ///     passing a string when interacting with vanilla keyword APIs (<c>CardModel.AddKeyword</c> /
        ///     <c>Keywords.Contains</c>).
        ///     解析为 <c>id</c> minted 的 <c>CardKeyword</c> 值。与原版 keyword API
        ///     （<c>CardModel.AddKeyword</c> / <c>Keywords.Contains</c>）交互时，优先使用它而不是传字符串。
        /// </summary>
        public static bool TryGetCardKeyword(string id, out CardKeyword value)
        {
            if (TryGet(id, out var definition))
            {
                value = definition.CardKeywordValue;
                return true;
            }

            value = CardKeyword.None;
            return false;
        }

        /// <summary>
        ///     Resolves either a registered mod keyword id or a vanilla <see cref="CardKeyword" /> enum name.
        ///     Mod ids take precedence when a string could match both.
        ///     解析已注册 mod keyword id 或原版 <c>CardKeyword</c> enum 名称。当字符串两者都能匹配时，
        ///     mod id 优先。
        /// </summary>
        public static bool TryResolveCardKeyword(string idOrEnumName, out CardKeyword value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(idOrEnumName);

            return TryGetCardKeyword(idOrEnumName, out value) || Enum.TryParse(idOrEnumName.Trim(), true, out value);
        }

        /// <summary>
        ///     Returns the <see cref="CardKeyword" /> minted for <paramref name="id" /> or throws
        ///     <see cref="KeyNotFoundException" /> when unregistered.
        ///     返回为 <c>id</c> minted 的 <c>CardKeyword</c>；未注册时抛出
        ///     <see cref="KeyNotFoundException" />。
        /// </summary>
        public static CardKeyword GetCardKeyword(string id)
        {
            return Get(id).CardKeywordValue;
        }

        /// <summary>
        ///     Tries to resolve the string id that minted <paramref name="value" />.
        ///     尝试解析 minted <c>value</c> 的字符串 id。
        /// </summary>
        public static bool TryGetId(CardKeyword value, out string id)
        {
            lock (SyncRoot)
            {
                if (DefinitionsByCardKeyword.TryGetValue(value, out var def))
                {
                    id = def.Id;
                    return true;
                }
            }

            id = string.Empty;
            return false;
        }

        /// <summary>
        ///     Snapshot of all registered keyword definitions, stable-ordered by id.
        ///     所有已注册 keyword definition 的快照，按 id 稳定排序。
        /// </summary>
        public static ModKeywordDefinition[] GetDefinitionsSnapshot()
        {
            lock (SyncRoot)
            {
                return Definitions.Values
                    .OrderBy(def => def.Id, StringComparer.Ordinal)
                    .ToArray();
            }
        }

        /// <summary>
        ///     Builds a vanilla <see cref="IHoverTip" /> for <paramref name="id" /> using registered title, description, and
        ///     icon.
        ///     使用已注册的 title、description 和 icon 为 <c>id</c> 构建原版 <c>IHoverTip</c>。
        /// </summary>
        public static IHoverTip CreateHoverTip(string id)
        {
            var definition = Get(id);
            Texture2D? icon = null;

            if (!string.IsNullOrWhiteSpace(definition.IconPath) && ResourceLoader.Exists(definition.IconPath))
                icon = ResourceLoader.Load<Texture2D>(definition.IconPath);

            return new HoverTip(GetTitle(id), GetDescription(id), icon);
        }

        /// <summary>
        ///     Title <see cref="LocString" /> for the keyword.
        ///     keyword 的 title <c>LocString</c>。
        /// </summary>
        public static LocString GetTitle(string id)
        {
            var definition = Get(id);
            return new(definition.TitleTable, definition.TitleKey);
        }

        /// <summary>
        ///     Description <see cref="LocString" /> for the keyword.
        ///     keyword 的 description <c>LocString</c>。
        /// </summary>
        public static LocString GetDescription(string id)
        {
            var definition = Get(id);
            return new(definition.DescriptionTable, definition.DescriptionKey);
        }

        /// <summary>
        ///     BBCode snippet suitable for inline card text (gold title + period).
        ///     适合 inline card text 的 BBCode 片段（gold title + period）。
        /// </summary>
        public static string GetCardText(string id)
        {
            var period = new LocString("card_keywords", "PERIOD");
            return "[gold]" + GetTitle(id).GetFormattedText() + "[/gold]" + period.GetRawText();
        }

        private void EnsureMutable(string operation)
        {
            if (!IsFrozen)
                return;

            throw new InvalidOperationException(
                $"Cannot {operation} after keyword registration has been frozen ({_freezeReason ?? "unknown"}). " +
                "Register keywords from your mod initializer before model initialization.");
        }

        private static string NormalizeId(string id)
        {
            return id.Trim();
        }
    }
}
