using FluentValidation.Results;
using Socialize.Application.Common.Exceptions;

namespace Socialize.Application.Common.Media;

/// <summary>
/// Enforces the upload limits from the spec clarification: at most 4 images per post, ~5MB each,
/// JPEG/PNG/WebP only. Validates both the declared extension and the file's magic-number header
/// so a renamed/spoofed file is rejected (research R7, SC-009).
/// </summary>
public static class ImageValidator
{
    public const int MaxImageBytes = 5 * 1024 * 1024;
    public const int MaxImagesPerPost = 4;

    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

    private static readonly Dictionary<string, byte[]> MagicNumbers = new()
    {
        [".jpg"] = new byte[] { 0xFF, 0xD8, 0xFF },
        [".jpeg"] = new byte[] { 0xFF, 0xD8, 0xFF },
        [".png"] = new byte[] { 0x89, 0x50, 0x4E, 0x47 },
        [".webp"] = new byte[] { 0x52, 0x49, 0x46, 0x46 } // "RIFF"; WEBP marker follows at offset 8
    };

    public static void ValidateCount(int count)
    {
        if (count > MaxImagesPerPost)
        {
            throw new ValidationAppException(new[]
            {
                new ValidationFailure("images", $"A post may have at most {MaxImagesPerPost} images.")
            });
        }
    }

    public static void ValidateSingle(string fileName, long sizeBytes, ReadOnlySpan<byte> headerBytes)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(ext))
        {
            throw new ValidationAppException(new[]
            {
                new ValidationFailure("file", $"Unsupported image type '{ext}'. Allowed: jpg, jpeg, png, webp.")
            });
        }

        if (sizeBytes > MaxImageBytes)
        {
            throw new ValidationAppException(new[]
            {
                new ValidationFailure("file", $"Image exceeds the {MaxImageBytes / 1024 / 1024}MB limit.")
            });
        }

        var signature = MagicNumbers[ext];
        if (headerBytes.Length < signature.Length || !headerBytes[..signature.Length].SequenceEqual(signature))
        {
            throw new ValidationAppException(new[]
            {
                new ValidationFailure("file", "File content does not match its declared image type.")
            });
        }
    }
}
