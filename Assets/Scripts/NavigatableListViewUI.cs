using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public abstract class NavigatableListViewUI : NavigatableUI
{
    [SerializeField] protected string _listQuerySelector = null;

    protected ListView _list;

    /// <inheritdoc />
    protected override void Bind()
    {
        _list = _uiDocument.rootVisualElement.Q<ListView>(_listQuerySelector);
        Debug.Assert(_list != null, "Specified ListView not found in the UI Document.");

        _manipulator = new NavigatableUIManipulator(OnNavigate, OnNavigateSubmit, OnNavigateCancel, BindItem);
        _list.AddManipulator(_manipulator);
        _list.RegisterCallback<BlurEvent>(OnBlur);
        _list.RegisterCallback<FocusEvent>(OnFocus);

        _list.itemsSource = ItemSource();
        Focus();
    }

    /// <summary>
    /// Default handler for binding items on this list.
    /// </summary>
    protected virtual void BindItem(VisualElement item, int index)
    {
        item.RegisterCallback<FocusEvent>(OnItemFocus);
        item.RegisterCallback<ClickEvent>((_) => OnNavigateSubmit(null));
        item.RegisterCallback<PointerEnterEvent>((evt) =>
        {
            _list.SetSelection(index);
        }, TrickleDown.NoTrickleDown);

        // Allow derived classes to define additional behavior.
        OnBindItem(item, index);
    }

    /// <inheritdoc />
    protected override void Focus()
    {
        _list.Focus();
    }

    /// <summary>
    /// The list of items the list view should render.
    /// </summary>
    protected abstract IList ItemSource();

    /// <inheritdoc />
    protected override void OnNavigate(NavigationMoveEvent evt)
    {
        void HandleSelectionAndScroll(int index)
        {
            // @TODO: Handle (configurable) looping.
            if (index < 0 || index >= _list.itemsSource.Count)
                return;

            _list.selectedIndex = index;
            _list.ScrollToItem(index);
        }

        // @TODO: Handle different/configurable list orientations.
        switch (evt.direction)
        {
            case NavigationMoveEvent.Direction.Up:
                HandleSelectionAndScroll(_list.selectedIndex - 1);
                break;
            case NavigationMoveEvent.Direction.Down:
                HandleSelectionAndScroll(_list.selectedIndex + 1);
                break;
            case NavigationMoveEvent.Direction.Left:
                break;
            case NavigationMoveEvent.Direction.Right:
                break;
        }
    }

    /// <inheritdoc />
    protected override void Unbind()
    {
        if (_list != null && _manipulator != null)
        {
            _list.RemoveManipulator(_manipulator);
            _list.UnregisterCallback<BlurEvent>(OnBlur);
            _list.UnregisterCallback<FocusEvent>(OnFocus);
        }
    }

    private void OnBlur(BlurEvent evt)
    {
        _list.selectedIndex = -1;
    }

    private void OnFocus(FocusEvent evt)
    {
        _list.SetSelection(0);
    }

    private void OnItemFocus(FocusEvent evt)
    {
        OnItemFocused(evt);
    }
}
