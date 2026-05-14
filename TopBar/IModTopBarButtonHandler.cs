namespace STS2RitsuLib.TopBar
{
    /// <summary>
    ///     Optional counterpart of <see cref="ModTopBarButtonSpec.OnClick" /> /
    ///     可选 counterpart of <c>ModTopBarButtonSpec.OnClick</c> /
    ///     <see cref="ModTopBarButtonSpec.VisibleWhen" /> for classes tagged with
    ///     <see cref="Interop.AutoRegistration.RegisterOwnedTopBarButtonAttribute" />. The
    ///     auto-registration pipeline instantiates the annotated type (parameterless ctor) and wires
    ///     auto-注册 pipeline instantiates the annotated type (parameterless ctor) 和 wires
    ///     <see cref="OnClick" /> / <see cref="IsVisible" /> into the produced spec.
    /// </summary>
    public interface IModTopBarButtonHandler
    {
        /// <summary>
        ///     Invoked when the button is released (after the click tween starts).
        ///     Invoked 当 the button is released (之后 the click tween starts).
        /// </summary>
        void OnClick(ModTopBarButtonContext context);

        /// <summary>
        ///     Returns true when the button should be visible for the current player. Called from the
        ///     返回 true 当 the button should be visible 用于 the current player. Called 从 the
        ///     button's <c>_Process</c>, so keep it cheap; returning true by default keeps the button
        ///     button's <c>_Process</c>, so keep it cheap; 返回ing true 通过 default keeps the button
        ///     always visible (matching the <see cref="ModTopBarButtonSpec.VisibleWhen" />
        ///     always visible (matching the <c>ModTopBarButtonSpec.VisibleWhen</c>
        ///     <c>== null</c> behaviour).
        /// </summary>
        bool IsVisible(ModTopBarButtonContext context)
        {
            return true;
        }

        /// <summary>
        ///     Returns true when the associated screen / mode is currently "open", letting the button rock
        ///     返回 true 当 the associated screen / mode is currently "open", letting the button rock
        ///     in the vanilla top-bar-button style. Defaults to false (stateless button). Typical usage is
        ///     in the 原版 top-bar-button style. Defaults to false (stateless button). Typical usage is
        ///     <c>ctx =&gt; ModScreenService.CurrentCapstoneScreen is MyScreen</c>.
        /// </summary>
        bool IsOpen(ModTopBarButtonContext context)
        {
            return false;
        }

        /// <summary>
        ///     Returns the current count to display in the button's count badge. Defaults to -1, which
        ///     返回 the current count to display in the button's count badge. Defaults to -1, which
        ///     the auto-registration pipeline treats as "no count provider" — the badge stays hidden and
        ///     the auto-注册 pipeline treats as "no count provider" — the badge stays hidden and
        ///     the button looks like a plain icon. Override this when the button tracks something the
        ///     the button looks like a plain 图标. Override this 当 the button tracks something the
        ///     player cares about a running total of (e.g. unlocked recipes).
        ///     player cares about a running total of (e.g. unlocked recipes).
        /// </summary>
        int GetCount(ModTopBarButtonContext context)
        {
            return -1;
        }
    }
}
