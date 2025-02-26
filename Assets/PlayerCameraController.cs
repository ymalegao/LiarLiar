using Unity.Netcode;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    public Camera playerCamera; // Assign in the prefab
    public Transform playerTransform; // Assign in the prefab
    public Vector2 minBounds; // Set this in the Inspector
    public Vector2 maxBounds; // Set this in the Inspector

    private void LateUpdate()
    {
        if (!IsOwner || playerTransform == null) return;

        Vector3 newPosition = playerTransform.position;
        newPosition.z = playerCamera.transform.position.z; // Keep the camera's Z position

        // Clamp the position to keep it within bounds
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        playerCamera.transform.position = newPosition;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner) // Only enable for the local player
        {
            playerCamera.gameObject.SetActive(true);
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
        }
    }
}
