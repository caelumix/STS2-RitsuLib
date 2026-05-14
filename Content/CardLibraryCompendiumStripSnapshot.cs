using Godot;

namespace STS2RitsuLib.Content
{
    /// <summary>
    ///     Internal: siblings of the compendium filter parent captured before RitsuLib inserts mod pool filters.
    ///     Internal: siblings of the compendium 过滤 parent captured 之前 RitsuLib inserts mod pool 过滤.
    ///     Resolves snapshot indices for vanilla anchors; mod authors should use
    ///     解析 snapshot indices 用于 原版 anchors; mod authors should 使用
    ///     <see cref="CardLibraryCompendiumVanillaFilterNames" /> in placement rules, not this type.
    /// </summary>
    internal sealed class CardLibraryCompendiumStripSnapshot
    {
        private CardLibraryCompendiumStripSnapshot(IReadOnlyList<Node> siblingsInOrder)
        {
            OriginalSiblingsInOrder = siblingsInOrder;
        }

        /// <summary>
        ///     Original child sequence under the filter parent (0 = leftmost in the compendium strip as built by
        ///     Original child sequence under the 过滤 parent (0 = leftmost in the compendium strip as built by
        ///     the base game, before RitsuLib inserts any mod filter nodes).
        ///     该 base game, before RitsuLib inserts any mod filter nodes)。
        /// </summary>
        public IReadOnlyList<Node> OriginalSiblingsInOrder { get; }

        /// <summary>
        ///     Sibling count at snapshot time. Valid insertion indices for “into this list” simulation range from
        ///     Sibling count at snapshot time. Valid insertion indices 用于 “into this list” simulation range 从
        ///     0 to <c>Count</c> (inclusive of appending as <c>Count</c>).
        ///     中文说明：0 to <c>Count</c> (inclusive of appending as <c>Count</c>).
        /// </summary>
        public int Count => OriginalSiblingsInOrder.Count;

        /// <summary>
        ///     Captures the current <paramref name="filterParent" /> children as an ordered list.
        ///     Captures the current <c>过滤Parent</c> children as an ordered list.
        /// </summary>
        public static CardLibraryCompendiumStripSnapshot Capture(Node filterParent)
        {
            ArgumentNullException.ThrowIfNull(filterParent);
            var n = filterParent.GetChildCount();
            var list = new List<Node>(n);
            for (var i = 0; i < n; i++)
                list.Add(filterParent.GetChild(i));
            return new(list);
        }

        /// <summary>
        ///     Returns the sibling index of <paramref name="node" /> if it is a direct child in this snapshot
        ///     返回 the sibling index of <c>node</c> 如果 it is a direct child in this snapshot
        ///     (reference match).
        ///     中文说明：(reference match).
        /// </summary>
        public bool TryGetIndexOfNode(Node? node, out int index)
        {
            if (node is null)
            {
                index = -1;
                return false;
            }

            for (var i = 0; i < OriginalSiblingsInOrder.Count; i++)
                if (ReferenceEquals(OriginalSiblingsInOrder[i], node))
                {
                    index = i;
                    return true;
                }

            index = -1;
            return false;
        }
    }
}
