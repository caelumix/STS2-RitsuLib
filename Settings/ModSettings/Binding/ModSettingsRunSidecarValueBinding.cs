using STS2RitsuLib.Settings.RunSidecar;
using STS2RitsuLib.Utils.Persistence;

namespace STS2RitsuLib.Settings
{
    /// <summary>
    ///     Binds a settings field to JSON stored under <see cref="ModRunSidecarStore" />, validated against the active
    ///     Binds a 设置 field to JSON stored under <c>ModRunSidecarStore</c>, 有效ated against the active
    ///     run fingerprint. Safe for mod reload; does not alter vanilla synchronized save payloads.
    ///     跑局 fingerprint. Safe 用于 mod re加载; does not alter 原版 synchronized 保存 payload.
    /// </summary>
    public sealed class ModSettingsRunSidecarValueBinding<TModel, TValue>(
        string modId,
        string dataKey,
        Func<TModel, TValue> getter,
        Action<TModel, TValue> setter)
        : IModSettingsValueBinding<TValue>, IRunSidecarModSettingsBinding, IModSettingsBindingSemantics
        where TModel : class, new()
    {
        /// <inheritdoc />
        public ModSettingsValueSemantics Semantics => ModSettingsValueSemantics.RunSnapshot;

        /// <inheritdoc />
        public string ModId { get; } = modId;

        /// <inheritdoc />
        public string DataKey { get; } = dataKey;

        /// <inheritdoc />
        public SaveScope Scope => SaveScope.Global;

        /// <inheritdoc />
        public TValue Read()
        {
            var status = ModRunSidecarStore.TryReadModel<TModel>(ModId, out var model);
            if (status != ModRunSidecarReadStatus.Ok)
                model = new();

            return getter(model);
        }

        /// <inheritdoc />
        public void Write(TValue value)
        {
            var status = ModRunSidecarStore.TryReadModel<TModel>(ModId, out var model);
            if (status != ModRunSidecarReadStatus.Ok)
                model = new();

            setter(model, value);
            ModRunSidecarStore.TryWriteModel(ModId, model);
            ModSettingsBindingWriteEvents.NotifyValueWritten(this);
        }

        /// <inheritdoc />
        public void Save()
        {
        }
    }
}
