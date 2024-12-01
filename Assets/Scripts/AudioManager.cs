using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton instance
    public AudioSource audioSource; // AudioSource attached to this GameObject

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevent destruction on scene change
        }
        else
        {
            Destroy(gameObject); // Ensure only one AudioManager exists
        }
    }

    private void Start()
    {
        // Ensure the AudioSource is attached to this GameObject
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component missing from AudioManager GameObject.");
            }
        }
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
        else
        {
            Debug.LogError("AudioSource is not set on the AudioManager.");
        }
    }

    public float GetVolume()
    {
        return audioSource != null ? audioSource.volume : 1f; // Return default volume if audioSource is null
    }
}