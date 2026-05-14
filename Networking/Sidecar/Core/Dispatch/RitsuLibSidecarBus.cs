namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Opcode dispatch for sidecar payloads. Registration uses the same 64-bit opcodes as
    ///     Opcode dispatch 用于 sidecar payload. Registration 使用 the same 64-bit opcodes as
    ///     <see cref="RitsuLibSidecarOpcodes.For" />.
    /// </summary>
    public static class RitsuLibSidecarBus
    {
        private static readonly Lock Gate = new();

        private static readonly Dictionary<ulong, Action<RitsuLibSidecarDispatchContext>> Handlers = [];
        private static readonly List<PendingWaiter> Waiters = [];

        /// <summary>
        ///     Registers or replaces a handler for an opcode. Unregister when leaving multiplayer to avoid leaks.
        ///     注册 or replaces a handler for an opcode. Unregister when leaving multiplayer to avoid leaks。
        /// </summary>
        public static void RegisterHandler(ulong opcode, Action<RitsuLibSidecarDispatchContext> handler)
        {
            ArgumentNullException.ThrowIfNull(handler);
            lock (Gate)
            {
                Handlers[opcode] = handler;
            }
        }

        /// <summary>
        ///     Removes a handler for the opcode, if present.
        ///     Removes a handler 用于 the opcode, 如果 present.
        /// </summary>
        public static void UnregisterHandler(ulong opcode)
        {
            lock (Gate)
            {
                Handlers.Remove(opcode);
            }
        }

        /// <summary>
        ///     Removes all opcode handlers (e.g. when leaving multiplayer).
        ///     Removes all opcode handlers (e.g. 当 leaving multiplayer).
        /// </summary>
        public static void ClearHandlers()
        {
            lock (Gate)
            {
                Handlers.Clear();
            }
        }

        /// <summary>
        ///     Number of active <see cref="WaitForNextAsync" /> waiters (snapshot under lock).
        ///     中文说明：Number of active <c>WaitForNextAsync</c> waiters (snapshot under lock).
        ///     Number of active <c>WaitForNextAsync</c> waiters (snapshot under lock).
        ///     中文说明：Number of active <c>WaitForNextAsync</c> waiters (snapshot under lock).
        /// </summary>
        public static int GetPendingWaiterCount()
        {
            lock (Gate)
            {
                return Waiters.Count;
            }
        }

        /// <summary>
        ///     Removes every pending <see cref="WaitForNextAsync" /> waiter and completes each task as canceled. Does
        ///     Removes every pending <c>WaitForNextAsync</c> waiter 和 completes each task as canceled. Does
        ///     not remove opcode handlers (including built-in control handlers).
        ///     中文说明：not remove opcode handlers (including built-in control handlers).
        /// </summary>
        public static void CancelAllPendingWaits()
        {
            List<PendingWaiter> pending;
            lock (Gate)
            {
                pending = [..Waiters];
                Waiters.Clear();
            }

            foreach (var w in pending)
                w.Tcs.TrySetCanceled();
        }

        /// <summary>
        ///     Waits once for a matching opcode packet, useful for request/reply control flows.
        ///     Waits once 用于 a matching opcode packet, 使用ful 用于 request/reply control flows.
        /// </summary>
        /// <remarks>
        ///     Timeout uses <see cref="CancellationToken.None" /> on <see cref="Task.Delay(TimeSpan, CancellationToken)" />;
        ///     Timeout 使用 <c>CancellationToken.None</c> on <c>Task.Delay(TimeSpan, CancellationToken)</c>;
        ///     user cancellation is observed through <paramref name="cancellationToken" /> separately so both paths
        ///     使用r cancellation is observed through <c>cancellationToken</c> separately so both 路径
        ///     can complete the waiter without linking tokens. The completed task’s continuations are not marshaled to
        ///     can complete the waiter 带有out linking tokens. The completed task’s continuations are not marshaled to
        ///     the Godot main loop; use
        ///     the Godot main loop; 使用
        ///     <see cref="RitsuLibSidecarGodotMainLoopScheduling.ContinueOnGodotMainLoopAsync{T}(Task{T})" /> when needed.
        /// </remarks>
        public static Task<RitsuLibSidecarDispatchContext> WaitForNextAsync(
            ulong opcode,
            TimeSpan timeout,
            Func<RitsuLibSidecarDispatchContext, bool>? predicate = null,
            bool consumeOnMatch = true,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<RitsuLibSidecarDispatchContext>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var waiter = new PendingWaiter
            {
                Opcode = opcode,
                ConsumeOnMatch = consumeOnMatch,
                Predicate = predicate,
                Tcs = tcs,
            };

            lock (Gate)
            {
                Waiters.Add(waiter);
            }

            if (timeout > TimeSpan.Zero)
                _ = Task.Delay(timeout, CancellationToken.None).ContinueWith(
                    _ => TryTimeoutWaiter(waiter),
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);

            if (cancellationToken.CanBeCanceled)
                cancellationToken.Register(() => TryCancelWaiter(waiter, cancellationToken));

            return tcs.Task;
        }

        /// <summary>
        ///     When a waiter was registered but the matching send failed, removes it and completes the task with
        ///     当 a waiter was 已注册 but the matching send failed, removes it 和 completes the task 带有
        ///     <paramref name="exception" /> so the waiter list does not leak.
        /// </summary>
        internal static bool TryFailWaitIfStillPending(Task<RitsuLibSidecarDispatchContext> waitTask,
            Exception exception)
        {
            PendingWaiter? found = null;
            lock (Gate)
            {
                for (var i = 0; i < Waiters.Count; i++)
                {
                    var w = Waiters[i];
                    if (!ReferenceEquals(w.Tcs.Task, waitTask))
                        continue;

                    Waiters.RemoveAt(i);
                    found = w;
                    break;
                }
            }

            return found?.Tcs.TrySetException(exception) ?? false;
        }

        private static void TryTimeoutWaiter(PendingWaiter waiter)
        {
            bool removed;
            lock (Gate)
            {
                removed = Waiters.Remove(waiter);
            }

            if (!removed)
                return;

            waiter.Tcs.TrySetException(new TimeoutException("Sidecar wait timed out"));
        }

        private static void TryCancelWaiter(PendingWaiter waiter, CancellationToken cancellationToken)
        {
            bool removed;
            lock (Gate)
            {
                removed = Waiters.Remove(waiter);
            }

            if (!removed)
                return;

            waiter.Tcs.TrySetCanceled(cancellationToken);
        }

        internal static void Dispatch(in RitsuLibSidecarDispatchContext context)
        {
            Action<RitsuLibSidecarDispatchContext>? handler;
            PendingWaiter? matchedWaiter = null;
            var consumeByWaiter = false;
            lock (Gate)
            {
                Handlers.TryGetValue(context.Opcode, out handler);
                for (var i = 0; i < Waiters.Count; i++)
                {
                    var w = Waiters[i];
                    if (w.Opcode != context.Opcode)
                        continue;

                    if (w.Predicate != null && !w.Predicate(context))
                        continue;

                    Waiters.RemoveAt(i);
                    matchedWaiter = w;
                    consumeByWaiter = w.ConsumeOnMatch;
                    break;
                }
            }

            matchedWaiter?.Tcs.TrySetResult(context);
            if (consumeByWaiter)
                return;

            handler?.Invoke(context);
        }

        private sealed class PendingWaiter
        {
            public required ulong Opcode { get; init; }
            public required bool ConsumeOnMatch { get; init; }
            public required Func<RitsuLibSidecarDispatchContext, bool>? Predicate { get; init; }
            public required TaskCompletionSource<RitsuLibSidecarDispatchContext> Tcs { get; init; }
        }
    }
}
