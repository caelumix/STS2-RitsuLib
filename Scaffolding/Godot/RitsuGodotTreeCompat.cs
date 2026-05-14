using Godot;
using MegaCrit.Sts2.Core.Nodes;

namespace STS2RitsuLib.Scaffolding.Godot
{
    /// <summary>
    ///     Tree mutations aligned with vanilla <c>GodotTreeExtensions</c> (0.104.x). Game 0.103.2 omits
    ///     <c>MoveChildSafely</c>; referencing it from mods breaks multi-version builds. Use these helpers instead of
    ///     calling game extension methods for merchant-booth style layout.
    ///     与原版 <c>GodotTreeExtensions</c>（0.104.x）对齐的树变更 helper。游戏 0.103.2 缺少
    ///     <c>MoveChildSafely</c>；mod 直接引用它会破坏多版本构建。merchant-booth 风格布局请使用这些 helper，
    ///     不要直接调用游戏扩展方法。
    /// </summary>
    public static class RitsuGodotTreeCompat
    {
        /// <summary>
        ///     Same branching as <c>MegaCrit.Sts2.Core.Helpers.GodotTreeExtensions.AddChildSafely</c> on current game
        ///     branches that ship it.
        ///     与当前游戏分支中提供的 <c>MegaCrit.Sts2.Core.Helpers.GodotTreeExtensions.AddChildSafely</c>
        ///     使用相同分支逻辑。
        /// </summary>
        public static void AddChildSafely(Node parent, Node? child)
        {
            if (child == null || !GodotObject.IsInstanceValid(parent))
                return;

            if (NGame.IsMainThread() && (parent.IsNodeReady() || !parent.IsInsideTree()))
            {
                parent.AddChild(child);
                return;
            }

            parent.CallDeferred(Node.MethodName.AddChild, child);
        }

        /// <summary>
        ///     Same branching as <c>MegaCrit.Sts2.Core.Helpers.GodotTreeExtensions.MoveChildSafely</c> where the game
        ///     provides it (0.104+); self-contained for 0.103.2 reference assemblies.
        ///     在游戏提供 <c>MegaCrit.Sts2.Core.Helpers.GodotTreeExtensions.MoveChildSafely</c> 的版本中（0.104+）
        ///     与其使用相同分支逻辑；同时保持对 0.103.2 引用程序集的自包含兼容。
        /// </summary>
        public static void MoveChildSafely(Node parent, Node? child, int index)
        {
            if (child == null || !GodotObject.IsInstanceValid(parent))
                return;

            if (NGame.IsMainThread() && (parent.IsNodeReady() || !parent.IsInsideTree()))
            {
                parent.MoveChild(child, index);
                return;
            }

            parent.CallDeferred(Node.MethodName.MoveChild, child, index);
        }
    }
}
