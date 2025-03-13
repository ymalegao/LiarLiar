using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetTask : MonoBehaviour
{
  public string jobName; // Name of the task to assign player 
  public GameTask assignedTask; // The task assigned to this player
  public TaskManager TM;

  private Camera playerCamera;
  public GameObject minigameCamera;

  public TMP_Text TaskText;

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
    playerCamera = GetComponentInChildren<Camera>();
    TaskText = GameObject.Find("TaskText").GetComponent<TMP_Text>();
    TaskText.text = "Task: " + this.assignedTask.name;
  }


  public void ActivatePlayerCamera()
  {
      if(playerCamera.gameObject.activeSelf){
        return;
      }
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
          switch(assignedTask.name.ToLower())
          {
              case "fish":
                  // fishingManager.SetActive(true);
                  break;
              case "egg":
                  EggSpawner.Instance.SetPlayer(this);
                  break;
              case "wood":
                  // woodManager.SetActive(true);
                  break;
          }
      }
      else
      {
          Debug.LogError("Cameras not properly assigned in GetTask script.");
      }
  }

  void Update() { }
}