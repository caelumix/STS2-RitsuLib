using System.Collections.Concurrent;
using Godot;

namespace STS2RitsuLib.Settings
{
    /// <summary>
    ///     Safe clipboard text reads: try/catch, per-process-frame cache, and <see cref="InvalidateCache" /> after writes.
    ///     Safe clipboard text reads: try/catch, per-process-frame cache, 和 <c>InvalidateCache</c> 之后 writes.
    ///     Reduces Windows "Unable to open clipboard" spam when many menus query paste state in one frame.
    ///     Reduces Windows "Unable to open clipboard" spam 当 many menus query paste state in one frame.
    /// </summary>
    public static class ModSettingsClipboardAccess
    {
        private static ulong _cacheFrame = ulong.MaxValue;
        private static string _cacheText = string.Empty;

        /// <summary>
        ///     Clears the in-memory clipboard cache so the next read hits the OS again (call after writing the clipboard).
        ///     Clears the in-memory clipboard cache so the next read hits the OS again (call 之后 writing the clipboard).
        /// </summary>
        public static void InvalidateCache()
        {
            _cacheFrame = ulong.MaxValue;
        }

        /// <summary>
        ///     Returns false if the clipboard is empty, unavailable, or an error occurred.
        ///     返回 false if the clipboard is empty, unavailable, or an error occurred。
        /// </summary>
        public static bool TryGetText(out string text)
        {
            text = string.Empty;
            var frame = Engine.GetProcessFrames();
            if (_cacheFrame == frame)
            {
                if (string.IsNullOrWhiteSpace(_cacheText))
                    return false;
                text = _cacheText;
                return true;
            }

            _cacheFrame = frame;
            try
            {
                _cacheText = DisplayServer.ClipboardGet() ?? string.Empty;
            }
            catch
            {
                _cacheText = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(_cacheText))
                return false;
            text = _cacheText;
            return true;
        }
    }

    /// <summary>
    ///     Raised before a binding value is copied; set <see cref="SuppressDefaultClipboardWrite" /> to true to handle
    ///     Raised 之前 a binding value is copied; 设置 <c>SuppressDefaultClipboardWrite</c> to true to handle
    ///     clipboard writes yourself.
    ///     中文说明：clipboard writes yourself.
    /// </summary>
    public sealed class ModSettingsCopyActionEventArgs(
        IModSettingsBinding binding,
        Type valueType,
        object? value,
        ModSettingsClipboardScope scope)
        : EventArgs
    {
        /// <summary>
        ///     Binding whose value is being copied.
        ///     中文说明：Binding whose value is being copied.
        /// </summary>
        public IModSettingsBinding Binding { get; } = binding;

        /// <summary>
        ///     CLR type of the value being copied.
        ///     中文说明：CLR type of the value being copied.
        /// </summary>
        public Type ValueType { get; } = valueType;

        /// <summary>
        ///     Current value snapshot passed to serializers.
        ///     当前 value snapshot passed to serializers。
        /// </summary>
        public object? Value { get; } = value;

        /// <summary>
        ///     Self-only vs. subtree copy semantics.
        ///     中文说明：Self-only vs. subtree copy semantics.
        /// </summary>
        public ModSettingsClipboardScope Scope { get; } = scope;

        /// <summary>
        ///     When set by a handler, the default JSON envelope write is skipped.
        ///     当 设置 通过 a handler, the default JSON envelope write is skipped.
        /// </summary>
        public bool SuppressDefaultClipboardWrite { get; set; }
    }

