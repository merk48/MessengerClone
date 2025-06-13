using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.Service.Features.Files.Helpers
{
    // You may need an extension method or switch for this:
    public static class FileCategoryExtensions
    {
        public static enMediaType MediaType(this enFileCategory category) => category switch
        {
            enFileCategory.Avatar => enMediaType.Image,
            enFileCategory.MessageImage => enMediaType.Image,
            enFileCategory.MessageVideo => enMediaType.Video,
            enFileCategory.MessageAudio => enMediaType.Audio,
            enFileCategory.ChatBackground => enMediaType.Image,
            enFileCategory.Emoji => enMediaType.Image,
            enFileCategory.GroupCover => enMediaType.Image,
            enFileCategory.StatusImage => enMediaType.Image,
            enFileCategory.StatusVideo => enMediaType.Video,
            enFileCategory.Banner => enMediaType.Image,
            enFileCategory.System => enMediaType.Image,
            enFileCategory.Temporary => enMediaType.Image, // or make this dynamic
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };
    }
}
