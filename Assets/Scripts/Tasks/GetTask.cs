using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTask : MonoBehaviour
{
  public string jobName; // Name of the task to assign player 
  public GameTask assignedTask; // The task assigned to this player
  public TaskManager TM;

  private Camera playerCamera;
  public GameObject minigameCamera;

  public GameTask GetAssignedTask()
  {
    return TM.AssignTaskToPlayer();
  }

  public void SetTask(GameTask task)
  {
    assignedTask = task;
    Debug.Log("Task assigned to player: " + task.position);
  }

  void Start()
  {
    // Initialize task manager
    this.TM = FindFirstObjectByType<TaskManager>();
    this.SetTask(this.GetAssignedTask());
    playerCamera = GetComponentInChildren<Camera>();
  }


  public void ActivatePlayerCamera()
  {
      minigameCamera = this.assignedTask.camera;
      if (playerCamera != null && minigameCamera != null)
      {
          playerCamera.gameObject.SetActive(true);

          minigameCamera.gameObject.SetActive(false);
      }
      else
      {
          Debug.LogError("Cameras not properly assigned in GetTask script.");
      }
  }

      public void ActivateMinigameCamera()
  {
      minigameCamera = this.assignedTask.camera;
      if (playerCamera != null && minigameCamera != null)
      {
          playerCamera.gameObject.SetActive(false);
          minigameCamera.gameObject.SetActive(true);
      }
      else
      {
          Debug.LogError("Cameras not properly assigned in GetTask script.");
      }
  }

  void Update() { }
}