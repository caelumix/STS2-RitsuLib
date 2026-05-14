using MegaCrit.Sts2.Core.Timeline;
using STS2RitsuLib.Keywords;

namespace STS2RitsuLib.Interop.AutoRegistration
{
    /// <summary>
    ///     Base metadata for declarative registrations discovered by the ritsulib auto-registration pipeline.
    ///     ritsulib 自动注册管线发现的声明式注册基础元数据。
    /// </summary>
    public abstract class AutoRegistrationAttribute : Attribute
    {
        /// <summary>
        ///     Local ordering within the same registration phase. Lower values run first.
        ///     同一注册阶段内的局部排序。数值越小越先运行。
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        ///     When true on an attribute declared on a base type, the same registration is applied to concrete
        ///     derived types (duplicate signatures from direct declarations on the derived type are skipped).
        ///     当基类上声明的 attribute 将此项设为 true 时，同一注册会应用到具体派生类型
        ///     （派生类型直接声明产生的重复签名会被跳过）。
        /// </summary>
        public bool Inherit { get; set; }
    }

    /// <summary>
    ///     Base metadata for content registrations dispatched through <c>ModContentRegistry</c>.
    ///     通过 <c>ModContentRegistry</c> 分发的内容注册基础元数据。
    /// </summary>
    public abstract class ContentRegistrationAttribute : AutoRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as a character model.
    ///     将带注解的类型注册为 character model。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterCharacterAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as an act model.
    ///     将带注解的类型注册为 act model。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterActAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as a monster model.
    ///     将带注解的类型注册为 monster model。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterMonsterAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as a power model.
    ///     将带注解的类型注册为 power model。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterPowerAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as an orb model.
    ///     将带注解的类型注册为 orb model。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterOrbAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as an enchantment model.
    ///     将带注解的类型注册为 enchantment model。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterEnchantmentAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as an affliction model.
    ///     将带注解的类型注册为 affliction model。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterAfflictionAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as an achievement model.
    ///     将带注解的类型注册为 achievement model。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterAchievementAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as a singleton model.
    ///     将带注解的类型注册为 singleton model。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterSingletonAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as a good daily modifier.
    ///     将带注解的类型注册为 good daily modifier。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterGoodModifierAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as a bad daily modifier.
    ///     将带注解的类型注册为 bad daily modifier。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterBadModifierAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as a shared card pool.
    ///     将带注解的类型注册为 shared card pool。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterSharedCardPoolAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as a shared relic pool.
    ///     将带注解的类型注册为 shared relic pool。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterSharedRelicPoolAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as a shared potion pool.
    ///     将带注解的类型注册为 shared potion pool。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterSharedPotionPoolAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as a shared event.
    ///     将带注解的类型注册为 shared event。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterSharedEventAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as a shared ancient event.
    ///     将带注解的类型注册为 shared ancient event。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterSharedAncientAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as a global encounter.
    ///     将带注解的类型注册为 global encounter。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterGlobalEncounterAttribute : ContentRegistrationAttribute;

    /// <summary>
    ///     Base metadata for pool-backed model registrations that can override fixed public entry generation.
    ///     可覆盖固定 public entry 生成的 pool-backed model 注册基础元数据。
    /// </summary>
    /// <param name="poolType">
    ///     Target pool model type.
    ///     目标 pool model 类型。
    /// </param>
    public abstract class ModelPublicEntryRegistrationAttributeBase(Type poolType) : ContentRegistrationAttribute
    {
        /// <summary>
        ///     Target pool model type.
        ///     目标 pool model 类型。
        /// </summary>
        public Type PoolType { get; } = poolType;

        /// <summary>
        ///     Optional stable author-chosen type-name stem.
        ///     作者可选指定的稳定类型名 stem。
        /// </summary>
        public string? StableEntryStem { get; set; }

        /// <summary>
        ///     Optional full fixed public entry override.
        ///     可选的完整固定 public entry 覆盖。
        /// </summary>
        public string? FullPublicEntry { get; set; }
    }

    /// <summary>
    ///     Registers the annotated type as a card in the given pool.
    ///     将带注解的类型注册为给定 pool 中的 card。
    /// </summary>
    /// <param name="poolType">
    ///     Target card pool type.
    ///     目标 card pool 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterCardAttribute(Type poolType) : ModelPublicEntryRegistrationAttributeBase(poolType);

