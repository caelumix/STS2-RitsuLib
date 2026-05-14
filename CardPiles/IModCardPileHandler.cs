namespace STS2RitsuLib.CardPiles
{
    /// <summary>
    ///     Optional handler contract implemented by classes that declare themselves with
    ///     <see cref="STS2RitsuLib.Interop.AutoRegistration.RegisterOwnedCardPileAttribute" />. When the
    ///     attribute sees a type implementing this interface, the auto-registration pipeline instantiates
    ///     the type once (requires a parameterless constructor) and wires its
    ///     <see cref="OnOpen" /> method into <see cref="ModCardPileSpec.OnOpen" />.
    ///     由声明 <c>STS2RitsuLib.Interop.AutoRegistration.RegisterOwnedCardPileAttribute</c> 的类可选实现的
    ///     handler contract。当 attribute 看到某个类型实现此接口时，auto-registration 管线会实例化该类型一次
    ///     （需要无参构造函数），并把它的 <c>OnOpen</c> 方法接入 <c>ModCardPileSpec.OnOpen</c>。
    /// </summary>
    /// <remarks>
    ///     The interface is entirely optional — annotated types may leave the button to open the default
    ///     <c>NCardPileScreen</c>. Handler instances are cached per registered pile, so the same instance
    ///     services every click for that pile's lifetime.
    ///     此接口完全可选；带注解类型可以让按钮打开默认 <c>NCardPileScreen</c>。handler 实例按已注册 pile 缓存，
    ///     因此同一实例会服务该 pile 生命周期内的每次点击。
    /// </remarks>
    public interface IModCardPileHandler
    {
        /// <summary>
        ///     Invoked when the pile's UI button is released. See <see cref="ModCardPileSpec.OnOpen" /> for
        ///     the full contract (empty-pile short-circuit, open-default-screen toggle, etc.).
        ///     当 pile 的 UI 按钮释放时调用。完整契约（空 pile 短路、默认画面开关等）参见
        ///     <see cref="ModCardPileSpec.OnOpen" />。
        /// </summary>
        void OnOpen(ModCardPileOpenContext context);
    }
}
