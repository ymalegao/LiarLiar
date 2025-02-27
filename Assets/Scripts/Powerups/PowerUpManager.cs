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

        if (visionEffectPrefab != null)
        {
          Debug.Log("VisionEffect prefab found, creating instance");
          localVisionEffect = Instantiate(visionEffectPrefab);
          DontDestroyOnLoad(localVisionEffect.gameObject);
          if (localVisionEffect != null)
          {
            Debug.Log("VisionEffect instance created successfully");
          }
          else
          {
            Debug.LogError("Failed to create VisionEffect instance");
          }
        }
        else
        {
          Debug.LogError("VisionEffect prefab is null!");
        }
      }
      else
      {
        Debug.Log("Destroying duplicate PowerUpManager");
        Destroy(gameObject);
      }
    }

    private void Update()
    {
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
      // Call the ClientRpc on the target player
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