using Reflex.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(OptionsScreenManager))]
public class OptionsManager : MonoBehaviour
{
    public List<Resolution> AvailableResolutions { get; private set; }
    public bool FileExists => _fileHandler.FileExists;
    public Options Options { get; private set; }
    public OptionsScreenManager OptionsScreen { get; private set; }

    [Inject] private readonly AudioManager _audioManager;

    [SerializeField] private string _fileName = "config";
    [SerializeField] private Options _defaultOptions;

    private SaveFileHandler<Options> _fileHandler;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _fileHandler = new(Application.persistentDataPath, _fileName, false);
        OptionsScreen = GetComponent<OptionsScreenManager>();
    }

    /// <summary>
    /// Apply all the settings for the given options object.
    /// </summary>
    public void ApplyOptions(Options options, bool applyVideo = true)
    {
        if (options == null)
        {
            Debug.LogError("Attempted to apply null options object.");
            return;
        }

        // Audio
        SetMasterVolume(options.Audio.MasterVolume);
        SetAmbienceVolume(options.Audio.AmbienceVolume);
        SetDialogueVolume(options.Audio.DialogueVolume);
        SetMusicVolume(options.Audio.MusicVolume);
        SetSFXVolume(options.Audio.SFXVolume);

        // Video
        if (applyVideo)
        {
            SetDisplayMode(options.Video.DisplayMode);
            SetResolution(options.Video.Resolution);
            SetVsync(options.Video.Vsync);
        }
    }

    /// <summary>
    /// Reset to default options.
    /// </summary>
    public void DefaultOptions()
    {
        Options = _defaultOptions;
        SaveOptions();
    }

    /// <summary>
    /// Load options from a file.
    /// </summary>
    public void LoadOptions()
    {
        Options = _fileHandler.Load();
    }

    /// <summary>
    /// Save options to a file.
    /// </summary>
    public void SaveOptions()
    {
        _fileHandler.Save(Options);
    }

    /// <summary>
    /// Apply loaded options after all other managers have been booted.
    /// </summary>
    [Inject]
    private void ApplyOnBoot()
    {
        AvailableResolutions = Screen.resolutions
            .Select(r => new { r.width, r.height })
            .Distinct() // Get each unique resolution by width and height
            .Select(r => new Resolution { width = r.width, height = r.height, refreshRateRatio = Screen.currentResolution.refreshRateRatio })
            .ToList();

        // Load options.
        if (FileExists)
        {
            LoadOptions();
            ApplyOptions(Options);

            return;
        }

        // Apply defaults.
        Debug.Log("No Options config file found. Applying defaults.");
        DefaultOptions();

        // Load video settings into options from Unity Player settings.
        Options.Video.DisplayMode = Screen.fullScreenMode;
        Options.Video.Resolution.Width = Screen.currentResolution.width;
        Options.Video.Resolution.Height = Screen.currentResolution.height;
        Options.Video.Vsync = QualitySettings.vSyncCount >= 1 ? true : false;

        ApplyOptions(Options, false);
    }

    #region Options Handlers

    #region Audio Options

    /// <summary>
    /// Set the master volume by percentage integer (0 to 100).
    /// </summary>
    public void SetMasterVolume(int value)
    {
        Options.Audio.MasterVolume = value;
        _audioManager.SetVolume(AudioManager.MASTER_CHANNEL, value);
    }

    /// <summary>
    /// Set the ambience volume by percentage integer (0 to 100).
    /// </summary>
    public void SetAmbienceVolume(int value)
    {
        Options.Audio.AmbienceVolume = value;
        _audioManager.SetVolume(AudioManager.AMBIENCE_CHANNEL, value);
    }

    /// <summary>
    /// Set the dialogue volume by percentage integer (0 to 100).
    /// </summary>
    public void SetDialogueVolume(int value)
    {
        Options.Audio.DialogueVolume = value;
        _audioManager.SetVolume(AudioManager.DIALOGUE_CHANNEL, value);
    }

    /// <summary>
    /// Set the music volume by percentage integer (0 to 100).
    /// </summary>
    public void SetMusicVolume(int value)
    {
        Options.Audio.MusicVolume = value;
        _audioManager.SetVolume(AudioManager.MUSIC_CHANNEL, value);
    }

    /// <summary>
    /// Set the sound effect volume by percentage integer (0 to 100).
    /// </summary>
    public void SetSFXVolume(int value)
    {
        Options.Audio.SFXVolume = value;
        _audioManager.SetVolume(AudioManager.SFX_CHANNEL, value);
    }

    #endregion

    #region Video Options

    /// <summary>
    /// Set the display mode from the options screen.
    /// 0 = Fullscreen,
    /// 1 = Borderless Windowed,
    /// 2 = Windowed
    /// </summary>
    public void SetDisplayMode(int value)
    {
        FullScreenMode fsMode = FullScreenMode.Windowed;

        if (value == 0 && Application.platform == RuntimePlatform.WindowsPlayer)
            fsMode = FullScreenMode.ExclusiveFullScreen;
        else if (value == 0 && Application.platform == RuntimePlatform.OSXPlayer)
            fsMode = FullScreenMode.MaximizedWindow;
        else if (value == 0 || value == 1)
            fsMode = FullScreenMode.FullScreenWindow;

        SetDisplayMode(fsMode);
    }

    /// <summary>
    /// Set the display mode.
    /// </summary>
    public void SetDisplayMode(FullScreenMode fsMode)
    {
        Options.Video.DisplayMode = fsMode;
        Screen.SetResolution(Options.Video.Resolution.Width, Options.Video.Resolution.Height, fsMode);
    }

    /// <summary>
    /// Set the resolution by index of available resolutions from the options screen.
    /// </summary>
    public void SetResolution(int value)
    {
        SetResolution(AvailableResolutions[value]);
    }

    /// <summary>
    /// Set the resolution from a resolution object.
    /// </summary>
    public void SetResolution(Resolution resolution)
    {
        SetResolution(resolution.width, resolution.height);
    }

    /// <summary>
    /// Set the resolution from an options object.
    /// </summary>
    public void SetResolution(Options.VideoOptions.ResolutionOptions resolution)
    {
        SetResolution(resolution.Width, resolution.Height);
    }

    /// <summary>
    /// Set the resolution by width and height.
    /// </summary>
    public void SetResolution(int width, int height)
    {
        Options.Video.Resolution.Width = width;
        Options.Video.Resolution.Height = height;

        Screen.SetResolution(width, height, Options.Video.DisplayMode);
    }

    public void SetVsync(bool vsync)
    {
        Options.Video.Vsync = vsync;
        QualitySettings.vSyncCount = vsync ? 1 : 0;
    }

    #endregion

    #endregion
}
