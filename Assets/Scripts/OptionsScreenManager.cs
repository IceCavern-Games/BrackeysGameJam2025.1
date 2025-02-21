using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(OptionsManager))]
[RequireComponent(typeof(UIDocument))]
public class OptionsScreenManager : NavigatableStaticUI<VisualElement, VisualElement>
{
    public event Action ScreenClosed;
    public event Action ScreenOpened;

    public bool IsOpen => _uiDocument.rootVisualElement.style.display == DisplayStyle.Flex;

    private Label _footerLabel;
    private OptionsManager _optionsManager;
    private bool _suppressNavigationCancel = false;

    protected override void Awake()
    {
        base.Awake();

        _optionsManager = GetComponent<OptionsManager>();
        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        _footerLabel = _uiDocument.rootVisualElement.Q<Label>(name = "options-footer-text");
    }

    /// <summary>
    /// Close the options screen.
    /// </summary>
    public void Close()
    {
        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        _optionsManager.SaveOptions();
        Unbind();
        ScreenClosed?.Invoke();
    }

    /// <summary>
    /// Open the options screen.
    /// </summary>
    public void Open()
    {
        _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;

        // @NOTE: Since the pause button and navigation cancel buttons can be the same,
        //        we need to ensure the menu doesn't close itself immediately after
        //        the navigation manipulator is registered on the same frame.
        _suppressNavigationCancel = true;
        StartCoroutine(ResetNavigationCancelSuppression());

        Bind();
        ScreenOpened?.Invoke();
    }

    /// <inheritdoc />
    protected override bool CanRender()
    {
        return IsOpen;
    }

    /// <inheritdoc />
    protected override void OnBindItem(VisualElement item, int index)
    {
        switch (item.name)
        {
            case "video-display-mode":
                var displayField = item as CarouselField;

                displayField.index = _optionsManager.Options.Video.DisplayMode switch
                { // 0 = Fullscreen, 1 = Borderless, 2 = Windowed
                    FullScreenMode.ExclusiveFullScreen => 0,
                    FullScreenMode.FullScreenWindow => 1,
                    FullScreenMode.MaximizedWindow => 0,
                    FullScreenMode.Windowed => 2,
                    _ => 2
                };

                item.RegisterCallback<ChangeEvent<string>>(OnVideoDisplayModeChanged);
                break;
            case "video-resolution":
                var resField = item as CarouselField;

                // Fill out available resolutions.
                resField.choices = _optionsManager.AvailableResolutions.Select((resolution) => $"{resolution.width} x {resolution.height}").ToList();
                string currentRes = $"{_optionsManager.Options.Video.Resolution.Width} x {_optionsManager.Options.Video.Resolution.Height}";
                resField.index = resField.choices.IndexOf(currentRes);

                item.RegisterCallback<ChangeEvent<string>>(OnVideoResolutionChanged);
                break;
        }
    }

    /// <inheritdoc />
    protected override void OnItemFocused(FocusEvent evt)
    {
        DisplayFooterFromTooltip(evt.target as VisualElement);
    }

    /// <inheritdoc />
    protected override void OnNavigateCancel(NavigationCancelEvent evt)
    {
        if (_suppressNavigationCancel)
            return;

        if (IsOpen)
            Close();
    }

    /// <summary>
    /// Display the tooltip of the focused menu item in the footer.
    /// </summary>
    private void DisplayFooterFromTooltip(VisualElement target)
    {
        _footerLabel.text = target.tooltip ?? string.Empty;
    }

    /// <summary>
    /// Disable navigation cancel suppression after one frame.
    /// </summary>
    private IEnumerator ResetNavigationCancelSuppression()
    {
        yield return null; // Wait for one frame
        _suppressNavigationCancel = false;
    }

    #region Option Changed Events

    private void OnVideoDisplayModeChanged(ChangeEvent<string> evt)
    {
        _optionsManager.SetDisplayMode(evt.newValue switch
        {
            "Fullscreen" => 0,
            "Borderless" => 1,
            "Windowed" => 2,
            _ => 2
        });
    }

    private void OnVideoResolutionChanged(ChangeEvent<string> evt)
    {
        if (evt.newValue == null)
            return;

        var resolution = evt.newValue.Split(" x ");
        _optionsManager.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]));
    }

    #endregion
}
