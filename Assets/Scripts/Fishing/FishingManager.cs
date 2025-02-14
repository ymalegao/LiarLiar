using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingManager : MonoBehaviour, MinigameManager
{
    public GameObject GameCanvas { get; set; }
    public RectTransform bait;

    private RectTransform canvasTransform;

    private void Awake()
    {
        GameCanvas = GameObject.Find("FishCanvas");
        canvasTransform = GameCanvas.GetComponent<RectTransform>();
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
        MoveBait();
    }

    private void MoveBait()
    {
        if (bait == null || canvasTransform == null) return;

        // Get mouse position in screen space
        Vector2 mousePosition = Input.mousePosition;

        // Convert screen position to UI Canvas local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform, mousePosition, null, out Vector2 localPoint
        );

        // Apply the position to the bait
        bait.anchoredPosition = localPoint;
    }
}