using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCButtonController : MonoBehaviour
{
    public enum NPCState
    {
        Default,
        Suspicious,
        Innocent
    }

    public NPCState currentState = NPCState.Default;
    private Button npcButton;
    private Image buttonImage;
    private TextMeshProUGUI buttonText;

    // Define colors for each state
    public Color defaultColor = Color.black;
    public Color suspiciousColor = Color.red;
    public Color innocentColor = Color.green;

    private void Awake()
    {
        npcButton = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (npcButton == null || buttonImage == null || buttonText == null)
        {
            Debug.LogError("NPCButtonState: Missing required components.");
            return;
        }

        currentState = NPCState.Default;
        UpdateButtonAppearance();
    }

    private void OnButtonClick()
    {
        // Cycle through the states
        currentState = (NPCState)(((int)currentState + 1) % System.Enum.GetValues(typeof(NPCState)).Length);
        UpdateButtonAppearance();
    }

    public void OnGreenButton()
    {
        if (currentState == NPCState.Innocent)
        {
            currentState = NPCState.Default;
        }
        else
        {
            currentState = NPCState.Innocent;
        }
       
        UpdateButtonAppearance();
    }

    public void OnRedButton()

    {
        if (currentState == NPCState.Suspicious)
        {
            currentState = NPCState.Default;
        }
        else
        {
            currentState = NPCState.Suspicious;
        }
        
        UpdateButtonAppearance();
    }

    private void UpdateButtonAppearance()
    {
        switch (currentState)
        {
            case NPCState.Default:
                buttonText.color = defaultColor;
                break;
            case NPCState.Suspicious:
                buttonText.color = suspiciousColor;
                break;
            case NPCState.Innocent:
                buttonText.color = innocentColor;
                break;
        }
    }
}
