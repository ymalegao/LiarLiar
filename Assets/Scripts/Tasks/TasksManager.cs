using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    //List of Tasks in the game. Add and modify this dictionary to create and edit tasks. 
    public Dictionary<string, Task> tasks = new Dictionary<string, Task>(){
        {"fish", new Task("fish", new Vector2(0f,0f))}
    };

    void Start()
    {
    }


    // Assign tasks to players
    public Task AssignTaskToPlayer(string  taskName)
    {

        return(tasks[taskName]); //return the task that matches the player
        Debug.Log("Tasks assigned to player  " + taskName);
    }

    void Update()
    {
        GetTask[] players = FindObjectsOfType<GetTask>();
        foreach (var player in players)
        {
            Task task = player.assignedTask;

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