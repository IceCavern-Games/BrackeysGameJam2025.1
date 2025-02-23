using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class NavigatableStaticUI<TTarget, TItem> : NavigatableUI where TTarget : VisualElement where TItem : VisualElement
{
    [SerializeField] protected string _containerQuerySelector = null;
    [SerializeField] protected string _listItemsQuerySelector = null;

    protected List<TItem> _items = new();
    protected TTarget _target;

    /// <inheritdoc />
    protected override void Bind()
    {
        _target = UIUtils.QueryElement<TTarget>(_uiDocument.rootVisualElement, _containerQuerySelector);

        Debug.Assert(_target != null, "Specified element not found in the UI Document.");

        _items = UIUtils.QueryElements<TItem>(_target, _listItemsQuerySelector);

        _manipulator = new NavigatableUIManipulator(OnNavigate, OnNavigateSubmit, OnNavigateCancel);
        _target.AddManipulator(_manipulator);
        BindItems();

        Focus();
    }

    /// <summary>
    /// Focus the target so navigation events occur.
    /// </summary>
    protected override void Focus()
    {
        _items[0].Focus();
    }

    /// <inheritdoc />
    protected override void Unbind()
    {
        if (_target != null && _manipulator != null)
        {
            _target.RemoveManipulator(_manipulator);
            UnbindItems();
        }
    }

    /// <summary>
    /// Bind events to the list items.
    /// </summary>
    private void BindItems()
    {
        if (_items.Count() == 0)
            return;

        for (int i = 0; i < _items.Count(); i++)
        {
            // Ensure the item is focusable.
            _items[i].focusable = true;

            _items[i].RegisterCallback<FocusEvent>(OnItemFocus);
            _items[i].RegisterCallback<ClickEvent>(OnItemClick);
            _items[i].RegisterCallback<PointerEnterEvent>(OnItemPointerEnter);

            OnBindItem(_items[i], i);
        }
    }

    private void OnItemClick(ClickEvent evt)
    {
        OnNavigateSubmit(new NavigationSubmitEvent() { target = evt.currentTarget });
    }

    private void OnItemFocus(FocusEvent evt)
    {
        _items.ForEach(it => it.RemoveFromClassList("selected"));
        (evt.target as VisualElement).AddToClassList("selected");

        OnItemFocused(evt);
    }

    private void OnItemPointerEnter(PointerEnterEvent evt)
    {
        (evt.target as VisualElement).Focus();
    }

    /// <summary>
    /// Unbind events from the list items.
    /// </summary>
    private void UnbindItems()
    {
        if (_items.Count() == 0)
            return;

        for (int i = 0; i < _items.Count(); i++)
        {
            _items[i].UnregisterCallback<FocusEvent>(OnItemFocus);
            _items[i].UnregisterCallback<ClickEvent>(OnItemClick);
            _items[i].UnregisterCallback<PointerEnterEvent>(OnItemPointerEnter);

            OnUnbindItem(_items[i], i);
        }
    }
}
