using System;
using UnityEngine.UIElements;


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
