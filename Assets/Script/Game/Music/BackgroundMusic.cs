using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance; // Singleton to keep only one music player

    private void Awake()
    {
        // If an instance already exists and it's not this, destroy this duplicate
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Otherwise, set this as the instance and persist it across scenes
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Play the audio if it's not already playing
        AudioSource audioSource = GetComponent<AudioSource>();
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
