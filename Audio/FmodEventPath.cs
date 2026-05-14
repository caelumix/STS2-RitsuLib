namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     FMOD Studio event path (e.g. <c>event:/sfx/block_gain</c>). Implicitly converts to <see cref="string" />.
    ///     FMOD Studio 事件 路径 (e.g. <c>事件:/sfx/block_gain</c>). Implicitly converts to <c>string</c>.
    /// </summary>
    /// <param name="Value">
    ///     Raw Studio path string.
    ///     Raw Studio 路径 string.
    /// </param>
    public readonly record struct FmodEventPath(string Value)
    {
        /// <summary>
        ///     True when <see cref="Value" /> is null or empty.
        ///     当 <c>Value</c> is null or empty 时为 true。
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(Value);

        /// <summary>
        ///     Returns the wrapped path string.
        ///     返回 the wrapped path string。
        /// </summary>
        public static implicit operator string(FmodEventPath path)
        {
            return path.Value;
        }

        /// <summary>
        ///     Wraps a string as an <see cref="FmodEventPath" />.
        ///     Wraps a string as an <c>Fmod事件路径</c>.
        /// </summary>
        public static implicit operator FmodEventPath(string value)
        {
            return new(value);
        }

        /// <summary>
        ///     Returns <see cref="Value" />.
        ///     返回 <c>Value</c>。
        /// </summary>
        public override string ToString()
        {
            return Value;
        }
    }
}
