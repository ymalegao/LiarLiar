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
      if (Input.GetKeyDown(KeyCode.F))
      {
        Debug.Log("F key pressed on client");
        ulong otherSeekerId = GetOtherSeekerId(); // Get the other seeker's ID
        if (otherSeekerId != 0) // Ensure we found a valid ID
        {
          Debug.Log($"F pressed. LocalClientId: {NetworkManager.Singleton.LocalClientId}, Targeting Player: {otherSeekerId}");
          ApplyVisionReductionServerRpc(otherSeekerId); // Apply effect to the other player
        }
        else
        {
          Debug.LogWarning("No other seeker found to apply vision reduction.");
        }
      }
    }

    // ServerRpc to apply vision reduction to the target player
    [ServerRpc(RequireOwnership = false)]
    public void ApplyVisionReductionServerRpc(ulong targetPlayerId)
    {
      if (!IsServer)
      {
        Debug.LogError("❌ ApplyVisionReductionServerRpc called, but no server is active!");
        return;
      }

      Debug.Log($"✅ Server is active. Applying vision effect to Player {targetPlayerId}");
      ApplyVisionReductionClientRpc(targetPlayerId);
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

    private ulong GetOtherSeekerId()
    {
      foreach (var client in NetworkManager.Singleton.ConnectedClients)
      {
        if (client.Key != NetworkManager.Singleton.LocalClientId) // Exclude the local player
        {
          Debug.Log($"Found other seeker: {client.Key}");
          return client.Key; // Return the first other seeker found
        }
      }
      return 0; // Return 0 if no other seeker is found
    }
  }
}