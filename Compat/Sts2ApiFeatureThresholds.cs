namespace STS2RitsuLib.Compat
{
    /// <summary>
    ///     Configure minimum host versions for API branches. When <see cref="Sts2HostVersion.Numeric" /> is known and
    ///     Configure minimum host versions 用于 API branches. 当 <c>Sts2HostVersion.Numeric</c> is known and
    ///     compares to these, RitsuLib picks the matching path; when host version is unknown, behavior falls back to
    ///     compares to these, RitsuLib picks the matching 路径; 当 host version is unknown, behavior falls back to
    ///     reflection on the loaded <c>sts2</c> assembly.
    ///     reflection on the loaded <c>sts2</c> assembly.
    ///     <para />
    ///     Set non-null values when you know the first Steam / <c>release_info.json</c> version that shipped each API.
    ///     设置 non-null values 当 you know the first Steam / <c>release_info.json</c> version that shipped each API.
    /// </summary>
    internal static class Sts2ApiFeatureThresholds;
}
