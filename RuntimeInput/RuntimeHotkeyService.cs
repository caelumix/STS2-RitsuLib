using Godot;
using MegaCrit.Sts2.Core.Nodes;

namespace STS2RitsuLib.RuntimeInput
{
    /// <summary>
    ///     Provides a settings-independent runtime hotkey API that parses persisted binding strings and
    ///     Provides a 设置-independent runtime hotkey API that parses persisted binding strings and
    ///     registers input callbacks against a shared router node.
    ///     注册 input callbacks against a shared router node。
    /// </summary>
    public static class RuntimeHotkeyService
    {
        private static readonly Lock SyncRoot = new();
        private static RuntimeHotkeyRouterNode? _router;
        private static IDisposable? _lifecycleSubscription;

        /// <summary>
        ///     Ensures the shared router will be attached when the game root becomes ready.
        ///     Ensures the shared router will be attached 当 the game root becomes ready.
        /// </summary>
        public static void Initialize()
        {
            lock (SyncRoot)
            {
                _lifecycleSubscription ??= RitsuLibFramework.SubscribeLifecycle<GameReadyEvent>(evt =>
                {
                    EnsureRouterAttached(evt.Game);
                });
            }
        }

        /// <summary>
        ///     Returns read-only snapshots for all currently registered runtime hotkeys.
        ///     返回 read-only snapshots for all currently registered runtime hotkeys。
        /// </summary>
        public static IReadOnlyList<RuntimeHotkeyRegistrationInfo> GetRegisteredHotkeys()
        {
            lock (SyncRoot)
            {
                return _router?.GetRegistrationInfos() ?? [];
            }
        }

        /// <summary>
        ///     Returns detailed read-only snapshots for all currently registered runtime hotkeys, including every binding.
        ///     返回 detailed read-only snapshots for all currently registered runtime hotkeys, including every binding。
        /// </summary>
        public static IReadOnlyList<RuntimeHotkeyRegistrationDetails> GetRegisteredHotkeyDetails()
        {
            lock (SyncRoot)
            {
                return _router?.GetRegistrationDetails() ?? [];
            }
        }

        /// <summary>
        ///     Tries to return the currently registered hotkey snapshot for a stable registration id.
        ///     Tries to 返回 the currently 已注册 hotkey snapshot 用于 a stable 注册 id.
        /// </summary>
        /// <param name="id">
        ///     Stable registration id to locate.
        ///     稳定的 registration id to locate。
        /// </param>
        /// <param name="registrationInfo">
        ///     Registration snapshot when a matching id exists.
        ///     Registration snapshot 当 a matching id exists.
        /// </param>
        /// <returns>
        ///     <c>true</c> when a matching registration was found.
        ///     <c>true</c> 当 a matching 注册 was found.
        /// </returns>
        public static bool TryGetRegisteredHotkey(string id, out RuntimeHotkeyRegistrationInfo registrationInfo)
        {
            lock (SyncRoot)
            {
                var info = _router?.GetRegistrationInfoById(id);
                if (info != null)
                {
                    registrationInfo = info;
                    return true;
                }

                registrationInfo = null!;
                return false;
            }
        }

        /// <summary>
        ///     Attempts to normalize a persisted binding string into the runtime hotkey canonical format.
        ///     Attempts to normalize a persisted binding string into the runtime hotkey canonical 用于mat.
        /// </summary>
        /// <param name="bindingText">
        ///     Binding text to normalize.
        ///     中文说明：Binding text to normalize.
        /// </param>
        /// <param name="normalizedBinding">
        ///     Canonical binding string when parsing succeeds.
        ///     Canonical binding string 当 parsing succeeds.
        /// </param>
        /// <returns>
        ///     <c>true</c> when the binding string was parsed successfully.
        ///     <c>true</c> 当 the binding string was parsed successfully.
        /// </returns>
        public static bool TryNormalizeBinding(string? bindingText, out string normalizedBinding)
        {
            return RuntimeHotkeyParser.TryParse(bindingText, out _, out normalizedBinding);
        }