    /// <summary>
    ///     Registers the annotated type as a relic in the given pool.
    ///     将带注解的类型注册为给定 pool 中的 relic。
    /// </summary>
    /// <param name="poolType">
    ///     Target relic pool type.
    ///     目标 relic pool 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterRelicAttribute(Type poolType) : ModelPublicEntryRegistrationAttributeBase(poolType);

    /// <summary>
    ///     Registers the annotated type as a potion in the given pool.
    ///     将带注解的类型注册为给定 pool 中的 potion。
    /// </summary>
    /// <param name="poolType">
    ///     Target potion pool type.
    ///     目标 potion pool 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterPotionAttribute(Type poolType) : ModelPublicEntryRegistrationAttributeBase(poolType);

    /// <summary>
    ///     Base metadata for character starter-content registrations.
    ///     角色初始内容注册的基础元数据。
    /// </summary>
    /// <param name="characterType">
    ///     Target character model type.
    ///     目标 character model 类型。
    /// </param>
    /// <param name="count">
    ///     How many copies to register.
    ///     要注册的份数。
    /// </param>
    public abstract class CharacterStarterRegistrationAttributeBase(Type characterType, int count = 1)
        : ContentRegistrationAttribute
    {
        /// <summary>
        ///     Target character model type.
        ///     目标 character model 类型。
        /// </summary>
        public Type CharacterType { get; } = characterType;

        /// <summary>
        ///     How many copies to register.
        ///     要注册的份数。
        /// </summary>
        public int Count { get; } = count;
    }

    /// <summary>
    ///     Registers the annotated card type as starter content for a character.
    ///     将带注解的 card 类型注册为某个角色的初始内容。
    /// </summary>
    /// <param name="characterType">
    ///     Target character model type.
    ///     目标 character model 类型。
    /// </param>
    /// <param name="count">
    ///     How many copies to register.
    ///     要注册的份数。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterCharacterStarterCardAttribute(Type characterType, int count = 1)
        : CharacterStarterRegistrationAttributeBase(characterType, count);

    /// <summary>
    ///     Registers the annotated relic type as starter content for a character.
    ///     将带注解的 relic 类型注册为某个角色的初始内容。
    /// </summary>
    /// <param name="characterType">
    ///     Target character model type.
    ///     目标 character model 类型。
    /// </param>
    /// <param name="count">
    ///     How many copies to register.
    ///     要注册的份数。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterCharacterStarterRelicAttribute(Type characterType, int count = 1)
        : CharacterStarterRegistrationAttributeBase(characterType, count);

    /// <summary>
    ///     Registers the annotated potion type as starter content for a character.
    ///     将带注解的 potion 类型注册为某个角色的初始内容。
    /// </summary>
    /// <param name="characterType">
    ///     Target character model type.
    ///     目标 character model 类型。
    /// </param>
    /// <param name="count">
    ///     How many copies to register.
    ///     要注册的份数。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterCharacterStarterPotionAttribute(Type characterType, int count = 1)
        : CharacterStarterRegistrationAttributeBase(characterType, count);

    /// <summary>
    ///     Base metadata for act-scoped registrations.
    ///     act 范围注册的基础元数据。
    /// </summary>
    /// <param name="actType">
    ///     Target act model type.
    ///     目标 act model 类型。
    /// </param>
    public abstract class ActScopedRegistrationAttributeBase(Type actType) : ContentRegistrationAttribute
    {
        /// <summary>
        ///     Target act model type.
        ///     目标 act model 类型。
        /// </summary>
        public Type ActType { get; } = actType;
    }

    /// <summary>
    ///     Registers the annotated encounter type for the given act.
    ///     将带注解的 encounter 类型注册到给定 act。
    /// </summary>
    /// <param name="actType">
    ///     Target act model type.
    ///     目标 act model 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterActEncounterAttribute(Type actType) : ActScopedRegistrationAttributeBase(actType);

    /// <summary>
    ///     Registers the annotated event type for the given act.
    ///     将带注解的 event 类型注册到给定 act。
    /// </summary>
    /// <param name="actType">
    ///     Target act model type.
    ///     目标 act model 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterActEventAttribute(Type actType) : ActScopedRegistrationAttributeBase(actType);

