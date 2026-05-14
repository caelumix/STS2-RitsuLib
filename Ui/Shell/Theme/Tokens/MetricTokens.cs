namespace STS2RitsuLib.Ui.Shell.Theme
{
    /// <summary>
    ///     Numeric metrics grouped by intent.
    ///     Numeric metrics grouped 通过 intent.
    /// </summary>
    /// <param name="Radius">
    ///     Corner radius scale.
    ///     中文说明：Corner radius scale.
    /// </param>
    /// <param name="BorderWidth">
    ///     Border width scale.
    ///     中文说明：Border width scale.
    /// </param>
    /// <param name="Entry">
    ///     Entry / row sizing.
    ///     中文说明：Entry / row sizing.
    /// </param>
    /// <param name="Slider">
    ///     Slider sizing.
    ///     中文说明：Slider sizing.
    /// </param>
    /// <param name="Choice">
    ///     Choice / stepper row sizing.
    ///     中文说明：Choice / stepper row sizing.
    /// </param>
    /// <param name="Color">
    ///     Color picker row sizing.
    ///     中文说明：Color picker row sizing.
    /// </param>
    /// <param name="StringEntry">
    ///     String editor sizing.
    ///     中文说明：String editor sizing.
    /// </param>
    /// <param name="Keybinding">
    ///     Keybinding capture sizing.
    ///     中文说明：Keybinding capture sizing.
    /// </param>
    /// <param name="Overlay">
    ///     Floating overlay sizing.
    ///     中文说明：Floating overlay sizing.
    /// </param>
    /// <param name="Sidebar">
    ///     Sidebar navigation sizing and behavior.
    ///     Sidebar navigation sizing 和 behavior.
    /// </param>
    /// <param name="FontSize">
    ///     Font size scale.
    ///     中文说明：Font size scale.
    /// </param>
    public sealed record MetricTokens(
        RadiusMetrics Radius,
        BorderWidthMetrics BorderWidth,
        EntryMetrics Entry,
        SliderMetrics Slider,
        ChoiceMetrics Choice,
        ColorRowMetrics Color,
        StringEntryMetrics StringEntry,
        KeybindingMetrics Keybinding,
        OverlayMetrics Overlay,
        SidebarMetrics Sidebar,
        FontSizeMetrics FontSize);

    /// <summary>
    ///     Corner radius scale.
    ///     中文说明：Corner radius scale.
    /// </summary>
    /// <param name="Default">
    ///     Shared StyleBox corner radius.
    ///     中文说明：Shared StyleBox corner radius.
    /// </param>
    /// <param name="Validation">
    ///     Corner radius for validation chrome.
    ///     Corner radius 用于 有效ation chrome.
    /// </param>
    /// <param name="Overlay">
    ///     Corner radius for floating overlay panels.
    ///     Corner radius 用于 floating overlay panels.
    /// </param>
    public sealed record RadiusMetrics(int Default, int Validation, int Overlay);

    /// <summary>
    ///     Border width scale.
    ///     中文说明：Border width scale.
    /// </summary>
    /// <param name="Thin">
    ///     Hairline border.
    ///     中文说明：Hairline border.
    /// </param>
    /// <param name="Normal">
    ///     Default emphasis border.
    ///     默认 emphasis border。
    /// </param>
    /// <param name="Thick">
    ///     Strong emphasis border.
    ///     中文说明：Strong emphasis border.
    /// </param>
    /// <param name="Overlay">
    ///     Border width for floating overlay panels.
    ///     Border width 用于 floating overlay panels.
    /// </param>
    public sealed record BorderWidthMetrics(int Thin, int Normal, int Thick, int Overlay);

    /// <summary>
    ///     Entry / row sizing.
    ///     中文说明：Entry / row sizing.
    /// </summary>
    /// <param name="ValueMinWidth">
    ///     Default minimum width for compact value widgets.
    ///     默认 minimum width for compact value widgets。
    /// </param>
    /// <param name="ValueMinHeight">
    ///     Fixed vertical size for value column widgets.
    ///     Fixed vertical size 用于 value column widgets.
    /// </param>
    /// <param name="MiniStepperButtonSize">
    ///     Square size for compact stepper buttons.
    ///     Square size 用于 compact stepper buttons.
    /// </param>
    public sealed record EntryMetrics(float ValueMinWidth, float ValueMinHeight, int MiniStepperButtonSize);

    /// <summary>
    ///     Slider sizing.
    ///     中文说明：Slider sizing.
    /// </summary>
    /// <param name="RowMinWidth">
    ///     Slider row minimum width.
    ///     中文说明：Slider row minimum width.
    /// </param>
    /// <param name="TrackMinWidth">
    ///     Minimum width reserved for the HSlider track.
    ///     最小 width reserved for the HSlider track。
    /// </param>
    /// <param name="ValueFieldWidth">
    ///     Width of the inline numeric field next to sliders.
    ///     中文说明：Width of the inline numeric field next to sliders.
    /// </param>
    /// <param name="ValueFieldHeight">
    ///     Height of the inline numeric field next to sliders.
    ///     中文说明：Height of the inline numeric field next to sliders.
    /// </param>
    public sealed record SliderMetrics(
        float RowMinWidth,
        float TrackMinWidth,
        float ValueFieldWidth,
        float ValueFieldHeight);

    /// <summary>
    ///     Choice (stepper) row sizing.
    ///     中文说明：Choice (stepper) row sizing.
    /// </summary>
    /// <param name="RowMinWidth">
    ///     Stepper row total width.
    ///     中文说明：Stepper row total width.
    /// </param>
    /// <param name="CenterMinWidth">
    ///     Minimum width for the center label area.
    ///     最小 width for the center label area。
    /// </param>
    public sealed record ChoiceMetrics(float RowMinWidth, float CenterMinWidth);

    /// <summary>
    ///     Color picker row sizing.
    ///     中文说明：Color picker row sizing.
    /// </summary>
    /// <param name="RowMinWidth">
    ///     Color row minimum width.
    ///     中文说明：Color row minimum width.
    /// </param>
    /// <param name="SwatchSize">
    ///     Swatch size.
    ///     中文说明：Swatch size.
    /// </param>
    public sealed record ColorRowMetrics(float RowMinWidth, float SwatchSize);

    /// <summary>
    ///     String editor sizing.
    ///     中文说明：String editor sizing.
    /// </summary>
    /// <param name="MinWidth">
    ///     Single-line string entry minimum width.
    ///     中文说明：Single-line string entry minimum width.
    /// </param>
    /// <param name="MultilineMinHeight">
    ///     Multiline string entry minimum height.
    ///     中文说明：Multiline string entry minimum height.
    /// </param>
    public sealed record StringEntryMetrics(float MinWidth, float MultilineMinHeight);

    /// <summary>
    ///     Keybinding capture sizing.
    ///     中文说明：Keybinding capture sizing.
    /// </summary>
    /// <param name="BlockWidth">
    ///     Keybinding block width.
    ///     中文说明：Keybinding block width.
    /// </param>
    /// <param name="CaptureMinWidth">
    ///     Minimum width of the keybinding capture button.
    ///     最小 width of the keybinding capture button。
    /// </param>
    /// <param name="HintFontSize">
    ///     Font size for keybinding helper text.
    ///     Font size 用于 keybinding helper text.
    /// </param>
    public sealed record KeybindingMetrics(float BlockWidth, float CaptureMinWidth, int HintFontSize);

    /// <summary>
    ///     Floating overlay panel sizing.
    ///     中文说明：Floating overlay panel sizing.
    /// </summary>
    /// <param name="PaddingH">
    ///     Horizontal padding.
    ///     中文说明：Horizontal padding.
    /// </param>
    /// <param name="PaddingV">
    ///     Vertical padding.
    ///     中文说明：Vertical padding.
    /// </param>
    public sealed record OverlayMetrics(int PaddingH, int PaddingV);

    /// <summary>
    ///     Sidebar navigation sizing and behavior.
    ///     Sidebar navigation sizing 和 behavior.
    /// </summary>
    /// <param name="Width">
    ///     Fixed sidebar column width.
    ///     中文说明：Fixed sidebar column width.
    /// </param>
    /// <param name="PageRowMinHeight">
    ///     Minimum row height for a page tab.
    ///     最小 row height for a page tab。
    /// </param>
    /// <param name="SectionRowMinHeight">
    ///     Minimum row height for a section jump row.
    ///     最小 row height for a section jump row。
    /// </param>
    /// <param name="ModListSeparation">
    ///     Vertical gap between mod cards.
    ///     Vertical gap between mod 卡牌s.
    /// </param>
    /// <param name="ModCardInnerSeparation">
    ///     Vertical gap inside an expanded mod card.
    ///     Vertical gap inside an expanded mod 卡牌.
    /// </param>
    /// <param name="PageTreeSeparation">
    ///     Gap between stacked root page rows.
    ///     中文说明：Gap between stacked root page rows.
    /// </param>
    /// <param name="SectionRailSeparation">
    ///     Gap between section rail rows.
    ///     中文说明：Gap between section rail rows.
    /// </param>
    /// <param name="CardInnerMargin">
    ///     Inner margin for compact sidebar mod cards.
    ///     Inner margin 用于 compact sidebar mod 卡牌s.
    /// </param>
    /// <param name="ShowInlinePageCount">
    ///     When true, expanded mod cards show an inline "N pages" line.
    ///     为 true 时，expanded mod cards show an inline "N pages" line。
    /// </param>
    public sealed record SidebarMetrics(
        float Width,
        float PageRowMinHeight,
        float SectionRowMinHeight,
        int ModListSeparation,
        int ModCardInnerSeparation,
        int PageTreeSeparation,
        int SectionRailSeparation,
        int CardInnerMargin,
        bool ShowInlinePageCount);

    /// <summary>
    ///     Font size scale used across the shell.
    ///     Font size scale used across the shell.
    /// </summary>
    /// <param name="Button">
    ///     Default font size for standard buttons.
    ///     默认 font size for standard buttons。
    /// </param>
    /// <param name="MiniButton">
    ///     Font size for compact (mini) buttons.
    ///     Font size 用于 compact (mini) buttons.
    /// </param>
    /// <param name="ValueLabel">
    ///     Font size for dropdown faces and stepper center labels.
    ///     Font size 用于 dropdown faces 和 stepper center labels.
    /// </param>
    /// <param name="PopupRow">
    ///     Font size for dropdown / popup rows.
    ///     Font size 用于 dropdown / popup rows.
    /// </param>
    /// <param name="HintSmall">
    ///     Font size for small inline hints.
    ///     Font size 用于 small inline hints.
    /// </param>
    /// <param name="Tooltip">
    ///     Font size for native control tooltips (TooltipLabel).
    ///     Font size 用于 native control tooltips (TooltipLabel).
    /// </param>
    /// <param name="Grip">
    ///     Font size for glyph-style grip labels.
    ///     Font size 用于 glyph-style grip labels.
    /// </param>
    /// <param name="PillCount">
    ///     Font size for list count badges.
    ///     Font size 用于 list count badges.
    /// </param>
    /// <param name="Secondary">
    ///     Font size for secondary text.
    ///     Font size 用于 secondary text.
    /// </param>
    /// <param name="HeaderArrow">
    ///     Font size for header arrow glyphs.
    ///     Font size 用于 header arrow glyphs.
    /// </param>
    /// <param name="HeaderTitle">
    ///     Font size for collapsible header titles.
    ///     Font size 用于 collapsible header titles.
    /// </param>
    /// <param name="HeaderSubtitle">
    ///     Font size for header subtitles.
    ///     Font size 用于 header subtitles.
    /// </param>
    /// <param name="PageDescription">
    ///     Font size for page descriptions in the toolbar area.
    ///     Font size 用于 page descriptions in the toolbar area.
    /// </param>
    /// <param name="OverlayTitle">
    ///     Font size for floating overlay titles.
    ///     Font size 用于 floating overlay titles.
    /// </param>
    /// <param name="OverlayBody">
    ///     Font size for floating overlay body lines.
    ///     Font size 用于 floating overlay body lines.
    /// </param>
    /// <param name="OverlayPath">
    ///     Font size for floating overlay path labels.
    ///     Font size 用于 floating overlay 路径 labels.
    /// </param>
    /// <param name="SettingsEntryButton">
    ///     Font size for the vanilla settings entry button label.
    ///     Font size 用于 the 原版 设置 entry button label.
    /// </param>
    /// <param name="SettingLineTitle">
    ///     Primary title font size for each settings entry row label.
    ///     Primary title font size 用于 each 设置 entry row label.
    /// </param>
    public sealed record FontSizeMetrics(
        int Button,
        int MiniButton,
        int ValueLabel,
        int PopupRow,
        int HintSmall,
        int Tooltip,
        int Grip,
        int PillCount,
        int Secondary,
        int HeaderArrow,
        int HeaderTitle,
        int HeaderSubtitle,
        int PageDescription,
        int OverlayTitle,
        int OverlayBody,
        int OverlayPath,
        int SettingsEntryButton,
        int SettingLineTitle);
}
