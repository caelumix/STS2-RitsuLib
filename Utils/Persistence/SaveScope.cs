namespace STS2RitsuLib.Utils.Persistence
{
    /// <summary>
    ///     Defines the scope of save data storage
    ///     Defines the scope of 保存 data storage
    /// </summary>
    public enum SaveScope
    {
        /// <summary>
        ///     Global scope - data is shared across all profiles
        ///     Global scope - data is shared across all 档案s
        /// </summary>
        Global,

        /// <summary>
        ///     Profile scope - data is specific to the current profile
        ///     档案 scope - data is specific to the current 档案
        /// </summary>
        Profile,

        /// <summary>
        ///     Per-run sidecar scope under the current profile (client-local).
        ///     Per-跑局 sidecar scope under the current 档案 (client-local).
        /// </summary>
        /// <remarks>
        ///     This scope requires a run fingerprint stem context to resolve a durable path.
        ///     This scope requires a 跑局 fingerprint stem context to 解析 a durable 路径.
        /// </remarks>
        RunSidecar,

        /// <summary>
        ///     In-memory scope (not persisted).
        ///     中文说明：In-memory scope (not persisted).
        /// </summary>
        InMemory,
    }
}
