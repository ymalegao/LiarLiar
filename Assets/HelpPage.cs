using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HelpPage : MonoBehaviour
{

    [SerializeField] private GameObject helpScreen;

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

        }
    }
}
