namespace HaWeb.Settings.ParsingState;
using System.Text;

public enum CommentType {
    Comment,
    Subcomment
}

public class CommentState : HaWeb.HTMLParser.IState {
    internal string Category;
    internal CommentType Type;

    public CommentState(string category, CommentType type) {
        this.Category = category;
        this.Type = type;
    }

    public void SetupState() { }
}