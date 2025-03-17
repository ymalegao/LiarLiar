using UnityEngine;
using System.Collections.Generic;

public class PlaySound : MonoBehaviour
{
    public AudioSource audioSource;
    private HashSet<GameObject> playersInTrigger = new HashSet<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInTrigger.Add(other.gameObject);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInTrigger.Remove(other.gameObject);
            if (playersInTrigger.Count == 0)
            {
                audioSource.Stop();
            }
        }
    }
}
