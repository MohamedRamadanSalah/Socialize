using System.Globalization;
using System.Text;

namespace Socialize.Application.Common.Pagination;

/// <summary>
/// Opaque keyset (CreatedAt, Id) cursor for stable, index-friendly pagination (research R5).
/// </summary>
public static class CursorCodec
{
    public static string Encode(DateTimeOffset createdAt, Guid id) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{createdAt:O}|{id}"));

    /// <summary>
    /// Returns true when the cursor is absent (caller should return the first page) or valid
    /// (caller should continue from <paramref name="value"/>). Returns false when a cursor was
    /// supplied but is malformed — callers must then return an EMPTY page (spec Edge Cases), not an error.
    /// </summary>
    public static bool TryDecode(string? cursor, out (DateTimeOffset CreatedAt, Guid Id)? value)
    {
        if (string.IsNullOrWhiteSpace(cursor))
        {
            value = null;
            return true;
        }

        try
        {
            var raw = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            var parts = raw.Split('|');
            if (parts.Length != 2)
            {
                value = null;
                return false;
            }

            var createdAt = DateTimeOffset.Parse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var id = Guid.Parse(parts[1]);
            value = (createdAt, id);
            return true;
        }
        catch
        {
            value = null;
            return false;
        }
    }
}
