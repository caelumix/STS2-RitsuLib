using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;

namespace STS2RitsuLib.Utils
{
    /// <summary>
    /// 对<see cref="ModCardVars"/>的便捷封装
    /// 用于创建便捷的计算变量
    /// </summary>
    public static class ComputedDynamicVarHelper
    {
        /// <summary>
        /// 创建基础计算变量（含目标参数版本）。
        /// 不经过任何 Hook 处理，直接返回委托的计算结果。
        /// </summary>
        public static ComputedDynamicVar CreateBaseVar(string name, decimal baseValue,
            Func<CardModel?, Creature?, decimal> calc)
        {
            return ModCardVars.Computed(name, baseValue,
                calc,
                (card, _, target, _) => calc(card, target));
        }

        /// <summary>
        /// 创建基础计算变量。
        /// 不经过任何 Hook 处理，直接返回委托的计算结果。
        /// </summary>
        public static ComputedDynamicVar CreateBaseVar(string name, decimal baseValue,
            Func<CardModel?, decimal> calc)
        {
            return ModCardVars.Computed(name, baseValue,
                (card, _) => calc(card),
                (card, _, _, _) => calc(card));
        }

        /// <summary>
        /// 创建伤害计算变量，委托的计算结果会自动经过 <see cref="Hook.ModifyDamage"/> 处理，
        /// </summary>
        public static ComputedDynamicVar CreateDamageVar(string name, decimal baseValue,
            Func<CardModel?, Creature?, decimal> calc, ValueProp prop = ValueProp.Move)
        {
            return ModCardVars.Computed(name, baseValue,
                calc,
                (card, mode, target, runHooks) => ApplyHooks(card, mode, target, runHooks, prop, true, calc)
            );
        }

        /// <summary>
        /// 创建格挡计算变量，委托的计算结果会自动经过 <see cref="Hook.ModifyBlock"/> 处理，
        /// </summary>
        public static ComputedDynamicVar CreateBlockVar(string name, decimal baseValue,
            Func<CardModel?, Creature?, decimal> calc, ValueProp prop = ValueProp.Move)
        {
            return ModCardVars.Computed(name, baseValue,
                calc,
                (card, mode, target, runHooks) => ApplyHooks(card, mode, target, runHooks, prop, false, calc)
            );
        }

        private static decimal ApplyHooks(CardModel? card, CardPreviewMode mode, Creature? target,
            bool runHooks,
            ValueProp prop, bool isDamage, Func<CardModel?, Creature?, decimal> calc)
        {
            var baseValue = calc(card, target);

            if (card is { RunState: not null, CombatState: not null } && runHooks)
            {
                return isDamage
                    ? Hook.ModifyDamage(card.RunState, card.CombatState, target, card.Owner.Creature,
                        baseValue, prop, card, ModifyDamageHookType.All, mode, out _)
                    : Hook.ModifyBlock(card.CombatState, card.Owner.Creature, baseValue, prop, card, null, out _);
            }

            return baseValue;
        }
    }
}
