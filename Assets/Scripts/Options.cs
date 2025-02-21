using System;
using UnityEngine;

/// <summary>
/// Serializable representation of the game's config/options.
/// </summary>
[Serializable]
public class Options
{
    [Serializable]
    public class AudioOptions
    {
        public int MasterVolume = 100;
        public int AmbienceVolume = 100;
        public int DialogueVolume = 100;
        public int MusicVolume = 100;
        public int SFXVolume = 100;
    }

    // [Serializable]
    // public class InputOptions
    // {

    // }

    [Serializable]
    public class VideoOptions
    {
        [Serializable]
        public class ResolutionOptions
        {
            public int Height;
            public int Width;
        }

        public FullScreenMode DisplayMode;
        public ResolutionOptions Resolution;
    }

    public AudioOptions Audio;
    // public InputOptions Input;
    public VideoOptions Video;
}
