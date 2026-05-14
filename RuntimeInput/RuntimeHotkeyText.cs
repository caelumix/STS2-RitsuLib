using STS2RitsuLib.Settings;

namespace STS2RitsuLib.RuntimeInput
{
    /// <summary>
    ///     Deferred runtime hotkey metadata text that can be fixed or resolved dynamically at read time.
    ///     Deferred runtime hotkey metadata text that can be fixed 或 resolved dynamically at read time.
    /// </summary>
    public abstract class RuntimeHotkeyText
    {
        /// <summary>
        ///     Resolves the text for the current locale or runtime state.
        ///     解析 the text for the current locale or runtime state。
        /// </summary>
        public abstract string Resolve();

        /// <summary>
        ///     Creates fixed text that never changes.
        ///     创建 fixed text that never changes。
        /// </summary>
        public static RuntimeHotkeyText Literal(string text)
        {
            return new LiteralRuntimeHotkeyText(text);
        }

        /// <summary>
        ///     Creates text resolved dynamically each time metadata is read.
        ///     创建 text resolved dynamically each time metadata is read。
        /// </summary>
        public static RuntimeHotkeyText Dynamic(Func<string> resolver)
        {
            ArgumentNullException.ThrowIfNull(resolver);
            return new DynamicRuntimeHotkeyText(resolver);
        }

        /// <summary>
        ///     Implicitly wraps a fixed string.
        ///     中文说明：Implicitly wraps a fixed string.
        /// </summary>
        public static implicit operator RuntimeHotkeyText(string text)
        {
            return Literal(text);
        }

        /// <summary>
        ///     Implicitly wraps deferred mod-settings text.
        ///     Implicitly wraps deferred mod-设置 text.
        /// </summary>
        public static implicit operator RuntimeHotkeyText(ModSettingsText text)
        {
            ArgumentNullException.ThrowIfNull(text);
            return Dynamic(text.Resolve);
        }

        /// <summary>
        ///     Implicitly wraps a deferred string resolver.
        ///     Implicitly wraps a deferred string 解析r.
        /// </summary>
        public static implicit operator RuntimeHotkeyText(Func<string> resolver)
        {
            return Dynamic(resolver);
        }

        private sealed class LiteralRuntimeHotkeyText(string text) : RuntimeHotkeyText
        {
            public override string Resolve()
            {
                return text;
            }
        }

        private sealed class DynamicRuntimeHotkeyText(Func<string> resolver) : RuntimeHotkeyText
        {
            public override string Resolve()
            {
                return resolver();
            }
        }
    }
}
