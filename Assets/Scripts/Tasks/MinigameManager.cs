using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface MinigameManager
{
    GameObject GameCanvas { get; set; }
    void StartGame();
    void EndGame();
    void UpdateUI();     // Updates UI elements during the minigame
    void ResetState();   // Resets the game state when needed
}
