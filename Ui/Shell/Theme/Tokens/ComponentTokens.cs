using Godot;

namespace STS2RitsuLib.Ui.Shell.Theme
{
    /// <summary>
    ///     Component tokens grouped by component, then variant/state.
    ///     Component tokens grouped 通过 component, then variant/state.
    /// </summary>
    /// <param name="SidebarCard">
    ///     Sidebar mod cards (default and selected).
    ///     Sidebar mod 卡牌s (default 和 selected).
    /// </param>
    /// <param name="ChromeMenu">
    ///     Compact action / menu chrome.
    ///     Comp章节 action / menu chrome.
    /// </param>
    /// <param name="PageToolbarTray">
    ///     Tray behind per-page toolbar controls.
    ///     中文说明：Tray behind per-page toolbar controls.
    /// </param>
    /// <param name="ListShell">
    ///     Outer container for scrollable lists.
    ///     Outer container 用于 scrollable lists.
    /// </param>
    /// <param name="ListItem">
    ///     List item card variants.
    ///     List item 卡牌 variants.
    /// </param>
    /// <param name="ListEditor">
    ///     Inline list-editor surface.
    ///     中文说明：Inline list-editor surface.
    /// </param>
    /// <param name="Pill">
    ///     Pill-shaped controls (tags, compact buttons).
    ///     Pill-shaped controls (tags, compact buttons).
    /// </param>
    /// <param name="Toggle">
    ///     Settings toggle states.
    ///     设置 toggle states.
    /// </param>
    /// <param name="Slider">
    ///     Slider grab thumb tones.
    ///     中文说明：Slider grab thumb tones.
    /// </param>
    /// <param name="Dropdown">
    ///     Dropdown faces and rows.
    ///     Dropdown faces 和 rows.
    /// </param>
    /// <param name="Stepper">
    ///     Stepper face states.
    ///     中文说明：Stepper face states.
    /// </param>
    /// <param name="DragHandle">
    ///     Drag handles for reorderable lists.
    ///     Drag handles 用于 reorderable lists.
    /// </param>
    /// <param name="Collapsible">
    ///     Collapsible section headers.
    ///     中文说明：Collapsible section headers.
    /// </param>
    /// <param name="SidebarBtn">
    ///     Sidebar tree buttons (page / section / mod / utility / depth variations).
    ///     中文说明：Sidebar tree buttons (page / section / mod / utility / depth variations).
    /// </param>
    /// <param name="SidebarRail">
    ///     Section rail background.
    ///     Section rail 背景.
    /// </param>
    /// <param name="TextButton">
    ///     Inline text-button tones (accent / danger / neutral).
    ///     中文说明：Inline text-button tones (accent / danger / neutral).
    /// </param>
    /// <param name="StringValidation">
    ///     String editor validation chrome (neutral / invalid).
    ///     String editor 有效ation chrome (neutral / invalid).
    /// </param>
    /// <param name="OverlayPanel">
    ///     Floating overlay panel chrome.
    ///     中文说明：Floating overlay panel chrome.
    /// </param>
    /// <param name="ChoiceCenter">
    ///     Choice-center label highlight gradient.
    ///     中文说明：Choice-center label highlight gradient.
    /// </param>
    public sealed record ComponentTokens(
        SidebarCardTokens SidebarCard,
        ChromeMenuTokens ChromeMenu,
        PageToolbarTrayTokens PageToolbarTray,
        ListShellTokens ListShell,
        ListItemTokens ListItem,
        ListEditorTokens ListEditor,
        PillTokens Pill,
        ToggleTokens Toggle,
        SliderTokens Slider,
        DropdownTokens Dropdown,
        StepperTokens Stepper,
        DragHandleTokens DragHandle,
        CollapsibleTokens Collapsible,
        SidebarBtnTokens SidebarBtn,
        SidebarRailTokens SidebarRail,
        TextButtonTokens TextButton,
        StringValidationTokens StringValidation,
        OverlayPanelTokens OverlayPanel,
        ChoiceCenterTokens ChoiceCenter);

    /// <summary>
    ///     Sidebar mod card tokens.
    ///     Sidebar mod 卡牌 tokens.
    /// </summary>
    /// <param name="Default">
    ///     Default state (background + border).
    ///     默认 state (background + border)。
    /// </param>
    /// <param name="Selected">
    ///     Selected state.
    ///     中文说明：Selected state.
    /// </param>
    /// <param name="Shadow">
    ///     Drop shadow tint shared by both states.
    ///     Drop shadow tint shared 通过 both states.
    /// </param>
    public sealed record SidebarCardTokens(BgBorder Default, BgBorder Selected, Color Shadow);

