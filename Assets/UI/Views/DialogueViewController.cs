using System;
using UnityEngine.UIElements;

public class DialogueViewController
{
    #region UI Element References

    private VisualElement _dialogueBox;

    #endregion

    public bool Shown { get; private set; } = false;

    private Action _callback;

    public void Initialize(VisualElement visualElement, Action callback)
    {
        _callback = callback;
        _dialogueBox = visualElement.Q<DialogueBox>();

        RenderDialogueBox();
    }

    /// <summary>
    /// Show (or hide) the dialogue box.
    /// </summary>
    public void ShowDialogueBox(bool show = true)
    {
        Shown = show;
        RenderDialogueBox();

        if (show)
        {
            _dialogueBox.RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
            _dialogueBox.RegisterCallback<NavigationSubmitEvent>(OnNavigationSubmit);
            _dialogueBox.RegisterCallback<NavigationCancelEvent>(OnNavigationCancel);

            _dialogueBox.focusable = true;
            _dialogueBox.Focus();
        }
        else
        {
            _dialogueBox.UnregisterCallback<NavigationMoveEvent>(OnNavigationMove);
            _dialogueBox.UnregisterCallback<NavigationSubmitEvent>(OnNavigationSubmit);
            _dialogueBox.UnregisterCallback<NavigationCancelEvent>(OnNavigationCancel);

            _dialogueBox.focusable = false;
        }
    }

    private void OnNavigationMove(NavigationMoveEvent evt)
    {
        evt.StopPropagation();
    }

    private void OnNavigationSubmit(NavigationSubmitEvent evt)
    {
        _callback?.Invoke();
    }

    private void OnNavigationCancel(NavigationCancelEvent evt)
    {
        _callback?.Invoke();
    }

    /// <summary>
    /// Display the dialogue box.
    /// </summary>
    private void RenderDialogueBox()
    {
        _dialogueBox.style.display = Shown ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
