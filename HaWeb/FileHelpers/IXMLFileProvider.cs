namespace HaWeb.FileHelpers;
using Microsoft.Extensions.FileProviders;
using System.Xml.Linq;
using HaWeb.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding;

public interface IXMLFileProvider {
        public List<IFileInfo>? GetWorkingTreeFiles();
        public IFileInfo? SaveHamannFile(XElement element, string basefilepath, ModelStateDictionary ModelState);
        public List<IFileInfo>? GetHamannFiles();
        public (DateTime PullTime, string Hash)? GetGitData();
        public void Reload(IConfiguration config);
        public bool HasChanged();
        public void DeleteHamannFile(string filename);
        public void Scan();
}