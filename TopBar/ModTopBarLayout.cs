using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2RitsuLib.CardPiles.Nodes;
using NVec2 = System.Numerics.Vector2;

namespace STS2RitsuLib.TopBar
{
    /// <summary>
    ///     Single source of truth for where mod top-bar buttons land relative to the vanilla
    ///     Single source of truth 用于 where mod top-bar buttons land relative to the 原版
    ///     <c>%Deck</c> button. Both pile-backed
    ///     <see cref="STS2RitsuLib.CardPiles.ModCardPileUiStyle.TopBarDeck" /> buttons and
    ///     action-backed <see cref="ModTopBarButtonRegistry" /> buttons funnel through this helper so
    ///     action-backed <c>ModTopBarButton注册表</c> buttons funnel through this helper so
    ///     the two systems cannot disagree about slot ordering / direction — addressing the user
    ///     the two systems cannot disagree about slot ordering / direction — addressing the 使用r
    ///     feedback that having <i>two</i> independent layout algorithms was a "meaningless split".
    ///     中文说明：feedback that having <i>two</i> independent layout algorithms was a "meaningless split".
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The actual layout container for the right-side cluster is
    ///         The actual layout container 用于 the right-side cluster is
    ///         <c>RootSceneContainer/Run/GlobalUi/TopBar/RightAlignedStuff</c> — and crucially it is
    ///         an <see cref="HBoxContainer" /> (<c>separation=0</c>, <c>alignment=end</c>) that
    ///         中文说明：an <c>HBoxContainer</c> (<c>separation=0</c>, <c>alignment=end</c>) that
    ///         auto-lays out its children left-to-right in registration order. The canonical vanilla
    ///         auto-lays out its children left-to-right in 注册 order. The canonical 原版
    ///         child sequence is <c>[SaveIndicator][Padding][TimerContainer][Map][DeckContainer][PauseButton]</c>
    ///         child sequence is <c>[保存Indicator][Padding][TimerContainer][Map][DeckContainer][PauseButton]</c>
    ///         — so to drop a mod button "just to the left of the deck" we need two things:
    ///         中文说明：— so to drop a mod button "just to the left of the deck" we need two things:
    ///         <list type="number">
    ///             <item>the button must be parented under this HBoxContainer, and</item>
    ///             <item>its child index must be immediately before <c>DeckContainer</c>'s index.</item>
    ///         </list>
    ///         Because the container is auto-laid-out, we <b>do not</b> set
    ///         Beca使用 the container is auto-laid-out, we <b>do not</b> 设置
    ///         <see cref="Control.Position" /> ourselves — doing so would fight the HBoxContainer and
    ///         produce the "button appears nowhere visible" bug from the previous iteration.
    ///         produce the "button appears nowhere visible" bug 从 the previous iteration.
    ///     </para>
    ///     <para>
    ///         Slot ordering: mod buttons are inserted in registration order, each one claiming the
    ///         Slot ordering: mod buttons are inserted in 注册 order, each one claiming the
    ///         slot <i>immediately before</i> the deck container / deck button. The newest
    ///         slot <i>immediately 之前</i> the deck container / deck button. The newest
    ///         registration ends up closest to the deck; the earliest registration ends up furthest
    ///         注册 ends up closest to the deck; the earliest 注册 ends up furthest
    ///         left. This keeps every mod button contiguous with the vanilla deck icon and avoids
    ///         left. This keeps every mod button contiguous 带有 the 原版 deck 图标 和 avoids
    ///         splitting them into "pile row" / "action row".
    ///         splitting them into "pile row" / "action row".
    ///     </para>
    /// </remarks>
    public static class ModTopBarLayout
    {
        /// <summary>
        ///     Returns the real right-side layout container (<c>RightAlignedStuff</c>, an
        ///     返回 the real right-side layout container (<c>RightAlignedStuff</c>, an
        ///     <see cref="HBoxContainer" />) — the parent of either <c>%Deck</c> directly or the
        ///     <c>DeckContainer</c> that wraps it. Returns null when the top bar hasn't resolved
        ///     <see cref="NTopBar.Deck" /> yet.
        /// </summary>
        public static Control? GetRightAlignedContainer(NTopBar topBar)
        {
            ArgumentNullException.ThrowIfNull(topBar);
            var deck = topBar.Deck;
            if (deck == null)
                return null;
            // The deck may sit inside a `DeckContainer` MarginContainer; walk up until we hit
            // the HBoxContainer so the lookup is stable across scene refactors that add / remove the
            // wrapper.
            var cursor = deck.GetParent();
            while (cursor is { } node)
            {
                if (node is HBoxContainer hbox)
                    return hbox;
                cursor = node.GetParent();
            }

            return deck.GetParent() as Control;
        }

        /// <summary>
        ///     Returns the direct child of the right-aligned container that ultimately contains
        ///     返回 the direct child of the right-aligned container that ultimately 包含
        ///     <c>%Deck</c> — often the <c>DeckContainer</c> <see cref="MarginContainer" />
        ///     that wraps the deck button. We insert mod buttons immediately <i>before</i> this node
        ///     that wraps the deck button. We insert mod buttons immediately <i>之前</i> this node
        ///     so they land "just to the left of the deck", matching the existing vanilla placement
        ///     so they land "just to the left of the deck", matching the existing 原版 placement
        ///     of Map / DeckContainer / PauseButton.
        ///     of Map / DeckContainer / PauseButton.
        /// </summary>
        public static Node? GetDeckSlotAnchor(NTopBar topBar)
        {
            var container = GetRightAlignedContainer(topBar);
            var deck = topBar.Deck;
            if (container == null || deck == null)
                return null;
            Node cursor = deck;
            while (cursor.GetParent() is { } parent && parent != container)
                cursor = parent;
            return cursor.GetParent() == container ? cursor : null;
        }

