using UnityEngine;
using Unity.Netcode;

public class FakeNPCMovement : NetworkBehaviour
{
  [SerializeField] private float speed = 3.0f;
  private Vector2 movementInput;
  private Rigidbody2D rb;
  private Animator animator;

  private void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();

    if (!IsOwner) // Ensures only the player controlling this FakeNPC can move it
    {
      enabled = false;
    }
  }

  private void Update()
  {
    if (!IsOwner) return;

    movementInput.x = Input.GetAxisRaw("Horizontal");
    movementInput.y = Input.GetAxisRaw("Vertical");

    animator.SetFloat("npc_horizontal", movementInput.x);
    animator.SetFloat("npc_vertical", movementInput.y);
    animator.SetFloat("npc_speed", movementInput.magnitude);
  }

  private void FixedUpdate()
  {
    if (!IsOwner) return;

    rb.velocity = movementInput.normalized * speed;
  }
}
