using UnityEngine;

public class AudioProximity : MonoBehaviour
{
    private AudioSource audioSource;
    private BoxCollider2D lakeCollider;

    public float maxDistance = 5f; // Distance at which audio is at minVolume
    public float minVolume = 0.1f;
    public float maxVolume = 1.0f;
    private bool isAudioPlaying = false;

    private Transform player;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lakeCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Find the player if not already assigned
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                // Debug.Log("Player found: " + player.name);
            }
            else
            {
                Debug.LogWarning("Player not found yet...");
                return; // Skip rest of Update if player isn't found
            }
        }

        // Find the closest point on the lake's collider
        Vector2 closestPoint = lakeCollider.ClosestPoint(player.position);
        Debug.DrawLine(player.position, closestPoint, Color.green); // Visualize distance in Scene View
        // Debug.Log("Closest point on lake: " + closestPoint);

        // Measure distance from the closest edge of the lake
        float distance = Vector2.Distance(closestPoint, player.position);
        // Debug.Log("Player Distance from Lake: " + distance);

        // Adjust volume based on distance
        float volume = Mathf.Lerp(maxVolume, minVolume, distance / maxDistance);
        volume = Mathf.Clamp(volume, minVolume, maxVolume);

        audioSource.volume = volume;
        Debug.Log("Audio Volume Set To: " + volume);

        if (distance <= maxDistance && !isAudioPlaying)
        {
            // Start playing the audio when close enough
            audioSource.Play();
            isAudioPlaying = true;
            // Debug.Log("Audio Started: Player is within proximity.");
        }
        else if (distance > maxDistance && isAudioPlaying)
        {
            // Stop audio when player moves away
            audioSource.Stop();
            isAudioPlaying = false;
            // Debug.Log("Audio Stopped: Player is too far away.");
        }

    }
}
