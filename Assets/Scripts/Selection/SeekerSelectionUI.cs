using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;
using System.Linq;
using TMPro;
using System;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;


public class SeekerSelectionUI : NetworkBehaviour
{
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject characterButtonPrefab;
    [SerializeField] private string spellCasterName = "DefaultCaster";
    [SerializeField] private AudioSource buttonAudioSource;

    private List<GameObject> allCharacters = new List<GameObject>();
    private List<Sprite> characterSprites = new List<Sprite>();
    private GameObject currentlySelectedNPC;
    private Image currentlySelectedImage;

    private NetworkVariable<bool> hostGuessedCorrectly = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> clientGuessedCorrectly = new NetworkVariable<bool>(false);
    

    private void Start()
    {

        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }

        if (characterButtonPrefab != null)
        {
            characterButtonPrefab.SetActive(false);
        }
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

        if (buttonAudioSource != null)
        {
            buttonAudioSource.Play(); // Play button sound
        }

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

        if (currentlySelectedNPC == null)
        {
            Debug.LogError("❌ No character selected!");
            return;
        }
        Debug.Log("here");
        ConfirmSelectionServerRpc(currentlySelectedNPC.name);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ConfirmSelectionServerRpc(string selectedNPCName, ServerRpcParams rpcParams = default)
    {

        if (buttonAudioSource != null)
        {
            buttonAudioSource.Play(); // Play button sound
        }

        bool isCorrect = selectedNPCName == spellCasterName;

        ulong senderId = rpcParams.Receive.SenderClientId;
        bool isHost = senderId == NetworkManager.Singleton.ConnectedClientsIds[0]; // Host is always the first client in the list

        if (isHost)
        {
            hostGuessedCorrectly.Value = isCorrect;
            Debug.Log($"Host guessed correctly: {isCorrect}");
        }
        else
        {
            clientGuessedCorrectly.Value = isCorrect;
        }

        Debug.Log("Player has made their selections!");
        DetermineOutcome();
    }

    private void DetermineOutcome()
    {
        if (hostGuessedCorrectly.Value && clientGuessedCorrectly.Value)
        {
            // Both players guessed correctly
            SetOutcomeClientRpc("You Win!", "You Win!");
        }
        else if (hostGuessedCorrectly.Value)
        {
            // Host guessed correctly, client did not
            Debug.Log("Host guessed correctly, client did not");
            SetOutcomeClientRpc("You Win!", "You Lose!");
        }
        else if (clientGuessedCorrectly.Value)
        {
            // Client guessed correctly, host did not
            SetOutcomeClientRpc("You Lose!", "You Win!");
        }
        else if (!hostGuessedCorrectly.Value )
        {
            // Both players guessed incorrectly
            Debug.Log("Host guessed wrong,");

            SetOutcomeClientRpc("You Lose!", "You Win!");
        }

        else
        {
            Debug.LogError("❌ No outcome determined!");
            return;
        }

        // Disable all cameras before loading the end scene
        DisableAllCameras();

        DisableCamerasClientRpc();

        // Load the end game scene for all players
        LoadSceneForAll("End Game");

        // Reset selections for the next round
        ResetSelections();
    }

    [ClientRpc]
    private void SetOutcomeClientRpc(string hostOutcome, string clientOutcome)
    {
        if (IsHost)
        {
            PlayerPrefs.SetString("EndGameMessage", hostOutcome);
        }
        else
        {
            PlayerPrefs.SetString("EndGameMessage", clientOutcome);
        }
    }

    private void LoadSceneForAll(string sceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private void ResetSelections()
    {
        hostGuessedCorrectly.Value = false;
        clientGuessedCorrectly.Value = false;
    }

    private void DisableAllCameras()
    {
        Debug.Log("Disabling all cameras...");
        foreach (Camera cam in Camera.allCameras)
        {
            Debug.Log("Disabling camera: " + cam.name);
            cam.gameObject.SetActive(false);
        }
    }

    [ClientRpc]
    private void DisableCamerasClientRpc()
    {
        Debug.Log("Disabling cameras on client...");
        DisableAllCameras();
    }
}
