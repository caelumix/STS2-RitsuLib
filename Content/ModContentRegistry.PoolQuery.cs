namespace STS2RitsuLib.Content
{
    public sealed partial class ModContentRegistry
    {
        /// <summary>
        ///     Returns CLR types registered into <paramref name="poolType" /> that are owned by <paramref name="modId" />
        ///     (via <see cref="TryGetOwnerModId" />). Use after the content manifest has registered pool entries.
        ///     返回已注册到 <c>poolType</c> 且由 <c>modId</c> 拥有的 CLR 类型
        ///     （通过 <c>TryGetOwnerModId</c> 判断）。应在内容 manifest 注册池条目后使用。
        /// </summary>
        public static IReadOnlyList<Type> GetRegisteredModelsInPool(string modId, Type poolType)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentNullException.ThrowIfNull(poolType);

            lock (SyncRoot)
            {
                return RegisteredPoolContent
                    .Where(e => e.PoolType == poolType &&
                                TryGetOwnerModId(e.ModelType, out var oid) &&
                                string.Equals(oid, modId, StringComparison.OrdinalIgnoreCase))
                    .Select(static e => e.ModelType)
                    .Distinct()
                    .ToArray();
            }
        }
    }
}
