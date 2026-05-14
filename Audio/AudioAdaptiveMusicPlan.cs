namespace STS2RitsuLib.Audio
{
    /// <summary>
    ///     Declares room/combat/victory music sources that should follow the game's lifecycle transitions.
    ///     中文说明：Declares room/combat/victory music sources that should follow the game's lifecycle transitions.
    /// </summary>
    public sealed class AudioAdaptiveMusicPlan
    {
        /// <summary>
        ///     Music source to use while the player is in a room outside combat.
        ///     Music source to 使用 while the player is in a room outside combat.
        /// </summary>
        public AudioSource? RoomSource { get; init; }

        /// <summary>
        ///     Music source to use while combat is active.
        ///     Music source to 使用 while combat is active.
        /// </summary>
        public AudioSource? CombatSource { get; init; }

        /// <summary>
        ///     Music source to use after combat victory, when provided.
        ///     Music source to 使用 之后 combat victory, 当 provided.
        /// </summary>
        public AudioSource? VictorySource { get; init; }

        /// <summary>
        ///     Restores vanilla run music when the adaptive handle is stopped.
        ///     Restores 原版 跑局 music 当 the adaptive handle is stopped.
        /// </summary>
        public bool RestoreVanillaMusicOnStop { get; init; } = true;

        /// <summary>
        ///     Restores vanilla run music after combat ends instead of returning to the room override.
        ///     Restores 原版 跑局 music 之后 combat ends instead of 返回ing to the room override.
        /// </summary>
        public bool RestoreVanillaMusicOnCombatEnd { get; init; } = true;

        /// <summary>
        ///     Refreshes vanilla room track and ambience when entering a room with no room override.
        ///     Refreshes 原版 room track 和 ambience 当 entering a room 带有 no room override.
        /// </summary>
        public bool RefreshVanillaRoomStateOnRoomEnter { get; init; } = true;

        /// <summary>
        ///     Playback options applied when starting room music.
        ///     Playback options applied 当 starting room music.
        /// </summary>
        public AudioPlaybackOptions RoomOptions { get; init; } = new() { Scope = AudioLifecycleScope.Room };

        /// <summary>
        ///     Playback options applied when starting combat music.
        ///     Playback options applied 当 starting combat music.
        /// </summary>
        public AudioPlaybackOptions CombatOptions { get; init; } = new() { Scope = AudioLifecycleScope.Combat };

        /// <summary>
        ///     Playback options applied when starting victory music.
        ///     Playback options applied 当 starting victory music.
        /// </summary>
        public AudioPlaybackOptions VictoryOptions { get; init; } = new() { Scope = AudioLifecycleScope.Combat };
    }
}