    /// <summary>
    ///     Chrome action menu tokens (hover toggles between two states).
    ///     Chrome action menu tokens (hover toggles between two states).
    /// </summary>
    /// <param name="Default">
    ///     Resting state.
    ///     中文说明：Resting state.
    /// </param>
    /// <param name="Hover">
    ///     Hover state.
    ///     中文说明：Hover state.
    /// </param>
    public sealed record ChromeMenuTokens(BgBorder Default, BgBorder Hover);

    /// <summary>
    ///     Page toolbar tray tokens.
    ///     中文说明：Page toolbar tray tokens.
    /// </summary>
    /// <param name="Bg">
    ///     Toolbar tray background.
    ///     Toolbar tray 背景.
    /// </param>
    /// <param name="Border">
    ///     Toolbar tray border.
    ///     中文说明：Toolbar tray border.
    /// </param>
    public sealed record PageToolbarTrayTokens(Color Bg, Color Border);

    /// <summary>
    ///     List shell (outer) tokens.
    ///     中文说明：List shell (outer) tokens.
    /// </summary>
    /// <param name="Bg">
    ///     Shell background.
    ///     Shell 背景.
    /// </param>
    /// <param name="Border">
    ///     Shell border.
    ///     中文说明：Shell border.
    /// </param>
    /// <param name="Shadow">
    ///     Shell drop shadow.
    ///     中文说明：Shell drop shadow.
    /// </param>
    public sealed record ListShellTokens(Color Bg, Color Border, Color Shadow);

    /// <summary>
    ///     List item card tokens (default vs accent emphasis).
    ///     List item 卡牌 tokens (default vs accent emphasis).
    /// </summary>
    /// <param name="Default">
    ///     Resting state.
    ///     中文说明：Resting state.
    /// </param>
    /// <param name="Accent">
    ///     Accent / selected emphasis.
    ///     中文说明：Accent / selected emphasis.
    /// </param>
    /// <param name="Shadow">
    ///     Card drop shadow tint.
    ///     卡牌 drop shadow tint.
    /// </param>
    public sealed record ListItemTokens(BgBorder Default, BgBorder Accent, Color Shadow);

    /// <summary>
    ///     Inline list editor surface tokens.
    ///     中文说明：Inline list editor surface tokens.
    /// </summary>
    /// <param name="Bg">
    ///     Editor background.
    ///     Editor 背景.
    /// </param>
    /// <param name="Border">
    ///     Editor border.
    ///     中文说明：Editor border.
    /// </param>
    public sealed record ListEditorTokens(Color Bg, Color Border);

    /// <summary>
    ///     Pill / tag tokens.
    ///     中文说明：Pill / tag tokens.
    /// </summary>
    /// <param name="Default">
    ///     Resting state.
    ///     中文说明：Resting state.
    /// </param>
    /// <param name="Hover">
    ///     Hover state.
    ///     中文说明：Hover state.
    /// </param>
    public sealed record PillTokens(BgBorder Default, BgBorder Hover);

    /// <summary>
    ///     Toggle tokens. Borders inherit if a state omits them.
    ///     Toggle tokens. Borders inherit 如果 a state omits them.
    /// </summary>
    /// <param name="On">
    ///     Pressed/on state.
    ///     中文说明：Pressed/on state.
    /// </param>
    /// <param name="Off">
    ///     Off state.
    ///     中文说明：Off state.
    /// </param>
    /// <param name="OffHover">
    ///     Off + hover state (border falls back to <see cref="Off" />).
    ///     中文说明：Off + hover state (border falls back to <c>Off</c>).
    /// </param>
    /// <param name="Disabled">
    ///     Disabled state.
    ///     中文说明：Disabled state.
    /// </param>
    /// <param name="Shadow">
    ///     Neutral (non-hover) shadow tint.
    ///     中文说明：Neutral (non-hover) shadow tint.
    /// </param>
    public sealed record ToggleTokens(
        BgBorder On,
        BgBorder Off,
        BgBorder OffHover,
        BgBorder Disabled,
        Color Shadow);

    /// <summary>
    ///     Slider grab tokens.
    ///     中文说明：Slider grab tokens.
    /// </summary>
    /// <param name="GrabHighlight">
    ///     Outer highlight tint.
    ///     中文说明：Outer highlight tint.
    /// </param>
    /// <param name="GrabShadow">
    ///     Inner shadow tint.
    ///     中文说明：Inner shadow tint.
    /// </param>
    public sealed record SliderTokens(Color GrabHighlight, Color GrabShadow);