        /// <summary>
        ///     Attaches <paramref name="button" /> to the right-aligned container (re-parenting when
        ///     Attaches <c>button</c> to the right-aligned container (re-parenting 当
        ///     necessary) and orders it so it sits immediately to the <b>left</b> of the deck-slot
        ///     necessary) 和 orders it so it sits immediately to the <b>left</b> of the deck-slot
        ///     anchor. The button is sized to match one vanilla top-bar slot
        ///     anchor. The button is sized to match one 原版 top-bar slot
        ///     (<c>80×80</c> minimum) and left at <see cref="Vector2.Zero" /> <see cref="Control.Position" />
        ///     (<c>80×80</c> minimum) 和 left at <c>Vector2.Zero</c> <c>Control.Position</c>
        ///     — the enclosing <see cref="HBoxContainer" /> drives the actual screen position.
        ///     — the enclosing <c>HBoxContainer</c> drives the actual screen position.
        ///     Returns true on success, false when the top bar isn't ready yet (caller should retry).
        ///     返回 true on success, false when the top bar isn't ready yet (caller should retry)。
        /// </summary>
        public static bool Place(NTopBar topBar, NModCardPileButton button, Vector2 offset = default)
        {
            ArgumentNullException.ThrowIfNull(topBar);
            ArgumentNullException.ThrowIfNull(button);

            var container = GetRightAlignedContainer(topBar);
            var anchor = GetDeckSlotAnchor(topBar);
            if (container == null || anchor == null)
                return false;

            return PlaceBeforeAnchor(container, anchor, button, offset);
        }

        /// <summary>
        ///     Attaches <paramref name="button" /> to the right-aligned container and places it immediately
        ///     Attaches <c>button</c> to the right-aligned container 和 places it immediately
        ///     <b>after</b> the deck slot anchor (typically between deck and pause).
        /// </summary>
        /// <returns>
        ///     False when the top bar anchor nodes are not ready yet.
        ///     False 当 the top bar anchor nodes are not ready yet.
        /// </returns>
        public static bool PlaceAfterDeck(NTopBar topBar, NModCardPileButton button, Vector2 offset = default)
        {
            ArgumentNullException.ThrowIfNull(topBar);
            ArgumentNullException.ThrowIfNull(button);

            var container = GetRightAlignedContainer(topBar);
            var anchor = GetDeckSlotAnchor(topBar);
            if (container == null || anchor == null)
                return false;

            return PlaceAfterAnchor(container, anchor, button, offset);
        }

        /// <summary>
        ///     Attaches <paramref name="button" /> to the right-aligned container and places it immediately
        ///     Attaches <c>button</c> to the right-aligned container 和 places it immediately
        ///     <b>before</b> the modifiers slot when available; falls back to the default deck-left placement.
        /// </summary>
        /// <returns>
        ///     False only when all candidate anchors are unavailable.
        ///     False only 当 all candidate anchors are un可用.
        /// </returns>
        public static bool PlaceBeforeModifiers(NTopBar topBar, NModCardPileButton button, Vector2 offset = default)
        {
            ArgumentNullException.ThrowIfNull(topBar);
            ArgumentNullException.ThrowIfNull(button);

            var container = GetRightAlignedContainer(topBar);
            if (container == null)
                return false;

            var modifiers = topBar.GetNodeOrNull<Control>("%Modifiers");
            if (modifiers == null)
                return Place(topBar, button, offset);

            Node anchor = modifiers;
            while (anchor.GetParent() is { } parent && parent != container)
                anchor = parent;
            return anchor.GetParent() != container
                ? Place(topBar, button, offset)
                : PlaceBeforeAnchor(container, anchor, button, offset);
        }

        private static bool PlaceBeforeAnchor(Control container, Node anchor, NModCardPileButton button, Vector2 offset)
        {
            AttachToContainer(container, button);

            var anchorIndex = anchor.GetIndex();
            var currentIndex = button.GetIndex();
            var targetIndex = currentIndex < anchorIndex ? anchorIndex - 1 : anchorIndex;
            if (currentIndex != targetIndex)
                container.MoveChild(button, targetIndex);

            button.ApplyVisualOffset(offset);
            return true;
        }

        private static bool PlaceAfterAnchor(Control container, Node anchor, NModCardPileButton button, Vector2 offset)
        {
            AttachToContainer(container, button);

            var anchorIndex = anchor.GetIndex();
            var currentIndex = button.GetIndex();
            var targetIndex = currentIndex < anchorIndex ? anchorIndex : anchorIndex + 1;
            if (currentIndex != targetIndex)
                container.MoveChild(button, targetIndex);

            button.ApplyVisualOffset(offset);
            return true;
        }

        private static void AttachToContainer(Control container, NModCardPileButton button)
        {
            if (button.GetParent() != container)
            {
                button.GetParent()?.RemoveChild(button);
                container.AddChildSafely(button);
            }

            button.Position = Vector2.Zero;
            button.Scale = Vector2.One;
        }

        /// <summary>
        ///     System.Numerics-flavoured overload for callers (e.g. <see cref="ModTopBarButtonSpec" />)
        ///     System.Numerics-flavoured over加载 用于 callers (e.g. <c>ModTopBarButtonSpec</c>)
        ///     that carry offsets as <see cref="NVec2" />.
        ///     that carry off设置 as <c>NVec2</c>.
        /// </summary>
        public static bool Place(NTopBar topBar, NModCardPileButton button, NVec2 offset)
        {
            return Place(topBar, button, new Vector2(offset.X, offset.Y));
        }
    }
}
