using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameTask
{
    public string name;
    public Vector2 position;
    public float completionRadius;
    public float completionTime;
    [HideInInspector] public bool isCompleted = false;
    [HideInInspector] public float taskTimer = 0f;
    [HideInInspector] public bool isActive= false;


    public GameTask(string name, Vector2 position, float completionRadius = 2.0f, float completionTime = 3.0f)
    {
        this.name = name;
        this.position = position;
        this.completionRadius = completionRadius;
        this.completionTime = completionTime;
    }
}