    /// <summary>
    ///     Dropdown face tokens covering the four interaction states.
    ///     Dropdown face tokens covering the four interaction states.
    /// </summary>
    /// <param name="Open">
    ///     Default open state (face + border).
    ///     默认 open state (face + border)。
    /// </param>
    /// <param name="Hover">
    ///     Hover state. Border falls back to <see cref="Open" />.
    ///     中文说明：Hover state. Border falls back to <c>Open</c>.
    /// </param>
    /// <param name="Pressed">
    ///     Pressed state. Border falls back to <see cref="Open" />.
    ///     中文说明：Pressed state. Border falls back to <c>Open</c>.
    /// </param>
    /// <param name="Focus">
    ///     Focus state (own border).
    ///     中文说明：Focus state (own border).
    /// </param>
    public sealed record DropdownTokens(
        BgBorder Open,
        BgBorder Hover,
        BgBorder Pressed,
        BgBorder Focus);

    /// <summary>
    ///     Stepper face tokens.
    ///     中文说明：Stepper face tokens.
    /// </summary>
    /// <param name="Default">
    ///     Default state (background + border).
    ///     默认 state (background + border)。
    /// </param>
    /// <param name="Hover">
    ///     Hover state.
    ///     中文说明：Hover state.
    /// </param>
    /// <param name="Neutral">
    ///     Hidden / neutral state used when the face has no visible affordance.
    ///     Hidden / neutral state used 当 the face has no visible affordance.
    /// </param>
    public sealed record StepperTokens(BgBorder Default, BgBorder Hover, BgBorder Neutral);

    /// <summary>
    ///     Drag handle tokens.
    ///     中文说明：Drag handle tokens.
    /// </summary>
    /// <param name="Default">
    ///     Resting state.
    ///     中文说明：Resting state.
    /// </param>
    /// <param name="Selected">
    ///     Selected (active) state.
    ///     Selected (active) state.
    /// </param>
    public sealed record DragHandleTokens(BgBorder Default, BgBorder Selected);

    /// <summary>
    ///     Collapsible section header tokens.
    ///     中文说明：Collapsible section header tokens.
    /// </summary>
    /// <param name="Default">
    ///     Resting state.
    ///     中文说明：Resting state.
    /// </param>
    /// <param name="Hover">
    ///     Hover state.
    ///     中文说明：Hover state.
    /// </param>
    /// <param name="Selected">
    ///     Selected (expanded) state.
    ///     中文说明：Selected (expanded) state.
    /// </param>
    /// <param name="Disabled">
    ///     Disabled state (content unavailable, but header may remain interactive).
    ///     Disabled state (content un可用, but header may remain interactive).
    /// </param>
    public sealed record CollapsibleTokens(BgBorder Default, BgBorder Hover, BgBorder Selected, BgBorder Disabled);

    /// <summary>
    ///     Sidebar tree button tokens. Each variant covers a distinct depth or kind of row.
    ///     Sidebar tree button tokens. Each variant covers a distinct depth 或 kind of row.
    /// </summary>
    /// <param name="Default">
    ///     Default page row.
    ///     默认 page row。
    /// </param>
    /// <param name="Hover">
    ///     Default page hover.
    ///     默认 page hover。
    /// </param>
    /// <param name="Selected">
    ///     Selected default page row.
    ///     中文说明：Selected default page row.
    /// </param>
    /// <param name="SelectedHover">
    ///     Selected default page hover.
    ///     中文说明：Selected default page hover.
    /// </param>
    /// <param name="UtilitySelected">
    ///     Selected utility row (only background tinted).
    ///     Selected utility row (only 背景 tinted).
    /// </param>
    /// <param name="IdleDeep">
    ///     Idle page at deeper depth.
    ///     中文说明：Idle page at deeper depth.
    /// </param>
    /// <param name="IdleDeepHover">
    ///     Idle deep + hover.
    ///     中文说明：Idle deep + hover.
    /// </param>
    /// <param name="IdleDeeper">
    ///     Idle page at the deepest tracked depth.
    ///     中文说明：Idle page at the deepest tracked depth.
    /// </param>
    /// <param name="IdleDeeperHover">
    ///     Idle deeper + hover.
    ///     中文说明：Idle deeper + hover.
    /// </param>
    /// <param name="Mod">
    ///     Mod row.
    ///     中文说明：Mod row.
    /// </param>
    /// <param name="ModHover">
    ///     Mod row hover.
    ///     中文说明：Mod row hover.
    /// </param>
    /// <param name="ModDeep">
    ///     Mod row at deeper depth.
    ///     中文说明：Mod row at deeper depth.
    /// </param>
    /// <param name="DeepBorder">
    ///     Border tint shared by deeper rows.
    ///     Border tint shared 通过 deeper rows.
    /// </param>
    /// <param name="DeepBorderHover">
    ///     Hover border tint for deeper rows.
    ///     Hover border tint 用于 deeper rows.
    /// </param>
    /// <param name="Shadow">
    ///     Drop shadow tint.
    ///     中文说明：Drop shadow tint.
    /// </param>
    public sealed record SidebarBtnTokens(
        BgBorder Default,
        BgBorder Hover,
        BgBorder Selected,
        BgBorder SelectedHover,
        BgBorder UtilitySelected,
        BgBorder IdleDeep,
        BgBorder IdleDeepHover,
        BgBorder IdleDeeper,
        BgBorder IdleDeeperHover,
        BgBorder Mod,
        BgBorder ModHover,
        BgBorder ModDeep,
        Color DeepBorder,
        Color DeepBorderHover,
        Color Shadow);

