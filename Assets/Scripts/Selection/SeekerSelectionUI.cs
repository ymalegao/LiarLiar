using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;


public class SeekerSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject characterButtonPrefab;
    
    private Dictionary<GameObject, bool> npcSelections = new Dictionary<GameObject, bool>();
    private List<GameObject> allCharacters = new List<GameObject>();
    private List<Sprite> characterSprites = new List<Sprite>();


    private void Start()
    {
        Debug.Log("SeekerSelectionUI Start");
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
        selectionPanel.SetActive(true);
    }
    else
    {
        Debug.Log("🚫 This client is NOT the Seeker.");
        gameObject.SetActive(false); // Hide UI
    }

}

    
    public void Initialize(List<GameObject> characters)
    {
        ClearSelectionUI();
        allCharacters = characters;
        characterSprites.Clear(); // Ensure it's empty before filling

        foreach (var character in allCharacters)
        {
            Debug.Log(character);
            characterSprites.Add(character.GetComponent<SpriteRenderer>().sprite);
        }
        PopulateSelectionUI();
        selectionPanel.SetActive(true);
        foreach (var character in characterSprites)
        {
            Debug.Log(character);
        }
    }
  
    public void ClearSelectionUI()
    {
      foreach (Transform child in gridParent)
      {
          if (child.gameObject != characterButtonPrefab)
          {
              Destroy(child.gameObject);
          }
      }
      npcSelections.Clear();
      
    }



    private void PopulateSelectionUI()
{
    
    for (int i = 0; i < allCharacters.Count; i++)
{
    Debug.Log(allCharacters[i]);
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

    // Fix: Capture current index in a separate variable
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
}
