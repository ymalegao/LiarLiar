using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{


  public GameObject fishingManager;
  public GameObject woodManager;
  public GameObject eggManager;
    
  //List of Tasks in the game. Add and modify this dictionary to create and edit tasks.
  //public GameObject ...; for atri        
  public Dictionary<string, GameTask> tasks;
  private void Awake()
  {
    tasks = new Dictionary<string, GameTask>(){
        {"fish", new GameTask("fish", fishingManager, new Vector2(4f,10f))},
        {"wood", new GameTask("wood", woodManager, new Vector2(-11f,33f))},
        {"egg", new GameTask("egg", eggManager, new Vector2(90f,60f))}
    };
  }

  void Start()
  {

  }


  // Assign tasks to players
  public GameTask AssignTaskToPlayer()
  {

    return (tasks["fish"]);

  }

  void Update()
  {
    GetTask[] players = FindObjectsOfType<GetTask>();
    foreach (var player in players)
    {
      GameTask task = player.assignedTask;

      if (task == null) continue;

      float distance = Vector2.Distance(player.transform.position, task.position);

      if (distance <= task.completionRadius)
      {

        if (Input.GetKeyDown(KeyCode.Return) && !task.isActive)
        {
          task.isActive = true;
        }

        if (task.isActive)
        {
          //Spawn Minigame gameManager
          if (!task.gameManager.activeSelf)
          {
            task.gameManager.SetActive(true);
          }

          task.taskTimer += Time.deltaTime;

          if (task.taskTimer >= task.completionTime)
          {
            task.taskTimer = 0f;
            task.isActive = false; // Reset flag after completion
            if (task.gameManager.activeSelf)
            {
              task.gameManager.SetActive(false);
            }
          }
        }
      }
      else
      {
        // Reset the timer if the player moves away before starting
        task.taskTimer = 0f;
        task.isActive = false;
      }
    }
  }

  void OnDrawGizmos()
  {
        tasks = new Dictionary<string, GameTask>(){
        {"fish", new GameTask("fish", fishingManager, new Vector2(4f,10f))},
        {"wood", new GameTask("wood",  woodManager, new Vector2(-11f,33f))},
        {"egg", new GameTask("egg", eggManager, new Vector2(90f,60f))}
    };
      // Visualize task locations in the editor
      Gizmos.color = Color.green;
      foreach (var (name,task) in tasks)
      {
          Gizmos.DrawWireSphere(task.position, task.completionRadius);
      }
  }
}
