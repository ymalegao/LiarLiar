using Unity.Netcode;
using UnityEngine;
using Unity.Services.Authentication;

public class ClientMappingSender : NetworkBehaviour
{
  public override void OnNetworkSpawn()
  {
    // Only run this on the local (owning) client.
    if (!IsOwner) return;

    // Grab the local client ID and AuthID.
    ulong clientId = NetworkManager.Singleton.LocalClientId;
    string authId = AuthenticationService.Instance.PlayerId;
    // Call the server RPC to update the mapping.
    ServerManager.Instance.UpdateMappingServerRpc(clientId, authId);
  }
}
