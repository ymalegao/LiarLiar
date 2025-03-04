using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGame : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI endGameText;
    void Start()
    {
        if (endGameText != null)
        {
            endGameText.text = PlayerPrefs.GetString("EndGameMessage", "Game Over");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