    /// <summary>
    ///     Registers the annotated ancient type for the given act.
    ///     将带注解的 ancient 类型注册到给定 act。
    /// </summary>
    /// <param name="actType">
    ///     Target act model type.
    ///     目标 act model 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterActAncientAttribute(Type actType) : ActScopedRegistrationAttributeBase(actType);

    /// <summary>
    ///     Base metadata for owned keyword registrations.
    ///     owned keyword 注册的基础元数据。
    /// </summary>
    /// <param name="localKeywordStem">
    ///     Local mod-scoped keyword stem.
    ///     mod 局部范围内的 keyword stem。
    /// </param>
    public abstract class KeywordRegistrationAttributeBase(string localKeywordStem) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Local mod-scoped keyword stem.
        ///     mod 局部范围内的 keyword stem。
        /// </summary>
        public string LocalKeywordStem { get; } = localKeywordStem;

        /// <summary>
        ///     Localization table containing the title key.
        ///     包含 title key 的本地化表。
        /// </summary>
        public string TitleTable { get; set; } = "card_keywords";

        /// <summary>
        ///     Optional explicit localization key for the title.
        ///     title 使用的可选显式本地化 key。
        /// </summary>
        public string? TitleKey { get; set; }

        /// <summary>
        ///     Optional localization table containing the description key.
        ///     包含 description key 的可选本地化表。
        /// </summary>
        public string? DescriptionTable { get; set; }

        /// <summary>
        ///     Optional explicit localization key for the description.
        ///     description 使用的可选显式本地化 key。
        /// </summary>
        public string? DescriptionKey { get; set; }

