namespace HaWeb.Models;
using HaDocument.Models;
using HaDocument.Interfaces;
using System.Text;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;

public class CommentModel {
    public string ParsedComment { get; private set; }
    public List<string>? ParsedSubComments { get; private set; }

    public CommentModel(string parsedComment, List<string>? parsedSubComments) {
        this.ParsedComment = parsedComment;
        this.ParsedSubComments = parsedSubComments;
    }
}