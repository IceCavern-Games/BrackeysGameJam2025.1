using UnityEngine.UIElements;

public class TransitionLoop : VisualElement
{
    public enum LoopType
    {
        Yoyo,
        A2B
    }

    #region UXML Factory/Attributes

    [System.Obsolete]
    public new class UxmlFactory : UxmlFactory<TransitionLoop, UxmlTraits> { }

    [System.Obsolete]
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private readonly UxmlBoolAttributeDescription _enabled = new UxmlBoolAttributeDescription
        { name = "enabled" };
        private readonly UxmlEnumAttributeDescription<LoopType> _loopType = new UxmlEnumAttributeDescription<LoopType>
        { name = "loop-type", defaultValue = LoopType.Yoyo, use = UxmlAttributeDescription.Use.Required };
        private readonly UxmlStringAttributeDescription _targetProperty = new UxmlStringAttributeDescription
        { name = "target-property", use = UxmlAttributeDescription.Use.Required };
        private readonly UxmlStringAttributeDescription _aClass = new UxmlStringAttributeDescription
        { name = "a-class", use = UxmlAttributeDescription.Use.Required };
        private readonly UxmlStringAttributeDescription _bClass = new UxmlStringAttributeDescription
        { name = "b-class" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            TransitionLoop loop = ve as TransitionLoop;
            loop.AClass = _aClass.GetValueFromBag(bag, cc);
            loop.BClass = _bClass.GetValueFromBag(bag, cc);
            loop.Type = _loopType.GetValueFromBag(bag, cc);
            loop.TargetProperty = _targetProperty.GetValueFromBag(bag, cc);
            loop.Enabled = _enabled.GetValueFromBag(bag, cc);
        }
    }

    #endregion
    #region UXML Attribute Getter/Setters

    private bool _enabled = false;

    public string AClass { get; private set; }
    public string BClass { get; private set; }
    public bool Enabled
    {
        get => _enabled;
        private set => Animate(value);
    }
    public string TargetProperty { get; private set; }
    public LoopType Type { get; private set; }

    #endregion

    private bool _cancelled = false;
    private VisualElement _parent;

    public TransitionLoop()
    {
        RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
    }

    public TransitionLoop(LoopType type, string targetProperty, string aClass, VisualElement parent = null) : this()
    {
        Type = type;
        TargetProperty = targetProperty;
        AClass = aClass;
        _parent = parent;
    }

    /// <summary>
    /// Start or stop the loop.
    /// </summary>
    public void Animate(bool shouldAnimate = true)
    {
        _enabled = shouldAnimate;
        _cancelled = false;

        if (_parent == null)
            return;

        if (_enabled == false)
        {
            _cancelled = true;
            return;
        }

        if (AClass == "" || TargetProperty == "")
            return;

        switch (Type)
        {
            case LoopType.A2B:
                if (BClass == "")
                    return;

                // @TODO: Maybe
                break;
            default:
                SetupYoyo();
                break;
        }
    }

    /// <summary>
    /// Get a reference to the parent and begin animating if enabled.
    /// </summary>
    private void OnAttachToPanel(AttachToPanelEvent evt)
    {
        if (_parent == null)
            _parent = parent;

        Animate(_enabled);
    }

    /// <summary>
    /// Stop all transitions when detaching.
    /// </summary>
    private void OnDetachFromPanel(DetachFromPanelEvent evt)
    {
        Stop();
    }

    /// <summary>
    /// Toggle the class after the transition.
    /// </summary>
    private void OnYoyo(TransitionEndEvent evt)
    {
        if (_cancelled)
        {
            Stop();
            return;
        }

        if (evt.currentTarget == _parent && evt.AffectsProperty(TargetProperty))
            _parent.ToggleInClassList(AClass);
    }

    /// <summary>
    /// Setup a yo-yo style looping animation, via toggling a class on/off.
    /// </summary>
    private void SetupYoyo()
    {
        _parent.RegisterCallback<TransitionEndEvent>(OnYoyo);
        _parent.schedule.Execute(() => _parent.ToggleInClassList(AClass));
    }

    /// <summary>
    /// Stop all transitions.
    /// </summary>
    private void Stop()
    {
        _parent.UnregisterCallback<TransitionEndEvent>(OnYoyo);
        _parent.RemoveFromClassList(AClass);
        _cancelled = false;
    }
}