        /// <summary>
        ///     Optional icon path used by hover-tip rendering.
        ///     hover-tip 渲染使用的可选图标路径。
        /// </summary>
        public string? IconPath { get; set; }
    }

    /// <summary>
    ///     Registers an owned keyword definition.
    ///     注册一个 owned keyword 定义。
    /// </summary>
    /// <param name="localKeywordStem">
    ///     Local mod-scoped keyword stem.
    ///     mod 局部范围内的 keyword stem。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterOwnedKeywordAttribute(string localKeywordStem)
        : KeywordRegistrationAttributeBase(localKeywordStem)
    {
        /// <summary>
        ///     Optional placement for inline card-description injection.
        ///     inline card-description 注入使用的可选位置。
        /// </summary>
        public ModKeywordCardDescriptionPlacement CardDescriptionPlacement { get; set; } =
            ModKeywordCardDescriptionPlacement.None;

        /// <summary>
        ///     Whether the keyword should appear in card hover tips.
        ///     此 keyword 是否应显示在 card hover tip 中。
        /// </summary>
        public bool IncludeInCardHoverTip { get; set; } = true;
    }

    /// <summary>
    ///     Registers an owned card keyword definition using the card-keyword localization convention.
    ///     使用 card-keyword 本地化约定注册一个 owned card keyword 定义。
    /// </summary>
    /// <param name="localKeywordStem">
    ///     Local mod-scoped keyword stem.
    ///     mod 局部范围内的 keyword stem。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterOwnedCardKeywordAttribute(string localKeywordStem)
        : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Local mod-scoped keyword stem.
        ///     mod 局部范围内的 keyword stem。
        /// </summary>
        public string LocalKeywordStem { get; } = localKeywordStem;

        /// <summary>
        ///     Optional icon path used by hover-tip rendering.
        ///     hover-tip 渲染使用的可选图标路径。
        /// </summary>
        public string? IconPath { get; set; }

        /// <summary>
        ///     Optional placement for inline card-description injection.
        ///     inline card-description 注入使用的可选位置。
        /// </summary>
        public ModKeywordCardDescriptionPlacement CardDescriptionPlacement { get; set; } =
            ModKeywordCardDescriptionPlacement.None;

        /// <summary>
        ///     Whether the keyword should appear in card hover tips.
        ///     此 keyword 是否应显示在 card hover tip 中。
        /// </summary>
        public bool IncludeInCardHoverTip { get; set; } = true;
    }

    /// <summary>
    ///     Registers an owned custom <see cref="MegaCrit.Sts2.Core.Entities.Cards.CardTag" /> id for this mod assembly.
    ///     为此 mod assembly 注册一个 owned custom <c>MegaCrit.Sts2.Core.Entities.Cards.CardTag</c> id。
    /// </summary>
    /// <param name="localCardTagStem">
    ///     Local stem; combined with the mod id via <c>GetQualifiedCardTagId</c>.
    ///     本地 stem；会通过 <c>GetQualifiedCardTagId</c> 与 mod id 组合。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterOwnedCardTagAttribute(string localCardTagStem) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Local mod-scoped card-tag stem.
        ///     mod 局部范围内的 card-tag stem。
        /// </summary>
        public string LocalCardTagStem { get; } = localCardTagStem;
    }

    /// <summary>
    ///     Registers the annotated type as a timeline epoch.
    ///     将带注解的类型注册为 timeline epoch。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterEpochAttribute : AutoRegistrationAttribute;

    /// <summary>
    ///     Registers the annotated type as a timeline story.
    ///     将带注解的类型注册为 timeline story。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterStoryAttribute : AutoRegistrationAttribute;

    /// <summary>
    ///     Binds the annotated epoch type into the given story column.
    ///     将带注解的 epoch 类型绑定到给定 story column。
    /// </summary>
    /// <param name="storyType">
    ///     Target story model type.
    ///     目标 story model 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterStoryEpochAttribute(Type storyType) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Target story model type.
        ///     目标 story model 类型。
        /// </summary>
        public Type StoryType { get; } = storyType;
    }

    /// <summary>
    ///     Places the annotated mod epoch into the first free slot in the given era column.
    ///     将带注解的 mod epoch 放入给定 era column 中第一个空 slot。
    /// </summary>
    /// <param name="era">
    ///     Target era column.
    ///     目标 era column。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class AutoTimelineSlotAttribute(EpochEra era) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Target era column.
        ///     目标 era column。
        /// </summary>
        public EpochEra Era { get; } = era;
    }

    /// <summary>
    ///     Places the annotated mod epoch into the nearest free column strictly before the given anchor era.
    ///     将带注解的 mod epoch 放入严格位于给定 anchor era 之前的最近空 column。
    /// </summary>
    /// <param name="anchorEra">
    ///     Anchor era whose left side should receive the epoch.
    ///     epoch 应放在其左侧的 anchor era。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class AutoTimelineSlotBeforeColumnAttribute(EpochEra anchorEra) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Anchor era whose left side should receive the epoch.
        ///     epoch 应放在其左侧的 anchor era。
        /// </summary>
        public EpochEra AnchorEra { get; } = anchorEra;
    }

    /// <summary>
    ///     Places the annotated mod epoch into the nearest free column strictly before the column of the reference epoch.
    ///     将带注解的 mod epoch 放入严格位于 reference epoch 所在 column 之前的最近空 column。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class AutoTimelineSlotBeforeEpochColumnAttribute(Type referenceEpochType) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Reference epoch whose column anchors the placement.
        ///     作为放置锚点的 reference epoch。
        /// </summary>
        public Type ReferenceEpochType { get; } = referenceEpochType;
    }

    /// <summary>
    ///     Places the annotated mod epoch into the nearest free column strictly after the given anchor era.
    ///     将带注解的 mod epoch 放入严格位于给定 anchor era 之后的最近空 column。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class AutoTimelineSlotAfterColumnAttribute(EpochEra anchorEra) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Anchor era whose right side should receive the epoch.
        ///     epoch 应放在其右侧的 anchor era。
        /// </summary>
        public EpochEra AnchorEra { get; } = anchorEra;
    }

    /// <summary>
    ///     Places the annotated mod epoch into the nearest free column strictly after the column of the reference epoch.
    ///     将带注解的 mod epoch 放入严格位于 reference epoch 所在 column 之后的最近空 column。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class AutoTimelineSlotAfterEpochColumnAttribute(Type referenceEpochType) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Reference epoch whose column anchors the placement.
        ///     作为放置锚点的 reference epoch。
        /// </summary>
        public Type ReferenceEpochType { get; } = referenceEpochType;
    }

    /// <summary>
    ///     Places the annotated mod epoch into the same era column as the given anchor era.
    ///     将带注解的 mod epoch 放入与给定 anchor era 相同的 era column。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class AutoTimelineSlotInColumnAttribute(EpochEra anchorEra) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Anchor era whose column should be shared.
        ///     要共享其 column 的 anchor era。
        /// </summary>
        public EpochEra AnchorEra { get; } = anchorEra;
    }

    /// <summary>
    ///     Places the annotated mod epoch into the same era column as the reference epoch.
    ///     将带注解的 mod epoch 放入与 reference epoch 相同的 era column。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class AutoTimelineSlotInEpochColumnAttribute(Type referenceEpochType) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Reference epoch whose column should be shared.
        ///     要共享其 column 的 reference epoch。
        /// </summary>
        public Type ReferenceEpochType { get; } = referenceEpochType;
    }

    /// <summary>
    ///     Registers an ArchaicTooth transcendence mapping from the annotated starter card type to the given ancient card.
    ///     注册从带注解的 starter card 类型到给定 ancient card 的 ArchaicTooth transcendence 映射。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterArchaicToothTranscendenceAttribute(Type ancientCardType) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Ancient card type produced by transcendence.
        ///     transcendence 生成的 ancient card 类型。
        /// </summary>
        public Type AncientCardType { get; } = ancientCardType;
    }

    /// <summary>
    ///     Registers a TouchOfOrobas refinement mapping from the annotated starter relic type to the given upgraded relic.
    ///     注册从带注解的 starter relic 类型到给定 upgraded relic 的 TouchOfOrobas refinement 映射。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterTouchOfOrobasRefinementAttribute(Type upgradedRelicType) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Upgraded relic type produced by refinement.
        ///     refinement 生成的 upgraded relic 类型。
        /// </summary>
        public Type UpgradedRelicType { get; } = upgradedRelicType;
    }

    /// <summary>
    ///     Registers explicit card unlock content for the annotated epoch and gates those cards behind it.
    ///     为带注解的 epoch 注册显式 card unlock content，并将这些 card gated 在它之后。
    /// </summary>
    /// <param name="cardTypes">
    ///     Card model types revealed by the epoch.
    ///     此 epoch 解锁显示的 card model 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterEpochCardsAttribute(params Type[] cardTypes) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Card model types revealed by the epoch.
        ///     此 epoch 解锁显示的 card model 类型。
        /// </summary>
        public IReadOnlyList<Type> CardTypes { get; } = cardTypes;
    }

    /// <summary>
    ///     Gates every registered card in the given pool behind the annotated epoch.
    ///     将给定 pool 中的每张已注册 card gated 在带注解的 epoch 之后。
    /// </summary>
    /// <param name="poolType">
    ///     Card pool model type.
    ///     card pool model 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireAllCardsInPoolAttribute(Type poolType) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Card pool model type.
        ///     card pool model 类型。
        /// </summary>
        public Type PoolType { get; } = poolType;
    }

    /// <summary>
    ///     Registers every relic in the given pool as unlock content for the annotated epoch and gates them behind it.
    ///     将给定 pool 中的每个 relic 注册为带注解 epoch 的 unlock content，并 gated 在它之后。
    /// </summary>
    /// <param name="poolType">
    ///     Relic pool model type.
    ///     relic pool model 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterEpochRelicsFromPoolAttribute(Type poolType) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Relic pool model type.
        ///     relic pool model 类型。
        /// </summary>
        public Type PoolType { get; } = poolType;
    }

    /// <summary>
    ///     Gates the annotated content type behind the given epoch.
    ///     将带注解的 content 类型 gated 在给定 epoch 之后。
    /// </summary>
    /// <param name="epochType">
    ///     Required epoch type.
    ///     所需 epoch 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireEpochAttribute(Type epochType) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Required epoch type.
        ///     所需 epoch 类型。
        /// </summary>
        public Type EpochType { get; } = epochType;
    }

    /// <summary>
    ///     Base metadata for character-to-epoch unlock registrations.
    ///     character-to-epoch 解锁注册的基础元数据。
    /// </summary>
    /// <param name="epochType">
    ///     Target epoch type.
    ///     目标 epoch 类型。
    /// </param>
    public abstract class CharacterEpochRegistrationAttributeBase(Type epochType) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Target epoch type.
        ///     目标 epoch 类型。
        /// </summary>
        public Type EpochType { get; } = epochType;
    }

    /// <summary>
    ///     Unlocks an epoch after completing any run as the annotated character.
    ///     使用带注解的角色完成任意 run 后解锁 epoch。
    /// </summary>
    /// <param name="epochType">
    ///     Target epoch type.
    ///     目标 epoch 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class UnlockEpochAfterRunAsAttribute(Type epochType)
        : CharacterEpochRegistrationAttributeBase(epochType);

    /// <summary>
    ///     Unlocks an epoch after winning a run as the annotated character.
    ///     使用带注解的角色赢得一次 run 后解锁 epoch。
    /// </summary>
    /// <param name="epochType">
    ///     Target epoch type.
    ///     目标 epoch 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class UnlockEpochAfterWinAsAttribute(Type epochType)
        : CharacterEpochRegistrationAttributeBase(epochType);

    /// <summary>
    ///     Unlocks an epoch after winning at or above a given ascension as the annotated character.
    ///     使用带注解的角色在给定进阶等级或更高等级获胜后解锁 epoch。
    /// </summary>
    /// <param name="epochType">
    ///     Target epoch type.
    ///     目标 epoch 类型。
    /// </param>
    /// <param name="ascensionLevel">
    ///     Minimum ascension level.
    ///     最低进阶等级。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class UnlockEpochAfterAscensionWinAttribute(Type epochType, int ascensionLevel)
        : CharacterEpochRegistrationAttributeBase(epochType)
    {
        /// <summary>
        ///     Minimum ascension level required for the unlock.
        ///     解锁所需的最低进阶等级。
        /// </summary>
        public int AscensionLevel { get; } = ascensionLevel;
    }

    /// <summary>
    ///     Unlocks an epoch after defeating a number of elites as the annotated character.
    ///     使用带注解的角色击败指定数量精英后解锁 epoch。
    /// </summary>
    /// <param name="epochType">
    ///     Target epoch type.
    ///     目标 epoch 类型。
    /// </param>
    /// <param name="requiredEliteWins">
    ///     Required elite victories.
    ///     所需精英胜利次数。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class UnlockEpochAfterEliteVictoriesAttribute(Type epochType, int requiredEliteWins = 15)
        : CharacterEpochRegistrationAttributeBase(epochType)
    {
        /// <summary>
        ///     Required elite victories.
        ///     所需精英胜利次数。
        /// </summary>
        public int RequiredEliteWins { get; } = requiredEliteWins;
    }

    /// <summary>
    ///     Unlocks an epoch after defeating a number of bosses as the annotated character.
    ///     使用带注解的角色击败指定数量 boss 后解锁 epoch。
    /// </summary>
    /// <param name="epochType">
    ///     Target epoch type.
    ///     目标 epoch 类型。
    /// </param>
    /// <param name="requiredBossWins">
    ///     Required boss victories.
    ///     所需 boss 胜利次数。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class UnlockEpochAfterBossVictoriesAttribute(Type epochType, int requiredBossWins = 15)
        : CharacterEpochRegistrationAttributeBase(epochType)
    {
        /// <summary>
        ///     Required boss victories.
        ///     所需 boss 胜利次数。
        /// </summary>
        public int RequiredBossWins { get; } = requiredBossWins;
    }

    /// <summary>
    ///     Unlocks an epoch after an ascension-one win as the annotated character.
    ///     使用带注解的角色取得进阶一胜利后解锁 epoch。
    /// </summary>
    /// <param name="epochType">
    ///     Target epoch type.
    ///     目标 epoch 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class UnlockEpochAfterAscensionOneWinAttribute(Type epochType)
        : CharacterEpochRegistrationAttributeBase(epochType);

    /// <summary>
    ///     Reveals ascension UI for the annotated character after the given epoch is revealed.
    ///     给定 epoch revealed 后，为带注解的角色显示 ascension UI。
    /// </summary>
    /// <param name="epochType">
    ///     Target epoch type.
    ///     目标 epoch 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RevealAscensionAfterEpochAttribute(Type epochType)
        : CharacterEpochRegistrationAttributeBase(epochType);

    /// <summary>
    ///     Grants the given epoch through the post-run character unlock flow for the annotated character.
    ///     通过带注解角色的 post-run character unlock flow 授予给定 epoch。
    /// </summary>
    /// <param name="epochType">
    ///     Target epoch type.
    ///     目标 epoch 类型。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class UnlockCharacterAfterRunAsAttribute(Type epochType)
        : CharacterEpochRegistrationAttributeBase(epochType);
}
