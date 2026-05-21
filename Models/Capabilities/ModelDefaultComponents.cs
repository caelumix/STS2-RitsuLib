using MegaCrit.Sts2.Core.Models;

namespace STS2RitsuLib.Models.Capabilities
{
    /// <summary>
    ///     Mutable list used while resolving a model's default components.
    ///     解析模型默认组件时使用的可变列表。
    /// </summary>
    public sealed class ModelDefaultComponentList
    {
        private readonly List<IModelComponent> _components = [];

        /// <summary>
        ///     Current components in default order.
        ///     当前默认顺序中的组件。
        /// </summary>
        public IReadOnlyList<IModelComponent> Components => _components;

        /// <summary>
        ///     Number of components in the list.
        ///     列表中的组件数量。
        /// </summary>
        public int Count => _components.Count;

        /// <summary>
        ///     Adds <paramref name="component" /> to the end of the list.
        ///     将 <paramref name="component" /> 添加到列表末尾。
        /// </summary>
        public IModelComponent Add(IModelComponent component)
        {
            ArgumentNullException.ThrowIfNull(component);
            _components.Add(component);
            return component;
        }

        /// <summary>
        ///     Creates a component and adds it to the end of the list.
        ///     创建组件并添加到列表末尾。
        /// </summary>
        public TComponent Add<TComponent>() where TComponent : class, IModelComponent
        {
            var component = CreateComponent<TComponent>();
            Add(component);
            return component;
        }

        /// <summary>
        ///     Creates a component of <paramref name="componentType" /> and adds it to the end of the list.
        ///     创建 <paramref name="componentType" /> 类型组件并添加到列表末尾。
        /// </summary>
        public IModelComponent Add(Type componentType)
        {
            var component = CreateComponent(componentType);
            Add(component);
            return component;
        }

        /// <summary>
        ///     Creates a registered component and adds it to the end of the list.
        ///     创建已注册组件并添加到列表末尾。
        /// </summary>
        public TComponent AddRegistered<TComponent>() where TComponent : class, IModelComponent
        {
            var component = CreateRegisteredComponent<TComponent>();
            Add(component);
            return component;
        }

        /// <summary>
        ///     Inserts a created component at <paramref name="index" />.
        ///     创建组件并插入到 <paramref name="index" />。
        /// </summary>
        public TComponent Insert<TComponent>(int index) where TComponent : class, IModelComponent
        {
            var component = CreateComponent<TComponent>();
            Insert(index, component);
            return component;
        }

        /// <summary>
        ///     Creates a component of <paramref name="componentType" /> and inserts it at <paramref name="index" />.
        ///     创建 <paramref name="componentType" /> 类型组件并插入到 <paramref name="index" />。
        /// </summary>
        public IModelComponent Insert(int index, Type componentType)
        {
            var component = CreateComponent(componentType);
            Insert(index, component);
            return component;
        }

        private static TComponent CreateComponent<TComponent>() where TComponent : class, IModelComponent
        {
            var componentId = ModelComponentRegistry.GetComponentId<TComponent>();
            if (componentId != null)
                return CreateRegisteredComponent<TComponent>(componentId);

            if (typeof(ModelComponent).IsAssignableFrom(typeof(TComponent)))
                throw new InvalidOperationException(
                    $"Model component type is not registered: {typeof(TComponent).FullName}");

            var component = Activator.CreateInstance(typeof(TComponent)) as TComponent;
            return component ?? throw new InvalidOperationException(
                $"Component type must have a public parameterless constructor: {typeof(TComponent).FullName}");
        }

        private static IModelComponent CreateComponent(Type componentType)
        {
            ArgumentNullException.ThrowIfNull(componentType);
            if (componentType.ContainsGenericParameters ||
                componentType.IsAbstract ||
                componentType.IsInterface ||
                !typeof(IModelComponent).IsAssignableFrom(componentType))
                throw new ArgumentException(
                    $"Type '{componentType.FullName}' must be a concrete implementation of IModelComponent.",
                    nameof(componentType));

            var componentId = ModelComponentRegistry.GetComponentId(componentType);
            if (componentId != null)
                return ModelComponentRegistry.Create(componentId);

            if (typeof(ModelComponent).IsAssignableFrom(componentType))
                throw new InvalidOperationException(
                    $"Model component type is not registered: {componentType.FullName}");

            var component = Activator.CreateInstance(componentType) as IModelComponent;
            return component ?? throw new InvalidOperationException(
                $"Component type must have a public parameterless constructor: {componentType.FullName}");
        }

