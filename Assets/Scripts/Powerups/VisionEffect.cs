using UnityEngine;
using UnityEngine.UI;

public class VisionEffect : MonoBehaviour
{
  public static VisionEffect Instance { get; private set; }

  [SerializeField] private Material fogMaterial;
  [SerializeField] private float normalVisionRadius = 5f;
  [SerializeField] private float reducedVisionRadius = 2f;
  [SerializeField] private float effectDuration = 10f;

  private Image fogImage;
  private bool isVisionReduced = false;

  private void Awake()
  {
    Debug.Log("VisionEffect Awake called");
    if (Instance != null)
    {
      Debug.LogError("Another instance of VisionEffect already exists!");
      Destroy(gameObject);
      return;
    }
    if (fogMaterial == null)
    {
      Debug.LogError("Fog material is not assigned!");
      return;
    }
    Instance = this;
    InitializeFogEffect();
  }

  private void InitializeFogEffect()
  {
    Debug.Log("Initializing fog effect");
    GameObject fogObj = new GameObject("VisionFog");
    fogObj.transform.SetParent(transform);

    Canvas canvas = fogObj.AddComponent<Canvas>();
    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    canvas.sortingOrder = 1;

    fogImage = fogObj.AddComponent<Image>();
    fogImage.material = fogMaterial;
    fogImage.color = new Color(0, 0, 0, 1);

    RectTransform rect = fogImage.rectTransform;
    rect.anchorMin = Vector2.zero;
    rect.anchorMax = Vector2.one;
    rect.sizeDelta = Vector2.zero;
    rect.offsetMin = Vector2.zero;
    rect.offsetMax = Vector2.zero;

    Debug.Log($"Fog effect initialized - Image: {fogImage != null}, Material: {fogImage.material != null}");
    fogObj.SetActive(false);
  }

  public void ReduceVision()
  {
    Debug.Log($"ReduceVision called - Fog GameObject active: {fogImage.gameObject.activeInHierarchy}, Alpha: {fogImage.color.a}");
    if (!isVisionReduced)
    {
      isVisionReduced = true;
      fogImage.gameObject.SetActive(true);
      fogMaterial.SetFloat("_VisionRadius", reducedVisionRadius);
      fogImage.color = new Color(0, 0, 0, 1); // Instant fade to black
      Debug.Log("Fog effect activated");
      Invoke(nameof(RestoreVision), effectDuration);
    }
  }

  private void RestoreVision()
  {
    Debug.Log("RestoreVision called");
    fogImage.gameObject.SetActive(false);
    isVisionReduced = false;
  }
}