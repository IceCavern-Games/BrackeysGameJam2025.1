using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SliderIntNavigationManipulator : Manipulator
{
    private readonly Action<int> _changeCallback;
    private readonly SliderInt _slider;

    private int _value;

    public SliderIntNavigationManipulator(SliderInt slider, Action<int> changeCallback)
    {
        _slider = slider;
        _changeCallback = changeCallback;
        _value = slider.value;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<NavigationMoveEvent>(OnNavigate);
        target.RegisterCallback<ChangeEvent<int>>(OnChange);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<NavigationMoveEvent>(OnNavigate);
        target.UnregisterCallback<ChangeEvent<int>>(OnChange);
    }

    private void OnChange(ChangeEvent<int> evt)
    {
        _changeCallback(evt.newValue);
    }

    private void OnNavigate(NavigationMoveEvent evt)
    {
        if (_slider == null) return;

        // Stop Unity from applying its default behavior
        evt.StopImmediatePropagation();

        // Apply the pageSize adjustment manually
        switch (evt.direction)
        {
            case NavigationMoveEvent.Direction.Left:
                _slider.value = (int)Mathf.Max(_slider.lowValue, _value - _slider.pageSize);
                _value = _slider.value;
                break;

            case NavigationMoveEvent.Direction.Right:
                _slider.value = (int)Mathf.Min(_slider.highValue, _value + _slider.pageSize);
                _value = _slider.value;
                break;
        }
    }

}
