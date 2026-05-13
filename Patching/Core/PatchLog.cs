using System.Collections.Concurrent;
using MegaCrit.Sts2.Core.Logging;

namespace STS2RitsuLib.Patching.Core
{
    /// <summary>
    ///     Logger registry for IPatchMethod types.
    ///     IPatchMethod 类型的日志器注册表。
    /// </summary>
    public static class PatchLog
    {
        private static readonly ConcurrentDictionary<Type, Logger> Registry = new();

        /// <summary>
        ///     Associates <paramref name="logger" /> with <paramref name="patchType" /> for <see cref="For(Type)" />.
        ///     将 <paramref name="logger" /> 与 <paramref name="patchType" /> 关联，供 <see cref="For(Type)" /> 使用。
        /// </summary>
        public static void Bind(Type patchType, Logger logger)
        {
            ArgumentNullException.ThrowIfNull(patchType);
            ArgumentNullException.ThrowIfNull(logger);

            Registry[patchType] = logger;
        }

        /// <summary>
        ///     Returns the bound logger for <paramref name="patchType" />, or <see cref="RitsuLibFramework.Logger" />.
        ///     返回 <paramref name="patchType" /> 绑定的日志器；未绑定时返回 <see cref="RitsuLibFramework.Logger" />。
        /// </summary>
        public static Logger For(Type patchType)
        {
            ArgumentNullException.ThrowIfNull(patchType);
            return Registry.TryGetValue(patchType, out var logger)
                ? logger
                : RitsuLibFramework.Logger;
        }

        /// <summary>
        ///     <see cref="For(Type)" /> for <typeparamref name="TPatch" />.
        ///     面向 <typeparamref name="TPatch" /> 的 <see cref="For(Type)" /> 简写。
        /// </summary>
        public static Logger For<TPatch>()
        {
            return For(typeof(TPatch));
        }
    }
}
