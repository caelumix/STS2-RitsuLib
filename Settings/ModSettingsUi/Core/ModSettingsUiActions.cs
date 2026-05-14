using System.Collections.Concurrent;

namespace STS2RitsuLib.Settings
{
    /// <summary>
    ///     Minimal host exposed when registering actions with <see cref="ModSettingsUiActionRegistry" /> (refresh and dirty
    ///     Minimal host exposed 当 registering actions 带有 <c>ModSettingsUiaction注册表</c> (refresh 和 dirty
    ///     marking).
    ///     中文说明：marking).
    /// </summary>
    public interface IModSettingsUiActionHost
    {
        /// <summary>
        ///     Requests a deferred UI rebuild (e.g. after list mutation).
        ///     Requests a deferred UI rebuild (e.g. 之后 list mutation).
        /// </summary>
        void RequestRefresh();

        /// <summary>
        ///     Marks <paramref name="binding" /> dirty so persistence runs on the next flush.
        ///     中文说明：Marks <c>binding</c> dirty so persistence runs on the next flush.
        ///     Marks <c>binding</c> dirty so persistence runs on the next flush.
        ///     中文说明：Marks <c>binding</c> dirty so persistence runs on the next flush.
        /// </summary>
        void MarkDirty(IModSettingsBinding binding);

        /// <summary>
        ///     Schedules the next deferred refresh as a full binding pass for the current page: every
        ///     Schedules the next deferred refresh as a full binding pass 用于 the current page: every
        ///     <c>RegisterRefresh</c> callback on that page runs once, without requiring <see cref="MarkDirty" /> per
        ///     binding. Use after batch-mutating many fields or a shared backing object. Persisted keys still need
        ///     binding. 使用 之后 batch-mutating many fields 或 a shared backing object. Persisted keys still need
        ///     <see cref="MarkDirty" /> on each binding you changed so <see cref="IModSettingsBinding.Save" /> runs.
        /// </summary>
        void RequestRefreshAfterDataModelBatchChange()
        {
            RequestRefresh();
        }
    }

    /// <summary>
    ///     Stable ids for built-in settings menu items (convention for extensions; not guaranteed unique).
    ///     稳定的 ids for built-in settings menu items (convention for extensions; not guaranteed unique)。
    /// </summary>
    public static class ModSettingsStandardActionIds
    {
        /// <summary>
        ///     Reset binding to its default value.
        ///     Re设置 binding to its default value.
        /// </summary>
        public const string ResetToDefault = "ritsulib.settings.resetDefault";

        /// <summary>
        ///     Copy current value to the clipboard envelope.
        ///     中文说明：Copy current value to the clipboard envelope.
        /// </summary>
        public const string Copy = "ritsulib.settings.copy";

        /// <summary>
        ///     Paste from the clipboard envelope into the binding.
        ///     Paste 从 the clipboard envelope into the binding.
        /// </summary>
        public const string Paste = "ritsulib.settings.paste";

        /// <summary>
        ///     Move a list item up.
        ///     中文说明：Move a list item up.
        /// </summary>
        public const string MoveUp = "ritsulib.settings.moveUp";

        /// <summary>
        ///     Move a list item down.
        ///     中文说明：Move a list item down.
        /// </summary>
        public const string MoveDown = "ritsulib.settings.moveDown";

        /// <summary>
        ///     Duplicate a list item.
        ///     中文说明：Duplicate a list item.
        /// </summary>
        public const string Duplicate = "ritsulib.settings.duplicate";

        /// <summary>
        ///     Remove a list item.
        ///     中文说明：Remove a list item.
        /// </summary>
        public const string Remove = "ritsulib.settings.remove";

        /// <summary>
        ///     Copy an entire settings page (chrome clipboard).
        ///     Copy an entire 设置 page (chrome clipboard).
        /// </summary>
        public const string PageCopy = "ritsulib.settings.page.copy";

        /// <summary>
        ///     Paste an entire settings page (chrome clipboard).
        ///     Paste an entire 设置 page (chrome clipboard).
        /// </summary>
        public const string PagePaste = "ritsulib.settings.page.paste";

        /// <summary>
        ///     Copy a single section (chrome clipboard).
        ///     中文说明：Copy a single section (chrome clipboard).
        /// </summary>
        public const string SectionCopy = "ritsulib.settings.section.copy";

        /// <summary>
        ///     Paste into a single section (chrome clipboard).
        ///     中文说明：Paste into a single section (chrome clipboard).
        /// </summary>
        public const string SectionPaste = "ritsulib.settings.section.paste";
    }

