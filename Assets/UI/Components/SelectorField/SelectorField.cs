using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SelectorField<T> : BaseSelectorField<T, T>
{
    internal static readonly BindingId indexProperty = nameof(index);

    internal override string GetListItemToDisplay(T value)
    {
        if (_formatListItemCallback != null)
            return _formatListItemCallback(value);

        return (value != null && _choices.Contains(value)) ? value.ToString() : string.Empty;
    }

    internal override string GetValueToDisplay()
    {
        if (_formatSelectedValueCallback != null)
            return _formatSelectedValueCallback(value);

        if (value != null)
            return value.ToString();

        return string.Empty;
    }

    internal void SetIndexWithoutNotify(int index)
    {
        _index = index;
        if (_index >= 0 && _index < choices.Count)
            SetValueWithoutNotify(choices[_index]);
        else
            SetValueWithoutNotify(default);
    }

    /// <summary>
    /// The currently selected index in the menu.
    /// Setting the index will update the ::ref::value field and send a property change notification.
    /// </summary>
    [CreateProperty, UxmlAttribute]
    public int index
    {
        get => _index;
        set
        {
            if (value != _index)
            {
                _index = value;
                this.value = _index >= 0 && _index < choices.Count ? choices[_index] : default;
                NotifyPropertyChanged(indexProperty);
            }
        }
    }

    /// <summary>
    /// The currently selected value in the menu.
    /// </summary>
    [HideInInspector]
    public override T value
    {
        get => base.value;
        set
        {
            var previousIndex = _index;
            _index = choices?.IndexOf(value) ?? -1;
            base.value = value;

            if (_index != previousIndex)
                NotifyPropertyChanged(indexProperty);
        }
    }

    /// <summary>
    /// Callback that provides a string representation used to populate the menu.
    /// </summary>
    public virtual Func<T, string> FormatListItemCallback
    {
        get => _formatListItemCallback;
        set
        {
            _formatListItemCallback = value;
        }
    }

    /// <summary>
    /// Callback that provides a string representation used to display the selected value.
    /// </summary>
    public virtual Func<T, string> FormatSelectedValueCallback
    {
        get => _formatSelectedValueCallback;
        set
        {
            _formatSelectedValueCallback = value;
            TextElement.text = GetValueToDisplay();
        }
    }

    /// <summary>
    /// USS class name of elements of this type.
    /// </summary>
    public new static readonly string ussClassName = "unity-selector-field";
    /// <summary>
    /// USS class name of labels in elements of this type.
    /// </summary>
    public new static readonly string labelUssClassName = ussClassName + "__label";
    /// <summary>
    /// USS class name of input elements in elements of this type.
    /// </summary>
    public new static readonly string inputUssClassName = ussClassName + "__input";

    private int _index = -1;

    public SelectorField() : this(null) { }

    public SelectorField(string label = null) : base(label)
    {
        AddToClassList(ussClassName);
        labelElement.AddToClassList(labelUssClassName);
        InputContainer.AddToClassList(inputUssClassName);

        RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
        _prevArrow.RegisterCallback<ClickEvent>(OnPrevButtonClick);
        _nextArrow.RegisterCallback<ClickEvent>(OnNextButtonClick);
    }

    public SelectorField(List<T> choices, T defaultValue, Func<T, string> formatSelectedValueCallback = null, Func<T, string> formatListItemCallback = null)
        : this(null, choices, defaultValue, formatSelectedValueCallback, formatListItemCallback) { }

    public SelectorField(string label, List<T> choices, T defaultValue, Func<T, string> formatSelectedValueCallback = null, Func<T, string> formatListItemCallback = null)
        : this(label)
    {
        if (defaultValue == null)
            throw new ArgumentNullException(nameof(defaultValue));

        this.choices = choices;

        if (!choices.Contains(defaultValue))
            throw new ArgumentException($"Default value {defaultValue} is not present in the list of possible values.");

        SetValueWithoutNotify(defaultValue);

        _formatListItemCallback = formatListItemCallback;
        _formatSelectedValueCallback = formatSelectedValueCallback;
    }

    public SelectorField(List<T> choices, int defaultIndex, Func<T, string> formatSelectedValueCallback = null, Func<T, string> formatListItemCallback = null)
        : this(null, choices, defaultIndex, formatSelectedValueCallback, formatListItemCallback) { }

    public SelectorField(string label, List<T> choices, int defaultIndex, Func<T, string> formatSelectedValueCallback = null, Func<T, string> formatListItemCallback = null)
        : this(label)
    {
        this.choices = choices;

        SetIndexWithoutNotify(defaultIndex);

        _formatListItemCallback = formatListItemCallback;
        _formatSelectedValueCallback = formatSelectedValueCallback;
    }

    public override void SetValueWithoutNotify(T newValue)
    {
        _index = choices?.IndexOf(newValue) ?? -1;
        base.SetValueWithoutNotify(newValue);
    }

    protected void NextChoice()
    {
        index++;

        // @NOTE: Setter will make anything above the choices.count go to -1.
        if (index == -1)
            index = 0;
    }

    protected void OnNavigationMove(NavigationMoveEvent evt)
    {
        switch (evt.direction)
        {
            case NavigationMoveEvent.Direction.Left:
                PrevChoice();
                break;
            case NavigationMoveEvent.Direction.Right:
                NextChoice();
                break;
        }
    }

    protected void OnNextButtonClick(ClickEvent evt)
    {
        evt.StopPropagation();
        NextChoice();
    }

    protected void OnPrevButtonClick(ClickEvent evt)
    {
        evt.StopPropagation();
        PrevChoice();
    }

    protected void PrevChoice()
    {
        index--;

        if (index < 0)
            index = choices.Count - 1;
    }
}
