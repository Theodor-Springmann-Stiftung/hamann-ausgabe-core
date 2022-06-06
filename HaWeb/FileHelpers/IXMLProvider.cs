namespace HaWeb.FileHelpers;
using Microsoft.Extensions.FileProviders;
using System.Xml.Linq;
using HaWeb.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding;

public interface IXMLProvider {
        public FileList? GetFiles(string prefix);
        public Task Save(XMLRootDocument doc, string basefilepath, ModelStateDictionary ModelState);
        public Task<IFileInfo?> SaveHamannFile(XElement element, string basefilepath, ModelStateDictionary ModelState);
        public List<IFileInfo>? GetHamannFiles();
}