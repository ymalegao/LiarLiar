using UnityEngine;
using Unity.Netcode;

namespace PowerUps
{
  public class PowerUpManager : NetworkBehaviour
  {
    public static PowerUpManager Instance { get; private set; }

    [SerializeField] private VisionEffect visionEffectPrefab;
    private static VisionEffect localVisionEffect;

    private void Awake()
    {
      Debug.Log("PowerUpManager Awake called");
      if (Instance == null)
      {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (IsServer || IsHost) // Only spawn if on server or host
        {
          NetworkObject netObj = GetComponent<NetworkObject>();
          if (netObj != null && !netObj.IsSpawned)
          {
            netObj.Spawn();
            Debug.Log("✅ PowerUpManager successfully spawned on network.");
          }
          else
          {
            Debug.LogError("❌ PowerUpManager is missing a NetworkObject component or is already spawned.");
          }
        }

        if (visionEffectPrefab != null)
        {
          localVisionEffect = Instantiate(visionEffectPrefab);
          DontDestroyOnLoad(localVisionEffect.gameObject);
        }
      }
      else
      {
        Destroy(gameObject);
      }
    }

    private void Update()
    {
      if (Instance == null)
      {
        Debug.LogError("PowerUpManager Instance is NULL in Update!");
        return;
      }

      // Check for input on both server and client
      if (false)
      {
        Debug.Log("F key pressed by " + (IsHost ? "host" : "client"));
        // Simply call the ServerRpc, we'll figure out who to apply the effect to on the server
        RequestApplyVisionReductionServerRpc();
      }
    }

    // ServerRpc to request applying vision reduction
    [ServerRpc(RequireOwnership = false)]
    public void RequestApplyVisionReductionServerRpc(ServerRpcParams rpcParams = default)
    {
      if (!IsServer)
      {
        Debug.LogError("❌ RequestApplyVisionReductionServerRpc called, but no server is active!");
        return;
      }

      // Get the sender's client ID from the RPC parameters
      ulong senderId = rpcParams.Receive.SenderClientId;
      Debug.Log($"✅ Server received vision reduction request from Player {senderId}");

      // Find all connected clients except the sender
      foreach (var client in NetworkManager.Singleton.ConnectedClients)
      {
        if (client.Key != senderId) // Exclude the player who initiated the request
        {
          Debug.Log($"Applying vision effect to Player {client.Key}");
          ApplyVisionReductionClientRpc(client.Key);
        }
      }
    }

    [ClientRpc]
    private void ApplyVisionReductionClientRpc(ulong targetPlayerId)
    {
      Debug.Log($"ClientRpc called for Player {targetPlayerId}. LocalClientId: {NetworkManager.Singleton.LocalClientId}");
      if (NetworkManager.Singleton.LocalClientId == targetPlayerId)
      {
        if (localVisionEffect != null)
        {
          localVisionEffect.ReduceVision();
          Debug.Log("Applying vision effect to local player");
        }
        else
        {
          Debug.LogError("VisionEffect is null!");
        }
      }
    }
  }
}
