using System.Text.Json.Nodes;

namespace STS2RitsuLib.Models.Capabilities
{
    /// <summary>
    ///     Serializable document for model components.
    ///     模型组件的可序列化文档。
    /// </summary>
    public sealed class ModelComponentSaveDocument
    {
        /// <summary>
        ///     Component entries in display/execution order.
        ///     按显示/执行顺序排列的组件条目。
        /// </summary>
        public List<ModelComponentSaveEntry> Components { get; set; } = [];
    }

    /// <summary>
    ///     Serializable state for one model component.
    ///     单个模型组件的可序列化状态。
    /// </summary>
    public sealed class ModelComponentSaveEntry
    {
        /// <summary>
        ///     Stable component id.
        ///     稳定组件 ID。
        /// </summary>
        public string Id { get; set; } = "";

        /// <summary>
        ///     Component state schema version.
        ///     组件状态 schema 版本。
        /// </summary>
        public int Schema { get; set; } = 1;

        /// <summary>
        ///     Serialized component state.
        ///     序列化组件状态。
        /// </summary>
        public JsonNode? Data { get; set; }
    }
}
