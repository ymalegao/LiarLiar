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
      if (Input.GetKeyDown(KeyCode.F))
      {
        Debug.Log($"F pressed. LocalClientId: {NetworkManager.Singleton.LocalClientId}, VisionEffect null? {localVisionEffect == null}");
        ApplyVisionReductionServerRpc(NetworkManager.Singleton.LocalClientId);
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