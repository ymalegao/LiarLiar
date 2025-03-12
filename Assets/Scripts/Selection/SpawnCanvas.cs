using UnityEngine;
using Unity.Netcode;

public class NetworkCanvasController : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // Ensure this canvas is visible for all clients
        gameObject.SetActive(true);
        Debug.Log($"Canvas activated on {(IsHost ? "Host" : "Client")}");
    }
}
