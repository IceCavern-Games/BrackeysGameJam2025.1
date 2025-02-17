using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void PlayDialogue(AudioClip sound)
    {
        // @TODO
    }
}
