using UnityEngine;
using System.Collections.Generic;

public class AudioZoneTrigger : MonoBehaviour
{
    private readonly List<AudioSource> _audioSourcesInZone = new List<AudioSource>();

    private void Start()
    {
        // Find all AudioSources within the trigger zone when the scene starts and disable them.
        InitializeAudioSources();
        DisableAudioSources();
    }

    private void InitializeAudioSources()
    {
        _audioSourcesInZone.Clear();
        Collider[] colliders = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents, transform.rotation);

        foreach (var col in colliders)
        {
            AudioSource audioSource = col.GetComponent<AudioSource>();

            if (audioSource != null && !_audioSourcesInZone.Contains(audioSource))
                _audioSourcesInZone.Add(audioSource);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            EnableAudioSources();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            DisableAudioSources();
    }

    private void DisableAudioSources()
    {
        foreach (var audioSource in _audioSourcesInZone)
        {
            if (audioSource != null)
                audioSource.enabled = false;
        }
    }

    private void EnableAudioSources()
    {
        foreach (var audioSource in _audioSourcesInZone)
        {
            if (audioSource != null)
                audioSource.enabled = true;
        }
    }
}
