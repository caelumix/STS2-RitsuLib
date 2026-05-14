using System.Text.Json;
using STS2RitsuLib.Utils.Persistence;

namespace STS2RitsuLib.Settings
{
    /// <summary>
    ///     Factory methods for <see cref="IModSettingsValueBinding{TValue}" /> and related wrappers.
    ///     Factory methods 用于 <c>IModSettingsValueBinding{TValue}</c> 和 related wrappers.
    /// </summary>
    public static class ModSettingsBindings
    {
        /// <summary>
        ///     Creates a binding against <typeparamref name="TModel" /> with an explicit <see cref="SaveScope" />.
        ///     创建 a binding against <c>TModel</c> with an explicit <c>SaveScope</c>。
        /// </summary>
        public static ModSettingsValueBinding<TModel, TValue> Create<TModel, TValue>(
            string modId,
            string dataKey,
            SaveScope scope,
            Func<TModel, TValue> getter,
            Action<TModel, TValue> setter)
            where TModel : class, new()
        {
            return new(modId, dataKey, scope, getter, setter);
        }

        /// <summary>
        ///     Shorthand for <see cref="SaveScope.Global" />.
        ///     Shorthand 用于 <c>保存Scope.Global</c>.
        /// </summary>
        public static ModSettingsValueBinding<TModel, TValue> Global<TModel, TValue>(
            string modId,
            string dataKey,
            Func<TModel, TValue> getter,
            Action<TModel, TValue> setter)
            where TModel : class, new()
        {
            return Create(modId, dataKey, SaveScope.Global, getter, setter);
        }

        /// <summary>
        ///     Shorthand for <see cref="SaveScope.Profile" />.
        ///     Shorthand 用于 <c>保存Scope.档案</c>.
        /// </summary>
        public static ModSettingsValueBinding<TModel, TValue> Profile<TModel, TValue>(
            string modId,
            string dataKey,
            Func<TModel, TValue> getter,
            Action<TModel, TValue> setter)
            where TModel : class, new()
        {
            return Create(modId, dataKey, SaveScope.Profile, getter, setter);
        }

        /// <summary>
        ///     Binds to per-run JSON under the framework profile tree (<c>run_sidecar/v1/{fingerprintStem}/</c> under
        ///     Binds to per-跑局 JSON under the framework 档案 tree (<c>跑局_sidecar/v1/{fingerprintStem}/</c> under
        ///     <see cref="Utils.Persistence.ProfileManager" /> for <see cref="Const.ModId" />), one file per consumer
        ///     mod in that folder, keyed by the vanilla run fingerprint. Client-local only; does not modify
        ///     mod in that folder, keyed 通过 the 原版 跑局 fingerprint. Client-local only; does not modify
        ///     <see cref="MegaCrit.Sts2.Core.Saves.SerializableRun" /> network payloads.
        /// </summary>
        public static ModSettingsRunSidecarValueBinding<TModel, TValue> RunSidecar<TModel, TValue>(
            string modId,
            string dataKey,
            Func<TModel, TValue> getter,
            Action<TModel, TValue> setter)
            where TModel : class, new()
        {
            return new(modId, dataKey, getter, setter);
        }

        /// <summary>
        ///     Non-persisted binding for previews and debug UI.
        ///     Non-persisted binding 用于 previews 和 debug UI.
        /// </summary>
        public static InMemoryModSettingsValueBinding<TValue> InMemory<TValue>(
            string modId,
            string dataKey,
            TValue initialValue)
        {
            return new(modId, dataKey, initialValue);
        }

        /// <summary>
        ///     Value binding backed by caller-supplied read/write/save delegates (external persistence, legacy configs,
        ///     Value binding backed 通过 caller-supplied read/write/保存 delegates (external persistence, legacy configs,
        ///     etc.). Uses <see cref="SaveScope.Global" /> by default.
        ///     etc.). 使用 <c>保存Scope.Global</c> 通过 default.
        /// </summary>
        public static ModSettingsCallbackValueBinding<TValue> Callback<TValue>(
            string modId,
            string dataKey,
            Func<TValue> read,
            Action<TValue> write,
            Action save,
            SaveScope scope = SaveScope.Global)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modId);
            ArgumentException.ThrowIfNullOrWhiteSpace(dataKey);
            ArgumentNullException.ThrowIfNull(read);
            ArgumentNullException.ThrowIfNull(write);
            ArgumentNullException.ThrowIfNull(save);

            return new(modId, dataKey, scope, read, write, save);
        }

        /// <summary>
        ///     Attaches a structured adapter for clipboard / JSON round-trip.
        ///     Attaches a structured adapter 用于 clipboard / JSON round-trip.
        /// </summary>
        public static StructuredModSettingsValueBinding<TValue> WithAdapter<TValue>(
            IModSettingsValueBinding<TValue> inner,
            IStructuredModSettingsValueAdapter<TValue> adapter)
        {
            return new(inner, adapter);
        }

        /// <summary>
        ///     Supplies a default when the store is empty; optional adapter for structured types.
        ///     当 the store is empty; optional adapter for structured types 时提供 a default。
        /// </summary>
        public static DefaultModSettingsValueBinding<TValue> WithDefault<TValue>(
            IModSettingsValueBinding<TValue> inner,
            Func<TValue> defaultValueFactory,
            IStructuredModSettingsValueAdapter<TValue>? adapter = null)
        {
            return new(inner, defaultValueFactory, adapter);
        }

        /// <summary>
        ///     Derives a child binding from a parent object graph.
        ///     Derives a child binding 从 a parent object graph.
        /// </summary>
        public static ProjectedModSettingsValueBinding<TSource, TValue> Project<TSource, TValue>(
            IModSettingsValueBinding<TSource> parent,
            string dataKey,
            Func<TSource, TValue> getter,
            Func<TSource, TValue, TSource> setter,
            IStructuredModSettingsValueAdapter<TValue>? adapter = null)
        {
            return new(parent, dataKey, getter, setter, adapter);
        }
    }

    /// <summary>
    ///     Built-in <see cref="IStructuredModSettingsValueAdapter{TValue}" /> implementations.
    ///     中文说明：Built-in <c>IStructuredModSettingsValueAdapter{TValue}</c> implementations.
    ///     Built-in <c>IStructuredModSettingsValueAdapter{TValue}</c> implementations.
    ///     中文说明：Built-in <c>IStructuredModSettingsValueAdapter{TValue}</c> implementations.
    /// </summary>
    public static class ModSettingsStructuredData
    {
        /// <summary>
        ///     JSON adapter using optional custom <see cref="JsonSerializerOptions" />.
        ///     JSON adapter using 可选 自定义 <c>JsonSerializerOptions</c>.
        /// </summary>
        public static IStructuredModSettingsValueAdapter<TValue> Json<TValue>(JsonSerializerOptions? options = null)
        {
            return new JsonStructuredValueAdapter<TValue>(options);
        }

        /// <summary>
        ///     List adapter; items use <paramref name="itemAdapter" /> or default JSON per element.
        ///     List adapter; items 使用 <c>itemAdapter</c> 或 default JSON per element.
        /// </summary>
        public static IStructuredModSettingsValueAdapter<List<TItem>> List<TItem>(
            IStructuredModSettingsValueAdapter<TItem>? itemAdapter = null,
            JsonSerializerOptions? options = null)
        {
            return new ListStructuredValueAdapter<TItem>(itemAdapter, options);
        }
    }
}
