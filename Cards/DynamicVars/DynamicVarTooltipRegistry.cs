using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Utils;

namespace STS2RitsuLib.Cards.DynamicVars
{
    /// <summary>
    ///     Weakly attached per-<see cref="DynamicVar" /> tooltip factories (not serialized with game data).
    ///     Weakly attached per-<c>DynamicVar</c> tooltip factories (not serialized 带有 game data).
    /// </summary>
    public static class DynamicVarTooltipRegistry
    {
        private static readonly AttachedState<DynamicVar, Func<DynamicVar, IHoverTip>?> TooltipFactories =
            new(() => null);

        /// <summary>
        ///     Associates <paramref name="dynamicVar" /> with <paramref name="tooltipFactory" />.
        ///     Associates <c>dynamicVar</c> 带有 <c>tooltipFactory</c>.
        /// </summary>
        public static void Set(DynamicVar dynamicVar, Func<DynamicVar, IHoverTip> tooltipFactory)
        {
            ArgumentNullException.ThrowIfNull(dynamicVar);
            ArgumentNullException.ThrowIfNull(tooltipFactory);
            TooltipFactories[dynamicVar] = tooltipFactory;
        }

        /// <summary>
        ///     Returns the registered factory for <paramref name="dynamicVar" />, if any.
        ///     返回 the registered factory for <c>dynamicVar</c>, if any。
        /// </summary>
        public static Func<DynamicVar, IHoverTip>? Get(DynamicVar dynamicVar)
        {
            ArgumentNullException.ThrowIfNull(dynamicVar);
            return TooltipFactories[dynamicVar];
        }

        /// <summary>
        ///     Invokes the registered factory for <paramref name="dynamicVar" />.
        ///     Invokes the 已注册 factory 用于 <c>dynamicVar</c>.
        /// </summary>
        public static IHoverTip? Create(DynamicVar dynamicVar)
        {
            ArgumentNullException.ThrowIfNull(dynamicVar);
            var factory = Get(dynamicVar);
            return factory?.Invoke(dynamicVar);
        }

        /// <summary>
        ///     Copies the tooltip factory from <paramref name="source" /> to <paramref name="destination" /> when present.
        ///     Copies the tooltip factory 从 <c>source</c> to <c>destination</c> 当 present.
        /// </summary>
        public static void CopyTo(DynamicVar source, DynamicVar destination)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(destination);

            var factory = Get(source);
            if (factory != null)
                TooltipFactories[destination] = factory;
        }
    }
}
