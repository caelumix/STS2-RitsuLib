using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;

namespace STS2RitsuLib.Screens
{
    /// <summary>
    ///     Lightweight mod-facing façade around <see cref="NCapstoneContainer" /> for opening and closing
    ///     Lightweight mod-facing façade around <c>NCapstoneContainer</c> 用于 opening 和 closing
    ///     custom <see cref="ICapstoneScreen" />s without depending on any specific ritsulib subsystem
    ///     自定义 <c>ICapstoneScreen</c>s 带有out depending on any specific ritsulib subsystem
    ///     (card piles, top-bar buttons, combat commands, …). This is the single public API any mod code
    ///     (卡牌 piles, top-bar buttons, combat commands, …). This is the single public API any mod code
    ///     should use when it wants to mount a screen as a full-page overlay.
    ///     should 使用 当 it wants to mount a screen as a full-page overlay.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The capstone container is a scene-owned singleton; during menus / between runs it may not
    ///         The capstone container is a 场景-owned singleton; 期间 menus / between runs it may not
    ///         yet exist. The helpers here therefore silently no-op when <see cref="NCapstoneContainer.Instance" />
    ///         yet exist. The helpers here therefore silently no-op 当 <c>NCapstoneContainer.Instance</c>
    ///         is null so callers do not need to guard every invocation.
    ///         中文说明：is null so callers do not need to guard every invocation.
    ///     </para>
    ///     <para>
    ///         When a capstone screen is already showing, <see cref="Open" /> closes it first so the new
    ///         当 a capstone screen is already showing, <c>Open</c> closes it first so the new
    ///         screen can take the stage — matching the behaviour users expect when clicking "view"-style
    ///         screen can take the stage — matching the behaviour 使用rs expect 当 clicking "view"-style
    ///         top-bar buttons that toggle or swap screens.
    ///         top-bar buttons that toggle 或 swap screens.
    ///     </para>
    /// </remarks>
    public static class ModScreenService
    {
        /// <summary>
        ///     The screen currently owning the capstone container, or null when the container is idle / not
        ///     The screen currently owning the capstone container, 或 null 当 the container is idle / not
        ///     yet instantiated.
        ///     中文说明：yet instantiated.
        /// </summary>
        public static ICapstoneScreen? CurrentCapstoneScreen => NCapstoneContainer.Instance?.CurrentCapstoneScreen;

        /// <summary>
        ///     True when a capstone is visible right now.
        ///     当 a capstone is visible right now 时为 true。
        /// </summary>
        public static bool IsCapstoneOpen => NCapstoneContainer.Instance is { InUse: true };

        /// <summary>
        ///     Mounts <paramref name="screen" /> inside <see cref="NCapstoneContainer" />. If a different
        ///     中文说明：Mounts <c>screen</c> inside <c>NCapstoneContainer</c>. If a different
        ///     capstone is already open, it is closed first; if the same instance is already mounted this
        ///     capstone is already open, it is closed first; 如果 the same instance is already mounted this
        ///     is a no-op.
        ///     中文说明：is a no-op.
        /// </summary>
        /// <param name="screen">
        ///     Screen to mount (must also be a Godot <see cref="Node" />).
        ///     中文说明：Screen to mount (must also be a Godot <c>Node</c>).
        /// </param>
        /// <returns>
        ///     True when the screen was mounted; false when no container is available.
        ///     当 the screen was mounted; false when no container is available 时为 true。
        /// </returns>
        public static bool Open(ICapstoneScreen screen)
        {
            ArgumentNullException.ThrowIfNull(screen);

            var container = NCapstoneContainer.Instance;
            if (container == null)
                return false;

            if (ReferenceEquals(container.CurrentCapstoneScreen, screen))
                return true;

            if (container.InUse)
                container.Close();

            container.Open(screen);
            return true;
        }

        /// <summary>
        ///     Closes the current capstone, if any. Returns true when a close actually happened.
        ///     Closes the current capstone, 如果 any. 返回 true 当 a close actually happened.
        /// </summary>
        public static bool Close()
        {
            var container = NCapstoneContainer.Instance;
            if (container is not { InUse: true })
                return false;

            container.Close();
            return true;
        }

        /// <summary>
        ///     Convenience toggle: if <paramref name="screen" /> is already the current capstone, close it;
        ///     Convenience toggle: 如果 <c>screen</c> is already the current capstone, close it;
        ///     otherwise open it.
        ///     中文说明：otherwise open it.
        /// </summary>
        public static bool Toggle(ICapstoneScreen screen)
        {
            ArgumentNullException.ThrowIfNull(screen);

            return ReferenceEquals(CurrentCapstoneScreen, screen) ? Close() : Open(screen);
        }
    }
}
