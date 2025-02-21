using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.UIElements;

[UxmlElement]
public abstract partial class BaseSelectorField<TValueType, TValueChoice> : BaseField<TValueType>
{
    internal static readonly BindingId choicesProperty = nameof(choices);
    internal static readonly BindingId textProperty = nameof(text);

    internal List<TValueChoice> _choices;
    internal Func<TValueChoice, string> _formatListItemCallback;
    internal Func<TValueChoice, string> _formatSelectedValueCallback;

    internal abstract string GetListItemToDisplay(TValueType item);

    /// <summary>
    /// This is the value to display to the user.
    /// </summary>
    internal abstract string GetValueToDisplay();

    [CreateProperty, UxmlAttribute]
    public List<TValueChoice> choices
    {
        get => _choices;
        set
        {
            _choices = value ?? throw new ArgumentNullException(nameof(value));

            // Make sure to update the text displayed.
            SetValueWithoutNotify(rawValue);
            NotifyPropertyChanged(choicesProperty);
        }
    }

    /// <summary>
    /// This is the text displayed to the user for the current selection of the option.
    /// </summary>
    [CreateProperty(ReadOnly = true)]
    public string text
    {
        get => TextElement.text;
    }

    /// <summary>
    /// USS class name of elements of this type.
    /// </summary>
    public new static readonly string ussClassName = "unity-base-selector-field";
    /// <summary>
    /// USS class name of text elements in elements of this type.
    /// </summary>
    public static readonly string textUssClassName = ussClassName + "__text";
    /// <summary>
    /// USS class name of previous arrow indicator in elements of this type.
    /// </summary>
    public static readonly string prevArrowUssClassName = ussClassName + "__prevArrow";
    /// <summary>
    /// USS class name of previous arrow indicator in elements of this type.
    /// </summary>
    public static readonly string nextArrowUssClassName = ussClassName + "__nextArrow";
    /// <summary>
    /// USS class name of labels in elements of this type.
    /// </summary>
    public new static readonly string labelUssClassName = ussClassName + "__label";
    /// <summary>
    /// USS class name of input elements in elements of this type.
    /// </summary>
    public new static readonly string inputUssClassName = ussClassName + "__input";

    protected VisualElement InputContainer { get; }
    protected Label TextElement { get; }

    protected readonly Button _prevArrow;
    protected readonly Button _nextArrow;

    internal BaseSelectorField() : this(null) { }

    internal BaseSelectorField(string label) : base(label, new VisualElement())
    {
        InputContainer = contentContainer.ElementAt(0);

        AddToClassList(ussClassName);
        labelElement.AddToClassList(labelUssClassName);

        TextElement = new Label { pickingMode = PickingMode.Ignore };
        TextElement.AddToClassList(textUssClassName);

        _prevArrow = new Button();
        _nextArrow = new Button();

        _prevArrow.AddToClassList(prevArrowUssClassName);
        _nextArrow.AddToClassList(nextArrowUssClassName);

        InputContainer.AddToClassList(inputUssClassName);
        InputContainer.Add(_prevArrow);
        InputContainer.Add(TextElement);
        InputContainer.Add(_nextArrow);

        choices = new List<TValueChoice>();
    }

    /// <summary>
    /// Allow changing value without triggering any change event.
    /// </summary>
    /// <param name="newValue">The new value.</param>
    public override void SetValueWithoutNotify(TValueType newValue)
    {
        base.SetValueWithoutNotify(newValue);
        ((INotifyValueChanged<string>)TextElement).SetValueWithoutNotify(GetValueToDisplay());
    }
}
