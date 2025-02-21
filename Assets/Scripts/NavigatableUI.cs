using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class NavigatableUI : MonoBehaviour
{
    protected Manipulator _manipulator;
    protected UIDocument _uiDocument;

    protected virtual void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    protected virtual void OnEnable()
    {
        if (CanRender())
            Bind();
    }

    protected virtual void OnDisable()
    {
        Unbind();
    }

    /// <summary>
    /// Register any callbacks/events/manipulators.
    /// </summary>
    protected virtual void Bind() { }

    /// <summary>
    /// Callback that determines if the list should render at all.
    /// </summary>
    protected virtual bool CanRender()
    {
        return true;
    }

    /// <summary>
    /// Focus the target element.
    /// </summary>
    protected virtual void Focus() { }

    /// <summary>
    /// Callback on BindItem that can be used for additional functionality.
    /// </summary>
    protected virtual void OnBindItem(VisualElement item, int index) { }

    /// <summary>
    /// Callback when an individual menu item is focused.
    /// </summary>
    protected virtual void OnItemFocused(FocusEvent evt) { }

    /// <summary>
    /// Callback for when navigation is performed.
    /// </summary>
    protected virtual void OnNavigate(NavigationMoveEvent evt) { }

    /// <summary>
    /// Callback for when the submit button is pressed and/or a list item  is clicked.
    /// </summary>
    protected virtual void OnNavigateSubmit(NavigationSubmitEvent evt) { }

    /// <summary>
    /// Callback for when the cancel button is pressed.
    /// </summary>
    protected virtual void OnNavigateCancel(NavigationCancelEvent evt) { }

    /// <summary>
    /// Callback on UnbindItem that can be used for additional functionality.
    /// </summary>
    protected virtual void OnUnbindItem(VisualElement item, int index) { }

    /// <summary>
    /// Clean up any bound callbacks/events/manipulators.
    /// </summary>
    protected virtual void Unbind() { }
}
