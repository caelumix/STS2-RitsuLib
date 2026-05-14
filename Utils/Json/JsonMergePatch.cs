using System.Text.Json;
using System.Text.Json.Nodes;

namespace STS2RitsuLib.Utils.Json
{
    /// <summary>
    ///     RFC 7386 JSON Merge Patch utilities for <see cref="JsonNode" /> DOM.
    ///     RFC 7386 JSON Merge Patch utilities 用于 <c>JsonNode</c> DOM.
    /// </summary>
    public static class JsonMergePatch
    {
        /// <summary>
        ///     Applies an RFC 7386 merge patch to <paramref name="target" /> and returns the merged result.
        ///     Applies an RFC 7386 merge patch to <c>target</c> 和 返回 the merged result.
        ///     When <paramref name="patch" /> is not an object, the result is the patch itself (replacement).
        ///     当 <c>patch</c> is not an object, the result is the patch itself (replacement).
        /// </summary>
        public static JsonNode? Apply(JsonNode? target, JsonNode? patch)
        {
            if (!TryGetObject(patch, out var patchObj))
                return IsJsonNull(patch) ? null : patch?.DeepClone();

            var output = TryGetObject(target, out var targetObj)
                ? targetObj.DeepClone() as JsonObject ?? new JsonObject()
                : new();
            ApplyInPlace(output, patchObj);
            return output;
        }

        /// <summary>
        ///     Applies an RFC 7386 merge patch to <paramref name="target" /> in-place.
        ///     中文说明：Applies an RFC 7386 merge patch to <c>target</c> in-place.
        /// </summary>
        public static void ApplyInPlace(JsonObject target, JsonObject patch)
        {
            ArgumentNullException.ThrowIfNull(target);
            ArgumentNullException.ThrowIfNull(patch);

            foreach (var kv in patch)
            {
                if (IsJsonNull(kv.Value))
                {
                    target.Remove(kv.Key);
                    continue;
                }

                if (TryGetObject(kv.Value, out var patchChild) &&
                    target.TryGetPropertyValue(kv.Key, out var existing) &&
                    TryGetObject(existing, out var existingObj))
                {
                    ApplyInPlace(existingObj, patchChild);
                    continue;
                }

                target[kv.Key] = kv.Value!.DeepClone();
            }
        }

        private static bool TryGetObject(JsonNode? node, out JsonObject obj)
        {
            obj = node as JsonObject ?? null!;
            return node is JsonObject;
        }

        private static bool IsJsonNull(JsonNode? node)
        {
            if (node == null)
                return true;

            return node.GetValueKind() == JsonValueKind.Null;
        }
    }
}
