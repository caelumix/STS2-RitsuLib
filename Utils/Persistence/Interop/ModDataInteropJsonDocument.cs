using System.Text.Json.Nodes;

namespace STS2RitsuLib.Utils.Persistence.Interop
{
    /// <summary>
    ///     JSON DOM persistence payload compatible with ModDataStore generic constraints.
    ///     JSON DOM persistence payload compatible 带有 ModDataStore generic constraints.
    ///     The logical document lives under <see cref="Root" />; file JSON is this wrapper plus optional schema version
    ///     The logical document lives under <c>Root</c>; file JSON is this wrapper plus 可选 schema version
    ///     fields.
    ///     中文说明：fields.
    /// </summary>
    public sealed class ModDataInteropJsonDocument
    {
        /// <summary>
        ///     Primary JSON DOM migrated and synchronized with interop providers.
        ///     Primary JSON DOM migrated 和 synchronized 带有 interop providers.
        /// </summary>
        public JsonNode? Root { get; set; } = new JsonObject();
    }
}
