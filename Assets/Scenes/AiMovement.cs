using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiMovement : MonoBehaviour
{
    // public GameObject player;

    //[SerializeField] private float speed = 5f;

    [SerializeField] Transform target;

    NavMeshAgent agent;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {

        agent.SetDestination(target.position);

        //float distance = Vector2.Distance(transform.position, player.transform.position);

    }
}
