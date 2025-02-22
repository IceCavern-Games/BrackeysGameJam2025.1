using Reflex.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(OptionsManager))]
[RequireComponent(typeof(UIDocument))]
public class OptionsScreenManager : NavigatableStaticUI<VisualElement, VisualElement>
{
    public event Action ScreenClosed;
    public event Action ScreenOpened;

    public bool IsOpen => _uiDocument.rootVisualElement.style.display == DisplayStyle.Flex;

    [Inject] private readonly InputManager _inputManager;

    private VisualElement _currentTab;
    private VisualElement _currentTabView;
    private Label _footerLabel;
    private readonly Dictionary<VisualElement, IManipulator> _manipulators = new();
    private ButtonPrompt _nextTabPrompt;
    private OptionsManager _optionsManager;
    private ButtonPrompt _prevTabPrompt;
    private List<VisualElement> _tabs;
    private List<VisualElement> _tabViews;
    private bool _suppressNavigationCancel = false;

    private InputAction _nextTabAction;
    private InputAction _prevTabAction;

    protected override void Awake()
    {
        base.Awake();

        _optionsManager = GetComponent<OptionsManager>();
        _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _footerLabel = _uiDocument.rootVisualElement.Q<Label>("options-footer-text");
        _nextTabPrompt = _uiDocument.rootVisualElement.Q<ButtonPrompt>("options-nav-right-prompt");
        _prevTabPrompt = _uiDocument.rootVisualElement.Q<ButtonPrompt>("options-nav-left-prompt");
        BindTabs();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        _nextTabAction.performed -= OnNextTabPressed;
        _prevTabAction.performed -= OnPrevTabPressed;

        for (int i = 0; i < _tabs.Count; i++)
        {
            int tabIndex = i;
            _tabs[i].UnregisterCallback<ClickEvent>(evt => OnTabClicked(tabIndex));
        }

        _inputManager.DeviceTypeChanged -= OnDeviceChanged;
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
        if (item is SliderInt slider)
        {
            var volumeSettings = new Dictionary<string, (Func<int> getValue, Action<int> setValue)>
            {
                { "audio-master-volume",   (() => _optionsManager.Options.Audio.MasterVolume,   _optionsManager.SetMasterVolume) },
                { "audio-ambience-volume", (() => _optionsManager.Options.Audio.AmbienceVolume, _optionsManager.SetAmbienceVolume) },
                { "audio-dialogue-volume", (() => _optionsManager.Options.Audio.DialogueVolume, _optionsManager.SetDialogueVolume) },
                { "audio-music-volume",    (() => _optionsManager.Options.Audio.MusicVolume,    _optionsManager.SetMusicVolume) },
                { "audio-sfx-volume",      (() => _optionsManager.Options.Audio.SFXVolume,      _optionsManager.SetSFXVolume) }
            };

            if (volumeSettings.TryGetValue(item.name, out var setting))
            {
                slider.value = setting.getValue();
                var manipulator = new SliderIntNavigationManipulator(slider, setting.setValue);
                item.AddManipulator(manipulator);
                _manipulators[item] = manipulator; // Store for removal
            }
        }
        else if (item is CarouselField carouselField)
        {
            switch (item.name)
            {
                case "video-display-mode":
                    carouselField.index = _optionsManager.Options.Video.DisplayMode switch
                    {
                        FullScreenMode.ExclusiveFullScreen => 0,
                        FullScreenMode.FullScreenWindow => 1,
                        FullScreenMode.MaximizedWindow => 0,
                        FullScreenMode.Windowed => 2,
                        _ => 2
                    };
                    item.RegisterCallback<ChangeEvent<string>>(OnVideoDisplayModeChanged);
                    break;

                case "video-resolution":
                    carouselField.choices = _optionsManager.AvailableResolutions
                        .Select(resolution => $"{resolution.width} x {resolution.height}")
                        .ToList();
                    string currentRes = $"{_optionsManager.Options.Video.Resolution.Width} x {_optionsManager.Options.Video.Resolution.Height}";
                    carouselField.index = carouselField.choices.IndexOf(currentRes);
                    item.RegisterCallback<ChangeEvent<string>>(OnVideoResolutionChanged);
                    break;
            }
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

    /// <inheritdoc />
    protected override void OnUnbindItem(VisualElement item, int index)
    {
        if (_manipulators.TryGetValue(item, out var manipulator))
        {
            item.RemoveManipulator(manipulator);
            _manipulators.Remove(item);
        }

        item.UnregisterCallback<ChangeEvent<string>>(OnVideoDisplayModeChanged);
        item.UnregisterCallback<ChangeEvent<string>>(OnVideoResolutionChanged);
    }

    private void BindTabs()
    {
        _tabs = _uiDocument.rootVisualElement.Query<VisualElement>().Class("options-nav-tab").ToList();
        _tabViews = _uiDocument.rootVisualElement.Query<VisualElement>().Class("options-tab-content").ToList();
        _currentTab = _tabs.FirstOrDefault(tab => tab.ClassListContains("selected"));
        _currentTabView = _tabViews.FirstOrDefault(view => view.ClassListContains("selected"));

        Debug.Assert(_tabs.Count != 0 && _tabViews.Count != 0 && _tabs.Count == _tabViews.Count, "Tabs and Tab Views are not setup correct!");
        Debug.Assert(_currentTab != null && _currentTabView != null, "Current selected tab could not be found.");

        for (int i = 0; i < _tabs.Count; i++)
        {
            int tabIndex = i;
            _tabs[i].RegisterCallback<ClickEvent>(evt => OnTabClicked(tabIndex));
        }
    }

    private void CycleTab(int direction)
    {
        int currentIndex = _tabs.IndexOf(_currentTab);
        int nextIndex = (currentIndex + direction + _tabs.Count) % _tabs.Count;

        CycleTabToIndex(nextIndex);
    }

    private void CycleTabToIndex(int index)
    {
        var nextTab = _tabs[index];
        var nextView = _tabViews[index];

        Unbind();

        _currentTab.RemoveFromClassList("selected");
        _currentTabView.RemoveFromClassList("selected");
        _currentTabView.AddToClassList("hidden");

        nextTab.AddToClassList("selected");
        nextView.AddToClassList("selected");
        nextView.RemoveFromClassList("hidden");

        _currentTab = nextTab;
        _currentTabView = nextView;

        Bind();
        Focus();
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

    private void SetPromptSprites()
    {
        _nextTabPrompt.Sprite = _inputManager.Prompts.GetBindingIconForAction(_nextTabAction);
        _prevTabPrompt.Sprite = _inputManager.Prompts.GetBindingIconForAction(_prevTabAction);
    }

    #region Event Handlers

    private void OnDeviceChanged(InputDevice device, string controlScheme)
    {
        SetPromptSprites();
    }

    [Inject]
    private void OnInject()
    {
        _inputManager.DeviceTypeChanged += OnDeviceChanged;

        _nextTabAction = _inputManager.Input.actions["NextTab"];
        _prevTabAction = _inputManager.Input.actions["PrevTab"];

        _nextTabAction.Enable();
        _prevTabAction.Enable();

        _nextTabAction.performed += OnNextTabPressed;
        _prevTabAction.performed += OnPrevTabPressed;

        SetPromptSprites();
    }

    private void OnNextTabPressed(InputAction.CallbackContext ctx)
    {
        CycleTab(1);
    }

    private void OnPrevTabPressed(InputAction.CallbackContext ctx)
    {
        CycleTab(-1);
    }

    private void OnTabClicked(int index)
    {
        CycleTabToIndex(index);
    }

    #endregion

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
