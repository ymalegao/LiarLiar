using System.Collections.Generic;
using UnityEngine;

public class NPCData
{
    public string Name; // The NPC's name
    public string Role; // Optional: Their role in the story (e.g., "Farmer", "Blacksmith")
    public Sprite Icon; // Optional: An icon for the NPC (assignable in Unity)
    public bool HasBeenInteractedWith; // Whether this NPC has already been interacted with
    public List<string> CluesGiven; // Clues given by this NPC
    public List<string> TruthsAndLiesGiven; // Truths and lies given by this NPC

    public NPCData(string name, string role, Sprite icon)
    {
        Name = name;
        Role = role;
        Icon = icon;
        HasBeenInteractedWith = false;
        CluesGiven = new List<string>();
        TruthsAndLiesGiven = new List<string>();
    }
}
