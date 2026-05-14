namespace STS2RitsuLib.Keywords
{
    /// <summary>
    ///     Whether <see cref="ModKeywordRegistry" /> still accepts new keyword registrations from mods.
    ///     <c>ModKeywordRegistry</c> 是否仍接受来自 mod 的新 keyword 注册。
    /// </summary>
    public enum KeywordRegistrationState
    {
        /// <summary>
        ///     Registrations are allowed until the framework freezes them (with other model registries).
        ///     在 framework 与其它 model registry 一起冻结前允许注册。
        /// </summary>
        Open = 0,

        /// <summary>
        ///     Further registration throws; the global keyword table is considered sealed.
        ///     后续注册会抛出异常；全局 keyword table 视为已 sealed。
        /// </summary>
        Frozen = 1,
    }
}
