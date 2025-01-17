using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcMovement : MonoBehaviour
{
    [SerializeField] private Transform target;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Find target dynamically if not assigned
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("Assigned target dynamically: " + target.name);
            }
            else
            {
                Debug.LogWarning("No Player object found in the scene!");
            }
        }
    }

    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            Debug.LogWarning("Target is null, cannot set destination.");
        }
    }

    public void AssignTarget(Transform playerTransform)
    {
        target = playerTransform;
        Debug.Log("Assigned target via method: " + target.name);
    }
}
