namespace HaWeb.FileHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

public static class XMLFileHelpers
{
    //  File Signatures Database (https://www.filesignatures.net/)
    private static readonly Dictionary<string, List<byte[]>> _fileSignature = new Dictionary<string, List<byte[]>>
    {
        { ".gif", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
        { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
        { ".jpeg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
            }
        },
        { ".jpg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
            }
        },
        { ".zip", new List<byte[]> 
            {
                new byte[] { 0x50, 0x4B, 0x03, 0x04 }, 
                new byte[] { 0x50, 0x4B, 0x4C, 0x49, 0x54, 0x45 },
                new byte[] { 0x50, 0x4B, 0x53, 0x70, 0x58 },
                new byte[] { 0x50, 0x4B, 0x05, 0x06 },
                new byte[] { 0x50, 0x4B, 0x07, 0x08 },
                new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 },
            }
        },
        { ".xml", new List<byte[]>
            {
                new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 0x22, 0x31, 0x2E, 0x30, 0x22, 0x3F, 0x3E },
                new byte[] { 0xEF, 0xBB, 0xBF, 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 0x22, 0x31, 0x2E, 0x30, 0x22, 0x3F, 0x3E },
                new byte[] { 0xEF, 0xBB, 0xBF, 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 0x22, 0x31, 0x2E, 0x30, 0x22, 0x20, 0x65, 0x6E, 0x63, 0x6F, 0x64, 0x69, 0x6E, 0x67, 0x3D, 0x22, 0x75, 0x74, 0x66, 0x2D, 0x38, 0x22, 0x3F, 0x3E },
                new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 0x22, 0x31, 0x2E, 0x30, 0x22, 0x20, 0x65, 0x6E, 0x63, 0x6F, 0x64, 0x69, 0x6E, 0x67, 0x3D, 0x22, 0x75, 0x74, 0x66, 0x2D, 0x38, 0x22, 0x3F, 0x3E }
            }
        }
    };

    // Unused as of rn, used to take a file and do the same sanity checks as below
    // public static async Task<byte[]> ProcessFormFile<T>(IFormFile formFile, ModelStateDictionary modelState, string[] permittedExtensions, long sizeLimit)
    // {
    //     var fieldDisplayName = string.Empty;

    //     // Use reflection to obtain the display name for the model
    //     // property associated with this IFormFile. If a display
    //     // name isn't found, error messages simply won't show
    //     // a display name.
    //     MemberInfo property =
    //         typeof(T).GetProperty(
    //             formFile.Name.Substring(formFile.Name.IndexOf(".",
    //             StringComparison.Ordinal) + 1));

    //     if (property != null)
    //     {
    //         if (property.GetCustomAttribute(typeof(DisplayAttribute)) is
    //             DisplayAttribute displayAttribute)
    //         {
    //             fieldDisplayName = $"{displayAttribute.Name} ";
    //         }
    //     }

    //     // Don't trust the file name sent by the client. To display
    //     // the file name, HTML-encode the value.
    //     var trustedFileNameForDisplay = WebUtility.HtmlEncode(
    //         formFile.FileName);

    //     // Check the file length. This check doesn't catch files that only have 
    //     // a BOM as their content.
    //     if (formFile.Length == 0)
    //     {
    //         modelState.AddModelError(formFile.Name, 
    //             $"{fieldDisplayName}({trustedFileNameForDisplay}) is empty.");

    //         return Array.Empty<byte>();
    //     }
        
    //     if (formFile.Length > sizeLimit)
    //     {
    //         var megabyteSizeLimit = sizeLimit / 1048576;
    //         modelState.AddModelError(formFile.Name,
    //             $"{fieldDisplayName}({trustedFileNameForDisplay}) exceeds " +
    //             $"{megabyteSizeLimit:N1} MB.");

    //         return Array.Empty<byte>();
    //     }

    //     try
    //     {
    //         using (var memoryStream = new MemoryStream())
    //         {
    //             await formFile.CopyToAsync(memoryStream);

    //             // Check the content length in case the file's only
    //             // content was a BOM and the content is actually
    //             // empty after removing the BOM.
    //             if (memoryStream.Length == 0)
    //             {
    //                 modelState.AddModelError(formFile.Name,
    //                     $"{fieldDisplayName}({trustedFileNameForDisplay}) is empty.");
    //             }

    //             if (!IsValidFileExtensionAndSignature(
    //                 formFile.FileName, memoryStream, permittedExtensions))
    //             {
    //                 modelState.AddModelError(formFile.Name,
    //                     $"{fieldDisplayName}({trustedFileNameForDisplay}) file " +
    //                     "type isn't permitted or the file's signature " +
    //                     "doesn't match the file's extension.");
    //             }
    //             else
    //             {
    //                 return memoryStream.ToArray();
    //             }
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         modelState.AddModelError(formFile.Name,
    //             $"{fieldDisplayName}({trustedFileNameForDisplay}) upload failed. " +
    //             $"Please contact the Help Desk for support. Error: {ex.HResult}");
    //     }

    //     return Array.Empty<byte>();
    // }

    public static async Task<byte[]> ProcessStreamedFile(
        MultipartSection section, ContentDispositionHeaderValue contentDisposition, 
        ModelStateDictionary modelState, string[] permittedExtensions, long sizeLimit)
    {
        try
        {
            using (var memoryStream = new MemoryStream())
            {
                await section.Body.CopyToAsync(memoryStream);

                // Check if the file is empty or exceeds the size limit.
                if (memoryStream.Length == 0)
                    modelState.AddModelError("File", "The file is empty.");
                else if (memoryStream.Length > sizeLimit)
                {
                    var megabyteSizeLimit = sizeLimit / 1048576;
                    modelState.AddModelError("File", $"The file exceeds {megabyteSizeLimit:N1} MB.");
                }

                // Check file extension and first bytes
                else if (!IsValidFileExtensionAndSignature(contentDisposition.FileName.Value, memoryStream, permittedExtensions))
                    modelState.AddModelError("File", "The file must be of the following specs:<br>" +
                        "1. The file must hava a .xml File-Extension<br>" +
                        "2. To make sure the file isn't executable the file must start with: <?xml version=\"1.0\" encoding=\"utf-8\"?> or <?xml version=\"1.0\"?>");
                
                // Return the File as a byte array
                else return memoryStream.ToArray();
            }
        }
        catch (Exception ex)
        {
            modelState.AddModelError("File", $"The upload failed. Error: {ex.HResult}");
        }
        
        return Array.Empty<byte>();
    }

    private static bool IsValidFileExtensionAndSignature(string fileName, Stream data, string[] permittedExtensions)
    {
        if (string.IsNullOrEmpty(fileName) || data == null || data.Length == 0)
            return false;

        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            return false;

        data.Position = 0;

        using (var reader = new BinaryReader(data))
        {     
            var signatures = _fileSignature[ext];
            var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
            return signatures.Any(signature => 
                headerBytes.Take(signature.Length).SequenceEqual(signature));
        }
    }
}