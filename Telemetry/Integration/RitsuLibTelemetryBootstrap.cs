using STS2RitsuLib.Settings;
using STS2RitsuLib.Telemetry.Diagnostics;
using STS2RitsuLib.Telemetry.Integration;
using STS2RitsuLib.Telemetry.RunHistory;

namespace STS2RitsuLib.Telemetry
{
    /// <summary>
    ///     Bootstraps RitsuLib's built-in telemetry applicant and runtime hooks.
    ///     启动 RitsuLib 内置 telemetry 申请方和运行时钩子。
    /// </summary>
    internal static class RitsuLibTelemetryBootstrap
    {
        private static bool _initialized;

        /// <summary>
        ///     Registers the built-in applicant, captures startup facts once, and hooks first-menu consent handling.
        ///     注册内置申请方、一次性采样启动事实，并挂接首次主菜单授权处理。
        /// </summary>
        internal static void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;
            RegisterRitsuLibApplicant();
            DiagnosticsTelemetryCollector.InitializeGlobalExceptionHandlers();
            TelemetrySettingsPages.EnsureRootPage();
            TelemetryRuntime.CaptureStartupSnapshot();
            RitsuLibFramework.SubscribeLifecycle<RunEndedEvent>(RunHistoryTelemetryCollector.CaptureEndedRun);
            RitsuLibFramework.SubscribeLifecycleOnce<MainMenuReadyEvent>(_ =>
                TelemetryConsentPromptCoordinator.TryRunFirstMainMenuFlow());
        }

        private static void RegisterRitsuLibApplicant()
        {
            TelemetryRegistry.RegisterApplicant(new()
            {
                ApplicantId = Const.ModId,
                OwnerModId = Const.ModId,
                DisplayName = "RitsuLib",
                Adapter = RitsuLibTelemetryConfiguration.CreateAdapter(),
                Requests =
                [
                    TelemetryRequest.BasicUsage(
                        T(
                            "ritsulib.telemetry.request.basicUsage.description",
                            "Session start, framework/game versions, build channel, platform, language, and anonymous install id.")),
                    TelemetryRequest.ModInventory(T(
                        "ritsulib.telemetry.request.modInventory.description",
                        "Registered mod inventory, load states, versions, and gameplay flags for compatibility analysis.")),
                    TelemetryRequest.Diagnostics(T(
                        "ritsulib.telemetry.request.diagnostics.description",
                        "Exception reports and framework runtime diagnostics.")),
                ],
            });
        }

        private static ModSettingsText T(string key, string fallback)
        {
            return ModSettingsText.I18N(ModSettingsLocalization.Instance, key, fallback);
        }
    }
}
