namespace STS2RitsuLib.Updates
{
    internal static class UpdateCheckSessionState
    {
        private static int _initialized;
        private static volatile bool _isCombatActive;
        private static volatile bool _isMainMenuActive;

        internal static bool IsCombatActive
        {
            get
            {
                Initialize();
                return _isCombatActive;
            }
        }

        internal static bool IsMainMenuActive
        {
            get
            {
                Initialize();
                return _isMainMenuActive;
            }
        }

        internal static void Initialize()
        {
            if (Interlocked.Exchange(ref _initialized, 1) != 0)
                return;

            RitsuLibFramework.SubscribeLifecycle<MainMenuReadyEvent>(_ =>
            {
                _isCombatActive = false;
                _isMainMenuActive = true;
                UpdateCheckNotificationQueue.FlushPending();
            });
            RitsuLibFramework.SubscribeLifecycle<RunStartedEvent>(_ => _isMainMenuActive = false);
            RitsuLibFramework.SubscribeLifecycle<RunLoadedEvent>(_ => _isMainMenuActive = false);
            RitsuLibFramework.SubscribeLifecycle<CombatStartingEvent>(_ =>
            {
                _isMainMenuActive = false;
                _isCombatActive = true;
            });
            RitsuLibFramework.SubscribeLifecycle<CombatEndedEvent>(_ => _isCombatActive = false);
            RitsuLibFramework.SubscribeLifecycle<CombatVictoryEvent>(_ => _isCombatActive = false);
            RitsuLibFramework.SubscribeLifecycle<RunEndedEvent>(_ => _isCombatActive = false);
        }
    }
}
