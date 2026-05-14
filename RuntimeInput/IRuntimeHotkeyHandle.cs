namespace STS2RitsuLib.RuntimeInput
{
    /// <summary>
    ///     Represents a registered runtime hotkey that can be rebound or unregistered explicitly by the caller.
    ///     Represents a 已注册 runtime hotkey that can be rebound 或 un已注册 explicitly 通过 the caller.
    /// </summary>
    public interface IRuntimeHotkeyHandle : IDisposable
    {
        /// <summary>
        ///     Gets the first current normalized binding string for this registration.
        ///     Gets the first current normalized binding string 用于 this 注册.
        /// </summary>
        string CurrentBinding { get; }

        /// <summary>
        ///     Gets all current normalized binding strings for this registration.
        ///     Gets all current normalized binding strings 用于 this 注册.
        /// </summary>
        IReadOnlyList<string> CurrentBindings { get; }

        /// <summary>
        ///     Gets whether this handle is still registered with the runtime hotkey router.
        ///     Gets whether this handle is still 已注册 带有 the runtime hotkey router.
        /// </summary>
        bool IsRegistered { get; }

        /// <summary>
        ///     Replaces the binding with a newly persisted binding string.
        ///     Replaces the binding 带有 a newly persisted binding string.
        /// </summary>
        /// <param name="bindingText">
        ///     Binding text to parse and apply.
        ///     Binding text to parse 和 apply.
        /// </param>
        /// <param name="normalizedBinding">
        ///     The normalized binding string if parsing succeeded.
        ///     该 normalized binding string if parsing succeeded。
        /// </param>
        /// <returns>
        ///     <c>true</c> when the new binding was parsed and applied.
        ///     <c>true</c> 当 the new binding was parsed 和 applied.
        /// </returns>
        bool TryRebind(string bindingText, out string normalizedBinding);

        /// <summary>
        ///     Replaces all bindings with newly persisted binding strings.
        ///     Replaces all bindings 带有 newly persisted binding strings.
        /// </summary>
        /// <param name="bindingTexts">
        ///     Binding texts to parse and apply.
        ///     Binding texts to parse 和 apply.
        /// </param>
        /// <param name="normalizedBindings">
        ///     Normalized binding strings if parsing succeeded.
        ///     Normalized binding strings 如果 parsing succeeded.
        /// </param>
        /// <returns>
        ///     <c>true</c> when all bindings were parsed and applied.
        ///     <c>true</c> 当 all bindings were parsed 和 applied.
        /// </returns>
        bool TryRebind(IEnumerable<string> bindingTexts, out IReadOnlyList<string> normalizedBindings);

        /// <summary>
        ///     Returns a read-only snapshot describing the current registration.
        ///     返回 a read-only snapshot describing the current registration。
        /// </summary>
        /// <param name="registrationInfo">
        ///     Registration snapshot when this handle is still active.
        ///     Registration snapshot 当 this handle is still active.
        /// </param>
        /// <returns>
        ///     <c>true</c> when this handle is still registered.
        ///     <c>true</c> 当 this handle is still 已注册.
        /// </returns>
        bool TryGetRegistrationInfo(out RuntimeHotkeyRegistrationInfo registrationInfo);

        /// <summary>
        ///     Removes this registration from the runtime hotkey router.
        ///     Removes this 注册 从 the runtime hotkey router.
        /// </summary>
        void Unregister();
    }
}
