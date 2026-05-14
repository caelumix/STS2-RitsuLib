namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Builds parameter maps for <see cref="IFmodOneShotPlayback" /> multi-parameter overloads.
    ///     Builds parameter maps 用于 <c>IFmodOneShotPlayback</c> multi-parameter over加载.
    /// </summary>
    public static class FmodParameterMap
    {
        /// <summary>
        ///     Builds an <see cref="AudioParameterSet" /> for the high-level playback API.
        ///     Builds an <c>AudioParameter设置</c> 用于 the high-level playback API.
        /// </summary>
        public static AudioParameterSet Set(params (string Name, float Value)[] pairs)
        {
            return AudioParameterSet.From(Of(pairs));
        }

        /// <summary>
        ///     Empty parameter map for overloads that require a dictionary instance.
        ///     Empty parameter map 用于 over加载 that require a dictionary instance.
        /// </summary>
        public static Dictionary<string, float> Empty()
        {
            return [];
        }

        /// <summary>
        ///     Single named parameter suitable for one-shot playback helpers.
        ///     Single named parameter suitable 用于 one-shot playback helpers.
        /// </summary>
        public static Dictionary<string, float> Single(string name, float value)
        {
            return new() { [name] = value };
        }

        /// <summary>
        ///     Builds a map from name/value tuples; duplicates last writer wins.
        ///     Builds a map 从 name/value tuples; duplicates last writer wins.
        /// </summary>
        public static Dictionary<string, float> Of(params (string Name, float Value)[] pairs)
        {
            if (pairs.Length == 0)
                return [];

            var d = new Dictionary<string, float>(pairs.Length);
            foreach (var (name, value) in pairs)
                d[name] = value;

            return d;
        }
    }
}
