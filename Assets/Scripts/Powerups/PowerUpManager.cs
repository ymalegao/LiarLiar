using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace PowerUps
{
  public class PowerUpManager : NetworkBehaviour
  {
    public static PowerUpManager Instance { get; private set; }

    [SerializeField] private VisionEffect visionEffectPrefab;
    private static VisionEffect localVisionEffect;

    // Store connected client IDs
    private List<ulong> connectedClientIds = new List<ulong>();

    private void Awake()
    {
      Debug.Log("PowerUpManager Awake called");
      if (Instance == null)
      {
        Instance = this;
        DontDestroyOnLoad(gameObject);

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

    private void Start()
    {
      Debug.Log("PowerUpManager Start called");
      Debug.Log($"IsServer: {IsServer}, IsHost: {IsHost}");
      if (IsServer || IsHost) // Only collect clients if on server or host
      {
        CollectConnectedClients();
      }
    }

    // Collect connected clients and send to clients
    private void CollectConnectedClients()
    {
      connectedClientIds.Clear();
      foreach (var client in NetworkManager.Singleton.ConnectedClients)
      {
        connectedClientIds.Add(client.Key);
      }
      SendConnectedClientsToClientsClientRpc(connectedClientIds.ToArray());
    }

    [ClientRpc]
    private void SendConnectedClientsToClientsClientRpc(ulong[] clientIds)
    {
      connectedClientIds = new List<ulong>(clientIds);
      Debug.Log($"Connected client IDs: {string.Join(", ", connectedClientIds)}");
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

        // Get the other seeker ID from the stored list
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
      foreach (var clientId in connectedClientIds)
      {
        if (clientId != NetworkManager.Singleton.LocalClientId) // Exclude the local player
        {
          Debug.Log($"Found other seeker: {clientId}");
          return clientId; // Return the first other seeker found
        }
      }
      return 0; // Return 0 if no other seeker is found
    }
  }
}