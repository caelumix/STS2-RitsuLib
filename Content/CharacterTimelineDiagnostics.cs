using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Characters;

namespace STS2RitsuLib.Content
{
    internal static class CharacterTimelineDiagnostics
    {
        private static readonly Lock Gate = new();
        private static readonly HashSet<Type> LoggedRuntimeTypes = [];

        internal static IEnumerable<CharacterModel> LogTimelinePolicy(IEnumerable<CharacterModel> characters)
        {
            foreach (var character in characters)
            {
                TryLogTimelinePolicy(character);
                yield return character;
            }
        }

        private static void TryLogTimelinePolicy(CharacterModel character)
        {
            var characterType = character.GetType();
            if (!ModContentRegistry.TryGetOwnerModId(characterType, out _))
                return;

            lock (Gate)
            {
                if (!LoggedRuntimeTypes.Add(characterType))
                    return;
            }

            var value = character is IModCharacterEpochTimelineRequirement requirement
                ? requirement.RequiresEpochAndTimeline.ToString()
                : "<not implemented>";

            RitsuLibFramework.Logger.Info(
                $"[Content] Character timeline: {character.Id} " +
                $"{nameof(IModCharacterEpochTimelineRequirement.RequiresEpochAndTimeline)}={value}");
        }
    }
}
