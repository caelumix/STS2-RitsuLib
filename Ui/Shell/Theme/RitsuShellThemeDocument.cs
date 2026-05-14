using System.Text.Json;
using System.Text.Json.Serialization;

namespace STS2RitsuLib.Ui.Shell.Theme
{
    /// <summary>
    ///     Deserialized W3C Design Tokens Format Module document for a shell theme. Group nodes are nested
    ///     Deserialized W3C Design Tokens Format Module document 用于 a shell theme. Group nodes are nested
    ///     dictionaries; leaf tokens carry <c>$value</c>, <c>$type</c>, and optional <c>$description</c>.
    ///     dictionaries; leaf tokens carry <c>$value</c>, <c>$type</c>, 和 可选 <c>$description</c>.
    /// </summary>
    public sealed class RitsuShellThemeDocument
    {
        private static readonly Lazy<JsonSerializerOptions> DefaultJsonOptions = new(() => new()
        {
            PropertyNameCaseInsensitive = true,
        });

        /// <summary>
        ///     Optional <c>$schema</c> URL for editors.
        ///     可选 <c>$schema</c> URL 用于 editors.
        /// </summary>
        [JsonPropertyName("$schema")]
        public string? SchemaReference { get; set; }

        /// <summary>
        ///     Theme format version (currently <c>1</c>).
        ///     Theme 用于mat version (currently <c>1</c>).
        /// </summary>
        [JsonPropertyName("themeFormatVersion")]
        public int? ThemeFormatVersion { get; set; }

        /// <summary>
        ///     Theme content revision for disk auto-upgrade. Higher values replace older disk copies.
        ///     Theme content revision 用于 disk auto-upgrade. Higher values replace older disk copies.
        /// </summary>
        [JsonPropertyName("themeVersion")]
        public int? ThemeVersion { get; set; }

        /// <summary>
        ///     Theme id (lowercase identifier).
        ///     中文说明：Theme id (lowercase identifier).
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        /// <summary>
        ///     Human-readable display name shown in pickers.
        ///     人类可读的 display name shown in pickers。
        /// </summary>
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        ///     Optional parent theme id; this theme is layered on top.
        ///     可选 parent theme id; this theme is layered on top.
        /// </summary>
        [JsonPropertyName("inherits")]
        public string? Inherits { get; set; }

        /// <summary>
        ///     Primitive tokens (raw values).
        ///     中文说明：Primitive tokens (raw values).
        /// </summary>
        [JsonPropertyName("core")]
        public JsonElement? Core { get; set; }

        /// <summary>
        ///     Semantic / alias tokens.
        ///     中文说明：Semantic / alias tokens.
        /// </summary>
        [JsonPropertyName("semantic")]
        public JsonElement? Semantic { get; set; }

        /// <summary>
        ///     Component tokens (component → variant → state).
        ///     中文说明：Component tokens (component → variant → state).
        /// </summary>
        [JsonPropertyName("components")]
        public JsonElement? Components { get; set; }

        /// <summary>
        ///     Per-scope overrides (<c>shell</c> / <c>modSettings</c> / <c>mod:&lt;modId&gt;</c>).
        ///     Per-scope overrides (<c>shell</c> / <c>mod设置</c> / <c>mod:&lt;modId&gt;</c>).
        /// </summary>
        [JsonPropertyName("scopes")]
        public Dictionary<string, JsonElement>? Scopes { get; set; }

        /// <summary>
        ///     Free-form mod extension blobs keyed by mod id.
        ///     Free-用于m mod extension blobs keyed 通过 mod id.
        /// </summary>
        [JsonPropertyName("extensions")]
        public Dictionary<string, JsonElement>? Extensions { get; set; }

        /// <summary>
        ///     Deserializes a <see cref="RitsuShellThemeDocument" /> from a JSON stream (case-insensitive properties).
        ///     Deserializes a <c>RitsuShellThemeDocument</c> 从 a JSON stream (case-insensitive properties).
        /// </summary>
        /// <param name="stream">
        ///     Input JSON stream.
        ///     中文说明：Input JSON stream.
        /// </param>
        /// <returns>
        ///     The deserialized document, or <see langword="null" /> on failure.
        ///     该 deserialized document, or <see langword="null" /> on failure。
        /// </returns>
        public static RitsuShellThemeDocument? Deserialize(Stream stream)
        {
            return JsonSerializer.Deserialize<RitsuShellThemeDocument>(stream, DefaultJsonOptions.Value);
        }
    }
}
