namespace Socialize.Application.Common.Pagination;

public static class PageSize
{
    public const int Default = 20;
    public const int Max = 50;

    public static int Clamp(int? requested) =>
        requested is null or <= 0 ? Default : Math.Min(requested.Value, Max);
}