    /// <summary>
    ///     Public snapshot of a clipboard envelope for paste validation (hides internal serialization types).
    ///     Public snapshot of a clipboard envelope 用于 paste 有效ation (hides internal serialization types).
    /// </summary>
    /// <param name="Kind">
    ///     Envelope discriminator (e.g. binding vs. chrome).
    ///     中文说明：Envelope discriminator (e.g. binding vs. chrome).
    /// </param>
    /// <param name="TypeName">
    ///     Serialized CLR or logical type name inside the payload.
    ///     Serialized CLR 或 logical type name inside the payload.
    /// </param>
    /// <param name="TargetSignature">
    ///     Binding identity when copied from a specific setting.
    ///     Binding identity 当 copied 从 a specific 设置ting.
    /// </param>
    /// <param name="SchemaSignature">
    ///     Adapter/schema version token for compatibility checks.
    ///     Adapter/schema version token 用于 compatibility checks.
    /// </param>
    /// <param name="Scope">
    ///     Self vs. subtree copy semantics.
    ///     中文说明：Self vs. subtree copy semantics.
    /// </param>
    /// <param name="Payload">
    ///     JSON or opaque payload string.
    ///     JSON 或 opaque payload string.
    /// </param>
    public sealed record ModSettingsClipboardEnvelopeView(
        string Kind,
        string TypeName,
        string TargetSignature,
        string SchemaSignature,
        ModSettingsClipboardScope Scope,
        string Payload);

    /// <summary>
    ///     Why a binding paste did not apply (for UI feedback).
    ///     Why a binding paste did not apply (用于 UI feedback).
    /// </summary>
    public enum ModSettingsPasteFailureReason
    {
        /// <summary>
        ///     Paste succeeded or no failure was classified.
        ///     Paste succeeded 或 no failure was classified.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Clipboard text was missing or unusable.
        ///     Clipboard text was missing 或 unusable.
        /// </summary>
        ClipboardEmpty = 1,

        /// <summary>
        ///     A registered paste rule returned <see cref="ModSettingsPasteVerdict.Deny" />.
        ///     一个 registered paste rule returned <c>ModSettingsPasteVerdict.Deny</c>。
        /// </summary>
        PasteRuleDenied = 2,

        /// <summary>
        ///     Envelope or payload did not match the target binding or adapter.
        ///     Envelope 或 payload did not match the target binding 或 adapter.
        /// </summary>
        TypeOrShapeMismatch = 3,
    }

    /// <summary>
    ///     Whether a paste attempt should be vetoed; <see cref="Deny" /> prevents writing to the target binding.
    ///     表示是否 a paste attempt should be vetoed; <c>Deny</c> prevents writing to the target binding。
    /// </summary>
    public enum ModSettingsPasteVerdict
    {
        /// <summary>
        ///     Continue with default rules (type name and schema signature match target; optional source-binding match).
        ///     Continue 带有 default rules (type name 和 schema signature match target; 可选 source-binding match).
        /// </summary>
        UseDefault = 0,

        /// <summary>
        ///     Reject paste into this target.
        ///     中文说明：Reject paste into this target.
        /// </summary>
        Deny = 1,
    }

    /// <summary>
    ///     Context for paste validation; <see cref="Envelope" /> is null when the clipboard is not a valid JSON envelope.
    ///     Context 用于 paste 有效ation; <c>Envelope</c> is null 当 the clipboard is not a 有效 JSON envelope.
    /// </summary>
    public sealed class ModSettingsPasteValidationContext
    {
        /// <summary>
        ///     Binding that would receive the paste.
        ///     中文说明：Binding that would receive the paste.
        /// </summary>
        public required IModSettingsBinding TargetBinding { get; init; }

        /// <summary>
        ///     Expected value type of <see cref="TargetBinding" />.
        ///     中文说明：Expected value type of <c>TargetBinding</c>.
        /// </summary>
        public required Type TargetValueType { get; init; }

        /// <summary>
        ///     Raw clipboard string (may or may not be a valid envelope).
        ///     Raw clipboard string (may 或 may not be a 有效 envelope).
        /// </summary>
        public required string ClipboardText { get; init; }

        /// <summary>
        ///     Parsed envelope metadata when <see cref="ClipboardText" /> is a known RitsuLib settings envelope.
        ///     Parsed envelope metadata 当 <c>ClipboardText</c> is a known RitsuLib 设置 envelope.
        /// </summary>
        public ModSettingsClipboardEnvelopeView? Envelope { get; init; }
    }

    /// <summary>
    ///     Tries to parse the clipboard into <typeparamref name="TValue" /> before default deserialization; if true, skips
    ///     Tries to parse the clipboard into <c>TValue</c> 之前 default deserialization; 如果 true, skips
    ///     <c>ModSettingsClipboardData.TryReadValue</c>.
    /// </summary>
    public delegate bool ModSettingsTryPasteApplier<TValue>(
        IModSettingsValueBinding<TValue> binding,
        IStructuredModSettingsValueAdapter<TValue> adapter,
        string clipboardText,
        out TValue value);

