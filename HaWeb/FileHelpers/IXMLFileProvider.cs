namespace HaWeb.FileHelpers;
using Microsoft.Extensions.FileProviders;
using System.Xml.Linq;
using HaWeb.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding;

public interface IXMLFileProvider {
        public event EventHandler<GitState?> FileChange;
        public event EventHandler<XMLParsingState?> NewState;
        public event EventHandler NewData;
        public event EventHandler ConfigReload;
        public List<IFileInfo>? GetWorkingTreeFiles();
        public IFileInfo? SaveHamannFile(XElement element, string basefilepath, ModelStateDictionary ModelState);
        public List<IFileInfo>? GetHamannFiles();
        public GitState? GetGitState();
        public void ParseConfiguration(IConfiguration config);
        public bool HasChanged();
        public void DeleteHamannFile(string filename);
        public void Scan();
        public void Reload();
}