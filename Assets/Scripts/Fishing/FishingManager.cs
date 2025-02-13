using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingManager : MonoBehaviour, MinigameManager
{
    public GameObject GameCanvas { get; set; }
    [SerializeField] private RectTransform fishingHookUI;

    private RectTransform canvasRect;

    private void Awake()
    {
        GameCanvas = GameObject.Find("FishCanvas");
        canvasRect = GameCanvas.GetComponent<RectTransform>();
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
        MoveFishingHookUI();
    }

    private void MoveFishingHookUI()
    {
        if (fishingHookUI == null || canvasRect == null) return;

        // Get mouse position in screen space
        Vector2 mousePosition = Input.mousePosition;

        // Convert screen position to UI Canvas local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, mousePosition, null, out Vector2 localPoint
        );

        // Apply the position to the UI hook
        fishingHookUI.anchoredPosition = localPoint;
    }
}