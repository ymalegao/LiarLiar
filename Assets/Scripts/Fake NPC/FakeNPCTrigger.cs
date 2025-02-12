using UnityEngine;
using Unity.Netcode;

public class FakeNPCTrigger : NetworkBehaviour
{
  public float interactionRange = 1.5f;
  public LayerMask interactableLayer;

  private void Update()
  {
    if (!IsOwner) return;

    if (Input.GetKeyDown(KeyCode.E))
    {
      CheckForMiniGame();
    }
  }

  private void CheckForMiniGame()
  {
    Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, interactableLayer);

    if (hit != null)
    {
      Debug.Log($"FakeNPC triggered mini-game at: {hit.name}");
      // MiniGameManager.Instance.StartMiniGameServerRpc();
    }
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, interactionRange);
  }
}
