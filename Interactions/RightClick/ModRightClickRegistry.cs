using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Models.Identity;
using STS2RitsuLib.Networking.Sidecar;

namespace STS2RitsuLib.Interactions.RightClick
{
    /// <summary>
    ///     Registry and dispatcher for model right-click interactions.
    ///     模型右键交互的注册表与分发器。
    /// </summary>
    public static class ModRightClickRegistry
    {
        private const string SidecarModuleId = "ritsulib";
        private const string SidecarActionKey = "model_right_click";
        private const string SidecarNonCombatRequestKey = "model_right_click_noncombat_request";
        private const string SidecarNonCombatApplyKey = "model_right_click_noncombat_apply";
        private const int InitialOffset = 0;

        private static readonly Lock Gate = new();

        private static readonly List<IModRightClickHandler> Handlers =
        [
            new InterfaceModelRightClickHandler(),
        ];

        private static readonly RitsuLibSidecarSyncActionDescriptor<ModRightClickSyncPayload> SyncActionDescriptor =
            new(
                SidecarModuleId,
                SidecarActionKey,
                SerializePayload,
                DeserializePayload,
                ExecuteSynced,
                GameActionType.CombatPlayPhaseOnly);

        private static readonly RitsuLibSidecarSyncMessageDescriptor<ModRightClickSyncPayload>
            NonCombatRequestDescriptor =
                new(
                    SidecarModuleId,
                    SidecarNonCombatRequestKey,
                    SerializePayload,
                    DeserializePayload,
                    HandleNonCombatRequest,
                    true);

        private static readonly RitsuLibSidecarSyncMessageDescriptor<ModRightClickSyncPayload>
            NonCombatApplyDescriptor =
                new(
                    SidecarModuleId,
                    SidecarNonCombatApplyKey,
                    SerializePayload,
                    DeserializePayload,
                    HandleNonCombatApply,
                    true);

        /// <summary>
        ///     Registers a custom right-click handler. Higher priority handlers run first.
        ///     注册自定义右键 handler；优先级越高越先运行。
        /// </summary>
        public static void Register(IModRightClickHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);
            lock (Gate)
            {
                if (Handlers.Contains(handler))
                    return;

                Handlers.Add(handler);
                Handlers.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            }
        }

        /// <summary>
        ///     Attempts to dispatch a local right-click request.
        ///     尝试分发一个本地右键请求。
        /// </summary>
        public static bool TryDispatch(ModRightClickContext context)
        {
            IModRightClickHandler[] handlers;
            lock (Gate)
            {
                handlers = [..Handlers];
            }

            return handlers.Any(handler => handler.TryHandle(context));
        }

        internal static void RegisterBuiltInSyncDescriptors()
        {
            RitsuLibSidecarSyncActions.Register(SyncActionDescriptor);
            RitsuLibSidecarSyncMessages.Register(NonCombatRequestDescriptor);
            RitsuLibSidecarSyncMessages.Register(NonCombatApplyDescriptor);
        }

        private static bool TryRequestSyncedModelAction(ModRightClickContext context)
        {
            if (!TryCreatePayload(context, out var payload))
                return false;
            if (!CombatManager.Instance.IsInProgress)
                return TryRequestNonCombatAction(payload);

            return RitsuLibSidecarSyncActions.RequestCombatPlayPhaseAction(
                RunManager.Instance,
                SyncActionDescriptor,
                payload,
                context.Player.NetId);
        }

        private static bool TryCreatePayload(ModRightClickContext context, out ModRightClickSyncPayload payload)
        {
            payload = default;
            if (!TryGetModelKind(context.Model, context.Player, out var kind))
                return false;
            if (!ModModelIdentityRegistry.TryGetToken(context.Model, out var token))
                return false;

            payload = new(
                context.Player.NetId,
                kind,
                token,
                context.Trigger);
            return true;
        }

        private static bool TryGetModelKind(
            AbstractModel model,
            Player player,
            out ModRightClickModelKind kind)
        {
            kind = default;
            switch (model)
            {
                case CardModel card when card.Owner == player:
                    kind = ModRightClickModelKind.Card;
                    return true;

                case RelicModel relic when relic.Owner == player:
                    kind = ModRightClickModelKind.Relic;
                    return true;

                case PowerModel power when IsPowerReachableForPlayer(power, player):
                    kind = ModRightClickModelKind.Power;
                    return true;

                case PotionModel potion when potion.Owner == player:
                    kind = ModRightClickModelKind.Potion;
                    return true;

                default:
                    return false;
            }
        }

        private static bool TryRequestNonCombatAction(ModRightClickSyncPayload payload)
        {
            var runManager = RunManager.Instance;
            var netService = runManager?.NetService;
            if (runManager == null || netService == null)
                return false;

            return netService.Type switch
            {
                NetGameType.Client => RitsuLibSidecarSyncMessages.SendToHost(
                    runManager,
                    NonCombatRequestDescriptor,
                    payload),
                _ => RitsuLibSidecarSyncMessages.Broadcast(
                    runManager,
                    NonCombatApplyDescriptor,
                    payload),
            };
        }

