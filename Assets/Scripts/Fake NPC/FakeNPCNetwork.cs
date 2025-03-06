using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class FakeNPCNetwork : NetworkBehaviour
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
    animator.SetFloat("npc_speed", moveVelocity.sqrMagnitude);


  }
  void FixedUpdate()
  {
    if (!IsOwner) return; // Only update the Rigidbody for the local player

    rb.velocity = moveVelocity;
  }
}
