using Godot;

namespace STS2RitsuLib.Ui.Shell.Theme
{
    /// <summary>
    ///     Theme-resolved font assets.
    ///     Theme-resolved font 资源.
    /// </summary>
    /// <param name="Body">
    ///     Regular body font.
    ///     中文说明：Regular body font.
    /// </param>
    /// <param name="BodyBold">
    ///     Emphasized body font.
    ///     中文说明：Emphasized body font.
    /// </param>
    /// <param name="Button">
    ///     Font used by compact and action buttons.
    ///     Font used 通过 compact 和 action buttons.
    /// </param>
    public sealed record FontTokens(Font Body, Font BodyBold, Font Button);
}
