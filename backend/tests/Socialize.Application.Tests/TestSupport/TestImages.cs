namespace Socialize.Application.Tests.TestSupport;

public static class TestImages
{
    public static MemoryStream ValidPng(int totalBytes = 128)
    {
        var bytes = new byte[totalBytes];
        var signature = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        Array.Copy(signature, bytes, signature.Length);
        return new MemoryStream(bytes);
    }

    public static MemoryStream Invalid(int totalBytes = 32)
    {
        var bytes = new byte[totalBytes];
        Array.Fill(bytes, (byte)0x00);
        return new MemoryStream(bytes);
    }
}