    /// <summary>
    ///     Sidebar section rail (background + outline).
    ///     Sidebar section rail (背景 + outline).
    /// </summary>
    /// <param name="Bg">
    ///     Rail background.
    ///     Rail 背景.
    /// </param>
    /// <param name="Border">
    ///     Rail outline.
    ///     中文说明：Rail outline.
    /// </param>
    public sealed record SidebarRailTokens(Color Bg, Color Border);

    /// <summary>
    ///     Inline text button tokens, organised by tone.
    ///     Inline text button tokens, organised 通过 tone.
    /// </summary>
    /// <param name="Accent">
    ///     Accent (highlighted) tone.
    ///     中文说明：Accent (highlighted) tone.
    /// </param>
    /// <param name="Danger">
    ///     Danger tone.
    ///     中文说明：Danger tone.
    /// </param>
    /// <param name="Neutral">
    ///     Neutral tone.
    ///     中文说明：Neutral tone.
    /// </param>
    public sealed record TextButtonTokens(
        TextButtonToneTokens Accent,
        TextButtonToneTokens Danger,
        TextButtonToneTokens Neutral);

    /// <summary>
    ///     Per-tone foreground + background pair for inline text buttons.
    ///     Per-tone 用于eground + 背景 pair 用于 inline text buttons.
    /// </summary>
    /// <param name="Fg">
    ///     Foreground (label) color.
    ///     中文说明：Foreground (label) color.
    /// </param>
    /// <param name="Bg">
    ///     Default tinted background (used for selected/hovered rows).
    ///     默认 tinted background (used for selected/hovered rows)。
    /// </param>
    /// <param name="BgHover">
    ///     Hover variation of the tinted background.
    ///     Hover variation of the tinted 背景.
    /// </param>
    public sealed record TextButtonToneTokens(Color Fg, Color Bg, Color BgHover);

    /// <summary>
    ///     String editor validation chrome (background + border for neutral / invalid states).
    ///     String editor 有效ation chrome (背景 + border 用于 neutral / invalid states).
    /// </summary>
    /// <param name="Neutral">
    ///     Neutral state.
    ///     中文说明：Neutral state.
    /// </param>
    /// <param name="Invalid">
    ///     Invalid state.
    ///     In有效 state.
    /// </param>
    public sealed record StringValidationTokens(BgBorder Neutral, BgBorder Invalid);

    /// <summary>
    ///     Floating overlay panel chrome.
    ///     中文说明：Floating overlay panel chrome.
    /// </summary>
    /// <param name="Bg">
    ///     Panel background.
    ///     Panel 背景.
    /// </param>
    /// <param name="Border">
    ///     Panel border.
    ///     中文说明：Panel border.
    /// </param>
    public sealed record OverlayPanelTokens(Color Bg, Color Border);

    /// <summary>
    ///     Highlight tones used by the choice-center widget.
    ///     Highlight tones used 通过 the choice-center widget.
    /// </summary>
    /// <param name="HighlightTop">
    ///     Top of the gradient.
    ///     中文说明：Top of the gradient.
    /// </param>
    /// <param name="HighlightBottom">
    ///     Bottom of the gradient.
    ///     中文说明：Bottom of the gradient.
    /// </param>
    public sealed record ChoiceCenterTokens(Color HighlightTop, Color HighlightBottom);

    /// <summary>
    ///     Common (background + border) pair shared by many component states.
    ///     Common (背景 + border) pair shared 通过 many component states.
    /// </summary>
    /// <param name="Bg">
    ///     Background fill.
    ///     背景 fill.
    /// </param>
    /// <param name="Border">
    ///     Border tint.
    ///     中文说明：Border tint.
    /// </param>
    public sealed record BgBorder(Color Bg, Color Border);
}
