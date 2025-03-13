using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{


  public GameObject fishingManager;
  public GameObject woodManager;
  public GameObject eggManager;

  public GameObject fishingCamera;
  public GameObject eggCamera;
    
  //List of Tasks in the game. Add and modify this dictionary to create and edit tasks.
  //public GameObject ...; for atri        
  public Dictionary<string, GameTask> tasks;
  private void Awake()
  {
    tasks = new Dictionary<string, GameTask>(){
        {"fish", new GameTask("fish", fishingManager, fishingCamera, new Vector2(4f,10f))},
        // {"wood", new GameTask("wood", woodManager, new Vector2(-11f,33f))},
        {"egg", new GameTask("egg", eggManager, eggCamera, new Vector2(90f,60f), 5.0f, 100.0f)}
    };
  }

  void Start()
  {
      //Disable all gameManager objects.
    //   foreach( KeyValuePair<string, GameTask> task in  tasks ){
    //       task.Value.gameManager.SetActive(false);
    //   }

  }


  // Assign tasks to players
  public GameTask AssignTaskToPlayer()
  {
    int randomIndex = Random.Range(0, tasks.Count);
    Debug.Log("Task assigned to player  " + tasks.ElementAt(randomIndex));
    return (tasks.ElementAt(randomIndex).Value); //return random task
    // return (tasks["fish"]);

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
          Debug.Log($"Task started: {task.name}");
        }

        if (task.isActive)
        {
          task.gameManager.SetActive(false);
          task.gameManager.SetActive(true);
          player.ActivateMinigameCamera();

          task.taskTimer += Time.deltaTime;
          if (task.taskTimer >= task.completionTime)
          {
            Debug.Log($"Task completed at {task.position} by {name}");
            task.taskTimer = 0f;
            task.isActive = false; // Reset flag after completion
            if (task.gameManager.activeSelf)
            {
              task.gameManager.SetActive(false);
            }
            player.ActivatePlayerCamera();
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
        {"fish", new GameTask("fish", fishingManager,fishingCamera,  new Vector2(4f,10f))},
        // {"wood", new GameTask("wood",  woodManager, new Vector2(-11f,33f))},
        {"egg", new GameTask("egg", eggManager, eggCamera, new Vector2(90f,60f))}
    };
      // Visualize task locations in the editor
      Gizmos.color = Color.green;
      foreach (var (name,task) in tasks)
      {
          Gizmos.DrawWireSphere(task.position, task.completionRadius);
      }
  }
}
