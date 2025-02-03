// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.Netcode;
// using Unity.Services.Authentication;
// using Unity.Collections; // ✅ Required for FixedString

// public class SetSprite : NetworkBehaviour
// {
//     public SpriteRenderer spriteRenderer;
//     public Sprite[] characterSprites;

//     private NetworkVariable<FixedString128Bytes> selectedCharacter = new NetworkVariable<FixedString128Bytes>(
//         new FixedString128Bytes("DefaultCharacter"), 
//         NetworkVariableReadPermission.Everyone, 
//         NetworkVariableWritePermission.Owner
//     );

//     public override void OnNetworkSpawn()
//     {
//         if (IsOwner)
//         {
//             // Get the character from the lobby
//             string character = LobbyManager.Instance.GetPlayerCharacter(AuthenticationService.Instance.PlayerId);
//             selectedCharacter.Value = new FixedString128Bytes(character); // ✅ Assign as FixedString128Bytes
//             Debug.Log("🟢 Selected Character: " + selectedCharacter.Value);
//         }

//         // Apply the character when spawned
//         ApplyCharacter(selectedCharacter.Value.ToString());

//         // Subscribe to changes
//         selectedCharacter.OnValueChanged += (oldValue, newValue) =>
//         {
//             ApplyCharacter(newValue.ToString());
//         };
//     }

//     private void ApplyCharacter(string characterName)
//     {
//         if (spriteRenderer == null)
//         {
//             spriteRenderer = GetComponent<SpriteRenderer>();
//         }

//         foreach (Sprite sprite in characterSprites)
//         {
//             if (sprite.name == characterName)
//             {
//                 spriteRenderer.sprite = sprite;
                
//                 Debug.Log("✅ Applied Sprite: " + characterName);
//                 return;
//             }
//         }

//         Debug.LogWarning("⚠️ Sprite not found: " + characterName);
//     }
// }
