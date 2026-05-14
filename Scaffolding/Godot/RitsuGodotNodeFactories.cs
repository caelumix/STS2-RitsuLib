using Godot;
using MegaCrit.Sts2.Core.Assets;

namespace STS2RitsuLib.Scaffolding.Godot
{
    /// <summary>
    ///     Explicit-only Godot node construction: call these from your own code paths. Does not patch global
    ///     <c>PackedScene.Instantiate</c>, so baselib scene conversion and vanilla loading keep exclusive control of their
    ///     hooks.
    ///     仅显式调用的 Godot 节点构造：从你自己的代码路径调用这些方法。它不会 patch 全局
    ///     <c>PackedScene.Instantiate</c>，因此 baselib 场景转换和原版加载仍独占控制自己的 hook。
    /// </summary>
    public static class RitsuGodotNodeFactories
    {
        /// <summary>
        ///     Creates <typeparamref name="TNode" /> from a loaded resource (e.g. <see cref="Texture2D" /> for creature /
        ///     merchant factories).
        ///     从已加载资源创建 <c>TNode</c>（例如 creature / merchant 工厂使用
        ///     <see cref="Texture2D" />）。
        /// </summary>
        public static TNode CreateFromResource<TNode>(object resource) where TNode : Node, new()
        {
            return RitsuGodotNodeFactoryRegistry.CreateFromResource<TNode>(resource);
        }

        /// <summary>
        ///     Instantiates <paramref name="scene" /> and runs the registered factory to produce <typeparamref name="TNode" />.
        ///     实例化 <c>scene</c>，并运行已注册工厂来生成 <c>TNode</c>。
        /// </summary>
        public static TNode CreateFromScene<TNode>(PackedScene scene) where TNode : Node, new()
        {
            return RitsuGodotNodeFactoryRegistry.CreateFromScene<TNode>(scene);
        }

        /// <summary>
        ///     Same as <see cref="CreateFromScene{TNode}(PackedScene)" /> but uses the given Godot instantiate edit
        ///     state (match vanilla callsites such as <c>PackedScene.GenEditState.Disabled</c>).
        ///     与 <c>CreateFromScene{TNode}(PackedScene)</c> 相同，但使用给定的 Godot instantiate edit
        ///     state（匹配 <c>PackedScene.GenEditState.Disabled</c> 等原版调用点）。
        /// </summary>
        public static TNode CreateFromScene<TNode>(PackedScene scene, PackedScene.GenEditState editState)
            where TNode : Node, new()
        {
            return RitsuGodotNodeFactoryRegistry.CreateFromScene<TNode>(scene, editState);
        }

        /// <summary>
        ///     Loads <paramref name="scenePath" /> via <see cref="PreloadManager.Cache" /> then
        ///     <see>
        ///         <cref>CreateFromScene{TNode}</cref>
        ///     </see>
        ///     .
        ///     通过 <c>PreloadManager.Cache</c> 加载 <c>scenePath</c>，然后调用
        ///     <see>
        ///         <cref>CreateFromScene{TNode}</cref>
        ///     </see>
        ///     。
        /// </summary>
        public static TNode CreateFromScenePath<TNode>(string scenePath) where TNode : Node, new()
        {
            return RitsuGodotNodeFactoryRegistry.CreateFromScenePath<TNode>(scenePath);
        }

        /// <inheritdoc cref="CreateFromScene{TNode}(PackedScene, PackedScene.GenEditState)" />
        public static TNode CreateFromScenePath<TNode>(string scenePath, PackedScene.GenEditState editState)
            where TNode : Node, new()
        {
            return RitsuGodotNodeFactoryRegistry.CreateFromScenePath<TNode>(scenePath, editState);
        }
    }
}
