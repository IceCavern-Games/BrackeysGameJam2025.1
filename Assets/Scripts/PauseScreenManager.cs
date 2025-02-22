using Reflex.Attributes;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PauseScreenManager : NavigatableStaticUI<VisualElement, VisualElement>
{
    public bool CanClose => _uiDocument.rootVisualElement.style.display == DisplayStyle.Flex && // Pause screen needs to be showing
        !_options.OptionsScreen.IsOpen &&                                                       // and options screen is not open
        !_suppressNavigationCancel;                                                             // and navigation cancel cannot be suppressed

    public bool IsOpen => _uiDocument.rootVisualElement.style.display == DisplayStyle.Flex ||   // Pause screen is showing
        (                                                                                       // or pause screen is hidden, options screen is open
            _uiDocument.rootVisualElement.style.display == DisplayStyle.None &&
            _options != null && _options.OptionsScreen.IsOpen
        );

    [Inject] private readonly GameManager _gameManager;
    [Inject] private readonly InputManager _input;
    [Inject] private readonly OptionsManager _options;

    private bool _suppressNavigationCancel = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;

        if (_options == null)
            return;

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

    /// <summary>
    /// Pause the game and open the pause screen.
    /// </summary>
    public void Pause()
    {
        Debug.Log("PAUSE CALLED");
        Time.timeScale = 0f;
        _input.Input.DeactivateInput();
        _input.Prompts.ShowPrompts(PromptMappings.PauseMenu);
        _input.SetCursorLock(false);
        Open();
    }

    /// <summary>
    /// Unpause the game and close the pause screen.
    /// </summary>
    public void Unpause()
    {
        Time.timeScale = 1f;
        _input.SetCursorLock(true);
        _input.Input.ActivateInput();
        _input.Prompts.ShowPrompts(PromptMappings.Gameplay);
        Close();
    }

    /// <inheritdoc />
    protected override bool CanRender()
    {
        return IsOpen;
    }

    /// <inheritdoc />
    protected override void OnNavigateSubmit(NavigationSubmitEvent evt)
    {
        var itemName = (evt.target as VisualElement).name;

        switch (itemName)
        {
            case "pause-view-continue":
                Unpause();
                break;
            case "pause-view-options":
                _options.OptionsScreen.Open();
                break;
            case "pause-view-quit":
                Unpause();
                _gameManager.LoadTitleScreen();
                break;
        }
    }

    /// <inheritdoc />
    protected override void OnNavigateCancel(NavigationCancelEvent evt)
    {
        if (_suppressNavigationCancel || _options.OptionsScreen.IsOpen)
            return;

        if (IsOpen)
            Unpause();
    }

    /// <summary>
    /// Close the pause screen.
    /// </summary>
    private void Close()
    {
        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        Unbind();
    }

    /// <summary>
    /// Open the pause screen.
    /// </summary>
    private void Open()
    {
        Debug.Log("PAUSE SCREEN OPEN CALLED");
        _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;

        // @NOTE: Since the pause button and navigation cancel buttons can be the same,
        //        we need to ensure the menu doesn't close itself immediately after
        //        the navigation manipulator is registered on the same frame.
        _suppressNavigationCancel = true;
        StartCoroutine(ResetNavigationCancelSuppression());

        Bind();
    }

    /// <summary>
    /// Disable navigation cancel suppression after one frame.
    /// </summary>
    private IEnumerator ResetNavigationCancelSuppression()
    {
        yield return null; // Wait for one frame
        _suppressNavigationCancel = false;
    }

    #region Event Handlers

    private void OnOptionsOpened()
    {
        Close();
    }

    private void OnOptionsClosed()
    {
        Open();
        _input.Prompts.ShowPrompts(PromptMappings.PauseMenu);
    }

    #endregion
}
