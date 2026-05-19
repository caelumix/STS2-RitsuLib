using Godot;
using MegaCrit.Sts2.Core.Models;

namespace STS2RitsuLib.Scaffolding.Cards.HandOutline
{
    /// <summary>
    ///     A small wrapper for one or more hand-outline rules. Prefer this for registration APIs so several conditions can
    ///     be kept together while the registry still resolves by priority.
    ///     一个包含一条或多条手牌描边规则的小包装。注册 API 优先使用此类型，便于把多个条件放在一起，同时仍由注册表按优先级解析。
    /// </summary>
    public readonly record struct ModCardHandOutlineRules
    {
        private readonly ModCardHandOutlineSwitchRule[] _rules;

        /// <summary>
        ///     Creates a rule set from one or more rules.
        ///     从一条或多条规则创建规则集。
        /// </summary>
        public ModCardHandOutlineRules(params ModCardHandOutlineSwitchRule[] rules)
        {
            ArgumentNullException.ThrowIfNull(rules);
            _rules = rules.ToArray();
        }

        /// <summary>
        ///     Creates a rule set from one or more rules.
        ///     从一条或多条规则创建规则集。
        /// </summary>
        public static ModCardHandOutlineRules Of(params ModCardHandOutlineSwitchRule[] rules)
        {
            return new(rules);
        }

        /// <summary>
        ///     Creates a single fixed-color rule set.
        ///     创建单条固定颜色规则集。
        /// </summary>
        public static ModCardHandOutlineRules Fixed(
            Func<CardModel, bool> when,
            Color color,
            int priority = 0,
            bool visibleWhenUnplayable = false)
        {
            return new(ModCardHandOutlineSwitchRule.Fixed(when, color, priority, visibleWhenUnplayable));
        }

        /// <summary>
        ///     Creates a single typed fixed-color rule set.
        ///     创建单条类型化固定颜色规则集。
        /// </summary>
        public static ModCardHandOutlineRules Fixed<TCard>(
            Func<TCard, bool> when,
            Color color,
            int priority = 0,
            bool visibleWhenUnplayable = false)
            where TCard : CardModel
        {
            return new(ModCardHandOutlineSwitchRule.Fixed(when, color, priority, visibleWhenUnplayable));
        }

        /// <summary>
        ///     Creates a single switch-style rule set.
        ///     创建单条 switch 风格规则集。
        /// </summary>
        public static ModCardHandOutlineRules Switch(
            Func<CardModel, Color?> colorWhen,
            int priority = 0,
            bool visibleWhenUnplayable = false,
            bool refreshEveryFrame = true)
        {
            return new(ModCardHandOutlineSwitchRule.Switch(colorWhen, priority, visibleWhenUnplayable,
                refreshEveryFrame));
        }

        /// <summary>
        ///     Creates a single typed switch-style rule set.
        ///     创建单条类型化 switch 风格规则集。
        /// </summary>
        public static ModCardHandOutlineRules Switch<TCard>(
            Func<TCard, Color?> colorWhen,
            int priority = 0,
            bool visibleWhenUnplayable = false,
            bool refreshEveryFrame = true)
            where TCard : CardModel
        {
            return new(ModCardHandOutlineSwitchRule.Switch(colorWhen, priority, visibleWhenUnplayable,
                refreshEveryFrame));
        }

        /// <summary>
        ///     Creates a single dynamic-color rule set.
        ///     创建单条动态颜色规则集。
        /// </summary>
        public static ModCardHandOutlineRules Dynamic(
            Func<CardModel, bool> when,
            Func<CardModel, Color> colorWhen,
            int priority = 0,
            bool visibleWhenUnplayable = false)
        {
            return new(ModCardHandOutlineSwitchRule.Dynamic(when, colorWhen, priority, visibleWhenUnplayable));
        }

        /// <summary>
        ///     Creates a single typed dynamic-color rule set.
        ///     创建单条类型化动态颜色规则集。
        /// </summary>
        public static ModCardHandOutlineRules Dynamic<TCard>(
            Func<TCard, bool> when,
            Func<TCard, Color> colorWhen,
            int priority = 0,
            bool visibleWhenUnplayable = false)
            where TCard : CardModel
        {
            return new(ModCardHandOutlineSwitchRule.Dynamic(when, colorWhen, priority, visibleWhenUnplayable));
        }

        internal IEnumerable<ModCardHandOutlineSwitchRule> Enumerate()
        {
            return _rules ?? [];
        }
    }
}