        /// <summary>
        ///     Returns the normalized binding string, or <paramref name="fallback" /> when parsing fails.
        ///     返回 the normalized binding string, or <c>fallback</c> when parsing fails。
        /// </summary>
        /// <param name="bindingText">
        ///     Binding text to normalize.
        ///     中文说明：Binding text to normalize.
        /// </param>
        /// <param name="fallback">
        ///     Fallback value returned when parsing fails.
        ///     Fallback value 返回ed 当 parsing fails.
        /// </param>
        public static string NormalizeOrDefault(string? bindingText, string fallback)
        {
            return RuntimeHotkeyParser.NormalizeOrDefault(bindingText, fallback);
        }

        /// <summary>
        ///     Registers a runtime hotkey directly from a persisted binding string.
        ///     注册 a runtime hotkey directly from a persisted binding string。
        /// </summary>
        /// <param name="bindingText">
        ///     Persisted binding string to parse.
        ///     中文说明：Persisted binding string to parse.
        /// </param>
        /// <param name="callback">
        ///     Callback invoked when the hotkey matches.
        ///     Callback invoked 当 the hotkey matches.
        /// </param>
        /// <param name="options">
        ///     Optional router behavior overrides.
        ///     可选 router behavior overrides.
        /// </param>
        /// <returns>
        ///     A handle that supports explicit rebind and unregister operations.
        ///     一个 handle that supports explicit rebind and unregister operations。
        /// </returns>
        /// <exception cref="FormatException">Thrown when <paramref name="bindingText" /> is invalid.</exception>
        public static IRuntimeHotkeyHandle Register(string bindingText, Action callback,
            RuntimeHotkeyOptions? options = null)
        {
            return Register([bindingText], callback, options);
        }

        /// <summary>
        ///     Registers one logical runtime hotkey against multiple persisted binding strings.
        ///     注册 one logical runtime hotkey against multiple persisted binding strings。
        /// </summary>
        /// <param name="bindingTexts">
        ///     Persisted binding strings to parse.
        ///     中文说明：Persisted binding strings to parse.
        /// </param>
        /// <param name="callback">
        ///     Callback invoked when any registered binding matches.
        ///     Callback invoked 当 any 已注册 binding matches.
        /// </param>
        /// <param name="options">
        ///     Optional router behavior overrides.
        ///     可选 router behavior overrides.
        /// </param>
        /// <returns>
        ///     A handle that supports explicit rebind and unregister operations.
        ///     一个 handle that supports explicit rebind and unregister operations。
        /// </returns>
        /// <exception cref="FormatException">Thrown when any binding is invalid.</exception>
        public static IRuntimeHotkeyHandle Register(IEnumerable<string> bindingTexts, Action callback,
            RuntimeHotkeyOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(bindingTexts);
            ArgumentNullException.ThrowIfNull(callback);
            Initialize();

            var bindings = ParseBindings(bindingTexts);
            if (bindings.Count == 0)
                throw new FormatException("Runtime hotkey registration requires at least one valid binding.");

            lock (SyncRoot)
            {
                EnsureRouterAttached(NGame.Instance);
                if (_router == null)
                    throw new InvalidOperationException("Runtime hotkey router is not available.");

                var handle = _router.Register(bindings, callback, options);
                RitsuLibFramework.Logger.Info(
                    $"[RuntimeHotkey] Registered '{string.Join("', '", bindings.Select(static b => b.CanonicalString))}'{FormatDebugName(options)}");
                return handle;
            }
        }

        private static List<RuntimeHotkeyBinding> ParseBindings(IEnumerable<string> bindingTexts)
        {
            var bindings = new List<RuntimeHotkeyBinding>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var bindingText in bindingTexts)
            {
                if (!RuntimeHotkeyParser.TryParse(bindingText, out var binding, out var normalizedBinding))
                    throw new FormatException($"Invalid runtime hotkey binding '{bindingText}'.");
                if (seen.Add(normalizedBinding))
                    bindings.Add(binding);
            }

            return bindings;
        }

        private static void EnsureRouterAttached(Node? gameNode)
        {
            if (_router != null && GodotObject.IsInstanceValid(_router))
                return;
            if (gameNode == null)
                return;

            _router = new() { Name = "RitsuRuntimeHotkeyRouter" };
            gameNode.AddChild(_router);
        }

        private static string FormatDebugName(RuntimeHotkeyOptions? options)
        {
            return string.IsNullOrWhiteSpace(options?.DebugName) ? string.Empty : $" for {options.DebugName}";
        }
    }
}
