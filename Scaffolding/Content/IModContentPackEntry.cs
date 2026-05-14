namespace STS2RitsuLib.Scaffolding.Content
{
    /// <summary>
    ///     Declarative pack step (timeline, unlocks, or other <see cref="ModContentPackContext" /> surfaces), like
    ///     <see cref="IContentRegistrationEntry" /> but for the full pack context.
    ///     声明式内容包步骤（时间线、解锁或其他 <c>ModContentPackContext</c> 表面），类似
    ///     <c>IContentRegistrationEntry</c>，但作用于完整内容包上下文。
    /// </summary>
    public interface IModContentPackEntry
    {
        /// <summary>
        ///     Runs this step during <see cref="ModContentPackBuilder.Apply" />.
        ///     在 <c>ModContentPackBuilder.Apply</c> 期间运行此步骤。
        /// </summary>
        void Apply(ModContentPackContext context);
    }
}
