namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Immutable parameter bag used by the high-level playback API.
    ///     Immutable parameter bag used 通过 the high-level playback API.
    /// </summary>
    public sealed class AudioParameterSet
    {
        private AudioParameterSet(IReadOnlyDictionary<string, float> values)
        {
            Values = values;
        }

        /// <summary>
        ///     Empty parameter set.
        ///     Empty parameter 设置.
        /// </summary>
        public static AudioParameterSet Empty { get; } = new(new Dictionary<string, float>());

        /// <summary>
        ///     Parameter values carried by this set.
        ///     Parameter values carried 通过 this 设置.
        /// </summary>
        public IReadOnlyDictionary<string, float> Values { get; }

        /// <summary>
        ///     Creates a parameter set from an existing dictionary.
        ///     创建 a parameter set from an existing dictionary。
        /// </summary>
        public static AudioParameterSet From(IReadOnlyDictionary<string, float>? values)
        {
            if (values is null || values.Count == 0)
                return Empty;

            return new(new Dictionary<string, float>(values));
        }

        /// <summary>
        ///     Returns a new parameter set with the given name/value applied.
        ///     返回 a new parameter set with the given name/value applied。
        /// </summary>
        public AudioParameterSet With(string name, float value)
        {
            var next = new Dictionary<string, float>(Values)
            {
                [name] = value,
            };
            return new(next);
        }
    }
}
