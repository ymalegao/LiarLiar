using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D rb;

    private Vector2 moveVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
{
    // Allow movement if the player is the owner or if the network is not running
    if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
    {
        if (!IsOwner) return; // Only process movement for the owning player
    }

    moveVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
}


    void FixedUpdate()
    {

        if (!IsOwner) return; // Only update the Rigidbody for the local player

        rb.velocity = moveVelocity * speed;
    }
}
