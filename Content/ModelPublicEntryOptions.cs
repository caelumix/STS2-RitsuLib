namespace STS2RitsuLib.Content
{
    /// <summary>
    ///     Controls how <see cref="ModContentRegistry" /> assigns the patched public <c>ModelDb.GetEntry(Type)</c> string
    ///     Controls how <c>ModContentRegistry</c> assigns the patched public <c>ModelDb.GetEntry(Type)</c> string
    ///     (the stable segment used in saves and localization keys for RitsuLib-registered models).
    ///     (the stable segment used in 保存 和 localization keys 用于 RitsuLib-已注册 Models).
    /// </summary>
    public readonly record struct ModelPublicEntryOptions
    {
        internal ModelPublicEntryOptions(ModelPublicEntryKind kind, string? value)
        {
            Kind = kind;
            Value = value;
        }

        /// <summary>
        ///     Uses the default rule: <c>&lt;MOD&gt;_&lt;CATEGORY&gt;_&lt;CLR_TYPE_NAME&gt;</c> (normalized).
        ///     使用 the default rule: <c>&lt;MOD&gt;_&lt;CATEGORY&gt;_&lt;CLR_TYPE_NAME&gt;</c> (normalized).
        /// </summary>
        public static ModelPublicEntryOptions FromTypeName => default;

        internal ModelPublicEntryKind Kind { get; }

        internal string? Value { get; }

        /// <summary>
        ///     Replaces the CLR type-name segment with a stable author-chosen stem (normalized).
        ///     Replaces the CLR type-name segment 带有 a stable author-chosen stem (normalized).
        ///     Final entry: <c>&lt;MOD&gt;_&lt;CATEGORY&gt;_&lt;STEM&gt;</c>.
        ///     中文说明：Final entry: <c>&lt;MOD&gt;_&lt;CATEGORY&gt;_&lt;STEM&gt;</c>.
        /// </summary>
        public static ModelPublicEntryOptions FromStem(string entryStem)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(entryStem);
            return new(ModelPublicEntryKind.Stem, entryStem);
        }

        /// <summary>
        ///     Uses the given public entry string verbatim after normalization (must match the patched entry format).
        ///     使用 the given public entry string verbatim 之后 normalization (must match the patched entry 用于mat).
        /// </summary>
        public static ModelPublicEntryOptions FromFullPublicEntry(string fullPublicEntry)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fullPublicEntry);
            return new(ModelPublicEntryKind.FullEntry, fullPublicEntry);
        }
    }

    internal enum ModelPublicEntryKind
    {
        FromTypeName = 0,
        Stem = 1,
        FullEntry = 2,
    }
}
