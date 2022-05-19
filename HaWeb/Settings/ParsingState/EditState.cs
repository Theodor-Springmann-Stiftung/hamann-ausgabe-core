namespace HaWeb.Settings.ParsingState;
using System.Text;

public class EditState : HaWeb.HTMLParser.IState {
    internal bool active_del;
    internal bool active_skipwhitespace;

    internal StringBuilder sb_edits;

    public EditState() {
        SetupState();
    }

    public void SetupState() {
        sb_edits = new StringBuilder();
        active_del = false;
        active_skipwhitespace = true;
    }
}