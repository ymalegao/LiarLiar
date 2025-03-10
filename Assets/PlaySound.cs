using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource audioSource; // Assign in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Change to whatever should trigger it
        {
            Debug.Log("Playing LAKE sound!");
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Change to whatever should trigger it
        {
            Debug.Log("Stopping LAKE sound!");
            audioSource.Stop();
        }
    }
}
