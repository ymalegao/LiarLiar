using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingManager : MonoBehaviour, MinigameManager
{
    public GameObject GameCanvas { get; set; }
    public RectTransform bait;

    private RectTransform canvasTransform;
    Vector2 canvasSize;
    public GameObject MinigameCanvasParent;
    [SerializeField] private GameObject fishPrefab;
    [SerializeField] private float spawnInterval = 2f;

    public Camera FishCamera; 

    private void Awake()
    {
        GameCanvas = MinigameCanvasParent.transform.Find("FishCanvas").gameObject;;
        canvasTransform = GameCanvas.GetComponent<RectTransform>();
        canvasSize = canvasTransform.rect.size;
        FishCamera = this.GetComponent<Camera>();
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
        if (GameCanvas != null)
        {
            GameCanvas.gameObject.SetActive(true);
        }
        StartCoroutine(SpawnFishRoutine());
    }

    public void EndGame()
    {
        if (GameCanvas != null)
        {
            GameCanvas.gameObject.SetActive(false);
        }
        StopAllCoroutines();

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
        if (bait == null || canvasTransform == null || FishCamera == null) return;

        // Get the mouse position in screen space
        Vector3 mouseScreenPos = Input.mousePosition;

        // Convert mouse position to world position using fishCamera
        Vector3 worldPosition = FishCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, FishCamera.nearClipPlane));

        // Convert world position to UI local position
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform, worldPosition, FishCamera, out localPoint
        );

        // Clamp the bait position inside the canvas bounds
        float clampedX = Mathf.Clamp(localPoint.x, -canvasSize.x / 2, canvasSize.x / 2);
        float clampedY = Mathf.Clamp(localPoint.y, -canvasSize.y / 2, canvasSize.y / 12);

        // Apply the clamped position
        bait.anchoredPosition = new Vector2(clampedX, clampedY);
    }

    private IEnumerator SpawnFishRoutine()
    {
        while (true)
        {
            SpawnFish();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnFish()
    {
        if (fishPrefab == null || canvasTransform == null) return;

        // Get the right edge of the canvas for spawning
        float spawnX = canvasTransform.rect.width / 2;
        float spawnY = Random.Range(-canvasSize.y / 2, canvasSize.y / 12);

        Vector2 spawnPosition = new Vector2(spawnX, spawnY);
        GameObject fish = Instantiate(fishPrefab, canvasTransform); 
        fish.GetComponent<RectTransform>().anchoredPosition = spawnPosition;
        fish.tag = "Fish";
    }
}