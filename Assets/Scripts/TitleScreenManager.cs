using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class TitleScreenManager : NavigatableStaticUI<VisualElement, VisualElement>
{
    [Inject] private readonly GameManager _gameManager;
    [Inject] private readonly InputManager _input;
    [Inject] private readonly OptionsManager _options;
    [Inject] private readonly UIManager _uiManager;

    protected override void OnEnable()
    {
        base.OnEnable();

        _options.OptionsScreen.ScreenOpened += OnOptionsOpened;
        _options.OptionsScreen.ScreenClosed += OnOptionsClosed;
    }

    protected override void OnDisable()
    {
        if (_options == null)
            return;

        _options.OptionsScreen.ScreenOpened -= OnOptionsOpened;
        _options.OptionsScreen.ScreenClosed -= OnOptionsClosed;
    }

    private void Start()
    {
        _uiManager.HideAllUI();
        _input.Prompts.ShowPrompts(PromptMappings.TitleMenu);
    }

    /// <inheritdoc />
    protected override void OnNavigateSubmit(NavigationSubmitEvent evt)
    {
        var itemName = (evt.target as VisualElement).name;

        switch (itemName)
        {
            case "title-start":
                _gameManager.LoadTheOffice();
                break;
            case "title-options":
                _options.OptionsScreen.Open();
                break;
            case "title-exit":
                _gameManager.ExitGame();
                break;
        }
    }

    private void OnOptionsOpened()
    {
        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        Unbind();
    }

    private void OnOptionsClosed()
    {
        _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        Bind();
        _input.Prompts.ShowPrompts(PromptMappings.TitleMenu);
    }
}
