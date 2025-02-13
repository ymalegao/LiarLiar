using Unity.Netcode;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    public Camera playerCamera; // Assign in the prefab

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
