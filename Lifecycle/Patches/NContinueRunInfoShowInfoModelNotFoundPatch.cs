using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Exceptions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Saves;
using STS2RitsuLib.Patching.Models;

namespace STS2RitsuLib.Lifecycle.Patches
{
    /// <summary>
    ///     <see cref="NContinueRunInfo.ShowInfo" /> uses <c>ModelDb.GetById</c> for act and character; missing mod
    ///     content throws during <see cref="NMainMenu._Ready" /> / <c>RefreshButtons</c> before the player presses Continue.
    ///     Fall back to the same error UI as a bad read result.
    ///     <c>NContinueRunInfo.ShowInfo</c> 会使用 <c>ModelDb.GetById</c> 获取章节和角色；缺失 mod 内容会在玩家点击
    ///     Continue 前的 <c>NMainMenu._Ready</c> / <c>RefreshButtons</c> 期间抛错。这里回退到与读取失败相同的错误 UI。
    /// </summary>
    public class NContinueRunInfoShowInfoModelNotFoundPatch : IPatchMethod
    {
        private static readonly Action<NContinueRunInfo> ShowError =
            AccessTools.MethodDelegate<Action<NContinueRunInfo>>(
                AccessTools.DeclaredMethod(typeof(NContinueRunInfo), "ShowError"));

        /// <inheritdoc />
        public static string PatchId => "ncontinue_run_info_show_info_model_not_found";

        /// <inheritdoc />
        public static string Description =>
            "When continue-run preview hits ModelNotFoundException, show NContinueRunInfo error state instead of crashing";

        /// <inheritdoc />
        public static bool IsCritical => false;

        /// <inheritdoc />
        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NContinueRunInfo), "ShowInfo", [typeof(SerializableRun)])];
        }

        /// <summary>
        ///     Harmony finalizer: swallows <see cref="ModelNotFoundException" /> from preview and shows the panel error
        ///     state.
        ///     Harmony finalizer：吞掉预览中的 <c>ModelNotFoundException</c> 并显示面板错误状态。
        /// </summary>
        // ReSharper disable InconsistentNaming
        public static Exception? Finalizer(Exception? __exception, NContinueRunInfo __instance)
            // ReSharper restore InconsistentNaming
        {
            if (__exception is not ModelNotFoundException modelNotFoundException)
                return __exception;

            RitsuLibFramework.Logger.Warn(
                "[Saves] Continue-run preview failed (model missing from ModelDb); showing error panel. Run save not modified. " +
                modelNotFoundException.Message);
            ShowError(__instance);
            return null;
        }
    }
}
