namespace Socialize.Application.Common.Exceptions;

/// <summary>Named distinctly from System.UnauthorizedAccessException so intent (401 vs 403) stays explicit.</summary>
public class UnauthorizedAppException : Exception
{
    public UnauthorizedAppException(string message = "Invalid credentials.") : base(message) { }
}
