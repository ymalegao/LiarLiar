using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioClip audioClip; // Assign in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSource playerAudioSource = other.GetComponent<AudioSource>();

            if (playerAudioSource != null && !playerAudioSource.isPlaying)
            {
                playerAudioSource.clip = audioClip; // Assign clip if needed
                playerAudioSource.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSource playerAudioSource = other.GetComponent<AudioSource>();

            if (playerAudioSource != null)
            {
                playerAudioSource.Stop();
            }
        }
    }
}
