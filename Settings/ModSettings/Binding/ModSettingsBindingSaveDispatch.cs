namespace STS2RitsuLib.Settings
{
    /// <summary>
    ///     Declares which bindings receive a direct <c>Save()</c> call when this binding's <see cref="IModSettingsBinding" />
    ///     Declares which bindings receive a direct <c>保存()</c> call 当 this binding's <c>IModSettingsBinding</c>
    ///     persistence runs. Used to deduplicate deferred flush work across decorator stacks.
    ///     persistence runs. used to deduplicate deferred flush work across decorator stacks.
    /// </summary>
    internal interface IModSettingsBindingSaveDispatch
    {
        /// <summary>
        ///     Non-recursive targets: bindings invoked immediately by this instance's <c>Save()</c> (typically one inner or
        ///     Non-recursive targets: bindings invoked immediately 通过 this instance's <c>保存()</c> (typically one inner or
        ///     parent).
        ///     中文说明：parent).
        /// </summary>
        IReadOnlyList<IModSettingsBinding> ImmediateSaveTargets { get; }
    }
}
