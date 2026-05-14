using Godot;

namespace STS2RitsuLib.Scaffolding.Godot
{
    /// <summary>
    ///     Packs a live node tree into a <see cref="PackedScene" /> when an API requires a scene resource (for example event
    ///     layout).
    ///     当 API 需要场景资源时（例如事件布局），将 live node tree 打包为 <c>PackedScene</c>。
    /// </summary>
    public static class RitsuGodotPackedSceneHelper
    {
        /// <summary>
        ///     Packs <paramref name="root" /> into a new <see cref="PackedScene" />, or returns <c>null</c> if packing fails.
        ///     将 <c>root</c> 打包到新的 <c>PackedScene</c>；打包失败时返回 <c>null</c>。
        /// </summary>
        public static PackedScene? PackRootOrNull(Node root)
        {
            ArgumentNullException.ThrowIfNull(root);
            var packed = new PackedScene();
            return packed.Pack(root) == Error.Ok ? packed : null;
        }
    }
}
