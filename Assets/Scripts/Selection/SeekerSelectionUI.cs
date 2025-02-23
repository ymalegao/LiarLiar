using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;
using System.Linq;
using TMPro;

public class SeekerSelectionUI : MonoBehaviour
{
  [SerializeField] private GameObject selectionPanel;
  [SerializeField] private Transform gridParent;
  [SerializeField] private GameObject characterButtonPrefab;
  [SerializeField] private TextMeshProUGUI nameTextPrefab;

  private List<GameObject> allCharacters = new List<GameObject>();
  private List<Sprite> characterSprites = new List<Sprite>();
  private GameObject currentlySelectedNPC;
  private Image currentlySelectedImage;

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
    while (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient)
    {
      yield return new WaitForSeconds(0.5f);
    }

    gameObject.SetActive(true);
    selectionPanel.SetActive(false);
  }

  public void Initialize(List<GameObject> characters)
  {
    ClearSelectionUI();
    allCharacters = new List<GameObject>(characters);
    characterSprites.Clear();

    foreach (var character in allCharacters)
    {
      SpriteRenderer spriteRenderer = character.GetComponent<SpriteRenderer>();
      if (spriteRenderer != null)
      {
        characterSprites.Add(spriteRenderer.sprite);
      }
    }

    Debug.Log($"Initializing UI with {allCharacters.Count} NPCs");
    PopulateSelectionUI();
    selectionPanel.SetActive(true);
  }

  public void ClearSelectionUI()
  {
    if (gridParent != null)
    {
      foreach (Transform child in gridParent)
      {
        if (child.gameObject != characterButtonPrefab)
        {
          Debug.Log($"Destroying child: {child.gameObject.name}");
          Destroy(child.gameObject);
        }
      }
    }
    currentlySelectedNPC = null;
    currentlySelectedImage = null;
    characterSprites.Clear();
    allCharacters.Clear();
  }

  private void PopulateSelectionUI()
  {
    Debug.Log($"Populating UI with {allCharacters.Count} characters...");
    HashSet<GameObject> seenObjects = new HashSet<GameObject>();

    for (int i = 0; i < allCharacters.Count; i++)
    {
      if (seenObjects.Contains(allCharacters[i]))
      {
        continue;
      }

      GameObject buttonObj = Instantiate(characterButtonPrefab, gridParent);
      buttonObj.SetActive(true);

      Button button = buttonObj.GetComponent<Button>();
      Image buttonImage = buttonObj.GetComponent<Image>();
      buttonImage.color = Color.white;

      TextMeshProUGUI nameText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
      if (nameText != null)
      {
        NPC npcComponent = allCharacters[i].GetComponent<NPC>();
        string npcName = npcComponent != null ? npcComponent.npcName : allCharacters[i].name;
        npcName = npcName.Split('(')[0].Trim();

        // Split the name into first and last name and add line break
        string[] nameParts = npcName.Split(' ');
        if (nameParts.Length >= 2)
        {
          nameText.text = $"{nameParts[0]}\n{string.Join(" ", nameParts.Skip(1))}";
        }
        else
        {
          nameText.text = npcName;
        }
      }

      if (i < characterSprites.Count)
      {
        buttonImage.sprite = characterSprites[i];
      }
      else
      {
        Debug.LogError($"Index {i} out of range for characterSprites (size: {characterSprites.Count})");
        continue;
      }

      seenObjects.Add(allCharacters[i]);

      GameObject selectedCharacter = allCharacters[i];
      button.onClick.AddListener(() => ToggleSelection(selectedCharacter, buttonImage));
    }
  }

  private void ToggleSelection(GameObject character, Image buttonImage)
  {
    if (character == currentlySelectedNPC)
    {
      currentlySelectedImage.color = Color.white;
      currentlySelectedNPC = null;
      currentlySelectedImage = null;
      return;
    }

    if (currentlySelectedImage != null)
    {
      currentlySelectedImage.color = Color.white;
    }

    currentlySelectedNPC = character;
    currentlySelectedImage = buttonImage;
    buttonImage.color = Color.red;
  }

  public void ConfirmSelection()
  {
    if (currentlySelectedNPC != null)
    {
      VerifySelection(currentlySelectedNPC);
      SendSelectionToServer(currentlySelectedNPC);
    }
    selectionPanel.SetActive(false);
  }

  private void SendSelectionToServer(GameObject selectedNPC)
  {
    if (NetworkManager.Singleton.IsServer)
    {
      Debug.Log($"Seeker selected NPC: {selectedNPC.name}");
    }
    else
    {
      Debug.Log("Sending selection to server...");
    }
  }

  private void VerifySelection(GameObject selectedNPC)
  {
    Debug.Log($"Selected NPC for verification: {selectedNPC.name}");
  }
}