    /// <summary>
    ///     Action context for a settings page (no binding).
    ///     action context 用于 a 设置 page (no binding).
    /// </summary>
    public sealed class ModSettingsPageUiContext(ModSettingsPage page, IModSettingsUiActionHost host)
    {
        /// <summary>
        ///     Page being targeted by page-level actions.
        ///     Page being targeted 通过 page-level actions.
        /// </summary>
        public ModSettingsPage Page { get; } = page;

        /// <summary>
        ///     Host for refresh and dirty propagation.
        ///     Host 用于 refresh 和 dirty propagation.
        /// </summary>
        public IModSettingsUiActionHost Host { get; } = host;
    }

    /// <summary>
    ///     Action context for a settings section (no standalone binding).
    ///     action context 用于 a 设置 section (no standalone binding).
    /// </summary>
    public sealed class ModSettingsSectionUiContext(
        ModSettingsPage page,
        ModSettingsSection section,
        IModSettingsUiActionHost host)
    {
        /// <summary>
        ///     Owning page.
        ///     中文说明：Owning page.
        /// </summary>
        public ModSettingsPage Page { get; } = page;

        /// <summary>
        ///     Section receiving section-level actions.
        ///     中文说明：Section receiving section-level actions.
        ///     Section receiving section-level actions.
        ///     中文说明：Section receiving section-level actions.
        /// </summary>
        public ModSettingsSection Section { get; } = section;

        /// <summary>
        ///     Host for refresh and dirty propagation.
        ///     Host 用于 refresh 和 dirty propagation.
        /// </summary>
        public IModSettingsUiActionHost Host { get; } = host;
    }

    /// <summary>
    ///     How much of a value subtree copy/paste should include (binding self vs. nested structure).
    ///     中文说明：How much of a value subtree copy/paste should include (binding self vs. nested structure).
    /// </summary>
    public enum ModSettingsClipboardScope
    {
        /// <summary>
        ///     Only the immediate binding value.
        ///     中文说明：Only the immediate binding value.
        /// </summary>
        Self = 0,

        /// <summary>
        ///     Include nested structured data where supported.
        ///     中文说明：Include nested structured data where supported.
        /// </summary>
        Subtree = 1,
    }

    /// <summary>
    ///     One command in the settings Actions menu or context menu.
    ///     One command in the 设置 actions menu 或 context menu.
    /// </summary>
    public sealed record ModSettingsMenuAction(string? Id, string Label, Func<bool> IsEnabled, Action Action)
    {
        /// <summary>
        ///     Creates an action with a fixed enabled flag.
        ///     创建 an action with a fixed enabled flag。
        /// </summary>
        public ModSettingsMenuAction(string label, bool enabled, Action action)
            : this(null, label, () => enabled, action)
        {
        }

        /// <summary>
        ///     Creates an action with a dynamic enabled predicate.
        ///     创建 an action with a dynamic enabled predicate。
        /// </summary>
        public ModSettingsMenuAction(string label, Func<bool> isEnabled, Action action)
            : this(null, label, isEnabled, action)
        {
        }

        /// <summary>
        ///     Creates an action with optional stable <paramref name="id" /> and fixed enabled flag.
        ///     创建 an action with optional stable <c>id</c> and fixed enabled flag。
        /// </summary>
        public ModSettingsMenuAction(string? id, string label, bool enabled, Action action)
            : this(id, label, () => enabled, action)
        {
        }
    }

    /// <summary>
    ///     Appends custom menu items for binding rows, list items, pages, and sections.
    ///     Appends 自定义 menu items 用于 binding rows, list items, pages, 和 sections.
    /// </summary>
    public static class ModSettingsUiActionRegistry
    {
        private static readonly ConcurrentDictionary<Type, BindingAppenderBag> BindingAppenders = new();
        private static readonly ConcurrentDictionary<Type, ListItemAppenderBag> ListItemAppenders = new();
        private static readonly PageAppenderBag PageAppenders = new();
        private static readonly SectionAppenderBag SectionAppenders = new();

        /// <summary>
        ///     Registers a callback that appends menu items for value bindings of type
        ///     Registers a callback that appends menu items 用于 value bindings of type
        ///     <typeparamref name="TValue" />.
        /// </summary>
        public static void RegisterBindingActionAppender<TValue>(
            Action<IModSettingsUiActionHost, IModSettingsValueBinding<TValue>, List<ModSettingsMenuAction>> append)
        {
            BindingAppenders.GetOrAdd(typeof(TValue), _ => new()).Add(append);
        }

        /// <summary>
        ///     Registers a callback that appends menu items for list rows of item type
        ///     Registers a callback that appends menu items 用于 list rows of item type
        ///     <typeparamref name="TItem" />.
        /// </summary>
        public static void RegisterListItemActionAppender<TItem>(
            Action<IModSettingsUiActionHost, ModSettingsListItemContext<TItem>, List<ModSettingsMenuAction>> append)
        {
            ListItemAppenders.GetOrAdd(typeof(TItem), _ => new()).Add(append);
        }

