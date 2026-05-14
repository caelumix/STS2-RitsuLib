using System.Text.Json;

namespace STS2RitsuLib.Ui.Shell.Theme
{
    /// <summary>
    ///     Mod-supplied default tokens that participate in every snapshot build, plus an optional callback
    ///     Mod-supplied default tokens that participate in every snapshot build, plus an 可选 callback
    ///     invoked after each snapshot is published.
    ///     invoked 之后 each snapshot is published.
    /// </summary>
    /// <param name="ModId">
    ///     Mod identifier (used to namespace <c>scopes.mod:&lt;modId&gt;</c> + extensions).
    ///     Mod identifier (used to namespace <c>scopes.mod:&lt;modId&gt;</c> + extensions).
    /// </param>
    /// <param name="Defaults">
    ///     Optional DTFM tree (object) merged before chain documents.
    ///     可选 DTFM tree (object) merged 之前 chain documents.
    /// </param>
    /// <param name="OnApply">
    ///     Optional callback fired after every theme rebuild with the new snapshot.
    ///     可选 callback fired 之后 every theme rebuild 带有 the new snapshot.
    /// </param>
    public sealed record RitsuShellThemeModRegistration(
        string ModId,
        JsonElement? Defaults,
        Action<RitsuShellTheme>? OnApply);
}
