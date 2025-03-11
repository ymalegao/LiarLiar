using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
public class LocaleSelector : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip localeChangeSound; // Assign in Inspector
    private AudioSource audioSource;

    private bool active = false;


    private void Start()
    {
        // Ensure there's an AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("[LocaleSelector] Missing AudioSource! Adding one.");
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void ChangeLocale(int localeID){
        if (active == true){
            return;
        }
        PlaySound();
        StartCoroutine(setLocale(localeID));
    }
    IEnumerator setLocale(int _localeID){
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        active = false;
    }

    public void PlaySound()
    {
        audioSource.PlayOneShot(localeChangeSound);
    }
}