    /// <summary>
    ///     Central entry for binding copy/paste: default behavior, registrable paste rules, and optional strict source-binding
    ///     Central entry 用于 binding copy/paste: default behavior, registrable paste rules, 和 可选 strict source-binding
    ///     match.
    ///     中文说明：match.
    /// </summary>
    public static class ModSettingsClipboardOperations
    {
        private static readonly List<Func<ModSettingsPasteValidationContext, ModSettingsPasteVerdict>> PasteRules = [];
        private static readonly Lock PasteRulesLock = new();
        private static readonly ConcurrentDictionary<Type, List<Delegate>> PasteAppliers = new();

        /// <summary>
        ///     When true, envelope <c>TargetSignature</c> must match the current binding (legacy strict paste).
        ///     为 true 时，envelope <c>TargetSignature</c> must match the current binding (legacy strict paste)。
        /// </summary>
        public static bool RequireMatchingSourceBindingForPaste { get; set; }

        /// <summary>
        ///     Raised before the default copy-to-clipboard for a binding; handlers may set
        ///     Raised 之前 the default copy-to-clipboard 用于 a binding; handlers may 设置
        ///     <see cref="ModSettingsCopyActionEventArgs.SuppressDefaultClipboardWrite" />.
        /// </summary>
        public static event Action<ModSettingsCopyActionEventArgs>? BindingValueCopyRequested;

        /// <summary>
        ///     Registers a paste rule; if any rule returns <see cref="ModSettingsPasteVerdict.Deny" />, paste is blocked.
        ///     注册 a paste rule; if any rule returns <c>ModSettingsPasteVerdict.Deny</c>, paste is blocked。
        /// </summary>
        public static void RegisterPasteRule(Func<ModSettingsPasteValidationContext, ModSettingsPasteVerdict> rule)
        {
            ArgumentNullException.ThrowIfNull(rule);
            lock (PasteRulesLock)
            {
                PasteRules.Add(rule);
            }
        }

        /// <summary>
        ///     Registers a custom paste parser for <typeparamref name="TValue" />; runs before built-in JSON/envelope handling.
        ///     注册 a custom paste parser for <c>TValue</c>; runs before built-in JSON/envelope handling。
        /// </summary>
        public static void RegisterPasteApplier<TValue>(ModSettingsTryPasteApplier<TValue> applier)
        {
            ArgumentNullException.ThrowIfNull(applier);
            PasteAppliers.GetOrAdd(typeof(TValue), _ => []).Add(applier);
        }

        /// <summary>
        ///     Runs copy hooks then writes the default clipboard envelope unless suppressed.
        ///     中文说明：Runs copy hooks then writes the default clipboard envelope unless suppressed.
        ///     runs copy hooks then writes the default clipboard envelope unless suppressed.
        ///     中文说明：runs copy hooks then writes the default clipboard envelope unless suppressed.
        /// </summary>
        public static void InvokeCopy<TValue>(IModSettingsValueBinding<TValue> binding,
            ModSettingsClipboardScope scope,
            IStructuredModSettingsValueAdapter<TValue> adapter,
            TValue value)
        {
            var args = new ModSettingsCopyActionEventArgs(binding, typeof(TValue), value, scope);
            var h = BindingValueCopyRequested;
            if (h != null)
                foreach (var @delegate in h.GetInvocationList())
                {
                    var d = (Action<ModSettingsCopyActionEventArgs>)@delegate;
                    d(args);
                }

            if (!args.SuppressDefaultClipboardWrite)
                ModSettingsClipboardData.CopyValue(binding, scope, adapter, value);
        }

