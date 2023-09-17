using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
// See https://aka.ms/new-console-template for more information
const string XML_PATH = "C:/Users/simon/source/hamann-xml/";
const string DEST_PATH = "C:/Users/simon/source/hamann-xml/";
const string GIT_PATH = "C:/Users/simon/source/hamann-xml/";
const string BRANCH_NAME = "main";

var xmls = FileOperations.GetXMLs(XML_PATH, GIT_PATH, BRANCH_NAME);
var cp = CharacterEntityReferences.GetCodePoints(xmls);

AutopsicNumberTransform.Collect(xmls);
AutopsicNumberTransform.Transform();
MarginalsTransform.Marginals = AutopsicNumberTransform.Marginals.SelectMany(x => x.Value).ToList();
MarginalsTransform.Transform();
FileOperations.SaveFile(AutopsicNumberTransform.Documents, DEST_PATH);

xmls = FileOperations.GetXMLs(DEST_PATH, null, null);

// Sets only whitespace entities
CharacterEntityReferences.Replace(xmls, cp.Where(x => x != null && String.IsNullOrWhiteSpace(x)));