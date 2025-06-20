using UnityEngine;

public class BackgroundPlayer : MonoBehaviour
{
    public AudioClip[] musicTracks;
    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    void Start()
    {
        if (musicTracks.Length == 0)
        {
            Debug.LogWarning("No music tracks assigned!");
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        PlayNextTrack();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    void PlayNextTrack()
    {
        if (musicTracks.Length == 0) return;

        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.Play();

        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
    }
}
