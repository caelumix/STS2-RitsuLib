using Godot;

namespace STS2RitsuLib.Utils
{
    /// <summary>
    ///     Factory helpers for Godot materials that mirror vanilla game shaders.
    ///     Factory helpers 用于 Godot 材质s that mirror 原版 game shaders.
    /// </summary>
    public static class MaterialUtils
    {
        private const string HsvShaderPath = "res://shaders/hsv.gdshader";
        private const string DoomBarShaderPath = "res://scenes/combat/doom_bar.gdshader";

        private static NoiseTexture2D? _vanillaDoomBarNoiseTexture;
        private static ShaderMaterial? _unmodulatedHsvMaterial;

        private static Shader? GameHsvShader => (Shader?)GD.Load<Shader>(HsvShaderPath)?.Duplicate();

        private static Shader? GameDoomBarShader => (Shader?)GD.Load<Shader>(DoomBarShaderPath)?.Duplicate();

        private static NoiseTexture2D VanillaDoomBarNoiseTexture =>
            _vanillaDoomBarNoiseTexture ??= CreateVanillaDoomBarNoiseTexture();

        /// <summary>
        ///     Builds a <c>ShaderMaterial</c> using the game's HSV shader with the given RGB parameters.
        ///     Builds a <c>Shader材质</c> using the game's HSV shader 带有 the given RGB parameters.
        /// </summary>
        public static ShaderMaterial CreateRgbShaderMaterial(float r, float g, float b)
        {
            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));
            var delta = max - min;

            float h = 0;
            if (delta != 0)
            {
                if (Mathf.IsEqualApprox(max, r)) h = (g - b) / delta + (g < b ? 6 : 0);
                else if (Mathf.IsEqualApprox(max, g)) h = (b - r) / delta + 2;
                else h = (r - g) / delta + 4;
                h /= 6;
            }

            var s = max == 0 ? 0 : delta / max;
            return CreateHsvShaderMaterial(h, s, max);
        }

        /// <summary>
        ///     Builds a <c>ShaderMaterial</c> using the game's HSV shader with the given parameters.
        ///     Builds a <c>Shader材质</c> using the game's HSV shader 带有 the given parameters.
        /// </summary>
        public static ShaderMaterial CreateHsvShaderMaterial(float h, float s, float v)
        {
            var shader = GameHsvShader ??
                         throw new InvalidOperationException($"Failed to load HSV shader ({HsvShaderPath}).");

            var material = new ShaderMaterial
            {
                Shader = shader,
            };

            material.SetShaderParameter("h", h);
            material.SetShaderParameter("s", s);
            material.SetShaderParameter("v", v);

            return material;
        }

        /// <summary>
        ///     Returns a <see cref="ShaderMaterial" /> built from the game's HSV shader configured to preserve the
        ///     返回 a <c>Shader材质</c> built 从 the game's HSV shader configured to preserve the
        ///     original colors (identity modulation: <c>h=0</c>, <c>s=1</c>, <c>v=1</c>).
        ///     中文说明：original colors (identity modulation: <c>h=0</c>, <c>s=1</c>, <c>v=1</c>).
        /// </summary>
        /// <remarks>
        ///     This is useful when you want to override a card frame's <c>FrameMaterial</c> without introducing any additional
        ///     This is 使用ful 当 you want to override a 卡牌 frame's <c>Frame材质</c> 带有out introducing any additional
        ///     color modulation, while still using the vanilla shader pipeline.
        ///     color modulation, while still using the 原版 shader pipeline.
        /// </remarks>
        public static ShaderMaterial CreateUnmodulatedHsvShaderMaterial()
        {
            _unmodulatedHsvMaterial ??= CreateHsvShaderMaterial(0f, 1f, 1f);
            return (ShaderMaterial)_unmodulatedHsvMaterial.Duplicate();
        }

        /// <summary>
        ///     Builds a <c>ShaderMaterial</c> using the game's doom health bar shader (<c>doom_bar.gdshader</c>) with the same
        ///     Builds a <c>Shader材质</c> using the game's doom health bar shader (<c>doom_bar.gdshader</c>) 带有 the same
        ///     noise settings as <c>health_bar.tscn</c> and a caller-supplied gradient.
        ///     noise 设置 as <c>health_bar.tscn</c> 和 a caller-supplied gradient.
        /// </summary>
        /// <remarks>
        ///     Typical use: <see cref="Combat.HealthBars.HealthBarForecastSegment.OverlayMaterial" /> on custom forecast
        ///     Typical 使用: <c>Combat.HealthBars.HealthBarForecastSegment.Overlay材质</c> on 自定义 用于ecast
        ///     overlays so they read like the vanilla doom strip (see also <c>CreateVanillaDoomBarGradientTexture</c>).
        ///     overlays so they read like the 原版 doom strip (see also <c>CreateVanillaDoomBarGradientTexture</c>).
        /// </remarks>
        public static ShaderMaterial CreateDoomBarShaderMaterial(GradientTexture1D gradientTexture)
        {
            ArgumentNullException.ThrowIfNull(gradientTexture);

            var shader = GameDoomBarShader;
            if (shader == null)
                throw new InvalidOperationException($"Failed to load doom bar shader ({DoomBarShaderPath}).");

            var material = new ShaderMaterial { Shader = shader };
            material.SetShaderParameter("noise_tex", VanillaDoomBarNoiseTexture);
            material.SetShaderParameter("gradient_tex", gradientTexture);
            return material;
        }

        /// <summary>
        ///     Gradient texture matching the vanilla doom bar segment in <c>health_bar.tscn</c>.
        ///     Gradient 纹理 matching the 原版 doom bar segment in <c>health_bar.tscn</c>.
        /// </summary>
        public static GradientTexture1D CreateVanillaDoomBarGradientTexture()
        {
            var gradient = new Gradient();
            gradient.AddPoint(0f, new(0.300863f, 0.162626f, 0.528347f));
            gradient.AddPoint(0.514583f, new(0.513726f, 0.254902f, 0.505882f));
            gradient.AddPoint(1f, new(0.354657f, 0.0421873f, 0.437114f));
            return new() { Gradient = gradient };
        }

        /// <summary>
        ///     Noise texture matching <c>health_bar.tscn</c> (Perlin, frequency 0.0383).
        ///     Noise 纹理 matching <c>health_bar.tscn</c> (Perlin, frequency 0.0383).
        /// </summary>
        public static NoiseTexture2D CreateVanillaDoomBarNoiseTexture()
        {
            var noise = new FastNoiseLite
            {
                NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin,
                Frequency = 0.0383f,
            };

            return new() { Noise = noise };
        }
    }
}
