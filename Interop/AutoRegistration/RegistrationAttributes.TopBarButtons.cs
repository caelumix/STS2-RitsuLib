using STS2RitsuLib.TopBar;

namespace STS2RitsuLib.Interop.AutoRegistration
{
    /// <summary>
    ///     Declaratively registers a mod-owned top-bar button (see
    ///     <see cref="ModTopBarButtonRegistry" />). Mirrors the ergonomics of
    ///     <see cref="RegisterOwnedCardPileAttribute" /> but targets the generic, pile-independent
    ///     top-bar button system.
    ///     声明式注册 mod-owned top-bar button（参见 <c>ModTopBarButtonRegistry</c>）。它对应
    ///     <c>RegisterOwnedCardPileAttribute</c> 的易用性，但目标是通用、与 pile 无关的 top-bar
    ///     button 系统。
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Place on any concrete class inside your mod assembly. The annotated class must implement
    ///         <see cref="IModTopBarButtonHandler" /> — ritsulib instantiates it once (parameterless
    ///         constructor required) and wires
    ///         <see cref="IModTopBarButtonHandler.OnClick" /> into <see cref="ModTopBarButtonSpec.OnClick" />
    ///         and <see cref="IModTopBarButtonHandler.IsVisible" /> into
    ///         <see cref="ModTopBarButtonSpec.VisibleWhen" />.
    ///         将其放在你的 mod assembly 中任意具体类上。带注解的类必须实现
    ///         <c>IModTopBarButtonHandler</c>；ritsulib 会实例化一次（需要无参构造函数），并将
    ///         <c>IModTopBarButtonHandler.OnClick</c> 接入 <c>ModTopBarButtonSpec.OnClick</c>，
    ///         将 <c>IModTopBarButtonHandler.IsVisible</c> 接入
    ///         <see cref="ModTopBarButtonSpec.VisibleWhen" />。
    ///     </para>
    ///     <para>
    ///         Localization follows the vanilla loc-table convention: title / description are resolved
    ///         against <c>static_hover_tips</c> using <c>"{id}.title"</c> and <c>"{id}.description"</c> where
    ///         <c>id</c> is the qualified button id.
    ///         本地化遵循原版 loc-table 约定：title / description 会基于 <c>static_hover_tips</c> 解析，
    ///         使用 <c>"{id}.title"</c> 和 <c>"{id}.description"</c>，其中 <c>id</c> 是 qualified button id。
    ///     </para>
    /// </remarks>
    /// <param name="localButtonStem">
    ///     Local, mod-scoped stem (matches <c>RegisterOwned(localStem, ...)</c>).
    ///     mod 局部范围内的 stem（匹配 <c>RegisterOwned(localStem, ...)</c>）。
    /// </param>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RegisterOwnedTopBarButtonAttribute(string localButtonStem) : AutoRegistrationAttribute
    {
        /// <summary>
        ///     Local, mod-scoped button stem.
        ///     mod 局部范围内的 button stem。
        /// </summary>
        public string LocalButtonStem { get; } = localButtonStem;

        /// <summary>
        ///     Godot resource path for the icon (e.g. <c>res://my_mod/icons/recipes.png</c>).
        ///     图标的 Godot ResourcePath（例如 <c>res://my_mod/icons/recipes.png</c>）。
        /// </summary>
        public string? IconPath { get; set; }

        /// <summary>
        ///     Sort order within this mod's top-bar buttons.
        ///     在此 mod 的 top-bar button 中的排序。
        /// </summary>
        public int ButtonOrder { get; set; }

        /// <summary>
        ///     Extra pixel offset X on top of the auto-stacked slot.
        ///     在 auto-stacked slot 基础上额外增加的 X 像素偏移。
        /// </summary>
        public float OffsetX { get; set; }

        /// <summary>
        ///     Extra pixel offset Y on top of the auto-stacked slot.
        ///     在 auto-stacked slot 基础上额外增加的 Y 像素偏移。
        /// </summary>
        public float OffsetY { get; set; }
    }
}
