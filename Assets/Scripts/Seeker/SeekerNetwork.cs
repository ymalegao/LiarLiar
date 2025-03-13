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

  [SerializeField] private float speed = 5f;
  [SerializeField] private Vector2 spawnPosition = new Vector2(5f, 5f);

  private Rigidbody2D rb;

  private Vector2 moveVelocity;
  public Animator animator;

    // Add these for the footstep audio
    private AudioSource audioSource;
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private float footstepInterval = 0.5f; // How often the footstep sound plays
    private float lastFootstepTime = 0f;


    // Start is called before the first frame update
    void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    audioSource = GetComponent<AudioSource>();

        if (IsOwner)
    {
      transform.position = spawnPosition;
    }

        if (footstepClip != null)
        {
            audioSource.clip = footstepClip;
        }

    }

  void Update()
  {
    if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
    {
      moveVelocity = Vector2.zero; // Reset movement input
      rb.velocity = Vector2.zero;  // Ensure movement stops
      animator.SetFloat("npc_speed", 0f);
      Debug.Log("Dialogue is active");
      return; // Skip movement logic
    }

    if (Input.GetKeyDown(KeyCode.Space))
    {
      randomNumber.Value = Random.Range(1, 100);
    }

    if (Input.GetKeyDown(KeyCode.F))
    {
      Debug.Log("F key pressed in SeekerNetwork");
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

        // Play footsteps if player is moving
        if (moveVelocity.sqrMagnitude > 0 && Time.time - lastFootstepTime > footstepInterval)
        {
            PlayFootsteps();
        }
        else if (moveVelocity.sqrMagnitude == 0)
        {
            StopFootsteps();
        }
    }
  void FixedUpdate()
  {
    if (!IsOwner) return; // Only update the Rigidbody for the local player

    rb.velocity = moveVelocity;
  }

    private void PlayFootsteps()
    {
        if (!audioSource.isPlaying) // Prevent overlapping footstep sounds
        {
            audioSource.Play();
        }
        lastFootstepTime = Time.time; // Update the time of the last footstep played
    }

    private void StopFootsteps()
    {
        if (audioSource.isPlaying) // Stop audio when the player stops moving
        {
            audioSource.Stop();
        }
    }
}