        private static TComponent CreateRegisteredComponent<TComponent>(string? componentId = null)
            where TComponent : class, IModelComponent
        {
            componentId ??= ModelComponentRegistry.GetComponentId<TComponent>();
            if (componentId == null)
                throw new InvalidOperationException(
                    $"Model component type is not registered: {typeof(TComponent).FullName}");

            var component = ModelComponentRegistry.Create(componentId) as TComponent;
            return component ?? throw new InvalidOperationException(
                $"Registered component '{componentId}' is not a '{typeof(TComponent).FullName}'.");
        }

        /// <summary>
        ///     Adds all <paramref name="components" /> to the end of the list.
        ///     将所有 <paramref name="components" /> 添加到列表末尾。
        /// </summary>
        public void AddRange(IEnumerable<IModelComponent> components)
        {
            ArgumentNullException.ThrowIfNull(components);
            foreach (var component in components)
                Add(component);
        }

        /// <summary>
        ///     Inserts <paramref name="component" /> at <paramref name="index" />.
        ///     在 <paramref name="index" /> 插入 <paramref name="component" />。
        /// </summary>
        public IModelComponent Insert(int index, IModelComponent component)
        {
            ArgumentNullException.ThrowIfNull(component);
            if (index < 0 || index > _components.Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index is outside the list bounds.");

            _components.Insert(index, component);
            return component;
        }

        /// <summary>
        ///     Inserts <paramref name="component" /> before the first component of type
        ///     <typeparamref name="TExisting" />.
        ///     将 <paramref name="component" /> 插入到第一个 <typeparamref name="TExisting" /> 组件之前。
        /// </summary>
        public bool InsertBefore<TExisting>(IModelComponent component) where TExisting : class, IModelComponent
        {
            var index = _components.FindIndex(static existing => existing is TExisting);
            if (index < 0)
                return false;

            Insert(index, component);
            return true;
        }

        /// <summary>
        ///     Inserts <paramref name="component" /> after the first component of type
        ///     <typeparamref name="TExisting" />.
        ///     将 <paramref name="component" /> 插入到第一个 <typeparamref name="TExisting" /> 组件之后。
        /// </summary>
        public bool InsertAfter<TExisting>(IModelComponent component) where TExisting : class, IModelComponent
        {
            var index = _components.FindIndex(static existing => existing is TExisting);
            if (index < 0)
                return false;

            Insert(index + 1, component);
            return true;
        }

        /// <summary>
        ///     Removes the first component of type <typeparamref name="TComponent" />.
        ///     移除第一个 <typeparamref name="TComponent" /> 类型组件。
        /// </summary>
        public TComponent? Remove<TComponent>() where TComponent : class, IModelComponent
        {
            var index = _components.FindIndex(static component => component is TComponent);
            if (index < 0)
                return null;

            var removed = (TComponent)_components[index];
            _components.RemoveAt(index);
            return removed;
        }

        /// <summary>
        ///     Removes every component of type <typeparamref name="TComponent" />.
        ///     移除所有 <typeparamref name="TComponent" /> 类型组件。
        /// </summary>
        public IReadOnlyList<TComponent> RemoveAll<TComponent>() where TComponent : class, IModelComponent
        {
            List<TComponent> removed = [];
            for (var i = _components.Count - 1; i >= 0; i--)
            {
                if (_components[i] is not TComponent component)
                    continue;

                _components.RemoveAt(i);
                removed.Add(component);
            }

            removed.Reverse();
            return removed;
        }

        /// <summary>
        ///     Replaces the first component of type <typeparamref name="TComponent" />.
        ///     替换第一个 <typeparamref name="TComponent" /> 类型组件。
        /// </summary>
        public bool Replace<TComponent>(IModelComponent replacement) where TComponent : class, IModelComponent
        {
            ArgumentNullException.ThrowIfNull(replacement);

            var index = _components.FindIndex(static component => component is TComponent);
            if (index < 0)
                return false;

            _components[index] = replacement;
            return true;
        }

        /// <summary>
        ///     Gets the first component of type <typeparamref name="TComponent" />.
        ///     获取第一个 <typeparamref name="TComponent" /> 类型组件。
        /// </summary>
        public TComponent? Get<TComponent>() where TComponent : class, IModelComponent
        {
            return _components.OfType<TComponent>().FirstOrDefault();
        }

        /// <summary>
        ///     Gets all components of type <typeparamref name="TComponent" />.
        ///     获取所有 <typeparamref name="TComponent" /> 类型组件。
        /// </summary>
        public IReadOnlyList<TComponent> GetAll<TComponent>() where TComponent : class, IModelComponent
        {
            return _components.OfType<TComponent>().ToArray();
        }

        /// <summary>
        ///     Returns true when the list contains a component of type <typeparamref name="TComponent" />.
        ///     当列表包含 <typeparamref name="TComponent" /> 类型组件时返回 true。
        /// </summary>
        public bool Contains<TComponent>() where TComponent : class, IModelComponent
        {
            return _components.Any(static component => component is TComponent);
        }

        internal IModelComponent[] ToArray()
        {
            return _components.ToArray();
        }
    }

