using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;
using System.Linq;



public class SeekerSelectionUI : MonoBehaviour
{
  [SerializeField] private GameObject selectionPanel;
  [SerializeField] private Transform gridParent;
  [SerializeField] private GameObject characterButtonPrefab;

  private Dictionary<GameObject, bool> npcSelections = new Dictionary<GameObject, bool>();
  private List<GameObject> allCharacters = new List<GameObject>();
  private List<Sprite> characterSprites = new List<Sprite>();
  private List<GameObject> correctFakeNPCs = new List<GameObject>(); // Stores actual Fake NPCs



  private void Start()
  {
    Debug.Log("SeekerSelectionUI Start");

    if (selectionPanel != null)
    {
      selectionPanel.SetActive(false);
    }

    if (characterButtonPrefab != null)
    {
      Debug.Log("Character button prefab is not null");
      characterButtonPrefab.SetActive(false);
    }

    Debug.Log("Waiting for Seeker role...");
    StartCoroutine(WaitForSeekerRole());
  }

  private IEnumerator WaitForSeekerRole()
  {
    Debug.Log("Waiting for ServerManager to spawn...");
    while (ServerManager.Instance == null || !ServerManager.Instance.IsSpawned)
    {
      Debug.Log("ServerManager not spawned yet...");
      yield return new WaitForSeconds(0.5f);
    }

    Debug.Log("ServerManager spawned. Checking if this client is the Seeker...");
    ulong myClientId = NetworkManager.Singleton.LocalClientId;
    Debug.Log($"My client ID: {myClientId}");
    Debug.Log($"Seeker client ID: {ServerManager.Instance.seekerClientId.Value}");
    if (ServerManager.Instance.seekerClientId.Value == myClientId)
    {
      Debug.Log("✅ This client is the Seeker.");
      gameObject.SetActive(true);
      selectionPanel.SetActive(false);
    }
    else
    {
      Debug.Log("🚫 This client is NOT the Seeker.");
      gameObject.SetActive(false); // Hide UI
    }

  }


  public void Initialize(List<GameObject> characters)
  {
    ClearSelectionUI(); // Ensure previous UI elements are removed
    allCharacters = new List<GameObject>(characters);
    characterSprites.Clear();
    foreach (GameObject selected in allCharacters)
    {
      Debug.Log($"🟡 allCharacters: {selected.name} | Instance ID: {selected.GetInstanceID()}");
    }
    foreach (var character in allCharacters)
    {
      SpriteRenderer spriteRenderer = character.GetComponent<SpriteRenderer>();
      if (spriteRenderer != null)
      {
        characterSprites.Add(spriteRenderer.sprite);
      }
      else
      {
        Debug.LogError("SpriteRenderer is missing on character prefab: " + character.name);
      }
    }

    // ✅ Get properly instantiated fake NPCs from ServerManager
    correctFakeNPCs = new List<GameObject>(ServerManager.Instance.GetFakeNPCs());

    foreach (GameObject fakeNPC in correctFakeNPCs)
    {
      Debug.Log($"✅ Fake NPC in game: {fakeNPC.name} | Instance ID: {fakeNPC.GetInstanceID()}");
    }

    Debug.Log($"Initializing UI with {allCharacters.Count} characters and {correctFakeNPCs.Count} fake NPCs.");
    PopulateSelectionUI();
    selectionPanel.SetActive(true);
  }




  public void ClearSelectionUI()
  {
    // Check that gridParent is not null
    if (gridParent != null)
    {
      foreach (Transform child in gridParent)
      {
        if (child.gameObject != characterButtonPrefab)
        {
          Debug.Log($"Destroying child: {child.gameObject.name}");
          Destroy(child.gameObject); // Destroy each button (including fake NPC buttons)
        }
      }
    }
    npcSelections.Clear(); // Clear previous selections
    characterSprites.Clear(); // Clear previous character sprites
    allCharacters.Clear(); // Ensure the list is fully cleared
  }

  private void PopulateSelectionUI()
  {
    Debug.Log($"Populating UI with {allCharacters.Count} characters...");

    HashSet<GameObject> seenObjects = new HashSet<GameObject>(); // Track the GameObject reference to prevent duplicate entries

    for (int i = 0; i < allCharacters.Count; i++)
    {
      // Skip adding duplicate characters (based on actual GameObject reference)
      if (seenObjects.Contains(allCharacters[i]))
      {
        //Debug.LogError($"🚨 Duplicate character detected: {allCharacters[i].name}");
        continue;
      }

      GameObject buttonObj = Instantiate(characterButtonPrefab, gridParent);
      buttonObj.SetActive(true);

      Button button = buttonObj.GetComponent<Button>();
      Image buttonImage = buttonObj.GetComponent<Image>();
      buttonImage.color = Color.white;

      if (i < characterSprites.Count)
      {
        buttonImage.sprite = characterSprites[i];
      }
      else
      {
        Debug.LogError($"Index {i} out of range for characterSprites (size: {characterSprites.Count})");
        continue;
      }

      npcSelections[allCharacters[i]] = false;

      // Track this GameObject to avoid duplicate instantiation
      seenObjects.Add(allCharacters[i]);

      // Capture current index properly
      GameObject selectedCharacter = allCharacters[i];
      button.onClick.AddListener(() => ToggleSelection(selectedCharacter, buttonImage));
    }
  }


  private void ToggleSelection(GameObject character, Image buttonImage)
  {
    npcSelections[character] = !npcSelections[character];
    buttonImage.color = npcSelections[character] ? Color.red : Color.white;
  }

  public void ConfirmSelection()
  {
    List<GameObject> selectedFakes = new List<GameObject>();

    foreach (var entry in npcSelections)
    {
      if (entry.Value) selectedFakes.Add(entry.Key);
    }

    VerifySelection(selectedFakes);
    SendSelectionToServer(selectedFakes);
    selectionPanel.SetActive(false);
  }


  private void SendSelectionToServer(List<GameObject> selectedFakes)
  {
    if (NetworkManager.Singleton.IsServer)
    {
      Debug.Log($"Seeker selected {selectedFakes.Count} fake NPCs.");
    }
    else
    {
      Debug.Log("Sending selection to server...");
    }
  }

  private void VerifySelection(List<GameObject> selectedFakes)
  {
    Debug.Log($"🔍 Verifying Selection. Fake NPCs in-game: {correctFakeNPCs.Count}");

    foreach (GameObject correctFake in correctFakeNPCs)
    {
      Debug.Log($"✅ Correct Fake NPC: {correctFake.name} | Instance ID: {correctFake.GetInstanceID()}");
    }

    Debug.Log($"🕵️‍♂️ Seeker Selected {selectedFakes.Count} NPCs:");
    int correctCount = 0;

    foreach (GameObject selected in selectedFakes)
    {
      Debug.Log($"🟡 Seeker Selected: {selected.name} | Instance ID: {selected.GetInstanceID()}");

      // Compare by reference, not Instance ID
      if (correctFakeNPCs.Contains(selected))
      {
        Debug.Log("🎯 OMG I FOUND A FAKE!!");
        correctCount++;
      }
      else
      {
        Debug.Log("❌ Seeker selected an incorrect NPC.");
      }
    }

    if (correctCount == correctFakeNPCs.Count && selectedFakes.Count == correctFakeNPCs.Count)
    {
      Debug.Log("🎯 Seeker correctly identified all fake NPCs! ✅");
    }
    else
    {
      Debug.Log($"❌ Seeker made incorrect choices. {correctCount}/{correctFakeNPCs.Count} were correct.");
    }
  }



}
