using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcMovement : MonoBehaviour
{

  //[SerializeField] private Transform target;

  private Transform[] points;
  [SerializeField] private float destPoint = 1.0f;

  private NavMeshAgent agent;
  private int currentPointIndex;
  private bool isStoppedForInteraction;
  [SerializeField] private Animator animator;

  // Start is called before the first frame update
  void Start()
  {
    agent = GetComponent<NavMeshAgent>();
    agent.updateRotation = false;
    agent.updateUpAxis = false;

    if (points.Length > 0)
    {
      SetRandomDestination();
    }
    else
    {
      Debug.LogWarning("No where to go!");
    }

    if (DialogueManager.Instance != null)
    {
      DialogueManager.Instance.OnDialogueEnd += ResumeMovement;
    }
  }

  private void OnDestroy()
  {
    // Unsubscribe from the DialogueManager's OnDialogueEnd event to avoid memory leaks
    if (DialogueManager.Instance != null)
    {
      DialogueManager.Instance.OnDialogueEnd -= ResumeMovement;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (isStoppedForInteraction) return; // Skip updates if stopped

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
        // Debug.Log("Current Point Index: " + currentPointIndex);

        // Set the destination

        Debug.Log($"{gameObject.name} moving to waypoint {currentPointIndex}: {points[currentPointIndex].position}");

        agent.SetDestination(points[currentPointIndex].position);
  }

  public void StopMovement()
  {
    Debug.Log("Stop the character");

    isStoppedForInteraction = true;
    //set NPC velocity to 0
    agent.velocity = Vector3.zero;
    agent.isStopped = true; // Halts movement
  }

  public void ResumeMovement()
  {
    isStoppedForInteraction = false;
    agent.isStopped = false; // Resumes movement
    Debug.Log("Resume Play\n\n");
  }

  public void SetWaypoints(Transform[] newWaypoints)
  {
    points = newWaypoints;
  }

}
