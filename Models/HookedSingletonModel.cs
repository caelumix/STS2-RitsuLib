using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace STS2RitsuLib.Models
{
    /// <summary>
    ///     Convenience <see cref="SingletonModel" /> base type that can self-subscribe to run and combat hooks.
    ///     Convenience <c>Singleton模型</c> base type that can self-subscribe to 跑局 和 combat hooks.
    ///     This avoids repeating reflection-based hook registration boilerplate in each singleton model.
    ///     This avoids repeating reflection-based hook 注册 boilerplate in each singleton 模型.
    /// </summary>
    public abstract class HookedSingletonModel : SingletonModel
    {
        private static readonly MethodInfo? SubscribeCombat =
            typeof(ModHelper).GetMethod("SubscribeForCombatStateHooks");

        private static readonly MethodInfo? SubscribeRunState =
            typeof(ModHelper).GetMethod("SubscribeForRunStateHooks");

        private static readonly Type? RunHookSubscriptionDelegateType =
            Type.GetType("MegaCrit.Sts2.Core.Modding.RunHookSubscriptionDelegate, sts2");

        private static readonly Type? CombatHookSubscriptionDelegateType =
            Type.GetType("MegaCrit.Sts2.Core.Modding.CombatHookSubscriptionDelegate, sts2");

        /// <summary>
        ///     Creates the singleton instance and optionally subscribes it to the corresponding hook streams.
        ///     创建 the singleton instance and optionally subscribes it to the corresponding hook streams。
        /// </summary>
        /// <param name="receiveCombatHooks">
        ///     When true, subscribes the singleton to combat hook callbacks.
        ///     为 true 时，subscribes the singleton to combat hook callbacks。
        /// </param>
        /// <param name="receiveRunHooks">
        ///     When true, subscribes the singleton to run hook callbacks.
        ///     为 true 时，subscribes the singleton to run hook callbacks。
        /// </param>
        protected HookedSingletonModel(bool receiveCombatHooks, bool receiveRunHooks)
        {
            ShouldReceiveCombatHooks = receiveCombatHooks;

            if (!receiveCombatHooks && !receiveRunHooks)
                return;

            if (SubscribeCombat == null || SubscribeRunState == null)
            {
                RitsuLibFramework.Logger.Warn(
                    $"[HookedSingletonModel] Singleton created but hook subscription is unavailable on this game branch: {GetType().FullName}");
                return;
            }

            if ((receiveRunHooks && RunHookSubscriptionDelegateType == null) ||
                (receiveCombatHooks && CombatHookSubscriptionDelegateType == null))
            {
                RitsuLibFramework.Logger.Warn(
                    $"[HookedSingletonModel] Singleton created but hook subscription delegate type is unavailable on this game branch: {GetType().FullName}");
                return;
            }

            if (receiveRunHooks)
                SubscribeRunState.Invoke(null,
                [
                    Id.Entry,
                    Delegate.CreateDelegate(
                        RunHookSubscriptionDelegateType!,
                        this,
                        GetType().GetMethod(nameof(RunSubModels), BindingFlags.Instance | BindingFlags.NonPublic)!
                    ),
                ]);

            if (receiveCombatHooks)
                SubscribeCombat.Invoke(null,
                [
                    Id.Entry,
                    Delegate.CreateDelegate(
                        CombatHookSubscriptionDelegateType!,
                        this,
                        GetType().GetMethod(nameof(CombatSubModels), BindingFlags.Instance | BindingFlags.NonPublic)!
                    ),
                ]);
        }

        /// <inheritdoc />
        public override bool ShouldReceiveCombatHooks { get; }

        /// <summary>
        ///     Provides the run-scoped sub-models that should receive run-state hook callbacks for this singleton.
        ///     Provides the 跑局-scoped sub-Models that should receive 跑局-state hook callbacks 用于 this singleton.
        /// </summary>
        /// <param name="runState">
        ///     The current run state.
        ///     该 current run state。
        /// </param>
        /// <returns>
        ///     The models to subscribe for run hooks.
        ///     该 models to subscribe for run hooks。
        /// </returns>
        protected IEnumerable<AbstractModel> RunSubModels(RunState runState)
        {
            return [this];
        }

        /// <summary>
        ///     Provides the combat-scoped sub-models that should receive combat-state hook callbacks for this singleton.
        ///     Provides the combat-scoped sub-Models that should receive combat-state hook callbacks 用于 this singleton.
        /// </summary>
        /// <param name="combatState">
        ///     The current combat state.
        ///     该 current combat state。
        /// </param>
        /// <returns>
        ///     The models to subscribe for combat hooks.
        ///     该 models to subscribe for combat hooks。
        /// </returns>
        protected IEnumerable<AbstractModel> CombatSubModels(CombatState combatState)
        {
            return [this];
        }
    }
}
