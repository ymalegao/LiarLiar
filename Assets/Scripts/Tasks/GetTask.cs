using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTask : MonoBehaviour
{
    public string jobName; // Name of the task to assign player 
    public Task assignedTask; // The task assigned to this player
    public void SetTask(Task task)
    {
        assignedTask = task;
        Debug.Log("Task assigned to player: " + task.position);
    }

    void start(){}

    void update(){}

}
