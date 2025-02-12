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
        text.color = Color.black;
        Debug.Log("Default");
        break;
      case State.Lie:
        text.color = Color.red;
        Debug.Log("Lie");
        break;
      case State.Truth:
        text.color = Color.green;
        Debug.Log("Truth");
        break;
    }

  }
  void Update()
  {

  }
}
