#if STS2_V_0_103_2
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
#else
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
#endif

namespace STS2RitsuLib.Combat.CardTargeting
{
    internal static class CardPlayUiFocus
    {
        internal static void AfterCardPlayFinished()
        {
#if STS2_V_0_103_2
            NCombatRoom.Instance?.Ui.Hand.DefaultFocusedControl.TryGrabFocus();
#else
            // 0.104.0+: NCardPlay now restores focus via screen context instead of directly focusing the hand.
            ActiveScreenContext.Instance.FocusOnDefaultControl();
#endif
        }
    }
}
