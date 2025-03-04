using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameTask
{
  public string name;

  public GameObject gameManager;

  public Vector2 position;
  public float completionRadius;
  public float completionTime;
  [HideInInspector] public bool isCompleted = false;
  [HideInInspector] public float taskTimer = 0f;
  [HideInInspector] public bool isActive = false;

  public GameObject camera; 


  public GameTask(string name, GameObject gameManager, GameObject camera,Vector2 position, float completionRadius = 2.0f, float completionTime = 10.0f)
  {
    this.name = name;
    this.gameManager = gameManager;
    this.position = position;
    this.completionRadius = completionRadius;
    this.completionTime = completionTime;
    this.camera = camera;

  }

}