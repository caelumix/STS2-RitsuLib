using Godot;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace STS2RitsuLib.Scaffolding.Godot
{
    /// <summary>
    ///     Non-generic factory entry point used by <see cref="RitsuGodotNodeFactoryRegistry" />.
    ///     由 <c>RitsuGodotNodeFactoryRegistry</c> 使用的非泛型工厂入口。
    /// </summary>
    internal abstract class RitsuGodotNodeFactory
    {
        public abstract Node CreateFromNode(Node source);

        /// <summary>
        ///     Builds a root node without running <see cref="CompleteBareRoot" /> (used for <c>Texture2D</c> → visuals).
        ///     构建根节点但不运行 <c>CompleteBareRoot</c>（用于 <c>Texture2D</c> → visuals）。
        /// </summary>
        public abstract Node CreateBareFromResource(object resource);

        /// <summary>
        ///     Fills unique slots / children for a bare root (same as <c>ConvertScene(target, null)</c>).
        ///     为 bare root 填充 unique slot / child（等同于 <c>ConvertScene(target, null)</c>）。
        /// </summary>
        public abstract void CompleteBareRoot(Node bare);
    }

    /// <summary>
    ///     Describes a named child expected under a converted Godot scene root (unique <c>%Name</c> or path lookup).
    ///     描述转换后的 Godot 场景根节点下期望存在的命名子节点（unique <c>%Name</c> 或路径查找）。
    /// </summary>
    internal interface IRitsuGodotNodeSlot
    {
        string Path { get; }
        bool UniqueName { get; }
        bool MakeNameUnique { get; }
        Type ExpectedNodeType { get; }
        bool IsValidName(Node node);
        bool IsValidType(Node node);
        bool IsValidUnique(Node node);
    }

    /// <summary>
    ///     Slot metadata for <see cref="RitsuGodotNodeFactory{T}" /> (mirrors baselib <c>NodeInfo&lt;T&gt;</c>).
    ///     <c>RitsuGodotNodeFactory{T}</c> 的 slot 元数据（对应 baselib 的 <c>NodeInfo&lt;T&gt;</c>）。
    /// </summary>
    internal sealed record RitsuGodotNodeSlot<TExpected>(string Path, bool MakeNameUnique = true) : IRitsuGodotNodeSlot
        where TExpected : Node
    {
        public StringName StringName { get; } = new(Path.StartsWith('%') ? Path[1..] : Path);
        public bool UniqueName { get; } = Path.StartsWith('%');

        public bool IsValidType(Node node)
        {
            return node is TExpected;
        }

        public bool IsValidName(Node node)
        {
            return node.Name.Equals(StringName);
        }

        public bool IsValidUnique(Node node)
        {
            return UniqueName && node is TExpected && node.Name.Equals(StringName);
        }

        public Type ExpectedNodeType => typeof(TExpected);
    }

    /// <summary>
    ///     Base class for typed procedural / scene conversion factories.
    ///     类型化 procedural / scene 转换工厂的基类。
    /// </summary>
    internal abstract class RitsuGodotNodeFactory<T> : RitsuGodotNodeFactory where T : Node, new()
    {
        protected readonly bool FlexibleStructure;
        protected readonly List<IRitsuGodotNodeSlot> NamedNodes;

        protected RitsuGodotNodeFactory(IEnumerable<IRitsuGodotNodeSlot> namedNodes)
        {
            NamedNodes = namedNodes.ToList();
            FlexibleStructure = NamedNodes.Count == 0 || NamedNodes.All(static s => s.UniqueName);
            RitsuGodotNodeFactoryRegistry.RegisterFactory<T>(this);
        }

        public override Node CreateFromNode(Node source)
        {
            if (source is T typed)
                return typed;

            var target = new T();
            ConvertScene(target, source);
            return target;
        }

        public override Node CreateBareFromResource(object resource)
        {
            return CreateBareFromResourceImpl(resource);
        }

        public override void CompleteBareRoot(Node bare)
        {
            ConvertScene((T)bare, null);
        }

        /// <summary>
        ///     When <paramref name="resource" /> is unsupported, throw with a clear message.
        ///     当不支持 <c>resource</c> 时，抛出带有清晰信息的异常。
        /// </summary>
        protected abstract T CreateBareFromResourceImpl(object resource);

        protected virtual void ConvertScene(T target, Node? source)
        {
            if (source != null)
            {
                target.Name = source.Name;
                switch (target)
                {
                    case Control targetControl when source is Control sourceControl:
                        CopyControlProperties(targetControl, sourceControl);
                        break;
                    case CanvasItem targetItem when source is CanvasItem sourceItem:
                        CopyCanvasItemProperties(targetItem, sourceItem);
                        break;
                }
            }

            TransferAndCreateNodes(target, source);
        }

        protected virtual void TransferAndCreateNodes(T target, Node? source)
        {
            if (source != null)
            {
                if (FlexibleStructure)
                {
                    target.AddChild(source);
                    source.Owner = target;
                    SetChildrenOwner(target, source);
                }
                else
                {
                    foreach (var child in source.GetChildren())
                    {
                        source.RemoveChild(child);
                        ClearSubtreeOwnersForReparent(child);
                        target.AddChild(child);
                        child.Owner = target;
                        SetChildrenOwner(target, child);
                    }

                    source.QueueFree();
                }
            }

            var uniqueNames = new List<IRitsuGodotNodeSlot>();
            var placeholder = new Node();
            foreach (var named in NamedNodes)
            {
                if (named.UniqueName)
                {
                    uniqueNames.Add(named);
                    continue;
                }

                var node = target.GetNodeOrNull(named.Path);
                if (node != null)
                {
                    if (!named.IsValidType(node))
                    {
                        node.ReplaceBy(placeholder);
                        node = ConvertNodeType(node, named.ExpectedNodeType);
                        placeholder.ReplaceBy(node);
                    }

                    if (!named.MakeNameUnique) continue;
                    node.UniqueNameInOwner = true;
                    node.Owner = target;
                }
                else
                {
                    GenerateNode(target, named);
                }
            }

            Dictionary<IRitsuGodotNodeSlot, Node> backupUniqueNodes = [];
            foreach (var child in target.GetChildrenRecursive<Node>())
                for (var index = 0; index < uniqueNames.Count; index++)
                {
                    var unique = uniqueNames[index];
                    if (unique.IsValidName(child))
                        backupUniqueNodes[unique] = child;
                    if (!unique.IsValidUnique(child))
                        continue;

                    child.UniqueNameInOwner = true;
                    child.Owner = target;
                    uniqueNames.RemoveAt(index);
                    break;
                }

            foreach (var missing in uniqueNames)
                if (backupUniqueNodes.TryGetValue(missing, out var node))
                {
                    if (!missing.IsValidType(node))
                    {
                        node.ReplaceBy(placeholder);
                        node = ConvertNodeType(node, missing.ExpectedNodeType);
                        placeholder.ReplaceBy(node);
                    }

                    node.UniqueNameInOwner = true;
                    node.Owner = target;
                }
                else
                {
                    GenerateNode(target, missing);
                }

            placeholder.QueueFree();
        }

        protected virtual Node ConvertNodeType(Node node, Type targetType)
        {
            throw new InvalidOperationException(
                $"Factory for {typeof(T).Name} cannot convert {node.GetType().Name} '{node.Name}' to {targetType.Name}.");
        }

        protected abstract void GenerateNode(T target, IRitsuGodotNodeSlot required);

        /// <summary>
        ///     Packed-scene children often still reference the old root as <see cref="Node.Owner" /> after
        ///     <c>RemoveChild</c>. Godot warns and can break unique-name resolution if reparenting under a new root with
        ///     the same scene name without clearing first (matches Godot log: inconsistent owner).
        ///     packed scene 的子节点在 <c>RemoveChild</c> 后经常仍把旧根节点作为 <c>Node.Owner</c>。
        ///     如果不先清空 owner 就 reparent 到同名的新根节点下，Godot 会警告并且可能破坏 unique-name 解析
        ///     （对应 Godot 日志：inconsistent owner）。
        /// </summary>
        private static void ClearSubtreeOwnersForReparent(Node node)
        {
            foreach (var descendant in node.GetChildren())
                ClearSubtreeOwnersForReparent(descendant);

            node.Owner = null;
        }

        protected static void SetChildrenOwner(Node target, Node child)
        {
            foreach (var grandchild in child.GetChildren())
            {
                grandchild.Owner = target;
                SetChildrenOwner(target, grandchild);
            }
        }

        protected static void CopyControlProperties(Control target, Control source)
        {
            CopyCanvasItemProperties(target, source);
            target.LayoutMode = source.LayoutMode;
            target.AnchorLeft = source.AnchorLeft;
            target.AnchorTop = source.AnchorTop;
            target.AnchorRight = source.AnchorRight;
            target.AnchorBottom = source.AnchorBottom;
            target.OffsetLeft = source.OffsetLeft;
            target.OffsetTop = source.OffsetTop;
            target.OffsetRight = source.OffsetRight;
            target.OffsetBottom = source.OffsetBottom;
            target.GrowHorizontal = source.GrowHorizontal;
            target.GrowVertical = source.GrowVertical;
            target.Size = source.Size;
            target.CustomMinimumSize = source.CustomMinimumSize;
            target.PivotOffset = source.PivotOffset;
            target.MouseFilter = source.MouseFilter;
            target.FocusMode = source.FocusMode;
            target.ClipContents = source.ClipContents;
        }

        protected static void CopyCanvasItemProperties(CanvasItem target, CanvasItem source)
        {
            target.Visible = source.Visible;
            target.Modulate = source.Modulate;
            target.SelfModulate = source.SelfModulate;
            target.ShowBehindParent = source.ShowBehindParent;
            target.TopLevel = source.TopLevel;
            target.ZIndex = source.ZIndex;
            target.ZAsRelative = source.ZAsRelative;
            target.YSortEnabled = source.YSortEnabled;
            target.TextureFilter = source.TextureFilter;
            target.TextureRepeat = source.TextureRepeat;
            target.Material = source.Material;
            target.UseParentMaterial = source.UseParentMaterial;

            if (target is not Node2D targetNode2D || source is not Node2D sourceNode2D) return;
            targetNode2D.Position = sourceNode2D.Position;
            targetNode2D.Rotation = sourceNode2D.Rotation;
            targetNode2D.Scale = sourceNode2D.Scale;
            targetNode2D.Skew = sourceNode2D.Skew;
        }
    }
}
