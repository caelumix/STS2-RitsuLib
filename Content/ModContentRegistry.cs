using System.Reflection;
using System.Text.RegularExpressions;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Diagnostics;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2RitsuLib.Content
{
    /// <summary>
    ///     Whether <see cref="ModContentRegistry" /> still accepts new registrations from mods.
    ///     表示 <c>ModContentRegistry</c> 是否仍接受来自 Mod 的新注册。
    /// </summary>
    public enum ContentRegistrationState
    {
        /// <summary>
        ///     Registrations are allowed until the framework freezes them.
        ///     在框架冻结注册之前允许继续注册。
        /// </summary>
        Open = 0,

        /// <summary>
        ///     Further registration throws; game content lists are considered sealed.
        ///     继续注册会抛出异常；游戏内容列表视为已封闭。
        /// </summary>
        Frozen = 1,
    }

    /// <summary>
    ///     Per-mod content registration surface: pool models, standalone models, act-scoped content, and stable public
    ///     entry overrides used by patched <see cref="ModelDb" /> identity.
    ///     每个 Mod 独立的内容注册入口：池模型、独立模型、按 Act 作用域的内容，以及补丁版
    ///     <c>ModelDb</c> 身份逻辑使用的稳定公开 entry 覆盖。
    /// </summary>
    public sealed partial class ModContentRegistry
    {
        private static readonly Lock SyncRoot = new();

        private static readonly Dictionary<string, ModContentRegistry> Registries =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<Type, string> FixedPublicEntryOverrides = [];

        private static readonly HashSet<(Type PoolType, Type ModelType)> RegisteredPoolContent = [];
        private static readonly List<CharacterStarterRegistration> RegisteredCharacterStarterContent = [];
        private static readonly HashSet<Type> RegisteredCharacters = [];
        private static readonly HashSet<Type> RegisteredActs = [];
        private static readonly HashSet<Type> RegisteredMonsters = [];
        private static readonly HashSet<Type> RegisteredPowers = [];
        private static readonly HashSet<Type> RegisteredOrbs = [];
        private static readonly HashSet<Type> RegisteredSharedCardPools = [];
        private static readonly HashSet<Type> RegisteredSharedEvents = [];
        private static readonly HashSet<Type> RegisteredSharedAncients = [];
        private static readonly Dictionary<Type, HashSet<Type>> RegisteredActEncounters = [];

        private static readonly HashSet<Type> RegisteredGlobalEncounters = [];
        private static readonly Dictionary<Type, HashSet<Type>> RegisteredActEvents = [];
        private static readonly Dictionary<Type, HashSet<Type>> RegisteredActAncients = [];
        private static readonly HashSet<Type> RegisteredEnchantments = [];
        private static readonly HashSet<Type> RegisteredAfflictions = [];
        private static readonly HashSet<Type> RegisteredAchievements = [];
        private static readonly HashSet<Type> RegisteredSingletons = [];
        private static readonly HashSet<Type> RegisteredBadges = [];
        private static readonly HashSet<Type> RegisteredSharedRelicPools = [];
        private static readonly HashSet<Type> RegisteredSharedPotionPools = [];
        private static readonly HashSet<Type> RegisteredGoodModifiers = [];
        private static readonly HashSet<Type> RegisteredBadModifiers = [];
        private static readonly Dictionary<Type, string> RegisteredTypeOwners = [];

        private readonly Logger _logger;
        private string? _freezeReason;

        private ModContentRegistry(string modId)
        {
            ModId = modId;
            _logger = RitsuLibFramework.CreateLogger(modId);
        }

        /// <summary>
        ///     Mod identifier this registry instance was created for (<see cref="For" />).
        ///     创建此注册表实例时对应的 Mod 标识符（参见 <c>For</c>）。
        /// </summary>
        public string ModId { get; }

        /// <summary>
        ///     True after <c>FreezeRegistrations</c> has run globally.
        ///     当全局 <c>FreezeRegistrations</c> 已运行后为 true。
        /// </summary>
        public static bool IsFrozen { get; private set; }

        /// <summary>
        ///     Convenience view of <see cref="IsFrozen" /> as <see cref="ContentRegistrationState" />.
        ///     将 <c>IsFrozen</c> 以 <c>ContentRegistrationState</c> 形式暴露的便捷视图。
        /// </summary>
        public static ContentRegistrationState State => IsFrozen
            ? ContentRegistrationState.Frozen
            : ContentRegistrationState.Open;

        /// <summary>
        ///     Resolves which mod registered <paramref name="modelType" />, if any.
        ///     解析 <c>modelType</c> 是由哪个 Mod 注册的（如果存在）。
        /// </summary>
        public static bool TryGetOwnerModId(Type modelType, out string modId)
        {
            ArgumentNullException.ThrowIfNull(modelType);

            lock (SyncRoot)
            {
                return RegisteredTypeOwners.TryGetValue(modelType, out modId!);
            }
        }

        /// <summary>
        ///     Returns the stable public entry string for a RitsuLib-registered model type (override or generated).
        ///     返回 RitsuLib 注册模型类型的稳定公开 entry 字符串（覆盖值或生成值）。
        /// </summary>
        public static bool TryGetFixedPublicEntry(Type modelType, out string entry)
        {
            ArgumentNullException.ThrowIfNull(modelType);

            if (!TryGetOwnerModId(modelType, out var modId))
            {
                entry = string.Empty;
                return false;
            }

            lock (SyncRoot)
            {
                if (FixedPublicEntryOverrides.TryGetValue(modelType, out var over))
                {
                    entry = over;
                    return true;
                }
            }

            entry = GetFixedPublicEntry(modId, modelType);
            return true;
        }

        /// <summary>
        ///     Builds the default normalized entry <c>MOD_CATEGORY_TYPENAME</c> for a type owned by
        ///     <paramref name="modId" />.
        ///     为 <c>modId</c> 拥有的类型构建默认规范化 entry：<c>MOD_CATEGORY_TYPENAME</c>。
        /// </summary>
        public static string GetFixedPublicEntry(string modId, Type modelType)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentNullException.ThrowIfNull(modelType);

            var modStem = NormalizePublicStem(modId);
            var categoryStem = NormalizePublicStem(ModelDb.GetCategory(modelType));
            var typeStem = NormalizePublicStem(modelType.Name);
            return $"{modStem}_{categoryStem}_{typeStem}";
        }

        /// <summary>
        ///     Builds a stable three-segment compound id: <c>{normalizedModId}_{TYPE}_{normalizedName}</c>
        ///     (underscore-separated). Mod and name use <see cref="NormalizePublicStem" />; the type segment is only
        ///     trimmed then uppercased with <c>ToUpperInvariant</c> (no stem normalization).
        ///     构建稳定的三段式复合 id：<c>{normalizedModId}_{TYPE}_{normalizedName}</c>（以下划线分隔）。
        ///     Mod 和名称段使用 <c>NormalizePublicStem</c>；类型段只会 trim 后用
        ///     <c>ToUpperInvariant</c> 转大写（不做 stem 规范化）。
        /// </summary>
        public static string GetCompoundId(string modId, string typeStem, string nameStem)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentException.ThrowIfNullOrWhiteSpace(nameStem);
            ArgumentNullException.ThrowIfNull(typeStem);

            var trimmedType = typeStem.Trim();
            if (trimmedType.Length == 0)
                throw new ArgumentException("Type segment cannot be empty or whitespace.", nameof(typeStem));

            var mod = NormalizePublicStem(modId);
            var type = trimmedType.ToUpperInvariant();
            var name = NormalizePublicStem(nameStem);
            return $"{mod}_{type}_{name}";
        }

        /// <summary>
        ///     Builds a mod-scoped keyword id: <c>{normalizedModId}_KEYWORD_{normalizedStem}</c>, matching the
        ///     three-segment convention used by <see cref="GetQualifiedCardPileId" /> and
        ///     <see cref="GetQualifiedTopBarButtonId" /> (all uppercase). Other mods can reference a provider’s keyword
        ///     by passing the same <paramref name="modId" /> and <paramref name="localKeywordStem" />.
        ///     构建 Mod 作用域关键词 id：<c>{normalizedModId}_KEYWORD_{normalizedStem}</c>，与
        ///     <c>GetQualifiedCardPileId</c> 和 <c>GetQualifiedTopBarButtonId</c> 使用的三段式约定一致
        ///     （全大写）。其他 Mod 可通过传入同一个 <c>modId</c> 和
        ///     <c>localKeywordStem</c> 引用提供方的关键词。
        /// </summary>
        public static string GetQualifiedKeywordId(string modId, string localKeywordStem)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentException.ThrowIfNullOrWhiteSpace(localKeywordStem);

            return GetCompoundId(modId, "KEYWORD", localKeywordStem);
        }

        /// <summary>
        ///     Builds a mod-scoped card-pile id using the ritsulib <c>MODID_CATEGORY_TYPENAME</c> public-entry
        ///     convention — three uppercase segments separated by underscores, aligning with
        ///     <see cref="GetFixedPublicEntry(string, Type)" /> and the vanilla <c>static_hover_tips</c> key
        ///     style (<c>DRAW_PILE</c>, <c>EXHAUST_PILE</c>, ...).
        ///     使用 RitsuLib 的 <c>MODID_CATEGORY_TYPENAME</c> 公开 entry 约定构建 Mod 作用域牌堆 id，
        ///     即三个以下划线分隔的大写段；它与 <c>GetFixedPublicEntry(string, Type)</c> 以及原版
        ///     <c>static_hover_tips</c> 键风格（<c>DRAW_PILE</c>、<c>EXHAUST_PILE</c> 等）保持一致。
        /// </summary>
        /// <remarks>
        ///     The returned string is the stem for <c>static_hover_tips.json</c> keys, so a pile registered by
        ///     mod <c>com.example.my-mod</c> with local stem <c>overflow_pile</c> uses id
        ///     <c>MYMOD_CARDPILE_OVERFLOW_PILE</c> and loc keys <c>MYMOD_CARDPILE_OVERFLOW_PILE.title</c> /
        ///     <c>.description</c> / <c>.empty</c>.
        ///     返回的字符串会作为 <c>static_hover_tips.json</c> 键的 stem。例如 Mod
        ///     <c>com.example.my-mod</c> 使用本地 stem <c>overflow_pile</c> 注册牌堆时，id 为
        ///     <c>MYMOD_CARDPILE_OVERFLOW_PILE</c>，本地化键为 <c>MYMOD_CARDPILE_OVERFLOW_PILE.title</c> /
        ///     <c>.description</c> / <c>.empty</c>。
        /// </remarks>
        public static string GetQualifiedCardPileId(string modId, string localPileStem)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentException.ThrowIfNullOrWhiteSpace(localPileStem);

            return GetCompoundId(modId, "CARDPILE", localPileStem);
        }

        /// <summary>
        ///     Builds a mod-scoped <see cref="MegaCrit.Sts2.Core.Entities.Cards.CardTag" /> id using the ritsulib
        ///     <c>MODID_CATEGORY_TYPENAME</c> convention with middle segment <c>CARDTAG</c>, aligned with
        ///     <see cref="GetQualifiedKeywordId" /> and <see cref="GetQualifiedCardPileId" />.
        ///     使用 RitsuLib 的 <c>MODID_CATEGORY_TYPENAME</c> 约定构建 Mod 作用域
        ///     <c>MegaCrit.Sts2.Core.Entities.Cards.CardTag</c> id，中间段固定为 <c>CARDTAG</c>，
        ///     与 <c>GetQualifiedKeywordId</c> 和 <c>GetQualifiedCardPileId</c> 对齐。
        /// </summary>
        public static string GetQualifiedCardTagId(string modId, string localTagStem)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentException.ThrowIfNullOrWhiteSpace(localTagStem);

            return GetCompoundId(modId, "CARDTAG", localTagStem);
        }

        /// <summary>
        ///     Builds a mod-scoped reward id using the ritsulib <c>MODID_CATEGORY_TYPENAME</c> convention
        ///     with middle segment <c>REWARD</c>.
        ///     使用 RitsuLib 的 <c>MODID_CATEGORY_TYPENAME</c> 约定构建 Mod 作用域 reward id，
        ///     中间段固定为 <c>REWARD</c>。
        /// </summary>
        public static string GetQualifiedRewardId(string modId, string localRewardStem)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentException.ThrowIfNullOrWhiteSpace(localRewardStem);

            return GetCompoundId(modId, "REWARD", localRewardStem);
        }

        /// <summary>
        ///     Builds a mod-scoped top-bar-button id in the ritsulib <c>MODID_CATEGORY_TYPENAME</c> public
        ///     entry style (uppercase, three segments, underscore-separated, middle segment fixed to
        ///     <c>TOPBARBUTTON</c>). Used by <see cref="STS2RitsuLib.TopBar.ModTopBarButtonRegistry" />; the
        ///     returned string is the stem for <c>static_hover_tips.json</c> title / description keys.
        ///     以 RitsuLib 的 <c>MODID_CATEGORY_TYPENAME</c> 公开 entry 风格构建 Mod 作用域顶部栏按钮 id
        ///     （大写、三段、以下划线分隔，中间段固定为 <c>TOPBARBUTTON</c>）。该 id 由
        ///     <c>STS2RitsuLib.TopBar.ModTopBarButtonRegistry</c> 使用；返回字符串会作为
        ///     <c>static_hover_tips.json</c> 标题/描述键的 stem。
        /// </summary>
        public static string GetQualifiedTopBarButtonId(string modId, string localButtonStem)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentException.ThrowIfNullOrWhiteSpace(localButtonStem);

            return GetCompoundId(modId, "TOPBARBUTTON", localButtonStem);
        }

        /// <summary>
        ///     Returns the singleton registry for <paramref name="modId" /> (created on first use).
        ///     返回 <c>modId</c> 对应的单例注册表（首次使用时创建）。
        /// </summary>
        public static ModContentRegistry For(string modId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);

            lock (SyncRoot)
            {
                if (Registries.TryGetValue(modId, out var registry))
                    return registry;

                registry = new(modId);
                Registries[modId] = registry;
                return registry;
            }
        }

        /// <summary>
        ///     Registers <typeparamref name="TCard" /> into <typeparamref name="TPool" /> with default public entry
        ///     naming.
        ///     使用默认公开 entry 命名，将 <c>TCard</c> 注册到 <c>TPool</c>。
        /// </summary>
        public void RegisterCard<TPool, TCard>()
            where TPool : CardPoolModel
            where TCard : CardModel
        {
            RegisterCard(typeof(TPool), typeof(TCard));
        }

        /// <summary>
        ///     Registers <paramref name="cardType" /> into <paramref name="poolType" /> with default public entry naming.
        ///     使用默认公开 entry 命名，将 <c>cardType</c> 注册到 <c>poolType</c>。
        /// </summary>
        public void RegisterCard(Type poolType, Type cardType)
        {
            RegisterCard(poolType, cardType, default);
        }

        /// <summary>
        ///     Registers <typeparamref name="TCard" /> into <typeparamref name="TPool" /> using
        ///     Registers <c>T卡牌</c> into <c>TPool</c> using
        ///     <paramref name="publicEntry" /> rules.
        /// </summary>
        public void RegisterCard<TPool, TCard>(ModelPublicEntryOptions publicEntry)
            where TPool : CardPoolModel
            where TCard : CardModel
        {
            RegisterCard(typeof(TPool), typeof(TCard), publicEntry);
        }

        /// <summary>
        ///     Registers <paramref name="cardType" /> into <paramref name="poolType" /> using
        ///     Registers <c>卡牌Type</c> into <c>poolType</c> using
        ///     <paramref name="publicEntry" /> rules.
        /// </summary>
        public void RegisterCard(Type poolType, Type cardType, ModelPublicEntryOptions publicEntry)
        {
            RegisterPoolModel(poolType, cardType, "card", publicEntry);
        }

        /// <summary>
        ///     Registers <typeparamref name="TRelic" /> into <typeparamref name="TPool" /> with default public entry
        ///     Registers <c>T遗物</c> into <c>TPool</c> 带有 default public entry
        ///     naming.
        ///     中文说明：naming.
        /// </summary>
        public void RegisterRelic<TPool, TRelic>()
            where TPool : RelicPoolModel
            where TRelic : RelicModel
        {
            RegisterRelic(typeof(TPool), typeof(TRelic));
        }

        /// <summary>
        ///     Registers <paramref name="relicType" /> into <paramref name="poolType" /> with default public entry naming.
        ///     注册 <c>relicType</c> into <c>poolType</c> with default public entry naming。
        /// </summary>
        public void RegisterRelic(Type poolType, Type relicType)
        {
            RegisterRelic(poolType, relicType, default);
        }

        /// <summary>
        ///     Registers <typeparamref name="TRelic" /> into <typeparamref name="TPool" /> using
        ///     Registers <c>T遗物</c> into <c>TPool</c> using
        ///     <paramref name="publicEntry" /> rules.
        /// </summary>
        public void RegisterRelic<TPool, TRelic>(ModelPublicEntryOptions publicEntry)
            where TPool : RelicPoolModel
            where TRelic : RelicModel
        {
            RegisterRelic(typeof(TPool), typeof(TRelic), publicEntry);
        }

        /// <summary>
        ///     Registers <paramref name="relicType" /> into <paramref name="poolType" /> using
        ///     Registers <c>遗物Type</c> into <c>poolType</c> using
        ///     <paramref name="publicEntry" /> rules.
        /// </summary>
        public void RegisterRelic(Type poolType, Type relicType, ModelPublicEntryOptions publicEntry)
        {
            RegisterPoolModel(poolType, relicType, "relic", publicEntry);
        }

        /// <summary>
        ///     Registers <typeparamref name="TPotion" /> into <typeparamref name="TPool" /> with default public entry
        ///     Registers <c>TPotion</c> into <c>TPool</c> 带有 default public entry
        ///     naming.
        ///     中文说明：naming.
        /// </summary>
        public void RegisterPotion<TPool, TPotion>()
            where TPool : PotionPoolModel
            where TPotion : PotionModel
        {
            RegisterPotion(typeof(TPool), typeof(TPotion));
        }

        /// <summary>
        ///     Registers <paramref name="potionType" /> into <paramref name="poolType" /> with default public entry naming.
        ///     注册 <c>potionType</c> into <c>poolType</c> with default public entry naming。
        /// </summary>
        public void RegisterPotion(Type poolType, Type potionType)
        {
            RegisterPotion(poolType, potionType, default);
        }

        /// <summary>
        ///     Registers <typeparamref name="TPotion" /> into <typeparamref name="TPool" /> using
        ///     中文说明：Registers <c>TPotion</c> into <c>TPool</c> using
        ///     <paramref name="publicEntry" /> rules.
        /// </summary>
        public void RegisterPotion<TPool, TPotion>(ModelPublicEntryOptions publicEntry)
            where TPool : PotionPoolModel
            where TPotion : PotionModel
        {
            RegisterPotion(typeof(TPool), typeof(TPotion), publicEntry);
        }

        /// <summary>
        ///     Registers <paramref name="potionType" /> into <paramref name="poolType" /> using
        ///     中文说明：Registers <c>potionType</c> into <c>poolType</c> using
        ///     <paramref name="publicEntry" /> rules.
        /// </summary>
        public void RegisterPotion(Type poolType, Type potionType, ModelPublicEntryOptions publicEntry)
        {
            RegisterPoolModel(poolType, potionType, "potion", publicEntry);
        }

        /// <summary>
        ///     Registers a mod character model for inclusion in <see cref="ModelDb.AllCharacters" />.
        ///     注册 a mod character model for inclusion in <c>ModelDb.AllCharacters</c>。
        /// </summary>
        public void RegisterCharacter<TCharacter>() where TCharacter : CharacterModel
        {
            RegisterCharacter(typeof(TCharacter));
        }

        /// <summary>
        ///     Registers <paramref name="characterType" /> for inclusion in <see cref="ModelDb.AllCharacters" />.
        ///     注册 <c>characterType</c> for inclusion in <c>ModelDb.AllCharacters</c>。
        /// </summary>
        public void RegisterCharacter(Type characterType)
        {
            RegisterStandaloneModel(RegisteredCharacters, characterType, typeof(CharacterModel), "character");
        }

        /// <summary>
        ///     Registers additional starter-deck copies of <typeparamref name="TCard" /> for <typeparamref name="TCharacter" />.
        ///     注册 additional starter-deck copies of <c>TCard</c> for <c>TCharacter</c>。
        ///     The target character may be registered before or after this call; resolution happens when the character model is
        ///     The target character may be 已注册 之前 或 之后 this call; resolution happens 当 the character 模型 is
        ///     queried. Matching uses the live instance CLR type; registrations against an assignable ancestor type also apply,
        ///     queried. Matching 使用 the live instance CLR type; 注册s against an assignable ancestor type also apply,
        ///     except a registration keyed only to <see cref="CharacterModel" /> itself.
        ///     except a 注册 keyed only to <c>Character模型</c> itself.
        /// </summary>
        public void RegisterCharacterStarterCard<TCharacter, TCard>(int count = 1)
            where TCharacter : CharacterModel
            where TCard : CardModel
        {
            RegisterCharacterStarterCard<TCharacter, TCard>(count, 0);
        }

        /// <summary>
        ///     Registers additional starter-deck copies of <typeparamref name="TCard" /> for <typeparamref name="TCharacter" />.
        ///     注册 additional starter-deck copies of <c>TCard</c> for <c>TCharacter</c>。
        ///     The target character may be registered before or after this call; resolution happens when the character model is
        ///     The target character may be 已注册 之前 或 之后 this call; resolution happens 当 the character 模型 is
        ///     queried. Matching uses the live instance CLR type; registrations against an assignable ancestor type also apply,
        ///     queried. Matching 使用 the live instance CLR type; 注册s against an assignable ancestor type also apply,
        ///     except a registration keyed only to <see cref="CharacterModel" /> itself.
        ///     except a 注册 keyed only to <c>Character模型</c> itself.
        /// </summary>
        public void RegisterCharacterStarterCard<TCharacter, TCard>(int count, int order)
            where TCharacter : CharacterModel
            where TCard : CardModel
        {
            RegisterCharacterStarterCard(typeof(TCharacter), typeof(TCard), count, order);
        }

        /// <summary>
        ///     Registers additional starter-deck copies of <paramref name="cardType" /> for <paramref name="characterType" />.
        ///     注册 additional starter-deck copies of <c>cardType</c> for <c>characterType</c>。
        /// </summary>
        public void RegisterCharacterStarterCard(Type characterType, Type cardType, int count = 1)
        {
            RegisterCharacterStarterCard(characterType, cardType, count, 0);
        }

        /// <summary>
        ///     Registers additional starter-deck copies of <paramref name="cardType" /> for <paramref name="characterType" />.
        ///     注册 additional starter-deck copies of <c>cardType</c> for <c>characterType</c>。
        /// </summary>
        public void RegisterCharacterStarterCard(Type characterType, Type cardType, int count, int order)
        {
            RegisterCharacterStarterModel(characterType, cardType, typeof(CardModel),
                CharacterStarterContentKind.Card,
                count, order);
        }

        /// <summary>
        ///     Registers additional starting relic copies of <typeparamref name="TRelic" /> for <typeparamref name="TCharacter" />
        ///     Registers additional starting 遗物 copies of <c>T遗物</c> 用于 <c>TCharacter</c>
        ///     .
        ///     中文说明：.
        ///     The target character may be registered before or after this call; resolution happens when the character model is
        ///     The target character may be 已注册 之前 或 之后 this call; resolution happens 当 the character 模型 is
        ///     queried. Matching uses the live instance CLR type; registrations against an assignable ancestor type also apply,
        ///     queried. Matching 使用 the live instance CLR type; 注册s against an assignable ancestor type also apply,
        ///     except a registration keyed only to <see cref="CharacterModel" /> itself.
        ///     except a 注册 keyed only to <c>Character模型</c> itself.
        /// </summary>
        public void RegisterCharacterStarterRelic<TCharacter, TRelic>(int count = 1)
            where TCharacter : CharacterModel
            where TRelic : RelicModel
        {
            RegisterCharacterStarterRelic<TCharacter, TRelic>(count, 0);
        }

        /// <summary>
        ///     Registers additional starting relic copies of <typeparamref name="TRelic" /> for <typeparamref name="TCharacter" />
        ///     Registers additional starting 遗物 copies of <c>T遗物</c> 用于 <c>TCharacter</c>
        ///     .
        ///     中文说明：.
        ///     The target character may be registered before or after this call; resolution happens when the character model is
        ///     The target character may be 已注册 之前 或 之后 this call; resolution happens 当 the character 模型 is
        ///     queried. Matching uses the live instance CLR type; registrations against an assignable ancestor type also apply,
        ///     queried. Matching 使用 the live instance CLR type; 注册s against an assignable ancestor type also apply,
        ///     except a registration keyed only to <see cref="CharacterModel" /> itself.
        ///     except a 注册 keyed only to <c>Character模型</c> itself.
        /// </summary>
        public void RegisterCharacterStarterRelic<TCharacter, TRelic>(int count, int order)
            where TCharacter : CharacterModel
            where TRelic : RelicModel
        {
            RegisterCharacterStarterRelic(typeof(TCharacter), typeof(TRelic), count, order);
        }

        /// <summary>
        ///     Registers additional starting relic copies of <paramref name="relicType" /> for <paramref name="characterType" />.
        ///     注册 additional starting relic copies of <c>relicType</c> for <c>characterType</c>。
        /// </summary>
        public void RegisterCharacterStarterRelic(Type characterType, Type relicType, int count = 1)
        {
            RegisterCharacterStarterRelic(characterType, relicType, count, 0);
        }

        /// <summary>
        ///     Registers additional starting relic copies of <paramref name="relicType" /> for <paramref name="characterType" />.
        ///     注册 additional starting relic copies of <c>relicType</c> for <c>characterType</c>。
        /// </summary>
        public void RegisterCharacterStarterRelic(Type characterType, Type relicType, int count, int order)
        {
            RegisterCharacterStarterModel(characterType, relicType, typeof(RelicModel),
                CharacterStarterContentKind.Relic, count, order);
        }

        /// <summary>
        ///     Registers additional starting potion copies of <typeparamref name="TPotion" /> for
        ///     Registers additional starting potion copies of <c>TPotion</c> 用于
        ///     <typeparamref name="TCharacter" />.
        ///     The target character may be registered before or after this call; resolution happens when the character model is
        ///     The target character may be 已注册 之前 或 之后 this call; resolution happens 当 the character 模型 is
        ///     queried. Matching uses the live instance CLR type; registrations against an assignable ancestor type also apply,
        ///     queried. Matching 使用 the live instance CLR type; 注册s against an assignable ancestor type also apply,
        ///     except a registration keyed only to <see cref="CharacterModel" /> itself.
        ///     except a 注册 keyed only to <c>Character模型</c> itself.
        /// </summary>
        public void RegisterCharacterStarterPotion<TCharacter, TPotion>(int count = 1)
            where TCharacter : CharacterModel
            where TPotion : PotionModel
        {
            RegisterCharacterStarterPotion<TCharacter, TPotion>(count, 0);
        }

        /// <summary>
        ///     Registers additional starting potion copies of <typeparamref name="TPotion" /> for
        ///     Registers additional starting potion copies of <c>TPotion</c> 用于
        ///     <typeparamref name="TCharacter" />.
        ///     The target character may be registered before or after this call; resolution happens when the character model is
        ///     The target character may be 已注册 之前 或 之后 this call; resolution happens 当 the character 模型 is
        ///     queried. Matching uses the live instance CLR type; registrations against an assignable ancestor type also apply,
        ///     queried. Matching 使用 the live instance CLR type; 注册s against an assignable ancestor type also apply,
        ///     except a registration keyed only to <see cref="CharacterModel" /> itself.
        ///     except a 注册 keyed only to <c>Character模型</c> itself.
        /// </summary>
        public void RegisterCharacterStarterPotion<TCharacter, TPotion>(int count, int order)
            where TCharacter : CharacterModel
            where TPotion : PotionModel
        {
            RegisterCharacterStarterPotion(typeof(TCharacter), typeof(TPotion), count, order);
        }

        /// <summary>
        ///     Registers additional starting potion copies of <paramref name="potionType" /> for <paramref name="characterType" />
        ///     Registers additional starting potion copies of <c>potionType</c> 用于 <c>characterType</c>
        ///     .
        ///     中文说明：.
        /// </summary>
        public void RegisterCharacterStarterPotion(Type characterType, Type potionType, int count = 1)
        {
            RegisterCharacterStarterPotion(characterType, potionType, count, 0);
        }

        /// <summary>
        ///     Registers additional starting potion copies of <paramref name="potionType" /> for <paramref name="characterType" />
        ///     Registers additional starting potion copies of <c>potionType</c> 用于 <c>characterType</c>
        ///     .
        ///     中文说明：.
        /// </summary>
        public void RegisterCharacterStarterPotion(Type characterType, Type potionType, int count, int order)
        {
            RegisterCharacterStarterModel(characterType, potionType, typeof(PotionModel),
                CharacterStarterContentKind.Potion, count, order);
        }

        /// <summary>
        ///     Registers a mod act model for inclusion in <see cref="ModelDb.Acts" />.
        ///     注册 a mod act model for inclusion in <c>ModelDb.Acts</c>。
        /// </summary>
        public void RegisterAct<TAct>() where TAct : ActModel
        {
            RegisterAct(typeof(TAct));
        }

        /// <summary>
        ///     Registers <paramref name="actType" /> for inclusion in <see cref="ModelDb.Acts" />.
        ///     注册 <c>actType</c> for inclusion in <c>ModelDb.Acts</c>。
        /// </summary>
        public void RegisterAct(Type actType)
        {
            RegisterStandaloneModel(RegisteredActs, actType, typeof(ActModel), "act");
        }

        /// <summary>
        ///     Registers a mod monster model type for RitsuLib tracking, <see cref="ModelDb" /> identity, dynamic injection, and
        ///     Registers a mod monster 模型 type 用于 RitsuLib tracking, <c>ModelDb</c> identity, dynamic injection, and
        ///     patched merge into <c>ModelDb.Monsters</c>.
        ///     patched merge into <c>ModelDb.Monsters</c>.
        /// </summary>
        public void RegisterMonster<TMonster>() where TMonster : MonsterModel
        {
            RegisterMonster(typeof(TMonster));
        }

        /// <summary>
        ///     Registers <paramref name="monsterType" /> for RitsuLib tracking and patched monster injection.
        ///     注册 <c>monsterType</c> for RitsuLib tracking and patched monster injection。
        /// </summary>
        public void RegisterMonster(Type monsterType)
        {
            RegisterStandaloneModel(RegisteredMonsters, monsterType, typeof(MonsterModel), "monster");
        }

        /// <summary>
        ///     Registers a mod power model for inclusion in <see cref="ModelDb.AllPowers" />.
        ///     注册 a mod power model for inclusion in <c>ModelDb.AllPowers</c>。
        /// </summary>
        public void RegisterPower<TPower>() where TPower : PowerModel
        {
            RegisterPower(typeof(TPower));
        }

        /// <summary>
        ///     Registers <paramref name="powerType" /> for inclusion in <see cref="ModelDb.AllPowers" />.
        ///     注册 <c>powerType</c> for inclusion in <c>ModelDb.AllPowers</c>。
        /// </summary>
        public void RegisterPower(Type powerType)
        {
            RegisterStandaloneModel(RegisteredPowers, powerType, typeof(PowerModel), "power");
        }

        /// <summary>
        ///     Registers a mod orb model for inclusion in <see cref="ModelDb.Orbs" />.
        ///     注册 a mod orb model for inclusion in <c>ModelDb.Orbs</c>。
        /// </summary>
        public void RegisterOrb<TOrb>() where TOrb : OrbModel
        {
            RegisterOrb(typeof(TOrb));
        }

        /// <summary>
        ///     Registers <paramref name="orbType" /> for inclusion in <see cref="ModelDb.Orbs" />.
        ///     注册 <c>orbType</c> for inclusion in <c>ModelDb.Orbs</c>。
        /// </summary>
        public void RegisterOrb(Type orbType)
        {
            RegisterStandaloneModel(RegisteredOrbs, orbType, typeof(OrbModel), "orb");
        }

        /// <summary>
        ///     Registers a mod enchantment model for RitsuLib tracking, fixed <see cref="ModelDb" /> entry identity, dynamic
        ///     Registers a mod enchantment 模型 用于 RitsuLib tracking, fixed <c>ModelDb</c> entry identity, dynamic
        ///     injection, and inclusion in patched <see cref="ModelDb.DebugEnchantments" />.
        ///     injection, 和 inclusion in patched <c>ModelDb.DebugEnchantments</c>.
        /// </summary>
        public void RegisterEnchantment<TEnchantment>() where TEnchantment : EnchantmentModel
        {
            RegisterEnchantment(typeof(TEnchantment));
        }

        /// <summary>
        ///     Registers <paramref name="enchantmentType" /> for patched enchantment injection.
        ///     注册 <c>enchantmentType</c> for patched enchantment injection。
        /// </summary>
        public void RegisterEnchantment(Type enchantmentType)
        {
            RegisterStandaloneModel(RegisteredEnchantments, enchantmentType, typeof(EnchantmentModel),
                "enchantment");
        }

        /// <summary>
        ///     Registers a mod affliction model for RitsuLib tracking, fixed entry identity, dynamic injection, and patched
        ///     Registers a mod affliction 模型 用于 RitsuLib tracking, fixed entry identity, dynamic injection, 和 patched
        ///     <see cref="ModelDb.DebugAfflictions" />.
        /// </summary>
        public void RegisterAffliction<TAffliction>() where TAffliction : AfflictionModel
        {
            RegisterAffliction(typeof(TAffliction));
        }

        /// <summary>
        ///     Registers <paramref name="afflictionType" /> for patched affliction injection.
        ///     注册 <c>afflictionType</c> for patched affliction injection。
        /// </summary>
        public void RegisterAffliction(Type afflictionType)
        {
            RegisterStandaloneModel(RegisteredAfflictions, afflictionType, typeof(AfflictionModel), "affliction");
        }

        /// <summary>
        ///     Registers a mod achievement model for fixed entry identity, dynamic injection, and patched
        ///     Registers a mod achievement 模型 用于 fixed entry identity, dynamic injection, 和 patched
        ///     <see cref="ModelDb.Achievements" />.
        /// </summary>
        public void RegisterAchievement<TAchievement>() where TAchievement : AchievementModel
        {
            RegisterAchievement(typeof(TAchievement));
        }

        /// <summary>
        ///     Registers <paramref name="achievementType" /> for patched achievement injection.
        ///     注册 <c>achievementType</c> for patched achievement injection。
        /// </summary>
        public void RegisterAchievement(Type achievementType)
        {
            RegisterStandaloneModel(RegisteredAchievements, achievementType, typeof(AchievementModel),
                "achievement");
        }

        /// <summary>
        ///     Registers a mod singleton model for fixed entry identity and dynamic injection (resolved via
        ///     Registers a mod singleton 模型 用于 fixed entry identity 和 dynamic injection (resolved via
        ///     <see cref="ModelDb.Singleton{T}" />).
        /// </summary>
        public void RegisterSingleton<TSingleton>() where TSingleton : SingletonModel
        {
            RegisterSingleton(typeof(TSingleton));
        }

        /// <summary>
        ///     Registers <paramref name="singletonType" /> for dynamic singleton injection.
        ///     注册 <c>singletonType</c> for dynamic singleton injection。
        /// </summary>
        public void RegisterSingleton(Type singletonType)
        {
            RegisterStandaloneModel(RegisteredSingletons, singletonType, typeof(SingletonModel), "singleton");
        }

        /// <summary>
        ///     Registers a custom badge template type.
        ///     注册 a custom badge template type。
        /// </summary>
        public void RegisterBadge<TBadge>() where TBadge : ModBadgeTemplate
        {
            RegisterBadge(typeof(TBadge));
        }

        /// <summary>
        ///     Registers a custom badge template type.
        ///     注册 a custom badge template type。
        /// </summary>
        public void RegisterBadge(Type badgeType)
        {
            EnsureMutable($"register badge '{badgeType.Name}'");
            EnsureBadgeType(badgeType, nameof(badgeType));

            lock (SyncRoot)
            {
                if (!RegisteredBadges.Add(badgeType))
                {
                    _logger.Debug($"[Content] Skipping duplicate badge registration: {badgeType.Name}");
                    return;
                }

                RememberOwner(badgeType);
            }

            _logger.Info($"[Content] Registered badge: {badgeType.Name}");
        }

        /// <summary>
        ///     Registers a mod modifier as a &quot;good&quot; daily modifier for patched <see cref="ModelDb.GoodModifiers" />.
        ///     注册 a mod modifier as a &quot;good&quot; daily modifier for patched <c>ModelDb.GoodModifiers</c>。
        /// </summary>
        public void RegisterGoodModifier<TModifier>() where TModifier : ModifierModel
        {
            RegisterGoodModifier(typeof(TModifier));
        }

        /// <summary>
        ///     Registers <paramref name="modifierType" /> as a good daily modifier.
        ///     注册 <c>modifierType</c> as a good daily modifier。
        /// </summary>
        public void RegisterGoodModifier(Type modifierType)
        {
            RegisterStandaloneModel(RegisteredGoodModifiers, modifierType, typeof(ModifierModel), "good modifier");
        }

        /// <summary>
        ///     Registers a mod modifier as a &quot;bad&quot; daily modifier for patched <see cref="ModelDb.BadModifiers" />.
        ///     注册 a mod modifier as a &quot;bad&quot; daily modifier for patched <c>ModelDb.BadModifiers</c>。
        /// </summary>
        public void RegisterBadModifier<TModifier>() where TModifier : ModifierModel
        {
            RegisterBadModifier(typeof(TModifier));
        }

        /// <summary>
        ///     Registers <paramref name="modifierType" /> as a bad daily modifier.
        ///     注册 <c>modifierType</c> as a bad daily modifier。
        /// </summary>
        public void RegisterBadModifier(Type modifierType)
        {
            RegisterStandaloneModel(RegisteredBadModifiers, modifierType, typeof(ModifierModel), "bad modifier");
        }

        /// <summary>
        ///     Registers a shared card pool model for inclusion in <see cref="ModelDb.AllSharedCardPools" />.
        ///     注册 a shared card pool model for inclusion in <c>ModelDb.AllSharedCardPools</c>。
        /// </summary>
        public void RegisterSharedCardPool<TPool>() where TPool : CardPoolModel
        {
            RegisterSharedCardPool(typeof(TPool));
        }

        /// <summary>
        ///     Registers <paramref name="poolType" /> for inclusion in <see cref="ModelDb.AllSharedCardPools" />.
        ///     注册 <c>poolType</c> for inclusion in <c>ModelDb.AllSharedCardPools</c>。
        /// </summary>
        public void RegisterSharedCardPool(Type poolType)
        {
            RegisterStandaloneModel(RegisteredSharedCardPools, poolType, typeof(CardPoolModel),
                "shared card pool");
        }

        /// <summary>
        ///     Registers a shared relic pool model for inclusion in patched <see cref="ModelDb.AllRelicPools" />.
        ///     注册 a shared relic pool model for inclusion in patched <c>ModelDb.AllRelicPools</c>。
        /// </summary>
        public void RegisterSharedRelicPool<TPool>() where TPool : RelicPoolModel
        {
            RegisterSharedRelicPool(typeof(TPool));
        }

        /// <summary>
        ///     Registers <paramref name="poolType" /> for inclusion in patched <see cref="ModelDb.AllRelicPools" />.
        ///     注册 <c>poolType</c> for inclusion in patched <c>ModelDb.AllRelicPools</c>。
        /// </summary>
        public void RegisterSharedRelicPool(Type poolType)
        {
            RegisterStandaloneModel(RegisteredSharedRelicPools, poolType, typeof(RelicPoolModel),
                "shared relic pool");
        }

        /// <summary>
        ///     Registers a shared potion pool model for inclusion in patched <see cref="ModelDb.AllPotionPools" />.
        ///     注册 a shared potion pool model for inclusion in patched <c>ModelDb.AllPotionPools</c>。
        /// </summary>
        public void RegisterSharedPotionPool<TPool>() where TPool : PotionPoolModel
        {
            RegisterSharedPotionPool(typeof(TPool));
        }

        /// <summary>
        ///     Registers <paramref name="poolType" /> for inclusion in patched <see cref="ModelDb.AllPotionPools" />.
        ///     注册 <c>poolType</c> for inclusion in patched <c>ModelDb.AllPotionPools</c>。
        /// </summary>
        public void RegisterSharedPotionPool(Type poolType)
        {
            RegisterStandaloneModel(RegisteredSharedPotionPools, poolType, typeof(PotionPoolModel),
                "shared potion pool");
        }

        /// <summary>
        ///     Registers a shared event model for inclusion in shared event enumerations.
        ///     注册 a shared event model for inclusion in shared event enumerations。
        /// </summary>
        public void RegisterSharedEvent<TEvent>() where TEvent : EventModel
        {
            RegisterSharedEvent(typeof(TEvent));
        }

        /// <summary>
        ///     Registers <paramref name="eventType" /> for inclusion in shared event enumerations.
        ///     注册 <c>eventType</c> for inclusion in shared event enumerations。
        /// </summary>
        public void RegisterSharedEvent(Type eventType)
        {
            RegisterStandaloneModel(RegisteredSharedEvents, eventType, typeof(EventModel), "shared event");
        }

        /// <summary>
        ///     Registers an encounter model scoped to <typeparamref name="TAct" />.
        ///     注册 an encounter model scoped to <c>TAct</c>。
        /// </summary>
        public void RegisterActEncounter<TAct, TEncounter>()
            where TAct : ActModel
            where TEncounter : EncounterModel
        {
            RegisterActEncounter(typeof(TAct), typeof(TEncounter));
        }

        /// <summary>
        ///     Registers <paramref name="encounterType" /> scoped to <paramref name="actType" />.
        ///     注册 <c>encounterType</c> scoped to <c>actType</c>。
        /// </summary>
        public void RegisterActEncounter(Type actType, Type encounterType)
        {
            RegisterScopedModel(RegisteredActEncounters, actType, encounterType, typeof(ActModel),
                typeof(EncounterModel), "act encounter");
        }

        /// <summary>
        ///     Registers an encounter model appended to <strong>every</strong> act’s
        ///     Registers an encounter 模型 appended to <strong>every</strong> 章节’s
        ///     <see cref="ActModel.GenerateAllEncounters" /> result (after vanilla and act-scoped mod encounters).
        ///     Use for elites / monsters / bosses that should appear in multiple acts; use
        ///     使用 用于 elites / monsters / bosses that should appear in multiple 章节s; 使用
        ///     <see cref="RegisterActEncounter{TAct,TEncounter}" /> when the encounter belongs to one act only.
        /// </summary>
        public void RegisterGlobalEncounter<TEncounter>() where TEncounter : EncounterModel
        {
            RegisterGlobalEncounter(typeof(TEncounter));
        }

        /// <summary>
        ///     Registers <paramref name="encounterType" /> as a global encounter.
        ///     注册 <c>encounterType</c> as a global encounter。
        /// </summary>
        public void RegisterGlobalEncounter(Type encounterType)
        {
            RegisterStandaloneModel(RegisteredGlobalEncounters, encounterType, typeof(EncounterModel),
                "global encounter");
        }

        /// <summary>
        ///     Registers an event model scoped to <typeparamref name="TAct" />.
        ///     注册 an event model scoped to <c>TAct</c>。
        /// </summary>
        public void RegisterActEvent<TAct, TEvent>()
            where TAct : ActModel
            where TEvent : EventModel
        {
            RegisterActEvent(typeof(TAct), typeof(TEvent));
        }

        /// <summary>
        ///     Registers <paramref name="eventType" /> scoped to <paramref name="actType" />.
        ///     注册 <c>eventType</c> scoped to <c>actType</c>。
        /// </summary>
        public void RegisterActEvent(Type actType, Type eventType)
        {
            RegisterScopedModel(RegisteredActEvents, actType, eventType, typeof(ActModel), typeof(EventModel),
                "act event");
        }

        /// <summary>
        ///     Registers a shared ancient event model for inclusion in ancient enumerations.
        ///     注册 a shared ancient event model for inclusion in ancient enumerations。
        /// </summary>
        public void RegisterSharedAncient<TAncient>() where TAncient : AncientEventModel
        {
            RegisterSharedAncient(typeof(TAncient));
        }

        /// <summary>
        ///     Registers <paramref name="ancientType" /> for inclusion in ancient enumerations.
        ///     注册 <c>ancientType</c> for inclusion in ancient enumerations。
        /// </summary>
        public void RegisterSharedAncient(Type ancientType)
        {
            RegisterStandaloneModel(RegisteredSharedAncients, ancientType, typeof(AncientEventModel),
                "shared ancient");
        }

        /// <summary>
        ///     Registers an ancient event model scoped to <typeparamref name="TAct" />.
        ///     注册 an ancient event model scoped to <c>TAct</c>。
        /// </summary>
        public void RegisterActAncient<TAct, TAncient>()
            where TAct : ActModel
            where TAncient : AncientEventModel
        {
            RegisterActAncient(typeof(TAct), typeof(TAncient));
        }

        /// <summary>
        ///     Registers <paramref name="ancientType" /> scoped to <paramref name="actType" />.
        ///     注册 <c>ancientType</c> scoped to <c>actType</c>。
        /// </summary>
        public void RegisterActAncient(Type actType, Type ancientType)
        {
            RegisterScopedModel(RegisteredActAncients, actType, ancientType, typeof(ActModel),
                typeof(AncientEventModel), "act ancient");
        }

        internal static void FreezeRegistrations(string reason)
        {
            lock (SyncRoot)
            {
                if (IsFrozen)
                    return;

                IsFrozen = true;
                foreach (var registry in Registries.Values)
                    registry._freezeReason = reason;
            }

            foreach (var registry in Registries.Values)
                registry._logger.Info($"[Content] Content registration is now frozen ({reason}).");

            RitsuLibFramework.PublishLifecycleEvent(
                new ContentRegistrationClosedEvent(reason, DateTimeOffset.UtcNow),
                nameof(ContentRegistrationClosedEvent)
            );
        }

        internal static IEnumerable<CharacterModel> AppendCharacters(IEnumerable<CharacterModel> source)
        {
            return AppendResolved(source, ResolveModels<CharacterModel>(RegisteredCharacters));
        }

        internal static IEnumerable<CharacterModel> GetModCharacters()
        {
            return ResolveModels<CharacterModel>(RegisteredCharacters);
        }

        /// <summary>
        ///     Snapshot of registered model types with owner and resolved/public-entry diagnostics.
        ///     Snapshot of 已注册 模型 types 带有 owner 和 resolved/public-entry diagnostics.
        /// </summary>
        public static ModContentRegisteredTypeSnapshot[] GetRegisteredTypeSnapshots()
        {
            lock (SyncRoot)
            {
                return RegisteredTypeOwners
                    .OrderBy(kvp => kvp.Value, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(kvp => kvp.Key.FullName, StringComparer.Ordinal)
                    .Select(kvp =>
                    {
                        var modelType = kvp.Key;
                        var modId = kvp.Value;
                        var modelDbId = TryGetModelDbId(modelType);
                        var expectedPublicEntry =
                            TryGetExpectedPublicEntry(modelType, modId, out var hasExplicitOverride);
                        var typeNamePublicEntry = TryGetTypeNamePublicEntry(modelType);
                        return new ModContentRegisteredTypeSnapshot(
                            modId,
                            modelType,
                            modelDbId,
                            expectedPublicEntry,
                            hasExplicitOverride,
                            typeNamePublicEntry);
                    })
                    .ToArray();
            }

            static ModelId? TryGetModelDbId(Type modelType)
            {
                try
                {
                    return ModelDb.GetId(modelType);
                }
                catch
                {
                    return null;
                }
            }

            static string? TryGetExpectedPublicEntry(Type modelType, string modId, out bool hasExplicitOverride)
            {
                if (FixedPublicEntryOverrides.TryGetValue(modelType, out var entry))
                {
                    hasExplicitOverride = true;
                    return entry;
                }

                try
                {
                    hasExplicitOverride = false;
                    return GetFixedPublicEntry(modId, modelType);
                }
                catch
                {
                    hasExplicitOverride = false;
                    return null;
                }
            }

            static string? TryGetTypeNamePublicEntry(Type modelType)
            {
                try
                {
                    var typeStem = NormalizePublicStem(modelType.Name);
                    var categoryStem = NormalizePublicStem(ModelDb.GetCategory(modelType));
                    return $"{categoryStem}_{typeStem}";
                }
                catch
                {
                    return null;
                }
            }
        }

        internal static Type[] GetRegisteredCharacterStarterCards(Type characterType)
        {
            return GetRegisteredCharacterStarterTypes(characterType, CharacterStarterContentKind.Card);
        }

        internal static Type[] GetRegisteredCharacterStarterRelics(Type characterType)
        {
            return GetRegisteredCharacterStarterTypes(characterType, CharacterStarterContentKind.Relic);
        }

        internal static Type[] GetRegisteredCharacterStarterPotions(Type characterType)
        {
            return GetRegisteredCharacterStarterTypes(characterType, CharacterStarterContentKind.Potion);
        }

        internal static IEnumerable<EventModel> AppendSharedEvents(IEnumerable<EventModel> source)
        {
            return AppendResolved(source, ResolveModels<EventModel>(RegisteredSharedEvents));
        }

        internal static IEnumerable<ActModel> AppendActs(IEnumerable<ActModel> source)
        {
            return AppendResolved(source, ResolveModels<ActModel>(RegisteredActs));
        }

        internal static IEnumerable<PowerModel> AppendPowers(IEnumerable<PowerModel> source)
        {
            return AppendResolved(source, ResolveModels<PowerModel>(RegisteredPowers));
        }

        internal static IEnumerable<OrbModel> AppendOrbs(IEnumerable<OrbModel> source)
        {
            return AppendResolved(source, ResolveModels<OrbModel>(RegisteredOrbs));
        }

        internal static IEnumerable<EnchantmentModel> AppendEnchantments(IEnumerable<EnchantmentModel> source)
        {
            return AppendResolved(source, ResolveModels<EnchantmentModel>(RegisteredEnchantments));
        }

        internal static IEnumerable<AfflictionModel> AppendAfflictions(IEnumerable<AfflictionModel> source)
        {
            return AppendResolved(source, ResolveModels<AfflictionModel>(RegisteredAfflictions));
        }

        internal static IReadOnlyList<AchievementModel> AppendAchievements(IReadOnlyList<AchievementModel> source)
        {
            var additional = ResolveModels<AchievementModel>(RegisteredAchievements);
            return additional.Length == 0 ? source : MergeDistinctByModelId(source, additional);
        }

        internal static IReadOnlyList<ModifierModel> AppendGoodModifiers(IReadOnlyList<ModifierModel> source)
        {
            var additional = ResolveModels<ModifierModel>(RegisteredGoodModifiers);
            return additional.Length == 0 ? source : MergeDistinctByModelId(source, additional);
        }

        internal static IReadOnlyList<ModifierModel> AppendBadModifiers(IReadOnlyList<ModifierModel> source)
        {
            var additional = ResolveModels<ModifierModel>(RegisteredBadModifiers);
            return additional.Length == 0 ? source : MergeDistinctByModelId(source, additional);
        }

        internal static IEnumerable<RelicPoolModel> AppendSharedRelicPools(IEnumerable<RelicPoolModel> source)
        {
            return AppendResolved(source, ResolveModels<RelicPoolModel>(RegisteredSharedRelicPools));
        }

        internal static IEnumerable<PotionPoolModel> AppendSharedPotionPools(IEnumerable<PotionPoolModel> source)
        {
            return AppendResolved(source, ResolveModels<PotionPoolModel>(RegisteredSharedPotionPools));
        }

        internal static IEnumerable<CardPoolModel> AppendSharedCardPools(IEnumerable<CardPoolModel> source)
        {
            return AppendResolved(source, ResolveModels<CardPoolModel>(RegisteredSharedCardPools));
        }

        internal static IEnumerable<EventModel> AppendActEvents(ActModel act, IEnumerable<EventModel> source)
        {
            return AppendResolved(source, ResolveScopedModels<EventModel>(RegisteredActEvents, act.GetType()));
        }

        internal static IEnumerable<EncounterModel> AppendActEncounters(ActModel act,
            IEnumerable<EncounterModel> source)
        {
            return AppendResolved(source, ResolveScopedModels<EncounterModel>(RegisteredActEncounters, act.GetType()));
        }

        internal static IEnumerable<EncounterModel> AppendGlobalEncounters(IEnumerable<EncounterModel> source)
        {
            return AppendResolved(source, ResolveModels<EncounterModel>(RegisteredGlobalEncounters));
        }

        internal static IEnumerable<MonsterModel> AppendRegisteredMonsters(IEnumerable<MonsterModel> source)
        {
            var additional = ResolveModels<MonsterModel>(RegisteredMonsters);
            return MergeDistinctByModelId(source, additional);
        }

        internal static IEnumerable<AncientEventModel> AppendSharedAncients(IEnumerable<AncientEventModel> source)
        {
            return AppendResolved(source, ResolveModels<AncientEventModel>(RegisteredSharedAncients));
        }

        internal static IEnumerable<AncientEventModel> AppendActAncients(ActModel act,
            IEnumerable<AncientEventModel> source)
        {
            return AppendResolved(source, ResolveScopedModels<AncientEventModel>(RegisteredActAncients, act.GetType()));
        }

        internal static Type[] GetRegisteredBadgeTypes()
        {
            lock (SyncRoot)
            {
                return RegisteredBadges
                    .OrderBy(static t => t.FullName ?? t.Name, StringComparer.Ordinal)
                    .ToArray();
            }
        }

        /// <summary>
        ///     Injects RitsuLib-registered types that live in <see cref="Assembly.IsDynamic" /> assemblies into
        ///     Injects RitsuLib-已注册 types that live in <c>Assembly.IsDynamic</c> assemblies into
        ///     <see cref="ModelDb" /> before <c>Init</c> finishes populating <c>_contentById</c>. Static mod DLL types are
        ///     picked up by the game's subtype scan; Reflection.Emit placeholder types are not, so they must be injected here.
        ///     picked up 通过 the game's subtype scan; Reflection.Emit placeholder types are not, so they must be injected here.
        /// </summary>
        internal static void InjectDynamicRegisteredModels()
        {
            Type[] typesToInject;

            lock (SyncRoot)
            {
                typesToInject = RegisteredPoolContent
                    .SelectMany(static entry => new[] { entry.PoolType, entry.ModelType })
                    .Concat(RegisteredCharacters)
                    .Concat(RegisteredActs)
                    .Concat(RegisteredMonsters)
                    .Concat(RegisteredPowers)
                    .Concat(RegisteredOrbs)
                    .Concat(RegisteredEnchantments)
                    .Concat(RegisteredAfflictions)
                    .Concat(RegisteredAchievements)
                    .Concat(RegisteredSingletons)
                    .Concat(RegisteredSharedCardPools)
                    .Concat(RegisteredSharedRelicPools)
                    .Concat(RegisteredSharedPotionPools)
                    .Concat(RegisteredGoodModifiers)
                    .Concat(RegisteredBadModifiers)
                    .Concat(RegisteredSharedEvents)
                    .Concat(RegisteredSharedAncients)
                    .Concat(RegisteredActEncounters.Values.SelectMany(static set => set))
                    .Concat(RegisteredGlobalEncounters)
                    .Concat(RegisteredActEvents.Values.SelectMany(static set => set))
                    .Concat(RegisteredActAncients.Values.SelectMany(static set => set))
                    .Distinct()
                    .Where(static t => t.Assembly.IsDynamic)
                    .OrderBy(static t => t.FullName ?? t.Name, StringComparer.Ordinal)
                    .ToArray();
            }

            foreach (var type in typesToInject)
                ModelDb.Inject(type);
        }

        private void RegisterPoolModel(Type poolType, Type modelType, string contentKind,
            ModelPublicEntryOptions publicEntry = default)
        {
            EnsureMutable($"register {contentKind} '{modelType.Name}' into pool '{poolType.Name}'");
            EnsureModelType(poolType, typeof(AbstractModel), nameof(poolType));
            EnsureModelType(modelType, typeof(AbstractModel), nameof(modelType));
            PrimeOwnedType(modelType);
            ApplyFixedPublicEntryForModel(modelType, publicEntry);
            RegistrationConflictDetector.ThrowIfModelIdConflicts(poolType);
            RegistrationConflictDetector.ThrowIfModelIdConflicts(modelType);

            lock (SyncRoot)
            {
                if (!RegisteredPoolContent.Add((poolType, modelType)))
                {
                    _logger.Debug(
                        $"[Content] Skipping duplicate {contentKind} registration: {modelType.Name} -> {poolType.Name}");
                    return;
                }

                RememberOwner(modelType);
            }

            ModHelper.AddModelToPool(poolType, modelType);
            _logger.Info($"[Content] Registered {contentKind}: {modelType.Name} -> {poolType.Name}");
        }

        private void RegisterCharacterStarterModel(Type characterType, Type modelType, Type expectedModelBaseType,
            CharacterStarterContentKind kind, int count, int order)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Starter content count must be positive.");

            EnsureMutable(
                $"register starter {kind.ToString().ToLowerInvariant()} '{modelType.Name}' for '{characterType.Name}'");
            EnsureModelType(characterType, typeof(CharacterModel), nameof(characterType));
            EnsureModelType(modelType, expectedModelBaseType, nameof(modelType));
            RegistrationConflictDetector.ThrowIfModelIdConflicts(characterType);
            RegistrationConflictDetector.ThrowIfModelIdConflicts(modelType);

            lock (SyncRoot)
            {
                RegisteredCharacterStarterContent.Add(new(characterType, modelType, kind, count, order));
            }

            _logger.Info(
                $"[Content] Registered starter {kind.ToString().ToLowerInvariant()}: {modelType.Name} x{count} -> {characterType.Name}");
        }

        private void RegisterStandaloneModel(
            HashSet<Type> registry,
            Type modelType,
            Type expectedBaseType,
            string contentKind)
        {
            EnsureMutable($"register {contentKind} '{modelType.Name}'");
            EnsureModelType(modelType, expectedBaseType, nameof(modelType));
            PrimeOwnedType(modelType);
            RegistrationConflictDetector.ThrowIfModelIdConflicts(modelType);

            lock (SyncRoot)
            {
                if (!registry.Add(modelType))
                {
                    _logger.Debug($"[Content] Skipping duplicate {contentKind} registration: {modelType.Name}");
                    return;
                }

                RememberOwner(modelType);
            }

            _logger.Info($"[Content] Registered {contentKind}: {modelType.Name}");
        }

        private void RegisterScopedModel(
            Dictionary<Type, HashSet<Type>> registry,
            Type scopeType,
            Type modelType,
            Type expectedScopeType,
            Type expectedModelBaseType,
            string contentKind)
        {
            EnsureMutable($"register {contentKind} '{modelType.Name}' for '{scopeType.Name}'");
            EnsureModelType(scopeType, expectedScopeType, nameof(scopeType));
            EnsureModelType(modelType, expectedModelBaseType, nameof(modelType));
            PrimeOwnedType(modelType);
            RegistrationConflictDetector.ThrowIfModelIdConflicts(scopeType);
            RegistrationConflictDetector.ThrowIfModelIdConflicts(modelType);

            lock (SyncRoot)
            {
                if (!registry.TryGetValue(scopeType, out var entries))
                {
                    entries = [];
                    registry[scopeType] = entries;
                }

                if (!entries.Add(modelType))
                {
                    _logger.Debug(
                        $"[Content] Skipping duplicate {contentKind} registration: {modelType.Name} -> {scopeType.Name}");
                    return;
                }

                RememberOwner(modelType);
            }

            _logger.Info($"[Content] Registered {contentKind}: {modelType.Name} -> {scopeType.Name}");
        }

        private void EnsureMutable(string operation)
        {
            if (!IsFrozen)
                return;

            throw new InvalidOperationException(
                $"Cannot {operation} after content registration has been frozen ({_freezeReason ?? "unknown"}). " +
                "Register content from your mod initializer before the game initializes ModelDb.");
        }

        private static void EnsureModelType(Type type, Type expectedBaseType, string paramName)
        {
            if (type.IsAbstract || type.IsInterface || !expectedBaseType.IsAssignableFrom(type))
                throw new ArgumentException(
                    $"Type '{type.FullName}' must be a concrete subtype of '{expectedBaseType.FullName}'.",
                    paramName
                );
        }

        private static void EnsureBadgeType(Type type, string paramName)
        {
            if (type.IsAbstract || type.IsInterface || !typeof(ModBadgeTemplate).IsAssignableFrom(type))
                throw new ArgumentException(
                    $"Type '{type.FullName}' must be a concrete subtype of '{typeof(ModBadgeTemplate).FullName}'.",
                    paramName
                );
        }

        private static TModel[] ResolveModels<TModel>(IEnumerable<Type> modelTypes)
            where TModel : AbstractModel
        {
            lock (SyncRoot)
            {
                return modelTypes
                    .OrderBy(static t => t.FullName ?? t.Name, StringComparer.Ordinal)
                    .Select(ModelDb.GetId)
                    .Select(ModelDb.GetById<TModel>)
                    .ToArray();
            }
        }

        private static TModel[] ResolveScopedModels<TModel>(Dictionary<Type, HashSet<Type>> registry,
            Type scopeType)
            where TModel : AbstractModel
        {
            lock (SyncRoot)
            {
                return !registry.TryGetValue(scopeType, out var modelTypes)
                    ? []
                    : modelTypes
                        .OrderBy(static t => t.FullName ?? t.Name, StringComparer.Ordinal)
                        .Select(ModelDb.GetId)
                        .Select(ModelDb.GetById<TModel>)
                        .ToArray();
            }
        }

        private static bool MatchesRegisteredStarterCharacter(Type registeredCharacterType, Type runtimeCharacterType)
        {
            if (registeredCharacterType == runtimeCharacterType)
                return true;

            if (!registeredCharacterType.IsAssignableFrom(runtimeCharacterType))
                return false;

            return registeredCharacterType != typeof(CharacterModel);
        }

        private static Type[] GetRegisteredCharacterStarterTypes(Type characterType, CharacterStarterContentKind kind)
        {
            ArgumentNullException.ThrowIfNull(characterType);

            lock (SyncRoot)
            {
                return RegisteredCharacterStarterContent
                    .Select(static (entry, index) => new { entry, index })
                    .OrderBy(static x => x.entry.Order)
                    .ThenBy(static x => x.index)
                    .Where(x => x.entry.Kind == kind && MatchesRegisteredStarterCharacter(x.entry.CharacterType,
                        characterType))
                    .SelectMany(static x => Enumerable.Repeat(x.entry.ModelType, x.entry.Count))
                    .ToArray();
            }
        }

        private static TModel[] AppendResolved<TModel>(IEnumerable<TModel> source,
            IEnumerable<TModel> additional)
            where TModel : AbstractModel
        {
            return source.Concat(additional).DistinctBy(static model => model.Id).ToArray();
        }

        private static List<TModel> MergeDistinctByModelId<TModel>(IEnumerable<TModel> first,
            IEnumerable<TModel> second)
            where TModel : AbstractModel
        {
            return first.Concat(second).DistinctBy(static m => m.Id).ToList();
        }

        /// <summary>
        ///     Normalizes a public id segment: non-alphanumeric collapsed to underscores, acronym/camel boundaries
        ///     中文说明：Normalizes a public id segment: non-alphanumeric collapsed to underscores, acronym/camel boundaries
        ///     split, repeated underscores merged, and final uppercase.
        ///     split, repeated underscores merged, 和 final uppercase.
        /// </summary>
        public static string NormalizePublicStem(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var normalized = NonAlphaNumericRegex().Replace(value.Trim(), "_");
            normalized = AcronymBoundaryRegex().Replace(normalized, "$1_$2");
            normalized = CamelBoundaryRegex().Replace(normalized, "$1_$2");
            normalized = RepeatedUnderscoreRegex().Replace(normalized, "_");
            return normalized.Trim('_').ToUpperInvariant();
        }

        private static string NormalizeFullPublicEntry(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var normalized = NonAlphaNumericRegex().Replace(value.Trim(), "_");
            normalized = AcronymBoundaryRegex().Replace(normalized, "$1_$2");
            normalized = CamelBoundaryRegex().Replace(normalized, "$1_$2");
            normalized = RepeatedUnderscoreRegex().Replace(normalized, "_");
            return normalized.Trim('_').ToUpperInvariant();
        }

        private void ApplyFixedPublicEntryForModel(Type modelType, ModelPublicEntryOptions options)
        {
            if (options.Kind == ModelPublicEntryKind.FromTypeName)
                return;

            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            var resolved = options.Kind switch
            {
                ModelPublicEntryKind.Stem =>
                    $"{NormalizePublicStem(ModId)}_{NormalizePublicStem(ModelDb.GetCategory(modelType))}_{NormalizePublicStem(options.Value!)}",
                ModelPublicEntryKind.FullEntry => NormalizeFullPublicEntry(options.Value!),
                _ => throw new ArgumentOutOfRangeException(nameof(options), options.Kind, null),
            };

            lock (SyncRoot)
            {
                if (FixedPublicEntryOverrides.TryGetValue(modelType, out var existing))
                {
                    if (!string.Equals(existing, resolved, StringComparison.Ordinal))
                        throw new InvalidOperationException(
                            $"Cannot change fixed public entry for '{modelType.FullName}' from '{existing}' to '{resolved}'.");

                    return;
                }

                FixedPublicEntryOverrides[modelType] = resolved;
            }
        }

        [GeneratedRegex("[^A-Za-z0-9]+")]
        private static partial Regex NonAlphaNumericRegex();

        [GeneratedRegex("([A-Z]+)([A-Z][a-z])")]
        private static partial Regex AcronymBoundaryRegex();

        [GeneratedRegex("([a-z0-9])([A-Z])")]
        private static partial Regex CamelBoundaryRegex();

        [GeneratedRegex("_+")]
        private static partial Regex RepeatedUnderscoreRegex();

        private void RememberOwner(Type type)
        {
            RegisteredTypeOwners[type] = ModId;
        }

        private void PrimeOwnedType(Type type)
        {
            lock (SyncRoot)
            {
                RegisteredTypeOwners[type] = ModId;
            }
        }

        private enum CharacterStarterContentKind
        {
            Card,
            Relic,
            Potion,
        }

        /// <summary>
        ///     Immutable snapshot row describing one registered model type and its identity metadata.
        ///     Immutable snapshot row describing one 已注册 模型 type 和 its identity metadata.
        /// </summary>
        public readonly record struct ModContentRegisteredTypeSnapshot
        {
            /// <summary>
            ///     Creates a registered-type snapshot row.
            ///     创建 a registered-type snapshot row。
            /// </summary>
            public ModContentRegisteredTypeSnapshot(
                string modId,
                Type modelType,
                ModelId? modelDbId,
                string? expectedPublicEntry,
                bool hasExplicitPublicEntryOverride,
                string? typeNamePublicEntry)
            {
                ModId = modId;
                ModelType = modelType;
                ModelDbId = modelDbId;
                ExpectedPublicEntry = expectedPublicEntry;
                HasExplicitPublicEntryOverride = hasExplicitPublicEntryOverride;
                TypeNamePublicEntry = typeNamePublicEntry;
            }

            /// <summary>
            ///     Owning mod id recorded at registration time.
            ///     Owning mod id recorded at 注册 time.
            /// </summary>
            public string ModId { get; }

            /// <summary>
            ///     Registered model CLR type.
            ///     Registered 模型 CLR type.
            /// </summary>
            public Type ModelType { get; }

            /// <summary>
            ///     Resolved runtime <c>ModelDb</c> id, if currently available.
            ///     resolved runtime <c>ModelDb</c> id, 如果 currently 可用.
            /// </summary>
            public ModelId? ModelDbId { get; }

            /// <summary>
            ///     Expected fixed public entry for this model under current registry rules.
            ///     Expected fixed public entry 用于 this 模型 under current 注册表 rules.
            /// </summary>
            public string? ExpectedPublicEntry { get; }

            /// <summary>
            ///     Whether the expected entry comes from an explicit override.
            ///     表示是否 the expected entry comes from an explicit override。
            /// </summary>
            public bool HasExplicitPublicEntryOverride { get; }

            /// <summary>
            ///     Type-name-derived public entry (<c>CATEGORY_TYPENAME</c>) when resolvable.
            ///     Type-name-derived public entry (<c>CATEGORY_TYPENAME</c>) 当 resolvable.
            /// </summary>
            public string? TypeNamePublicEntry { get; }
        }

        private readonly record struct CharacterStarterRegistration(
            Type CharacterType,
            Type ModelType,
            CharacterStarterContentKind Kind,
            int Count,
            int Order);
    }
}
