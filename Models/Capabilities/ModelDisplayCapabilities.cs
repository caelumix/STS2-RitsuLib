using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace STS2RitsuLib.Models.Capabilities
{
    /// <summary>
    ///     Known model asset path query scopes used by framework adapters.
    ///     框架 adapter 使用的已知模型资源路径查询作用域。
    /// </summary>
    public enum ModelAssetPathScope
    {
        /// <summary>
        ///     General model assets.
        ///     通用模型资源。
        /// </summary>
        General,

        /// <summary>
        ///     Assets needed while a run is active.
        ///     跑局中需要的资源。
        /// </summary>
        Run,

        /// <summary>
        ///     Assets needed by combat-facing views.
        ///     战斗侧视图需要的资源。
        /// </summary>
        Combat,

        /// <summary>
        ///     Assets needed by map or route views.
        ///     地图或路线视图需要的资源。
        /// </summary>
        Map,

        /// <summary>
        ///     Assets needed by character selection views.
        ///     选角视图需要的资源。
        /// </summary>
        CharacterSelect,
    }

    /// <summary>
    ///     Context passed to model asset path components.
    ///     传给模型资源路径组件的上下文。
    /// </summary>
    public readonly record struct ModelAssetPathContext(
        AbstractModel Model,
        ModelAssetPathScope Scope,
        object? RuntimeContext = null);

    /// <summary>
    ///     Optional component capability that contributes dynamic vars for any model text surface.
    ///     可选组件能力：为任意模型文本 surface 贡献动态变量。
    /// </summary>
    public interface IModelDynamicVarComponent
    {
        /// <summary>
        ///     Returns the component-owned dynamic-var set for <paramref name="model" />.
        ///     返回 <paramref name="model" /> 对应的组件自有动态变量集合。
        /// </summary>
        DynamicVarSet GetDynamicVars(AbstractModel model);
    }

    /// <summary>
    ///     Optional component capability that contributes hover tips for any model.
    ///     可选组件能力：为任意模型贡献悬停提示。
    /// </summary>
    public interface IModelHoverTipComponent
    {
        /// <summary>
        ///     Returns additional hover tips for <paramref name="model" />.
        ///     返回 <paramref name="model" /> 的额外悬停提示。
        /// </summary>
        IEnumerable<IHoverTip> GetHoverTips(AbstractModel model);
    }

    /// <summary>
    ///     Optional typed component capability that contributes hover tips for <typeparamref name="TModel" />.
    ///     可选类型化组件能力：为 <typeparamref name="TModel" /> 贡献悬停提示。
    /// </summary>
    public interface IModelHoverTipComponent<in TModel> where TModel : AbstractModel
    {
        /// <summary>
        ///     Returns additional hover tips for <paramref name="model" />.
        ///     返回 <paramref name="model" /> 的额外悬停提示。
        /// </summary>
        IEnumerable<IHoverTip> GetHoverTips(TModel model);
    }

    /// <summary>
    ///     Optional component capability that contributes asset paths for any model.
    ///     可选组件能力：为任意模型贡献资源路径。
    /// </summary>
    public interface IModelAssetPathComponent
    {
        /// <summary>
        ///     Returns additional asset paths.
        ///     返回额外资源路径。
        /// </summary>
        IEnumerable<string> GetAssetPaths(ModelAssetPathContext context);
    }

    /// <summary>
    ///     Optional typed component capability that contributes asset paths for <typeparamref name="TModel" />.
    ///     可选类型化组件能力：为 <typeparamref name="TModel" /> 贡献资源路径。
    /// </summary>
    public interface IModelAssetPathComponent<in TModel> where TModel : AbstractModel
    {
        /// <summary>
        ///     Returns additional asset paths.
        ///     返回额外资源路径。
        /// </summary>
        IEnumerable<string> GetAssetPaths(TModel model, ModelAssetPathContext context);
    }

    internal static partial class ModelComponentCapabilityHost
    {
        internal static IEnumerable<IHoverTip> GetHoverTips<TModel>(TModel model)
            where TModel : AbstractModel
        {
            foreach (var component in GetComponentSnapshot(model))
                switch (component)
                {
                    case IModelHoverTipComponent general:
                    {
                        IEnumerable<IHoverTip> tips = [];
                        TryRun(component, model, () => tips = general.GetHoverTips(model));
                        foreach (var tip in tips)
                            yield return tip;
                        break;
                    }
                    case IModelHoverTipComponent<TModel> typed:
                    {
                        IEnumerable<IHoverTip> tips = [];
                        TryRun(component, model, () => tips = typed.GetHoverTips(model));
                        foreach (var tip in tips)
                            yield return tip;
                        break;
                    }
                }
        }

        internal static IEnumerable<string> GetAssetPaths<TModel>(TModel model, ModelAssetPathContext context)
            where TModel : AbstractModel
        {
            foreach (var component in GetComponentSnapshot(model))
                switch (component)
                {
                    case IModelAssetPathComponent general:
                    {
                        IEnumerable<string> paths = [];
                        TryRun(component, model, () => paths = general.GetAssetPaths(context));
                        foreach (var path in paths)
                            yield return path;
                        break;
                    }
                    case IModelAssetPathComponent<TModel> typed:
                    {
                        IEnumerable<string> paths = [];
                        TryRun(component, model, () => paths = typed.GetAssetPaths(model, context));
                        foreach (var path in paths)
                            yield return path;
                        break;
                    }
                }
        }

        internal static IEnumerable<TCapability> GetCapabilities<TCapability>(AbstractModel model)
            where TCapability : class
        {
            return GetComponentSnapshot(model).OfType<TCapability>().ToArray();
        }

        internal static void AddDynamicVarsTo(AbstractModel model, LocString locString)
        {
            ArgumentNullException.ThrowIfNull(model);
            ArgumentNullException.ThrowIfNull(locString);

            foreach (var component in GetComponentSnapshot(model))
            {
                if (component is not IModelDynamicVarComponent dynamicVarComponent)
                    continue;

                DynamicVarSet? dynamicVars = null;
                TryRun(component, model, () => dynamicVars = dynamicVarComponent.GetDynamicVars(model));
                dynamicVars?.AddTo(locString);
            }
        }

        internal static void TryRun(IModelComponent component, AbstractModel model, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Warn(
                    $"[ModelComponents] Component capability '{component.GetType().FullName}' failed for {model.Id}: {ex.Message}");
            }
        }

        private static IReadOnlyList<IModelComponent> GetComponentSnapshot(AbstractModel model)
        {
            if (ModelComponents.TryGet(model, out var collection)) return collection.Components.ToArray();
            if (!ModelDefaultComponents.HasDefaultComponentSource(model))
                return [];

            collection = ModelComponents.Get(model);

            return collection.Components.ToArray();
        }
    }
}
