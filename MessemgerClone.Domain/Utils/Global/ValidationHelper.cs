using MessengerClone.Domain.Utils.Enums;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace MessengerClone.Service.Features.General.Helpers
{
    public static class ValidationHelper
    {
        public static readonly Regex PhoneRegex = new(@"^\+?[1-9]\d{1,14}$"); // E.164

        public static readonly int MaxNameLength = 100;
        public static readonly long MaxImageSize = 5 * 1024 * 1024; // 5MB
        public static readonly long MaxVideoSize = 50 * 1024 * 1024; // 50MB
        public static readonly long MaxAudioSize = 10 * 1024 * 1024; // 10MB
        public static readonly string[] ImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".heic", ".heif" };
        public static readonly string[] VideoExtensions = new[] { ".mp4", ".mov", ".webm", ".mkv", ".avi" };
        public static readonly string[] AudioExtensions = new[] { ".mp3", ".wav", ".ogg", ".aac" };

        public static readonly int MaxTitleLength = 100;
        public static readonly int MaxDescriptionLength = 500;
        public static bool HasAllowedExtension(IFormFile file, enMediaType type)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();

            return type switch
            {
                enMediaType.Image => ImageExtensions.Contains(extension),
                enMediaType.Video => VideoExtensions.Contains(extension),
                enMediaType.Audio => AudioExtensions.Contains(extension),
                _ => false
            };
        }

        public static bool IsWithinAllowedSize(IFormFile file, enMediaType type)
        {
            if (file == null)
                return false;

            long maxSize = type switch
            {
                enMediaType.Image => MaxImageSize, 
                enMediaType.Video => MaxVideoSize,
                enMediaType.Audio => MaxAudioSize,
                _ => 0
            };

            return file.Length <= maxSize;
        }

    }
}
