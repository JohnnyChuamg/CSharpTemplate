using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Text;

using Infrastructure.Mvc.Models;

namespace Infrastructure.Mvc.Extensions;

public static class HttpRequestExtension
{
    private static readonly FormOptions DefaultFormOptions = new ();

    public static async Task<(FormValueProvider FormData, List<FileContent> Files)> GetFormDataAsync(
        this HttpRequest request, string tempPath = "")
    {
        if (!request.ContentType.IsMultipartContentType())
        {
            throw new Exception($"Expected a multipart request, but got {request.ContentType}");
        }

        var formAccumulator = default(KeyValueAccumulator);

        var boundary = MediaTypeHeaderValue.Parse(request.ContentType)
            .GetBoundary(DefaultFormOptions.MultipartBoundaryLengthLimit);

        var reader = new MultipartReader(boundary, request.Body);

        var multipartSection = await reader.ReadNextSectionAsync();

        var files = new List<FileContent>();

        _ = string.Empty;

        var tempFolder = Guid.NewGuid().ToString("n");

        while (multipartSection != null)
        {
            if (ContentDispositionHeaderValue.TryParse(multipartSection.ContentDisposition, out var parsedValue))
            {
                var key = HeaderUtilities.RemoveQuotes(parsedValue.Name).Value;
                if (parsedValue.HasFileContentDisposition())
                {
                    if (string.IsNullOrWhiteSpace(tempPath))
                    {
                        continue;
                    }

                    var fileContent = new FileContent
                    {
                        Name = key,
                        FileName = (parsedValue.FileNameStar.Value ?? parsedValue.FileName.Value) ?? string.Empty
                    };
                    fileContent.FileName = string.Join("",
                        fileContent.FileName.Split(Path.GetInvalidFileNameChars())).Trim(['"']);
                    var path = $"{Guid.NewGuid():n}{Path.GetExtension(fileContent.FileName)}";
                    fileContent.File = Path.Combine(tempPath, tempFolder, path);
                    if (!Directory.Exists(Path.Combine(tempPath, tempFolder)))
                    {
                        Directory.CreateDirectory(Path.Combine(tempPath, tempFolder));
                    }

                    await using var fs = new FileStream(fileContent.File, FileMode.Create, FileAccess.Write);
                    await multipartSection.Body.CopyToAsync(fs);
                    files.Add(fileContent);
                }
                else if (parsedValue.HasFormDataContentDisposition())
                {
                    var encoding = GetEncoding(multipartSection);
                    using var streamReader = new StreamReader(multipartSection.Body, encoding,
                        detectEncodingFromByteOrderMarks: true, 1024, leaveOpen: true);
                    var text = await streamReader.ReadToEndAsync();
                    if (string.Equals(text, "undefined", StringComparison.OrdinalIgnoreCase))
                    {
                        text = string.Empty;
                    }
                    formAccumulator.Append(key,text);
                    if (formAccumulator.ValueCount > DefaultFormOptions.ValueCountLimit)
                    {
                        throw new InvalidDataException(
                            $"Form key count limit {formAccumulator.ValueCount}/{DefaultFormOptions.ValueCountLimit} exceeded.");
                    }
                }
            }
            multipartSection = await reader.ReadNextSectionAsync();
        }

        return (
            new FormValueProvider(BindingSource.Form, new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture), files);
    }

    private static Encoding GetEncoding(MultipartSection section)
    {
        if (!MediaTypeHeaderValue.TryParse(section.ContentType, out var parsedValue) ||
            Encoding.UTF8.Equals(parsedValue.Encoding))
        {
            return Encoding.UTF8;
        }

        return parsedValue.Encoding;
    }
}