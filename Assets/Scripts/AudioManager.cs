using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public const string MASTER_CHANNEL = "Master";
    public const string AMBIENCE_CHANNEL = "Ambience";
    public const string DIALOGUE_CHANNEL = "Dialogue";
    public const string MUSIC_CHANNEL = "Music";
    public const string SFX_CHANNEL = "SFX";

    public const string VOLUME_KEY = "Volume";

    [Header("Audio References")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _dialogueSource;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    private readonly Dictionary<string, float> _volumeMap = new() {
        { MASTER_CHANNEL, 0 },
        { AMBIENCE_CHANNEL, 0 },
        { DIALOGUE_CHANNEL, 0 },
        { MUSIC_CHANNEL, 0 },
        { SFX_CHANNEL, 0 },
    };

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Cache originally configured volume values.
        LoadMixerVolumes();
    }

    private void Start()
    {
        // Re-set mixer volumes after options data has been loaded.
        foreach (var key in _volumeMap.Keys.ToList())
            SetVolume(key, _volumeMap[key]);
    }

    /// <summary>
    /// Play a single (non-3D) sound effect on the dialogue channel.
    /// </summary>
    public void PlayDialogue(AudioClip sound)
    {
        _dialogueSource.PlayOneShot(sound);
    }

    /// <summary>
    /// Play a single (non-3D) sound effect.
    /// </summary>
    public void PlaySound(AudioClip sound)
    {
        _sfxSource.PlayOneShot(sound);
    }

    /// <summary>
    /// Set the volume for a channel by percentage (0.0 to 1.0).
    /// </summary>
    public void SetVolume(string channelKey, float value)
    {
        _volumeMap[channelKey] = Mathf.Clamp(value, 0.001f, 1);
        _audioMixer.SetFloat($"{channelKey}{VOLUME_KEY}", ConvertLevelToDb(_volumeMap[channelKey]));
    }

    /// <summary>
    /// Set the volume for a channel by percentage integer (0 to 100).
    /// </summary>
    public void SetVolume(string channelKey, int value)
    {
        SetVolume(channelKey, (float)value / 100);
    }

    /// <summary>
    /// Convert a decibel to a level percentage.
    /// </summary>
    private float ConvertDbToLevel(float db)
    {
        return Mathf.Pow(10, db / 20.0f);
    }

    /// <summary>
    /// Convert a level percentage to decibels.
    /// </summary>
    private float ConvertLevelToDb(float level)
    {
        return Mathf.Log10(level) * 20.0f;
    }

    /// <summary>
    /// Load volume levels from the audio mixer and set the appropriate level.
    /// </summary>
    private void LoadMixerVolumes()
    {
        foreach (var key in _volumeMap.Keys.ToList())
        {
            _audioMixer.GetFloat($"{key}{VOLUME_KEY}", out var mixerVolume);
            _volumeMap[key] = ConvertDbToLevel(mixerVolume);
        }
    }
}
