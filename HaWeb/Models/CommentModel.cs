namespace HaWeb.Models;
using HaDocument.Models;
using HaDocument.Interfaces;
using System.Text;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;

public class CommentModel {
    public string ID { get; private set; }
    public string ParsedComment { get; private set; }
    public List<string>? ParsedSubComments { get; private set; }

    public CommentModel(string parsedComment, List<string>? parsedSubComments, string id) {
        this.ID = id;
        this.ParsedComment = parsedComment;
        this.ParsedSubComments = parsedSubComments;
    }
}