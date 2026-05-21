using MegaCrit.Sts2.Core.Models;

namespace STS2RitsuLib.Models.Capabilities
{
    /// <summary>
    ///     Registry for component ids and factories.
    ///     组件 ID 与工厂的注册表。
    /// </summary>
    public static class ModelComponentRegistry
    {
        private static readonly Lock SyncRoot = new();

        private static readonly Dictionary<string, Func<IModelComponent>> Factories =
            new(StringComparer.Ordinal);

        private static readonly Dictionary<string, Type> TypesById =
            new(StringComparer.Ordinal);

        private static readonly Dictionary<Type, string> TypeIds = [];

        /// <summary>
        ///     Registers or replaces a component factory.
        ///     注册或替换组件工厂。
        /// </summary>
        public static void Register(string componentId, Type componentType, Func<IModelComponent> factory)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(componentId);
            ArgumentNullException.ThrowIfNull(componentType);
            ArgumentNullException.ThrowIfNull(factory);

            if (!typeof(IModelComponent).IsAssignableFrom(componentType))
                throw new ArgumentException("Component type must implement IModelComponent.", nameof(componentType));

            lock (SyncRoot)
            {
                if (TypesById.TryGetValue(componentId, out var existingType) &&
                    existingType != componentType)
                    throw new InvalidOperationException(
                        $"Model component id is already registered for '{existingType.FullName}': {componentId}");

                if (TypeIds.TryGetValue(componentType, out var existingId) &&
                    !string.Equals(existingId, componentId, StringComparison.Ordinal))
                    throw new InvalidOperationException($"Model component id is already registered: {componentId}");

                Factories[componentId] = factory;
                TypesById[componentId] = componentType;
                TypeIds[componentType] = componentId;
            }
        }

        /// <summary>
        ///     Registers a component factory.
        ///     注册组件工厂。
        /// </summary>
        public static void Register<TComponent>(string componentId, Func<TComponent> factory)
            where TComponent : IModelComponent
        {
            ArgumentNullException.ThrowIfNull(factory);
            Register(componentId, typeof(TComponent), () => factory());
        }

        /// <summary>
        ///     Registers a parameterless component factory.
        ///     注册无参组件工厂。
        /// </summary>
        public static void Register<TComponent>(string componentId)
            where TComponent : IModelComponent, new()
        {
            Register(componentId, static () => new TComponent());
        }

        /// <summary>
        ///     Creates a component by id.
        ///     通过 ID 创建组件。
        /// </summary>
        public static bool TryCreate(string componentId, out IModelComponent component)
        {
            lock (SyncRoot)
            {
                if (!Factories.TryGetValue(componentId, out var factory))
                {
                    component = null!;
                    return false;
                }

                component = factory();
                return true;
            }
        }

        /// <summary>
        ///     Creates a component by id or throws when no factory is registered.
        ///     通过 ID 创建组件；未注册工厂时抛出异常。
        /// </summary>
        public static IModelComponent Create(string componentId)
        {
            return TryCreate(componentId, out var component)
                ? component
                : throw new InvalidOperationException($"Model component id is not registered: {componentId}");
        }

        /// <summary>
        ///     Gets the registered component id for a component type, if any.
        ///     获取组件类型已注册的组件 ID（如果存在）。
        /// </summary>
        public static string? GetComponentId(Type componentType)
        {
            ArgumentNullException.ThrowIfNull(componentType);
            lock (SyncRoot)
            {
                return TypeIds.GetValueOrDefault(componentType);
            }
        }

        /// <summary>
        ///     Gets the registered component id for <typeparamref name="TComponent" />, if any.
        ///     获取 <typeparamref name="TComponent" /> 已注册的组件 ID（如果存在）。
        /// </summary>
        public static string? GetComponentId<TComponent>() where TComponent : IModelComponent
        {
            return GetComponentId(typeof(TComponent));
        }

        /// <summary>
        ///     Attempts to resolve the component type registered for <paramref name="componentId" />.
        ///     尝试解析 <paramref name="componentId" /> 注册的组件类型。
        /// </summary>
        public static bool TryGetComponentType(string componentId, out Type componentType)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(componentId);
            lock (SyncRoot)
            {
                return TypesById.TryGetValue(componentId, out componentType!);
            }
        }

        internal static void RegisterModelComponent(Type componentType, string componentId)
        {
            if (!typeof(ModelComponent).IsAssignableFrom(componentType))
                throw new ArgumentException("Component type must inherit ModelComponent.", nameof(componentType));

            Register(componentId, componentType, () => (IModelComponent)ModelDb.Get(componentType).MutableClone());
        }

        internal static string GetModelComponentId(Type componentType)
        {
            return GetComponentId(componentType) ??
                   throw new InvalidOperationException(
                       $"Model component type is not registered: {componentType.FullName}");
        }
    }
}
