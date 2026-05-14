using System.Text.Json;

namespace STS2RitsuLib.Ui.Shell.Theme
{
    /// <summary>
    ///     Deep-merges DTFM token trees while preserving leaf semantics.
    ///     中文说明：Deep-merges DTFM token trees while preserving leaf semantics.
    /// </summary>
    /// <remarks>
    ///     Group nodes (objects without <c>$value</c>) are merged recursively; leaf nodes (objects carrying
    ///     Group nodes (objects 带有out <c>$value</c>) are merged recursively; leaf nodes (objects carrying
    ///     <c>$value</c>) are replaced wholesale. Arrays and scalars are replaced wholesale.
    /// </remarks>
    internal static class RitsuShellThemeMerger
    {
        /// <summary>
        ///     Merges <paramref name="overlay" /> on top of <paramref name="baseTree" /> in-place. New keys are added,
        ///     中文说明：Merges <c>overlay</c> on top of <c>baseTree</c> in-place. New keys are added,
        ///     overlapping keys recurse for groups and replace for leaves.
        ///     overlapping keys recurse 用于 groups 和 replace 用于 leaves.
        /// </summary>
        /// <param name="baseTree">
        ///     The mutable base dictionary (modified in place).
        ///     该 mutable base dictionary (modified in place)。
        /// </param>
        /// <param name="overlay">
        ///     The overlay JSON element (must be an object).
        ///     该 overlay JSON element (must be an object)。
        /// </param>
        public static void MergeInto(Dictionary<string, object?> baseTree, JsonElement overlay)
        {
            if (overlay.ValueKind != JsonValueKind.Object)
                return;

            foreach (var pair in overlay.EnumerateObject())
            {
                var key = pair.Name;
                var value = pair.Value;

                if (IsLeafToken(value))
                {
                    baseTree[key] = CloneLeaf(value);
                    continue;
                }

                if (value.ValueKind != JsonValueKind.Object)
                {
                    baseTree[key] = ClonePrimitive(value);
                    continue;
                }

                if (baseTree.TryGetValue(key, out var existing) &&
                    existing is Dictionary<string, object?> existingGroup)
                {
                    MergeInto(existingGroup, value);
                    continue;
                }

                var newGroup = new Dictionary<string, object?>(StringComparer.Ordinal);
                MergeInto(newGroup, value);
                baseTree[key] = newGroup;
            }
        }

        /// <summary>
        ///     Determines whether a JSON object is a DTFM leaf token (<c>$value</c> present).
        ///     中文说明：Determines whether a JSON object is a DTFM leaf token (<c>$value</c> present).
        /// </summary>
        public static bool IsLeafToken(JsonElement element)
        {
            return element.ValueKind == JsonValueKind.Object && element.TryGetProperty("$value", out _);
        }

        private static object? ClonePrimitive(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => element.Clone(),
            };
        }

        private static LeafToken CloneLeaf(JsonElement element)
        {
            string? type = null;
            string? description = null;
            object? value = null;
            JsonElement? extensions = null;

            foreach (var prop in element.EnumerateObject())
                switch (prop.Name)
                {
                    case "$type":
                        type = prop.Value.GetString();
                        break;
                    case "$description":
                        description = prop.Value.GetString();
                        break;
                    case "$value":
                        value = ClonePrimitive(prop.Value);
                        break;
                    case "$extensions":
                        extensions = prop.Value.Clone();
                        break;
                }

            return new(value, type, description, extensions);
        }
    }

    /// <summary>
    ///     Cloned representation of a DTFM leaf token after merging. Holds the raw <c>$value</c> as a CLR primitive
    ///     Cloned representation of a DTFM leaf token 之后 merging. Holds the raw <c>$value</c> as a CLR primitive
    ///     (string, long, double, bool) and the <c>$type</c> hint.
    ///     (string, long, double, bool) 和 the <c>$type</c> hint.
    /// </summary>
    /// <param name="Value">
    ///     Raw value (string with optional <c>{ref}</c>, number, or boolean).
    ///     Raw value (string 带有 可选 <c>{ref}</c>, number, 或 boolean).
    /// </param>
    /// <param name="Type">
    ///     Token type hint (e.g. <c>color</c>, <c>dimension</c>, <c>fontFamily</c>).
    ///     中文说明：Token type hint (e.g. <c>color</c>, <c>dimension</c>, <c>fontFamily</c>).
    /// </param>
    /// <param name="Description">
    ///     Optional human description.
    ///     可选 human description.
    /// </param>
    /// <param name="Extensions">
    ///     Optional <c>$extensions</c> blob (vendor metadata).
    ///     可选 <c>$extensions</c> blob (vendor metadata).
    /// </param>
    internal sealed record LeafToken(object? Value, string? Type, string? Description, JsonElement? Extensions);
}
