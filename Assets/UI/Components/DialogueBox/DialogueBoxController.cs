public class DialogueBoxController
{
    #region UI Element References

    private DialogueBox _dialogue;

    #endregion

    private bool _hasMore = false;

    public bool ConfirmationShown { get; private set; } = false;
    public string Text { get; private set; } = "";
    public int VisibleCharacters { get; private set; } = 0;

    /// <summary>
    /// Initialize the DialogueBox and set all UI references.
    /// Should be ran OnEnable.
    /// </summary>
    public void Initialize(DialogueBox visualElement)
    {
        _dialogue = visualElement;

        RenderHasMore();
        RenderText();
    }

    /// <summary>
    /// Set whether or not there is more dialogue.
    /// </summary>
    public void SetHasMore(bool hasMore = true)
    {
        _hasMore = hasMore;
        RenderHasMore();
    }

    /// <summary>
    /// Set the text to be displayed on the dialogue box.
    /// If no visibleCharacter count is set, we'll display the max possible.
    /// </summary>
    public void SetText(string text, int visibleCharacters = -1)
    {
        Text = text;
        VisibleCharacters = visibleCharacters == -1 ? text.Length : visibleCharacters;
        RenderText();
    }

    public void SetVisibleCharacters(int visibleCharacters)
    {
        VisibleCharacters = visibleCharacters;
        RenderText();
    }

    /// <summary>
    /// Set whether or not the confirmation icon should be shown.
    /// </summary>
    public void ShowConfirmation(bool show = true)
    {
        ConfirmationShown = show;
        RenderConfirmation();
    }

    /// <summary>
    /// Display whether or not the confirmation icon should be shown.
    /// </summary>
    private void RenderConfirmation()
    {
        _dialogue.ShowConfirmation(ConfirmationShown);
    }

    /// <summary>
    /// Display whether or not there is more dialogue.
    /// </summary>
    private void RenderHasMore()
    {
        _dialogue.SetHasMore(_hasMore);
    }

    /// <summary>
    /// Display the text on the dialogue box.
    /// </summary>
    private void RenderText()
    {
        string visibleText = Text;

        if (VisibleCharacters != Text.Length)
            visibleText = Text[..VisibleCharacters] + "<alpha=#00>" + Text[VisibleCharacters..];

        _dialogue.SetText(visibleText);
    }
}
