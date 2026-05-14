using STS2RitsuLib.Content;

namespace STS2RitsuLib.CardPiles
{
    /// <summary>
    ///     Declarative card-pile row for content packs: register with <see cref="ModCardPileRegistry" /> in one call.
    ///     content pack 使用的声明式 card-pile 行：一次调用即可通过 <c>ModCardPileRegistry</c> 注册。
    /// </summary>
    public sealed record CardPileRegistrationEntry
    {
        /// <summary>
        ///     Creates a raw global card-pile registration row.
        ///     创建 raw global card-pile 注册行。
        /// </summary>
        /// <param name="id">
        ///     Registered pile id.
        ///     已注册 pile id。
        /// </param>
        /// <param name="spec">
        ///     Pile metadata.
        ///     pile 元数据。
        /// </param>
        public CardPileRegistrationEntry(string id, ModCardPileSpec spec)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentNullException.ThrowIfNull(spec);

            Id = id;
            Spec = spec;
        }

        /// <summary>
        ///     Registered pile id.
        ///     已注册 pile id。
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Pile metadata applied during registration.
        ///     注册期间应用的 pile 元数据。
        /// </summary>
        public ModCardPileSpec Spec { get; }

        /// <summary>
        ///     Registers this row against <paramref name="registry" />.
        ///     将此行注册到 <c>registry</c>。
        /// </summary>
        public void Register(ModCardPileRegistry registry)
        {
            ArgumentNullException.ThrowIfNull(registry);
            registry.Register(Id, Spec);
        }

        /// <summary>
        ///     Builds an owned pile id via <see cref="ModContentRegistry.GetQualifiedCardPileId" /> and registers it.
        ///     通过 <c>ModContentRegistry.GetQualifiedCardPileId</c> 构建 owned pile id 并注册。
        /// </summary>
        public static CardPileRegistrationEntry Owned(string modId, string localPileStem, ModCardPileSpec spec)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentException.ThrowIfNullOrWhiteSpace(localPileStem);
            ArgumentNullException.ThrowIfNull(spec);

            return new(ModContentRegistry.GetQualifiedCardPileId(modId, localPileStem), spec);
        }
    }
}
