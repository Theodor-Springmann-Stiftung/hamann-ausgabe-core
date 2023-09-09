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
using HaWeb.Models;
using System.Text;

public static class XMLFileHelpers {
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

    // public static List<FileModel>? ToFileModel(FileList? fileList) {
    //     if (fileList == null) return null;
    //     var fL = fileList.GetFileList();
    //     if (fL == null) return null;
    //     var ret = new List<FileModel>();
    //     foreach (var f in fL) {
    //         if (f.File == null) continue;
    //         ret.Add(ToFileModel(f));
    //     };
    //     return ret.OrderBy(x => x.LastModified).ToList();
    // }

    // // TODO: File State IsValid
    // public static FileModel ToFileModel(XMLRootDocument document) {
    //     string id = document.Prefix;
    //     var model = new FileModel(document.FileName, document.File.LastModified.LocalDateTime, true) { 
    //         Fields = document.Fields,
    //         Messages = document.GetLog(),
    //         Prefix = id
    //     };
    //     return model;
    // }

    public static bool ProcessFile(
        Stream file,
        string fileName,
        StringBuilder errorMessages,
        string[] permittedExtensions, 
        long sizeLimit) {
        try {
            // Check if the file is empty or exceeds the size limit.
            if (file.Length == 0) {
                errorMessages.AppendLine("Die Datei ist leer.");
                return false;
            }
            else if (file.Length > sizeLimit) {
                var megabyteSizeLimit = sizeLimit / 1048576;
                errorMessages.AppendLine($"Die Datei überschreitet das Größenlimit {megabyteSizeLimit:N1} MB.");
                return false;
            }

            // Return orderly, if signature & extension okay
            else return IsValidFileExtensionAndSignature(fileName, file, errorMessages, permittedExtensions);
            
        } catch (Exception ex) {
            errorMessages.AppendLine($"The upload failed. Error: {ex.Message}");
            return false;
        }
    }

    public static string? StreamToString(System.IO.Stream stream, ModelStateDictionary modelState) {
        string? ret = null;
        try {
            using (var rd = new StreamReader(stream, Encoding.UTF8)) {
                ret = rd.ReadToEnd();
                return ret;
            }
        }
        catch (Exception ex) {
            modelState.AddModelError("Error", "Reading of the message failed with " + ex.Message);
            return null;
        }
    } 

    private static bool IsValidFileExtensionAndSignature(string fileName, Stream data, StringBuilder errorMessages, string[] permittedExtensions) {
        if (string.IsNullOrEmpty(fileName) || data == null || data.Length == 0)
            return false;

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext)) {
            errorMessages.AppendLine("Dateiname endet nicht auf .xml");
            return false;
        }

        data.Position = 0;
        using (var reader = new BinaryReader(data)) {
            var signatures = _fileSignature[ext];
            var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
            if (!signatures.Any(signature =>
                headerBytes.Take(signature.Length).SequenceEqual(signature))) {
                    errorMessages.AppendLine("Datei muss mit <?xml version=\"1.0\" encoding=\"utf-8\"?> oder <?xml version=\"1.0\"?> beginnen.");
                    return false;
                };
        }
        return true;
    }
}