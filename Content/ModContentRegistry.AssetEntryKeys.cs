namespace STS2RitsuLib.Content
{
    public sealed partial class ModContentRegistry
    {
        /// <summary>
        ///     Canonical character entry key for replacement / programmatic maps: trimmed,
        ///     Canonical character entry key 用于 replacement / programmatic maps: trimmed,
        ///     <see cref="string.ToUpperInvariant" />. Lookup APIs also probe a legacy lowercase bucket from older
        ///     registrations.
        ///     注册s.
        /// </summary>
        public static string NormalizeCharacterAssetEntryKey(string characterEntry)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(characterEntry);
            return characterEntry.Trim().ToUpperInvariant();
        }

        /// <summary>
        ///     Canonical model <c>Id.Entry</c> segment for programmatic owned-visual rows. Matching against live models
        ///     Canonical 模型 <c>Id.Entry</c> segment 用于 programmatic owned-visual rows. Matching against live Models
        ///     remains ordinal-ignore-case.
        ///     中文说明：remains ordinal-ignore-case.
        /// </summary>
        public static string NormalizeOwnedModelIdEntry(string modelIdEntry)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modelIdEntry);
            return modelIdEntry.Trim().ToUpperInvariant();
        }
    }
}
