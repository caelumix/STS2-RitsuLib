using Godot;

namespace STS2RitsuLib.Scaffolding.Godot
{
    /// <summary>
    ///     Godot node helpers for packed-scene conversion and procedural roots.
    ///     用于 packed-scene 转换和 procedural root 的 Godot 节点 helper。
    /// </summary>
    public static class RitsuGodotNodeExtensions
    {
        /// <summary>
        ///     Adds <paramref name="child" /> with <see cref="Node.UniqueNameInOwner" /> so it resolves via
        ///     <c>GetNode("%Name")</c>.
        ///     添加带 <c>Node.UniqueNameInOwner</c> 的 <c>child</c>，使其可通过
        ///     <c>GetNode("%Name")</c> 解析。
        /// </summary>
        public static void AddUniqueChild(this Node owner, Node child, string? name = null)
        {
            ArgumentNullException.ThrowIfNull(owner);
            ArgumentNullException.ThrowIfNull(child);

            if (name != null)
                child.Name = name;

            child.UniqueNameInOwner = true;
            owner.AddChild(child);
            child.Owner = owner;
        }
    }
}
