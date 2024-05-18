using Microsoft.Net.Http.Headers;

namespace Infrastructure.Mvc.Extensions;

public static class HttpHeaderExtension
{
    public static string? GetBoundary(this MediaTypeHeaderValue contentType, int lengthLimit)
    {
        var value = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (value.Length > lengthLimit)
        {
            throw new Exception($"Multipart boundary length limit {lengthLimit} exceeded.");
        }

        return value;
    }

    public static bool IsMultipartContentType(this string contentType)
    {
        return !string.IsNullOrWhiteSpace(contentType) && contentType.Contains("multipart/", StringComparison.OrdinalIgnoreCase);
    }

    public static bool HasFormDataContentDisposition(this ContentDispositionHeaderValue? contentDisposition)
    {
        if (contentDisposition != null
            && contentDisposition.DispositionType.Equals("form-data")
            && string.IsNullOrWhiteSpace(contentDisposition.FileName.Value))
        {
            return string.IsNullOrWhiteSpace(contentDisposition.FileNameStar.Value);
        }

        return false;
    }

    public static bool HasFileContentDisposition(
        this ContentDispositionHeaderValue? contentDisposition)
    {
        if (contentDisposition == null || !contentDisposition.DispositionType.Equals("form-data")) return false;
        if (string.IsNullOrWhiteSpace(contentDisposition.FileName.Value))
        {
            return !string.IsNullOrWhiteSpace(contentDisposition.FileNameStar.Value);
        }

        return true;
    }
}