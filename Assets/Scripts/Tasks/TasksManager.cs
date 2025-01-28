using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public Dictionary<string, Task> tasks = new Dictionary<string, Task>(){
        {"fish", new Task("fish", new Vector2(0f,0f))}
    };
    public GetTask[] players; // All gameObjects with Player script 

    void Start()
    {
        AssignTasksToPlayers();
    }


    // Assign tasks to players
    public void AssignTasksToPlayers()
    {
         //add all players to players array
        players = FindObjectsOfType<GetTask>();

        for (int i = 0; i < players.Length; i++ )
        {
            players[i].SetTask(tasks[players[i].jobName]); // Assign each player a task
        }

        Debug.Log("Tasks assigned to all players.");
    }

    void Update()
    {
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