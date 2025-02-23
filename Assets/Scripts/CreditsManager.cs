using Reflex.Attributes;
using System;
using System.Collections;
using UnityEngine.UIElements;

public class CreditsManager : NavigatableStaticUI<VisualElement, VisualElement>
{
    public event Action ScreenClosed;
    public event Action ScreenOpened;

    public bool IsOpen => _uiDocument.rootVisualElement.style.display == DisplayStyle.Flex;

    [Inject] private readonly InputManager _inputManager;

    private bool _suppressNavigationCancel = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Close the credits screen.
    /// </summary>
    public void Close()
    {
        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        Unbind();
        ScreenClosed?.Invoke();
    }

    /// <summary>
    /// Open the options screen.
    /// </summary>
    public void Open()
    {
        _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;

        // @NOTE: Since the pause button and navigation cancel buttons can be the same,
        //        we need to ensure the menu doesn't close itself immediately after
        //        the navigation manipulator is registered on the same frame.
        _suppressNavigationCancel = true;
        StartCoroutine(ResetNavigationCancelSuppression());

        Bind();
        _inputManager.Prompts.ShowPrompts(PromptMappings.CreditsMenu);
        ScreenOpened?.Invoke();
    }

    /// <inheritdoc />
    protected override bool CanRender()
    {
        return IsOpen;
    }

    /// <inheritdoc />
    protected override void OnNavigateCancel(NavigationCancelEvent evt)
    {
        if (_suppressNavigationCancel)
            return;

        if (IsOpen)
            Close();
    }

    protected override void Focus()
    {
        _target.Focus();
    }

    /// <summary>
    /// Disable navigation cancel suppression after one frame.
    /// </summary>
    private IEnumerator ResetNavigationCancelSuppression()
    {
        yield return null; // Wait for one frame
        _suppressNavigationCancel = false;
    }
}
