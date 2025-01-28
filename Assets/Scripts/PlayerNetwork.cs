using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerNetwork : NetworkBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
{
        if (Input.GetKeyDown(KeyCode.Space))
        {
            randomNumber.Value = Random.Range(1, 100);
        }

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
