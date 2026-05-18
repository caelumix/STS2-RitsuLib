using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.Lifecycle.Patches
{
    /// <summary>
    ///     After the main menu node is ready, publishes the shared main-menu lifecycle event (deferred).
    ///     主菜单节点就绪后，延迟发布共享主菜单生命周期事件。
    /// </summary>
    public class NMainMenuReadyLifecyclePatch : IPatchMethod
    {
        /// <inheritdoc />
        public static string PatchId => "nmain_menu_ready_lifecycle";

        /// <inheritdoc />
        public static string Description =>
            "Main menu: publish deferred RitsuLib main-menu-ready lifecycle event";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NMainMenu), "_Ready")];
        }

        /// <summary>
        ///     Harmony postfix: schedule the lifecycle event after the menu finishes its ready work.
        ///     Harmony postfix：在菜单完成 ready 工作后调度生命周期事件。
        /// </summary>
        public static void Postfix()
        {
            Callable.From(PublishMainMenuReady).CallDeferred();
        }

        private static void PublishMainMenuReady()
        {
            RitsuLibFramework.PublishLifecycleEvent(
                new MainMenuReadyEvent(DateTimeOffset.UtcNow),
                nameof(MainMenuReadyEvent));
        }
    }
}
