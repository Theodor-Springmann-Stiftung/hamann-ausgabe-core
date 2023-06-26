namespace HaWeb.FileHelpers;
using HaDocument.Interfaces;
using HaDocument.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HaXMLReader.Interfaces;

public interface IHaDocumentWrappper {
    public ILibrary? SetLibrary(string filepath, ModelStateDictionary ModelState);
    public ILibrary GetLibrary();

    public int GetStartYear();
    public int GetEndYear();
    public List<Person>? GetAvailablePersons();
    public Dictionary<string, Person>? GetPersonsWithLetters();
    public void SetEndYear(int end);
}