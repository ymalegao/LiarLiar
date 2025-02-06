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
        PopulateSelectionUI();
        selectionPanel.SetActive(true);
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
    foreach (var character in allCharacters)
    {
        Debug.Log(character);
        GameObject buttonObj = Instantiate(characterButtonPrefab, gridParent);
        buttonObj.SetActive(true);
        Button button = buttonObj.GetComponent<Button>();
        Image buttonImage = buttonObj.GetComponent<Image>();

        SpriteRenderer spriteRenderer = character.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Sprite[] characterSprites = Resources.LoadAll<Sprite>(spriteRenderer.sprite.texture.name);
            
            if (characterSprites.Length > 0)
            {
                Debug.Log("found sprite texture :)");
                buttonImage.sprite = characterSprites[0]; 
            }
            else
            {
                Debug.LogWarning($"No sprites found for {spriteRenderer.sprite.texture.name}");
            }
        }

        npcSelections[character] = false;
        button.onClick.AddListener(() => ToggleSelection(character, buttonImage));
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
