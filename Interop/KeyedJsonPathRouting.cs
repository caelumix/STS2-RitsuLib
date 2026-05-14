namespace STS2RitsuLib.Interop
{
    /// <summary>
    ///     Optional JSON Pointer lists used by <see cref="KeyedJsonDomTransport" /> for subtree sync.
    ///     <c>KeyedJsonDomTransport</c> 用于 subtree sync 的可选 JSON Pointer 列表。
    /// </summary>
    /// <param name="PullPaths">
    ///     Pointers consumed when pulling from a keyed provider via node getters.
    ///     通过 node getter 从 keyed provider pull 时使用的 pointer。
    /// </param>
    /// <param name="PushPaths">
    ///     Pointers consumed when pushing document subtrees via node setters.
    ///     通过 node setter push document subtree 时使用的 pointer。
    /// </param>
    /// <param name="MergePushPaths">
    ///     Pointers consumed when pushing RFC 7386 merge payloads via merge-at hooks.
    ///     通过 merge-at hook push RFC 7386 merge payload 时使用的 pointer。
    /// </param>
    public sealed record KeyedJsonPathRouting(string[]? PullPaths, string[]? PushPaths, string[]? MergePushPaths);
}