        private static bool IsPowerReachableForPlayer(PowerModel power, Player player)
        {
            return power.Owner.Player == player ||
                   power.Owner.PetOwner == player ||
                   power.Owner.IsEnemy;
        }

        private static byte[] SerializePayload(ModRightClickSyncPayload payload)
        {
            var writer = new PacketWriter { WarnOnGrow = false };
            writer.WriteULong(payload.OwnerNetId);
            writer.WriteEnum(payload.Kind);
            writer.WriteFullModelId(payload.Token.ModelId);
            writer.WriteUInt(payload.Token.Identity.Value);

            writer.WriteBool(payload.Trigger.IsController);
            writer.WriteBool(payload.Trigger.Metadata != null);
            if (payload.Trigger.Metadata != null)
                writer.WriteString(payload.Trigger.Metadata);

            writer.ZeroByteRemainder();
            return writer.Buffer.AsSpan(InitialOffset, writer.BytePosition).ToArray();
        }

        private static ModRightClickSyncPayload DeserializePayload(ReadOnlySpan<byte> bytes)
        {
            var reader = new PacketReader();
            reader.Reset(bytes.ToArray());
            var ownerNetId = reader.ReadULong();
            var kind = reader.ReadEnum<ModRightClickModelKind>();
            var modelId = reader.ReadFullModelId();
            var identity = new ModModelIdentity(reader.ReadUInt());

            var isController = reader.ReadBool();
            var metadata = reader.ReadBool() ? reader.ReadString() : null;
            return new(
                ownerNetId,
                kind,
                new(identity, modelId),
                new(isController, metadata));
        }

        private static async Task ExecuteSynced(
            RitsuLibSidecarSyncActionContext<ModRightClickSyncPayload> context)
        {
            if (context.Message.OwnerNetId != context.OwnerNetId)
                return;

            await ExecutePayload(context.Message, context.PlayerChoiceContext, context.Action);
        }

        private static Task HandleNonCombatRequest(
            RitsuLibSidecarSyncMessageContext<ModRightClickSyncPayload> context)
        {
            if (!context.IsHostIngest ||
                RunManager.Instance?.NetService is not NetHostGameService ||
                context.Message.OwnerNetId != context.SenderNetId ||
                !TryGetPlayer(context.Message.OwnerNetId, out var player) ||
                !TryResolveModel(player, context.Message, out var model) ||
                model is not IModRightClickableModel)
                return Task.CompletedTask;

            _ = RitsuLibSidecarSyncMessages.Broadcast(
                RunManager.Instance,
                NonCombatApplyDescriptor,
                context.Message);
            return Task.CompletedTask;
        }

        private static Task HandleNonCombatApply(
            RitsuLibSidecarSyncMessageContext<ModRightClickSyncPayload> context)
        {
            return ExecutePayload(context.Message, null, null);
        }

        private static async Task ExecutePayload(
            ModRightClickSyncPayload payload,
            GameActionPlayerChoiceContext? playerChoiceContext,
            GenericHookGameAction? action)
        {
            if (!TryGetPlayer(payload.OwnerNetId, out var player))
                return;
            if (!TryResolveModel(player, payload, out var model))
                return;
            if (model is not IModRightClickableModel rightClickable)
                return;

            await rightClickable.OnRightClick(new(
                player,
                model,
                payload.Trigger,
                playerChoiceContext,
                action));
            model.InvokeExecutionFinished();
        }

        private static bool TryGetPlayer(ulong ownerNetId, out Player player)
        {
            player = RunManager.Instance.DebugOnlyGetState()
                ?.Players
                .FirstOrDefault(p => p.NetId == ownerNetId)!;
            return player != null;
        }

        private static bool TryResolveModel(
            Player player,
            ModRightClickSyncPayload payload,
            out AbstractModel model)
        {
            model = null!;
            if (!ModModelIdentityRegistry.TryResolve(payload.Token, out var resolved))
                return false;

            switch (payload.Kind)
            {
                case ModRightClickModelKind.Card:
                    if (resolved is not CardModel card || card.Owner != player ||
                        card.Pile?.Type != PileType.Hand)
                        return false;

                    model = card;
                    return true;

                case ModRightClickModelKind.Relic:
                    if (resolved is not RelicModel relic || relic.Owner != player)
                        return false;

                    model = relic;
                    return true;

                case ModRightClickModelKind.Power:
                    if (resolved is not PowerModel power || !IsPowerReachableForPlayer(power, player))
                        return false;

                    model = power;
                    return true;

                case ModRightClickModelKind.Potion:
                    if (resolved is not PotionModel potion || potion.Owner != player)
                        return false;

                    model = potion;
                    return true;

                default:
                    return false;
            }
        }

        private sealed class InterfaceModelRightClickHandler : IModRightClickHandler
        {
            public bool TryHandle(ModRightClickContext context)
            {
                if (context.Model is not IModRightClickableModel rightClickable)
                    return false;
                return rightClickable.CanHandleRightClickLocal(context) && TryRequestSyncedModelAction(context);
            }
        }

        private readonly record struct ModRightClickSyncPayload(
            ulong OwnerNetId,
            ModRightClickModelKind Kind,
            ModModelIdentityToken Token,
            ModRightClickTrigger Trigger);
    }
}
