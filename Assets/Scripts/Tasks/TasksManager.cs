using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    //List of Tasks in the game. Add and modify this dictionary to create and edit tasks. 
    public Dictionary<string, GameTask> tasks = new Dictionary<string, GameTask>(){
        {"fish", new GameTask("fish", new Vector2(0f,0f))},
        {"blacksmith", new GameTask("blacksmith", new Vector2(-10f,5f))}
    };

    void Start()
    {
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

            if (task == null || task.isCompleted) continue;

            float distance = Vector2.Distance(player.transform.position, task.position);

            if (distance <= task.completionRadius)
            {
                task.taskTimer += Time.deltaTime;

                if (task.taskTimer >= task.completionTime)
                {
                    task.isCompleted = true;
                    Debug.Log("Task completed by player: " + player.name + " at location: " + task.position);
                }
            }
            else
            {
                // Reset the timer if the player moves out of the task area
                task.taskTimer = 0f;
            }
        }
    }

    void OnDrawGizmos()
    {
        // Visualize task locations in the editor
        Gizmos.color = Color.green;
        foreach (var (name,task) in tasks)
        {
            Gizmos.DrawWireSphere(task.position, task.completionRadius);
        }
    }
}