    internal static class ModelDefaultComponents
    {
        private static readonly Lock SyncRoot = new();
        private static readonly List<ModelDefaultComponentModifierEntry> Modifiers = [];
        private static long _nextOrder;

        public static void Modify<TModel>(
            string modId,
            string modifierId,
            Action<TModel, ModelDefaultComponentList> modifier,
            int order = 0)
            where TModel : AbstractModel
        {
            ArgumentNullException.ThrowIfNull(modifier);
            Modify(
                modId,
                modifierId,
                typeof(TModel),
                (model, components) => modifier((TModel)model, components),
                order);
        }

        public static void Modify(
            string modId,
            string modifierId,
            Type ownerType,
            Action<AbstractModel, ModelDefaultComponentList> modifier,
            int order = 0)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentException.ThrowIfNullOrWhiteSpace(modifierId);
            ArgumentNullException.ThrowIfNull(ownerType);
            ArgumentNullException.ThrowIfNull(modifier);

            if (ownerType.ContainsGenericParameters ||
                ownerType.IsInterface ||
                !typeof(AbstractModel).IsAssignableFrom(ownerType))
                throw new ArgumentException(
                    $"Type '{ownerType.FullName}' must be an abstract model type or a concrete model type.",
                    nameof(ownerType));

            lock (SyncRoot)
            {
                if (Modifiers.Any(entry =>
                        string.Equals(entry.ModId, modId, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(entry.ModifierId, modifierId, StringComparison.Ordinal)))
                    throw new InvalidOperationException(
                        $"Default component modifier is already registered: {modId}/{modifierId}");

                Modifiers.Add(new(
                    modId,
                    modifierId,
                    ownerType,
                    order,
                    _nextOrder++,
                    modifier));
            }
        }

        internal static bool HasDefaultComponentSource(AbstractModel owner)
        {
            ArgumentNullException.ThrowIfNull(owner);
            if (owner is IModelComponentProvider)
                return true;

            var ownerType = owner.GetType();
            lock (SyncRoot)
            {
                return Modifiers.Any(entry => entry.OwnerType.IsAssignableFrom(ownerType));
            }
        }

        internal static IReadOnlyList<IModelComponent> Create(AbstractModel owner)
        {
            ArgumentNullException.ThrowIfNull(owner);

            var components = new ModelDefaultComponentList();
            if (owner is IModelComponentProvider provider)
                TryRunProvider(owner, provider, components);

            foreach (var modifier in GetModifiers(owner))
                TryRunModifier(owner, modifier, components);

            return components.ToArray();
        }

        private static ModelDefaultComponentModifierEntry[] GetModifiers(AbstractModel owner)
        {
            var ownerType = owner.GetType();
            lock (SyncRoot)
            {
                return Modifiers
                    .Where(entry => entry.OwnerType.IsAssignableFrom(ownerType))
                    .OrderBy(static entry => entry.Order)
                    .ThenBy(static entry => entry.RegistrationOrder)
                    .ToArray();
            }
        }

        private static void TryRunProvider(
            AbstractModel owner,
            IModelComponentProvider provider,
            ModelDefaultComponentList components)
        {
            try
            {
                provider.BuildDefaultComponents(components);
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Warn(
                    $"[ModelComponents] Default component provider failed for {owner.Id}: {ex.Message}");
            }
        }

        private static void TryRunModifier(
            AbstractModel owner,
            ModelDefaultComponentModifierEntry modifier,
            ModelDefaultComponentList components)
        {
            try
            {
                modifier.Modify(owner, components);
            }
            catch (Exception ex)
            {
                RitsuLibFramework.Logger.Warn(
                    $"[ModelComponents] Default component modifier '{modifier.ModId}/{modifier.ModifierId}' failed for {owner.Id}: {ex.Message}");
            }
        }

        private readonly record struct ModelDefaultComponentModifierEntry(
            string ModId,
            string ModifierId,
            Type OwnerType,
            int Order,
            long RegistrationOrder,
            // ReSharper disable once MemberHidesStaticFromOuterClass
            Action<AbstractModel, ModelDefaultComponentList> Modify);
    }
}
