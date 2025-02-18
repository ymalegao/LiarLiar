using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingManager : MonoBehaviour, MinigameManager
{
    public GameObject GameCanvas { get; set; }
    private RectTransform canvasTransform;
    Vector2 canvasSize;

    private void Awake()
    {
        GameCanvas = GameObject.Find("FishCanvas");
        canvasTransform = GameCanvas.GetComponent<RectTransform>();
        canvasSize = canvasTransform.rect.size;
    }

    private void OnEnable()
    {
        StartGame();
    }

    public void StartGame()
    {
        Debug.Log("Fishing minigame started!");
        if (GameCanvas != null)
        {
            GameCanvas.gameObject.SetActive(true);
        }
    }

    public void EndGame()
    {
        Debug.Log("Fishing minigame ended!");
        if (GameCanvas != null)
        {
            GameCanvas.gameObject.SetActive(false);
        }
    }

    public void UpdateUI()
    {
    
    }

    public void ResetState()
    {
        Debug.Log("Resetting fishing minigame state...");
    }

    private void Update()
    {
    }

}