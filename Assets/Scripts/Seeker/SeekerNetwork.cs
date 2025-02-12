using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class SeekerNetwork : NetworkBehaviour
{

  private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

  //Can also create custom data using a struct
  //This can we used as a NetworkVariable instead of <int> use <PlayerData>
  //Need to serialize any custom data
  public struct PlayerData
  {
    public int health;
    public int score;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
      serializer.SerializeValue(ref health);
      serializer.SerializeValue(ref score);
    }

  }

  public override void OnNetworkSpawn()
  {
    randomNumber.OnValueChanged += (int previousValue, int newValue) =>
    {
      Debug.Log(OwnerClientId + "; Random number changed from " + previousValue + " to " + newValue);
    };
  }


  [SerializeField] private float speed = 5f;
  private Rigidbody2D rb;

  private Vector2 moveVelocity;
  public Animator animator;

  // Start is called before the first frame update
  void Start()
  {
    rb = GetComponent<Rigidbody2D>();

  }

  void Update()
  {
    if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
    {
      rb.velocity = Vector2.zero; // Stop movement
      return; // Skip movement logic
    }
    if (Input.GetKeyDown(KeyCode.Space))
    {
      randomNumber.Value = Random.Range(1, 100);
    }

    if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
    {
      if (!IsOwner) return; // Only process movement for the owning player
    }

    float moveX = Input.GetAxis("Horizontal");
    float moveY = Input.GetAxis("Vertical");

    moveVelocity = new Vector2(moveX, moveY).normalized * speed;

    animator.SetFloat("npc_horizontal", moveX);
    animator.SetFloat("npc_vertical", moveY);
    if (moveVelocity.sqrMagnitude != 0)
    {
      //Debug.Log(moveVelocity.sqrMagnitude);
    }

    animator.SetFloat("npc_speed", moveVelocity.sqrMagnitude);


  }
  void FixedUpdate()
  {
    if (!IsOwner) return; // Only update the Rigidbody for the local player

    rb.velocity = moveVelocity;
  }
}
