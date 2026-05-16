using STS2RitsuLib.Utils.Persistence.Migration;

namespace STS2RitsuLib.RunData
{
    /// <summary>
    ///     Options for a run-saved data slot.
    ///     跑局保存数据槽位的选项。
    /// </summary>
    public sealed class RunSavedDataOptions
    {
        /// <summary>
        ///     Current schema version written for this slot.
        ///     此槽位写入的当前 schema 版本。
        /// </summary>
        public int SchemaVersion { get; init; } = 1;

        /// <summary>
        ///     Determines when the slot is written.
        ///     决定何时写入此槽位。
        /// </summary>
        public RunSavedDataWritePolicy WritePolicy { get; init; } = RunSavedDataWritePolicy.WhenSet;

        /// <summary>
        ///     Optional slot migrations.
        ///     可选的槽位迁移。
        /// </summary>
        public IReadOnlyList<IMigration>? Migrations { get; init; }
    }
}
