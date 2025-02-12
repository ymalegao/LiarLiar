using UnityEngine;
using System.Collections.Generic;

public class JournalTester : MonoBehaviour
{
  private void Start()
  {
    TestJournalManager();
  }

  private void TestJournalManager()
  {
    Debug.Log("🔍 Starting JournalManager Tests...");

    // Step 1: Get the JournalManager instance
    if (JournalManager.Instance == null)
    {
      Debug.LogError("❌ JournalManager instance not found!");
      return;
    }

    JournalManager journal = JournalManager.Instance;

    // Step 2: Register NPCs (assuming we have placeholder sprites)
    Sprite placeholderSprite = null;
    journal.RegisterNPC("Derek", "Derek Burton", "Farmer", placeholderSprite);
    journal.RegisterNPC("Adeline", "Adeline Burton", "Farmer’s Daughter", placeholderSprite);
    journal.RegisterNPC("Avery", "Avery Peters", "Baker", placeholderSprite);
    journal.RegisterNPC("Hadwin", "Hadwin Mitchell", "Wizard", placeholderSprite);
    journal.RegisterNPC("Ed", "Ed Porter", "Blacksmith", placeholderSprite);
    journal.RegisterNPC("Weston", "Weston McCarthy", "Tavern Owner", placeholderSprite);
    journal.RegisterNPC("Timothy", "Timothy Mullens", "Fisherman", placeholderSprite);
    journal.RegisterNPC("Carrie", "Carrie Humboldt", "Store Owner", placeholderSprite);

    Debug.Log("✅ NPCs Registered");

    // Step 3: Add NPC Clues
    journal.AddClueFromNPC("Derek", "Hadwin is going around screaming about a magic book.");
    journal.AddClueFromNPC("Adeline Burton", "Weston was at the lake reading a book early this morning.");
    journal.AddClueFromNPC("Avery Peters", "Ed was carrying papers with strange scribbles.");
    journal.AddClueFromNPC("Hadwin Mitchell", "My book is missing! Someone stole it from my chest last night!");
    journal.AddClueFromNPC("Ed Porter", "Hadwin kept talking about a mistake he made last night.");
    journal.AddClueFromNPC("Weston McCarthy", "Avery has been in love with Derek, but now he’s acting differently.");
    journal.AddClueFromNPC("Timothy Mullens", "Adeline has been sneaking around Hadwin’s tower.");
    journal.AddClueFromNPC("Carrie Humboldt", "I found a book with strange scribbles next to a tree stump.");

    Debug.Log("✅ Clues Added to Journal");

    // Step 4: Add Truths and Lies from NPCs
    journal.AddTruthsAndLiesFromNPC("Derek", new List<(string, bool)>
        {
            ("Hadwin is going around screaming about how his book holds some mighty spells that can only be casted with fancy materials by magic people.", true),
            ("Carrie was telling me about a book she just found. Seems interesting. It’s got lots of cool pictures and scribbles and such.", true),
            ("I don’t have very much time for reading these days. With Adeline and the farm, I’m workin like a dog!", false)
        });

    journal.AddTruthsAndLiesFromNPC("Adeline", new List<(string, bool)>
        {
            ("I saw Weston over by the lake reading a book really early this morning when I was running errands for Dad. He looked funny. Like he was talking to the lake or something!!", true),
            ("Dad is always always so busy! Never has time to do anything fun anymore. I have to find ways to have fun without him.", true),
            ("Whatever. I’ve been going to Hadwin’s and he has been teaching me some magic. Abracadabra!", false)
        });

    journal.AddTruthsAndLiesFromNPC("Avery", new List<(string, bool)>
        {
            ("I stopped by Carrie’s shop last night to pick up some stuff for my date tonight!.", true),
            ("On the way there I ran into Ed. He was being really suspicious. He was holding some papers with odd scribbles.", true),
            ("Anyways Carrie was mumbling some weird phrases. Sounded foreign I dont know. I always thought she was a little strange……", false)
        });

    journal.AddTruthsAndLiesFromNPC("Hadwin", new List<(string, bool)>
        {
            ("My book is missing! Someone stole it from my chest last night!!!", true),
            ("I was home all night!! There’s no way someone stole it.", false),
            ("Those spells in the book are dangerous. They can be triggered just by speaking the words.", true)
        });

    journal.AddTruthsAndLiesFromNPC("Ed", new List<(string, bool)>
        {
            ("Me and Hadwin were having a drink at Weston’s last night. ", true),
            ("Hadwin kept going on and on about a mistake he made earlier last night. He’s a bit of an odd-ball.", false),
            ("He was also telling me about how he has been secretly mentoring Timmy to be his apprentice. He’s getting old and needs the extra hands.", true)
        });

    journal.AddTruthsAndLiesFromNPC("Weston", new List<(string, bool)>
        {
            ("I’ve been so busy with the shop that I haven’t gotten to go relax. I haven’t had time to go to the lake in weeks! That’s my safe space.", true),
            ("At least I got to chat with my friends last night. Hadwin and Ed came in for a drink.", true),
            ("We talked about how Derek and Avery seem awfully friendly these days. Avery has been in love with Derek for years but he never felt the same. It’s like he’s under a spell or something.", false)
        });

    journal.AddTruthsAndLiesFromNPC("Timothy", new List<(string, bool)>
        {
            ("I would never get involved in the magic stuff. It’s too freaky for me. Besides, I’m busy with the fishing business.", true),
            ("However, Adeline was telling me all about how she wanted to do magic but Hadwin wouldn’t let her.", true),
            ("She said she’s been sneaking around the tower to watch him while he works.", false)
        });

    journal.AddTruthsAndLiesFromNPC("Carrie", new List<(string, bool)>
        {
            ("I have been so stressed all day! I’m going crazy, talking to myself like a mad woman.  ", true),
            ("Avery stopped by last night to pick up some stuff from the shop. I think she heard me. Hopefully she doesn’t think im insane!!!", true),
            ("I’ve been doing some reading to keep myself from going crazy. I stumbled across this magic fantasy book and it’s quite unusual.", false)
        });







    Debug.Log("✅ Truths and Lies Processed");

    // Step 5: Add Physical Evidence as Clues
    journal.AddClue("Hadwin’s receipt confirms he was at the tavern.");
    journal.AddClue("A love letter confirms Derek and Avery’s relationship.");
    journal.AddClue("Carrie’s book is actually a history book.");
    journal.AddClue("Tiny footsteps near the tower suggest Adeline was sneaking around.");
    journal.AddClue("Potion bottles at Timothy’s shack show Hadwin’s written instructions.");

    Debug.Log("✅ Evidence Clues Added");

    // Step 6: Check Journal Output
    Debug.Log("📝 Journal Testing Complete! Open the UI and verify entries.");
  }
}