        /// <summary>
        ///     Returns whether the current clipboard can be deserialized into <typeparamref name="TValue" /> for
        ///     返回 whether the current clipboard can be deserialized into <c>TValue</c> 用于
        ///     <paramref name="binding" /> after paste rules.
        /// </summary>
        public static bool CanPasteBindingValue<TValue>(IModSettingsValueBinding<TValue> binding,
            IStructuredModSettingsValueAdapter<TValue> adapter)
        {
            if (!ModSettingsClipboardAccess.TryGetText(out var clipboard))
                return false;

            var view = TryCreateEnvelopeView(clipboard);
            if (!RunPasteRules(binding, typeof(TValue), clipboard, view))
                return false;

            return TryInvokePasteApplier(binding, adapter, clipboard, out _) ||
                   ModSettingsClipboardData.TryReadValue(binding, adapter, out _, RequireMatchingSourceBindingForPaste);
        }

        /// <summary>
        ///     Attempts to paste into <paramref name="binding" />; returns false when clipboard or validation fails.
        ///     Attempts to paste into <c>binding</c>; 返回 false 当 clipboard 或 有效ation fails.
        /// </summary>
        public static bool TryPasteBindingValue<TValue>(IModSettingsValueBinding<TValue> binding,
            IStructuredModSettingsValueAdapter<TValue> adapter, out TValue value)
        {
            return TryPasteBindingValue(binding, adapter, out value, out _);
        }

        /// <summary>
        ///     Attempts to paste into <paramref name="binding" /> and reports a <paramref name="failureReason" /> when false.
        ///     Attempts to paste into <c>binding</c> 和 reports a <c>failureReason</c> 当 false.
        /// </summary>
        public static bool TryPasteBindingValue<TValue>(IModSettingsValueBinding<TValue> binding,
            IStructuredModSettingsValueAdapter<TValue> adapter, out TValue value,
            out ModSettingsPasteFailureReason failureReason)
        {
            failureReason = ModSettingsPasteFailureReason.None;
            value = default!;

            if (!ModSettingsClipboardAccess.TryGetText(out var clipboard))
            {
                failureReason = ModSettingsPasteFailureReason.ClipboardEmpty;
                return false;
            }

            var view = TryCreateEnvelopeView(clipboard);
            if (!RunPasteRules(binding, typeof(TValue), clipboard, view))
            {
                failureReason = ModSettingsPasteFailureReason.PasteRuleDenied;
                return false;
            }

            if (TryInvokePasteApplier(binding, adapter, clipboard, out value))
                return true;

            if (ModSettingsClipboardData.TryReadValue(binding, adapter, out value,
                    RequireMatchingSourceBindingForPaste))
                return true;

            failureReason = ModSettingsPasteFailureReason.TypeOrShapeMismatch;
            return false;
        }

        private static bool TryInvokePasteApplier<TValue>(IModSettingsValueBinding<TValue> binding,
            IStructuredModSettingsValueAdapter<TValue> adapter, string clipboardText, out TValue value)
        {
            if (!PasteAppliers.TryGetValue(typeof(TValue), out var list) || list.Count == 0)
            {
                value = default!;
                return false;
            }

            foreach (var d in list)
                if (((ModSettingsTryPasteApplier<TValue>)d)(binding, adapter, clipboardText, out value))
                    return true;

            value = default!;
            return false;
        }

        internal static ModSettingsClipboardEnvelopeView? TryCreateEnvelopeView(string clipboardText)
        {
            if (!ModSettingsClipboardData.TryDeserializeEnvelope(clipboardText, out var env) || env == null)
                return null;

            return new(
                env.Kind,
                env.TypeName,
                env.TargetSignature,
                env.SchemaSignature,
                env.Scope,
                env.Payload);
        }

        private static bool RunPasteRules(IModSettingsBinding binding, Type targetValueType, string clipboardText,
            ModSettingsClipboardEnvelopeView? view)
        {
            var ctx = new ModSettingsPasteValidationContext
            {
                TargetBinding = binding,
                TargetValueType = targetValueType,
                ClipboardText = clipboardText,
                Envelope = view,
            };

            List<Func<ModSettingsPasteValidationContext, ModSettingsPasteVerdict>> snapshot;
            lock (PasteRulesLock)
            {
                snapshot = [..PasteRules];
            }

            return snapshot.All(rule => rule(ctx) != ModSettingsPasteVerdict.Deny);
        }
    }
}
