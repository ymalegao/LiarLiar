using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HelpPage : MonoBehaviour
{

    [SerializeField] private GameObject helpScreen;
    [SerializeField] private AudioSource helpAudioSource;  // AudioSource for playing sound


    // Start is called before the first frame update
    void Start()
    {
        if (helpScreen != null)
        {
            helpScreen.SetActive(false);
        }
    }

    // Update is called once per frame
    public void toggleHelpScreen(){
        if (helpScreen != null)
        {
            bool isActive = !helpScreen.activeSelf;
            helpScreen.SetActive(isActive);

            // Play audio when the help menu is toggled
            if (helpAudioSource != null)
            {
                helpAudioSource.Play();  // Play the sound when the help menu is shown
            }

        }
    }
}
