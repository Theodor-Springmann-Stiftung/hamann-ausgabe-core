namespace HaWeb.FileHelpers;
using HaWeb.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public interface IXMLProvider {
        public FileList? GetFiles(string prefix);
        public Task Save(XMLRootDocument doc, string basefilepath, ModelStateDictionary ModelState);
}