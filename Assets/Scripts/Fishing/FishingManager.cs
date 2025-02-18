using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingManager : MonoBehaviour, MinigameManager
{
    public GameObject GameCanvas { get; set; }
    private RectTransform canvasTransform;
    Vector2 canvasSize;
    public GameObject MinigameCanvasParent;

    private void Awake()
    {
        GameCanvas = MinigameCanvasParent.transform.Find("FishCanvas").gameObject;;
        // canvasTransform = GameCanvas.GetComponent<RectTransform>();
        // canvasSize = canvasTransform.rect.size;
    }

    private void OnEnable()
    {
        StartGame();
    }

    private void OnDisable(){
        EndGame();
    }

    public void StartGame()
    {
        Debug.Log("Fishing minigame started!");
        GameCanvas.gameObject.SetActive(true);
    }

    public void EndGame()
    {
        Debug.Log("Fishing minigame ended!");
        GameCanvas.gameObject.SetActive(false);

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