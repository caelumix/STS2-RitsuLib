using STS2RitsuLib.Platform;

namespace STS2RitsuLib.Telemetry
{
    internal static class TelemetryRuntimeGate
    {
        internal const string MobileDisabledReason = "Telemetry is disabled on mobile hosts.";

        private static readonly Lock Sync = new();
        private static bool _loggedMobileDisabled;

        internal static bool IsDisabled => RitsuLibMobileSteamRuntime.SuppressNativeSteamIntegration;

        internal static bool TryNoOpForDisabledMobile()
        {
            if (!IsDisabled)
                return false;

            LogDisabledOnce();
            return true;
        }

        internal static void LogDisabledOnce()
        {
            if (!IsDisabled)
                return;

            lock (Sync)
            {
                if (_loggedMobileDisabled)
                    return;

                _loggedMobileDisabled = true;
            }

            RitsuLibFramework.Logger.Info(
                "[Telemetry] Telemetry is disabled on mobile hosts; telemetry APIs will run as no-op.");
        }
    }
}
