using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Unlocks;

namespace STS2RitsuLib.Content
{
    /// <summary>
    ///     Inputs for <see cref="ModContentRegistry" /> act-enter resolution (force / pool eligibility / weights). Passed when
    ///     <see cref="MegaCrit.Sts2.Core.Runs.RunManager.EnterAct" /> runs for <see cref="EnteringActIndex" />.
    ///     <c>ModContentRegistry</c> 进行进入 Act 解析时的输入（强制替换/池资格/权重）。
    ///     当 <c>MegaCrit.Sts2.Core.Runs.RunManager.EnterAct</c> 为 <c>EnteringActIndex</c> 运行时传入。
    /// </summary>
    public readonly record struct ActEnterResolveContext(
        RunManager RunManager,
        RunState RunState,
        int EnteringActIndex,
        Rng Rng,
        UnlockState UnlockState,
        bool IsMultiplayer);
}
