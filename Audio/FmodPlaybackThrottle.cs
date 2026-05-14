using System.Diagnostics;

namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Per-key cooldown for rapid triggers (ms, via <see cref="Stopwatch" /> ticks).
    ///     Per-key cooldown 用于 rapid triggers (ms, via <c>Stopwatch</c> ticks).
    /// </summary>
    public static class FmodPlaybackThrottle
    {
        private static readonly Lock Gate = new();
        private static readonly Dictionary<string, long> LastTicks = new(StringComparer.Ordinal);

        /// <summary>
        ///     Returns false if <paramref name="key" /> was used within <paramref name="cooldownMs" />.
        ///     返回 false if <c>key</c> was used within <c>cooldownMs</c>。
        /// </summary>
        public static bool TryEnter(string key, int cooldownMs)
        {
            if (cooldownMs <= 0)
                return true;

            var now = Stopwatch.GetTimestamp();
            var threshold = (long)(cooldownMs * Stopwatch.Frequency / 1000.0);

            lock (Gate)
            {
                if (LastTicks.TryGetValue(key, out var last) && now - last < threshold)
                    return false;

                LastTicks[key] = now;
                return true;
            }
        }

        /// <summary>
        ///     Removes cooldown state for <paramref name="key" /> so the next <see cref="TryEnter" /> may pass.
        ///     Removes cooldown state 用于 <c>key</c> so the next <c>TryEnter</c> may pass.
        /// </summary>
        public static void Clear(string key)
        {
            lock (Gate)
            {
                LastTicks.Remove(key);
            }
        }

        /// <summary>
        ///     Clears all throttle keys.
        ///     中文说明：Clears all throttle keys.
        /// </summary>
        public static void ClearAll()
        {
            lock (Gate)
            {
                LastTicks.Clear();
            }
        }
    }
}
