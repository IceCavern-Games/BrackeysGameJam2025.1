using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ButtonPrompt : VisualElement
{
    [UxmlAttribute]
    public Sprite Sprite
    {
        get => _sprite;
        set
        {
            _sprite = value;
            _promptImage.style.backgroundImage = new StyleBackground(value);
        }
    }

    [UxmlAttribute]
    public string Text
    {
        get => _promptText.text;
        set
        {
            _promptText.text = value;
            _promptText.style.display = value != string.Empty ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private readonly VisualElement _promptImage;
    private readonly Label _promptText;

    private Sprite _sprite;

    public ButtonPrompt()
    {
        _promptImage = new VisualElement { name = "image" };
        Add(_promptImage);

        _promptText = new Label { name = "text" };
        Add(_promptText);
    }
}
