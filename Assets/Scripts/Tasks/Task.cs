using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Task
{
    public string name;
    public Vector2 position;
    public float completionRadius; // Radius within which the player can complete the task
    public float completionTime; // Time required to complete the task
    [HideInInspector] public bool isCompleted = false;
    [HideInInspector] public float taskTimer = 0f;

    // Constructor
    public Task( string name, Vector2 position, float completionRadius  = 2.0f, float completionTime =  3.0f)
    {
        this.name = name;  
        this.position = position;
        this.completionRadius = completionRadius;
        this.completionTime = completionTime;
    }
}