        /// <summary>
        ///     Registers a callback that appends page-level menu items.
        ///     注册 a callback that appends page-level menu items。
        /// </summary>
        public static void RegisterPageActionAppender(
            Action<IModSettingsUiActionHost, ModSettingsPageUiContext, List<ModSettingsMenuAction>> append)
        {
            ArgumentNullException.ThrowIfNull(append);
            PageAppenders.Add(append);
        }

        /// <summary>
        ///     Registers a callback that appends section-level menu items.
        ///     注册 a callback that appends section-level menu items。
        /// </summary>
        public static void RegisterSectionActionAppender(
            Action<IModSettingsUiActionHost, ModSettingsSectionUiContext, List<ModSettingsMenuAction>> append)
        {
            ArgumentNullException.ThrowIfNull(append);
            SectionAppenders.Add(append);
        }

        internal static void AppendBindingActions<TValue>(IModSettingsUiActionHost host,
            IModSettingsValueBinding<TValue> binding, List<ModSettingsMenuAction> list)
        {
            if (BindingAppenders.TryGetValue(typeof(TValue), out var bag))
                bag.Invoke(host, binding, list);
        }

        internal static void AppendListItemActions<TItem>(IModSettingsUiActionHost host,
            ModSettingsListItemContext<TItem> itemContext, List<ModSettingsMenuAction> list)
        {
            if (ListItemAppenders.TryGetValue(typeof(TItem), out var bag))
                bag.Invoke(host, itemContext, list);
        }

        internal static void AppendPageActions(IModSettingsUiActionHost host, ModSettingsPageUiContext pageContext,
            List<ModSettingsMenuAction> list)
        {
            PageAppenders.Invoke(host, pageContext, list);
        }

        internal static void AppendSectionActions(IModSettingsUiActionHost host,
            ModSettingsSectionUiContext sectionContext,
            List<ModSettingsMenuAction> list)
        {
            SectionAppenders.Invoke(host, sectionContext, list);
        }

        private sealed class BindingAppenderBag
        {
            private readonly List<Delegate> _delegates = [];

            public void Add<TValue>(
                Action<IModSettingsUiActionHost, IModSettingsValueBinding<TValue>, List<ModSettingsMenuAction>> d)
            {
                _delegates.Add(d);
            }

            public void Invoke<TValue>(IModSettingsUiActionHost host, IModSettingsValueBinding<TValue> binding,
                List<ModSettingsMenuAction> sink)
            {
                foreach (var d in _delegates)
                    ((Action<IModSettingsUiActionHost, IModSettingsValueBinding<TValue>, List<ModSettingsMenuAction>>)d)
                        (host, binding, sink);
            }
        }

        private sealed class ListItemAppenderBag
        {
            private readonly List<Delegate> _delegates = [];

            public void Add<TItem>(
                Action<IModSettingsUiActionHost, ModSettingsListItemContext<TItem>, List<ModSettingsMenuAction>> d)
            {
                _delegates.Add(d);
            }

            public void Invoke<TItem>(IModSettingsUiActionHost host, ModSettingsListItemContext<TItem> itemContext,
                List<ModSettingsMenuAction> sink)
            {
                foreach (var d in _delegates)
                    ((Action<IModSettingsUiActionHost, ModSettingsListItemContext<TItem>, List<ModSettingsMenuAction>>)
                            d)
                        (host, itemContext, sink);
            }
        }

        private sealed class PageAppenderBag
        {
            private readonly
                List<Action<IModSettingsUiActionHost, ModSettingsPageUiContext, List<ModSettingsMenuAction>>>
                _delegates = [];

            public void Add(Action<IModSettingsUiActionHost, ModSettingsPageUiContext, List<ModSettingsMenuAction>> d)
            {
                _delegates.Add(d);
            }

            public void Invoke(IModSettingsUiActionHost host, ModSettingsPageUiContext pageContext,
                List<ModSettingsMenuAction> sink)
            {
                foreach (var d in _delegates)
                    d(host, pageContext, sink);
            }
        }

        private sealed class SectionAppenderBag
        {
            private readonly List<Action<IModSettingsUiActionHost, ModSettingsSectionUiContext,
                    List<ModSettingsMenuAction>>>
                _delegates = [];

            public void Add(
                Action<IModSettingsUiActionHost, ModSettingsSectionUiContext, List<ModSettingsMenuAction>> d)
            {
                _delegates.Add(d);
            }

            public void Invoke(IModSettingsUiActionHost host, ModSettingsSectionUiContext sectionContext,
                List<ModSettingsMenuAction> sink)
            {
                foreach (var d in _delegates)
                    d(host, sectionContext, sink);
            }
        }
    }
}
