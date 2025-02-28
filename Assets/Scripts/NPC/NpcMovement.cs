using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class NpcMovement : NetworkBehaviour
{
    private Vector3[] points; // Use Vector3[] instead of Transform[]
    [SerializeField] private float destPoint = 1.0f;
    private NavMeshAgent agent;
    private int currentPointIndex;
    private bool isStoppedForInteraction;
    [SerializeField] private Animator animator;
    private bool waypointsSet = false; // Flag to check if waypoints are set

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd += ResumeMovement;
        }

        // Wait for waypoints to be set before starting movement
        if (!waypointsSet)
        {
            Debug.LogWarning("Waiting for waypoints to be set...");
            return;
        }

        SetRandomDestination(); // Proceed if there are valid waypoints
    }

    private void OnDestroy()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd -= ResumeMovement;
        }
    }

    private void Update()
    {
        if (!waypointsSet || isStoppedForInteraction) return; // Skip updates if waypoints are not set or if stopped

        Vector2 movementDirection = new Vector2(agent.velocity.x, agent.velocity.y).normalized;

        // Pass direction to Animator
        animator.SetFloat("npc_horizontal", movementDirection.x);
        animator.SetFloat("npc_vertical", movementDirection.y);
        animator.SetFloat("npc_speed", agent.velocity.magnitude);

        if (!agent.pathPending && agent.remainingDistance <= destPoint)
        {
            SetRandomDestination();
        }
    }

    [ClientRpc]
    public void SyncWaypointsClientRpc(Vector3[] waypoints) // Use Vector3[] instead of Transform[]
    {
        Debug.Log($"Syncing waypoints: {waypoints.Length} waypoints received");
        SetWaypoints(waypoints);
    }

    private void SetRandomDestination()
    {
        if (points.Length == 0) return;

        // Pick a random index for the next destination
        int randomIndex = Random.Range(0, points.Length);

        // Ensure the NPC doesn't pick the same point consecutively
        while (randomIndex == currentPointIndex)
        {
            randomIndex = Random.Range(0, points.Length);
        }

        currentPointIndex = randomIndex;
        // Set the destination
        agent.SetDestination(points[currentPointIndex]);
    }

    public void StopMovement()
    {
        isStoppedForInteraction = true;
        agent.velocity = Vector3.zero;
        agent.isStopped = true; // Halts movement
        StopMovementClientRpc(); // Ensure all clients sync the stopped state
    }

    [ClientRpc]
    private void StopMovementClientRpc()
    {
        isStoppedForInteraction = true;
        agent.velocity = Vector3.zero;
        agent.isStopped = true; // Sync stop movement across all clients
    }

    public void ResumeMovement()
    {
        isStoppedForInteraction = false;
        agent.isStopped = false; // Resumes movement
        ResumeMovementClientRpc();
    }

    [ClientRpc]
    private void ResumeMovementClientRpc()
    {
        isStoppedForInteraction = false;
        agent.isStopped = false; // Sync resume movement across all clients
    }

    public void SetWaypoints(Vector3[] newWaypoints) // Use Vector3[] instead of Transform[]
    {
        if (newWaypoints == null || newWaypoints.Length == 0)
        {
            Debug.LogError("Waypoints are not assigned correctly!");
            return;
        }

        points = newWaypoints;
        waypointsSet = true; // Set the flag to true once waypoints are set

        if (agent != null && agent.isActiveAndEnabled)
        {
            SetRandomDestination(); // Start moving if waypoints are set and agent is active
        }
    }
}
