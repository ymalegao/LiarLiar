using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTask : MonoBehaviour
{
  public string jobName; // Name of the task to assign player 
  public GameTask assignedTask; // The task assigned to this player
  public TaskManager TM;

  public GameTask GetAssignedTask()
  {
    return TM.AssignTaskToPlayer();
  }

  public void SetTask(GameTask task)
  {
    assignedTask = task;
  }

  void Start()
  {
    // Initialize task manager
    this.TM = FindFirstObjectByType<TaskManager>();
    this.SetTask(this.GetAssignedTask());
  }

  void Update() { }
}