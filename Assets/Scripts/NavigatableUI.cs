using System;
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
    /// Clean up any bound callbacks/events/manipulators.
    /// </summary>
    protected virtual void Unbind() { }
}

/// <summary>
/// Manipulator to register navigation events and call any bound callbacks.
/// </summary>
public class NavigatableUIManipulator : Manipulator
{
    private readonly Action<VisualElement, int> _bindItem;
    private readonly Action<NavigationMoveEvent> _onNavigate;
    private readonly Action<NavigationSubmitEvent> _onSubmit;
    private readonly Action<NavigationCancelEvent> _onCancel;

    public NavigatableUIManipulator(Action<NavigationMoveEvent> navigate, Action<NavigationSubmitEvent> submit, Action<NavigationCancelEvent> cancel, Action<VisualElement, int> bind = null)
    {
        _onNavigate = navigate;
        _onSubmit = submit;
        _onCancel = cancel;
        _bindItem = bind;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
        target.RegisterCallback<NavigationSubmitEvent>(OnNavigationSubmit);
        target.RegisterCallback<NavigationCancelEvent>(OnNavigationCancel);

        if (target is ListView listView)
            listView.bindItem = _bindItem;
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        if (target is ListView listView)
            listView.bindItem = null;

        target.UnregisterCallback<NavigationMoveEvent>(OnNavigationMove);
        target.UnregisterCallback<NavigationSubmitEvent>(OnNavigationSubmit);
        target.UnregisterCallback<NavigationCancelEvent>(OnNavigationCancel);
    }

    private void OnNavigationMove(NavigationMoveEvent evt)
    {
        _onNavigate?.Invoke(evt);
    }

    private void OnNavigationSubmit(NavigationSubmitEvent evt)
    {
        _onSubmit?.Invoke(evt);
    }

    private void OnNavigationCancel(NavigationCancelEvent evt)
    {
        _onCancel?.Invoke(evt);
    }
}
