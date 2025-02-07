using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

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
        if (characterButtonPrefab != null)
        {
            characterButtonPrefab.SetActive(false);
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
