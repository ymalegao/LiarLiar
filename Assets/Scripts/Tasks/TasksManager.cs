using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    //List of Tasks in the game. Add and modify this dictionary to create and edit tasks.
    //public GameObject ...; for atri        
    public Dictionary<string, GameTask> tasks = new Dictionary<string, GameTask>(){
        {"fish", new GameTask("fish", null,new Vector2(0f,0f))},
        {"blacksmith", new GameTask("blacksmith", null,new Vector2(-10f,5f))}
    };
    private void Awake(){

    }

    void Start()
    {
        tasks["fish"].canvas = GameObject.Find("FishCanvas");
        tasks["blacksmith"].canvas = GameObject.Find("FishCanvas");

        //Disable all canvas objects.
        foreach( KeyValuePair<string, GameTask> task in  tasks ){
            task.Value.canvas.SetActive(false);
        }
    }


    // Assign tasks to players
    public GameTask AssignTaskToPlayer()
    {
        int randomIndex = Random.Range(0, tasks.Count);
        Debug.Log("Task assigned to player  " + tasks.ElementAt(randomIndex));
        return(tasks.ElementAt(randomIndex).Value); //return random task
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

                if (Input.GetKeyDown(KeyCode.Return) && !task.isActive){
                    task.isActive = true;
                    Debug.Log($"Task started: {task.name}");
                }
                
                if (task.isActive)
                {
                    //Spawn Minigame Canvas
                    if(!task.canvas.activeSelf){
                        task.canvas.SetActive(true);
                    }

                    task.taskTimer += Time.deltaTime;

                    if (task.taskTimer >= task.completionTime)
                    {
                        Debug.Log($"Task completed at {task.position} by {name}");
                        task.taskTimer = 0f;
                        task.isActive = false; // Reset flag after completion
                        if(task.canvas.activeSelf){
                            task.canvas.SetActive(false);
                        }
                    }
                }
            }
            else{
                // Reset the timer if the player moves away before starting
                task.taskTimer = 0f;
                task.isActive = false;
            }
        }
    }

    // void OnDrawGizmos()
    // {
    //     // Visualize task locations in the editor
    //     tasks = new Dictionary<string, GameTask>(){
    //         {"fish", new GameTask("fish", GameObject.Find("FishCanvas"),new Vector2(0f,0f))},
    //         {"blacksmith", new GameTask("blacksmith", GameObject.Find("FishCanvas"),new Vector2(-10f,5f))}
    //     };
    //     Gizmos.color = Color.green;
    //     foreach (var (name,task) in tasks)
    //     {
    //         Gizmos.DrawWireSphere(task.position, task.completionRadius);
    //     }
    // }
}