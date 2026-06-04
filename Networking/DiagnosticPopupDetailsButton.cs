using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace STS2RitsuLib.Networking
{
    internal static class DiagnosticPopupDetailsButton
    {
        private const string DetailsButtonScenePath = "res://scenes/ui/abandon_run_yes_button.tscn";
        private const string ButtonName = "RitsuDiagnosticsDetailsButton";

        public static void Add(NErrorPopup popup, string text, Action open)
        {
            var verticalPopup = popup.GetNodeOrNull<NVerticalPopup>("VerticalPopup");
            if (verticalPopup == null || verticalPopup.GetNodeOrNull<NPopupYesNoButton>(ButtonName) != null)
                return;

            var details = PreloadManager.Cache.GetScene(DetailsButtonScenePath)
                .Instantiate<NPopupYesNoButton>();
            details.Name = ButtonName;
            details.Visible = true;
            details.IsYes = false;
            details.FocusMode = Control.FocusModeEnum.All;
            PositionDetailsButton(verticalPopup, details);
            verticalPopup.AddChild(details);
            verticalPopup.MoveChild(details, verticalPopup.GetChildCount() - 1);
            details.Connect(NClickableControl.SignalName.Released, Callable.From<NClickableControl>(_ => open()));

            Callable.From(() =>
            {
                if (!GodotObject.IsInstanceValid(details))
                    return;

                details.SetText(text);
                details.DisconnectHotkeys();
                WireFocus(verticalPopup, details);
            }).CallDeferred();
        }

        private static void PositionDetailsButton(NVerticalPopup popup, Control details)
        {
            var anchor = popup.YesButton;
            var height = anchor.OffsetBottom - anchor.OffsetTop;
            var lift = height * 6f;
            details.AnchorLeft = anchor.AnchorLeft;
            details.AnchorTop = anchor.AnchorTop;
            details.AnchorRight = anchor.AnchorRight;
            details.AnchorBottom = anchor.AnchorBottom;
            details.OffsetLeft = anchor.OffsetLeft;
            details.OffsetTop = anchor.OffsetTop - lift;
            details.OffsetRight = anchor.OffsetRight;
            details.OffsetBottom = anchor.OffsetBottom - lift;
            details.GrowHorizontal = anchor.GrowHorizontal;
            details.GrowVertical = anchor.GrowVertical;
        }

        private static void WireFocus(NVerticalPopup popup, NPopupYesNoButton details)
        {
            var detailsPath = details.GetPath();
            var yesPath = popup.YesButton.GetPath();
            details.FocusMode = Control.FocusModeEnum.All;
            details.FocusNeighborTop = detailsPath;
            details.FocusNeighborBottom = yesPath;
            details.FocusNeighborLeft = detailsPath;
            details.FocusNeighborRight = detailsPath;

            popup.YesButton.FocusMode = Control.FocusModeEnum.All;
            popup.YesButton.FocusNeighborTop = detailsPath;

            if (popup.NoButton.Visible)
            {
                var noPath = popup.NoButton.GetPath();
                popup.NoButton.FocusMode = Control.FocusModeEnum.All;
                popup.NoButton.FocusNeighborTop = detailsPath;
                popup.NoButton.FocusNeighborBottom = noPath;
                popup.NoButton.FocusNeighborLeft = yesPath;
                popup.NoButton.FocusNeighborRight = yesPath;
                popup.YesButton.FocusNeighborBottom = yesPath;
                popup.YesButton.FocusNeighborLeft = noPath;
                popup.YesButton.FocusNeighborRight = noPath;
            }
            else
            {
                popup.YesButton.FocusNeighborBottom = yesPath;
                popup.YesButton.FocusNeighborLeft = yesPath;
                popup.YesButton.FocusNeighborRight = yesPath;
            }

            Callable.From(() => popup.YesButton.GrabFocus()).CallDeferred();
        }
    }
}
