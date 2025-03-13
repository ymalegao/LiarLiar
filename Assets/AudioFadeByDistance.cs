using UnityEngine;

public class AudioFadeByDistance : MonoBehaviour
{
    private Transform player;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] public float minVolume = 0f;
    [SerializeField] private float maxDistance = 20f;

    private void Update()
    {
        // Keep searching for the player if it's not found yet
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                return; // Exit early if player is still not found
            }
        }

        // Adjust volume based on player's distance
        float distance = Vector2.Distance(transform.position, player.position);
        float volume = Mathf.Clamp01(1 - (distance / maxDistance)) * maxVolume;

        Debug.Log("Distance: " + distance + " Volume: " + volume);

        audioSource.volume = Mathf.Lerp(audioSource.volume, volume, Time.deltaTime * 5f); // Smooth fading
    }
}
