using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class FishingManager : MonoBehaviour, MinigameManager
{
    public GameObject GameCanvas { get; set; }
    public GameObject bait;

    private RectTransform canvasTransform;
    Vector2 canvasSize;
    public GameObject MinigameCanvasParent;
    [SerializeField] private GameObject fishPrefab;
    [SerializeField] private GameObject sharkPrefab;
    [SerializeField] private float spawnInterval = 2f;

    public Camera FishCamera; 

    public TMP_Text scoreText;

    public TMP_Text GameOverText;
    public Button exitButton;

    public int score = 0; 

    private void Awake()
    {
        GameCanvas = MinigameCanvasParent.transform.Find("FishCanvas").gameObject;
        canvasTransform = GameCanvas.GetComponent<RectTransform>();
        canvasSize = canvasTransform.rect.size;
        FishCamera = this.GetComponentInChildren<Camera>();
        scoreText.text = "Score: 0";
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
        GameCanvas.gameObject.SetActive(true);
        StartCoroutine(SpawnFishRoutine());
        StartCoroutine(SpawnSharkRoutine());
    }

    public void EndGame()
    {
        if(!GameOverText.gameObject|| !exitButton ){return;}
        GameOverText.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        StopAllCoroutines();

    }

    public void Exit(){
        StopAllCoroutines();
        if (GameCanvas != null)
        {
            GameCanvas.gameObject.SetActive(false);
        }
    }

    public void UpdateUI()
    {
        scoreText.text = "Score: " + score; 
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

        Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f); // Z distance from camera

        // Convert screen position to world position
        bait.transform.position = FishCamera.ScreenToWorldPoint(screenPoint);

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
        if (fishPrefab == null || canvasTransform == null || FishCamera == null) return;

        // Get the right edge of the screen in world coordinates
        Vector3 screenPoint = new Vector3(Screen.width, Random.Range(0, Screen.height/2.0f), 10.0f); // Right edge, random height

        // Convert to world position
        Vector3 worldSpawnPosition = FishCamera.ScreenToWorldPoint(screenPoint);

        // Instantiate the fish and parent it to the canvas
        GameObject fish = Instantiate(fishPrefab);
        // fish.transform.SetParent(canvasTransform, false); // Parent to canvas but keep world position

        // Set fish position
        fish.transform.position = worldSpawnPosition;
        fish.transform.rotation = Quaternion.identity; // Reset rotation

        fish.tag = "Fish";
    }

    private IEnumerator SpawnSharkRoutine()
    {
        while (true)
        {
            SpawnShark();
            yield return new WaitForSeconds(1.4f);
        }
    }

    private void SpawnShark()
    {
        if (sharkPrefab == null || canvasTransform == null || FishCamera == null) return;

        // Get the right edge of the screen in world coordinates
        Vector3 screenPoint = new Vector3(Screen.width, Random.Range(0, Screen.height/2.0f), 10.0f); // Right edge, random height

        // Convert to world position
        Vector3 worldSpawnPosition = FishCamera.ScreenToWorldPoint(screenPoint);

        // Instantiate the fish and parent it to the canvas
        GameObject shark = Instantiate(sharkPrefab);
        // shark.transform.SetParent(canvasTransform, false); // Parent to canvas but keep world position

        // Set fish position
        shark.transform.position = worldSpawnPosition;
        shark.transform.rotation = Quaternion.Euler(0, 180, 0); // Reset rotation

        shark.tag = "Shark";
    }

}