using UnityEngine.UIElements;

[UxmlElement]
public partial class DialogueBox : VisualElement
{
    private const string _hasMoreClass = "DialogueBox--HasMore";
    private const string _showConfirmationClass = "DialogueBox--ShowConfirmation";

    #region UXML Attribute Getter/Setters

    [UxmlAttribute]
    public bool HasMore
    {
        get => _hasMore;
        set
        {
            _hasMore = value;

            if (value)
            {
                AddToClassList(_hasMoreClass);
                ConfirmationShown = false;
            }
            else
                RemoveFromClassList(_hasMoreClass);
        }
    }

    [UxmlAttribute]
    public bool ConfirmationShown
    {
        get => _showConfirmation;
        set
        {
            _showConfirmation = value;

            if (value)
            {
                AddToClassList(_showConfirmationClass);
                HasMore = false;
            }
            else
                RemoveFromClassList(_showConfirmationClass);
        }
    }

    [UxmlAttribute]
    public string Text
    {
        get => _text.text;
        private set => SetText(value);
    }

    private bool _hasMore = false;
    private bool _showConfirmation = false;

    #endregion
    #region UI Element References

    private readonly Label _text;
    private readonly Button _button;

    #endregion

    public DialogueBox()
    {
        _text = new Label { name = "DialogueBox__text" };
        Add(_text);

        _button = new Button { name = "DialogueBox__button" };

        TransitionLoop loop = new TransitionLoop(TransitionLoop.LoopType.Yoyo, "opacity", "dim", _button);
        loop.schedule.Execute(() => loop.Animate());
        _button.Add(loop);

        Add(_button);
    }

    public void SetHasMore(bool hasMore = true)
    {
        HasMore = hasMore;
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void ShowConfirmation(bool show = true)
    {
        ConfirmationShown = show;
    }
}
