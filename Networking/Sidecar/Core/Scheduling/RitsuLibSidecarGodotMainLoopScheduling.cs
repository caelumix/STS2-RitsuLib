using Godot;

namespace STS2RitsuLib.Networking.Sidecar
{
    /// <summary>
    ///     Defers work to the Godot scene tree main loop via <see cref="Callable.CallDeferred" />, and optional
    ///     Defers work to the Godot 场景 tree main loop via <c>Callable.CallDeferred</c>, 和 可选
    ///     continuation helpers for sidecar <see cref="Task" /> results.
    ///     continuation helpers 用于 sidecar <c>Task</c> results.
    /// </summary>
    public static class RitsuLibSidecarGodotMainLoopScheduling
    {
        /// <summary>
        ///     Queues <paramref name="action" /> on the Godot main loop when a <see cref="SceneTree" /> is available.
        ///     Queues <c>action</c> on the Godot main loop 当 a <c>场景Tree</c> is 可用.
        /// </summary>
        /// <returns>
        ///     <c>true</c> when the delegate was queued; <c>false</c> when the main loop is not available.
        ///     <c>true</c> 当 the delegate was queued; <c>false</c> 当 the main loop is not 可用.
        /// </returns>
        public static bool TryPostToMainLoop(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);
            if (Engine.GetMainLoop() is not SceneTree)
                return false;

            Callable.From(action).CallDeferred();
            return true;
        }

        /// <summary>
        ///     After <paramref name="task" /> completes, completes the returned task on the Godot main loop when
        ///     之后 <c>task</c> completes, completes the 返回ed task on the Godot main loop 当
        ///     possible; otherwise on the current continuation context.
        ///     中文说明：possible; otherwise on the current continuation context.
        /// </summary>
        public static Task ContinueOnGodotMainLoopAsync(this Task task)
        {
            return task.ContinueWith(
                t =>
                {
                    var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

                    if (!TryPostToMainLoop(Complete))
                        Complete();

                    return tcs.Task;

                    void Complete()
                    {
                        if (t.IsFaulted)
                        {
                            var ex = t.Exception?.GetBaseException() ?? t.Exception;
                            if (ex != null)
                                tcs.TrySetException(ex);
                            else
                                tcs.TrySetException(new InvalidOperationException("Sidecar task faulted."));
                        }
                        else if (t.IsCanceled)
                        {
                            tcs.TrySetCanceled();
                        }
                        else
                        {
                            tcs.TrySetResult();
                        }
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.RunContinuationsAsynchronously,
                TaskScheduler.Default).Unwrap();
        }

        /// <inheritdoc cref="ContinueOnGodotMainLoopAsync(Task)" />
        public static Task<T> ContinueOnGodotMainLoopAsync<T>(this Task<T> task)
        {
            return task.ContinueWith(
                t =>
                {
                    var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

                    if (!TryPostToMainLoop(Complete))
                        Complete();

                    return tcs.Task;

                    void Complete()
                    {
                        if (t.IsFaulted)
                        {
                            var ex = t.Exception?.GetBaseException() ?? t.Exception;
                            if (ex != null)
                                tcs.TrySetException(ex);
                            else
                                tcs.TrySetException(new InvalidOperationException("Sidecar task faulted."));
                        }
                        else if (t.IsCanceled)
                        {
                            tcs.TrySetCanceled();
                        }
                        else
                        {
                            tcs.TrySetResult(t.Result);
                        }
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.RunContinuationsAsynchronously,
                TaskScheduler.Default).Unwrap();
        }
    }
}
