using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameplayUIManager : MonoBehaviour
{
    private UIDocument _document;

    private VisualElement _interactContainer;
    private Label _interactLabel;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _interactContainer = _document.rootVisualElement.Q<VisualElement>(name: "interact-prompt");
        _interactLabel = _interactContainer.Q<Label>();

        if (_interactLabel.text == "Interact")
            HideInteractPrompt();
    }

    /// <summary>
    ///
    /// </summary>
    public void HideInteractPrompt()
    {
        _interactContainer.style.display = DisplayStyle.None;
    }

    /// <summary>
    ///
    /// </summary>
    public void SetInteractPrompt(string text)
    {
        _interactLabel.text = text;
        _interactContainer.style.display = DisplayStyle.Flex;
    }
}
