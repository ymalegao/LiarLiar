using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for IPointerClickHandler



public class JournalItemState : MonoBehaviour, IPointerClickHandler
{
  // Start is called before the first frame update
  public enum State
  {
    Default,
    Lie,
    Truth,
  }

  public State state = State.Default;
  private TextMeshProUGUI text;
  // Store the correct answer for this journal entry
  public bool isCorrectlyTruth; // true = Truth, false = Lie
  public event System.Action OnStateChanged;
public bool IsMarkedCorrectly()
{
    if (state == State.Default)
    {
        return false; // Default state should never be marked correct
    }
    return (state == State.Truth) == isCorrectlyTruth;
}


  // public Material outline;

  void Awake()
  {
    text = GetComponent<TextMeshProUGUI>();
    if (text == null)
    {
      Debug.LogError("TextMeshProUGUI component not found!");
      return;
    }
    updateStateUI();

  }

  public void OnPointerClick(PointerEventData eventData)
  {
    cycleState();
    
  }

  public void cycleState()
  {
    state = (State)(((int)state + 1) % System.Enum.GetValues(typeof(State)).Length);
    updateStateUI();
    CheckCorrectness();
    OnStateChanged?.Invoke();
  }

  public void SetState(State newState)
    {
        state = newState;
        updateStateUI();
    }

    public State GetState()
    {
        return state;
    }



  public void updateStateUI()
{
    if (text == null)
    {
        Debug.LogError("TextMeshProUGUI component not found!");
        return;
    }

    switch (state)
    {
        case State.Default:
            text.color = new Color(0, 0, 0, 1); // Force full black
            Debug.Log("Default");
            break;
        case State.Lie:
            text.color = new Color(1, 0, 0, 1); // Force full red
            Debug.Log("Lie");
            break;
        case State.Truth:
            text.color = new Color(0, 1, 0, 1); // Force full green
            Debug.Log("Truth");
            break;
    }
}

  private void CheckCorrectness()
  {
      bool playerMarkedAsTruth = (state == State.Truth);
      if (playerMarkedAsTruth == isCorrectlyTruth)
      {
          Debug.Log("✅ Correct!");
      }
      else
      {
          Debug.Log("❌ Incorrect!");
      }

      // Notify the JournalManager to update the correct count
      JournalManager.Instance.UpdateCorrectCount();
  }

  public void SetText(string textContent, bool isTruth)
  {
      if (text == null)
      {
          text = GetComponent<TextMeshProUGUI>();
      }
      text.text = textContent;
      isCorrectlyTruth = isTruth;
  }
  void Update()
  {

  }
}
