using Godot;

namespace STS2RitsuLib.Settings
{
    /// <summary>
    ///     Multicast notifications raised after <see cref="IModSettingsValueBinding{TValue}.Write" /> completes on built-in
    ///     Multicast notifications raised 之后 <c>IModSettingsValueBinding{TValue}.Write</c> completes on built-in
    ///     binding implementations. Subscribe from UI or tools that mirror settings elsewhere; use
    ///     binding implementations. Subscribe 从 UI 或 tools that mirror 设置 elsewhere; 使用
    ///     <see cref="SubscribeValueWrittenWhileNodeAlive" /> so subscriptions drop when a host node leaves the tree.
    /// </summary>
    public static class ModSettingsBindingWriteEvents
    {
        /// <summary>
        ///     Raised synchronously from binding <c>Write</c> bodies after the backing store has been updated.
        ///     Raised synchronously 从 binding <c>Write</c> bodies 之后 the backing store has been 更新d.
        /// </summary>
        public static event Action<IModSettingsBinding>? ValueWritten;

        internal static void NotifyValueWritten(IModSettingsBinding binding)
        {
            ValueWritten?.Invoke(binding);
        }

        /// <summary>
        ///     Subscribes while <paramref name="anchor" /> remains in the scene tree and unsubscribes automatically when it
        ///     Subscribes while <c>anchor</c> remains in the 场景 tree 和 unsubscribes automatically 当 it
        ///     exits (same delegate identity used for removal).
        ///     exits (same delegate identity used 用于 removal).
        /// </summary>
        public static void SubscribeValueWrittenWhileNodeAlive(Node anchor, Action<IModSettingsBinding> listener)
        {
            ArgumentNullException.ThrowIfNull(anchor);
            ArgumentNullException.ThrowIfNull(listener);

            ValueWritten += Wrapped;

            anchor.Connect(Node.SignalName.TreeExiting, Callable.From(() => ValueWritten -= Wrapped),
                (uint)GodotObject.ConnectFlags.OneShot);
            return;

            void Wrapped(IModSettingsBinding binding)
            {
                if (!GodotObject.IsInstanceValid(anchor))
                    return;
                listener(binding);
            }
        }
    }
}
