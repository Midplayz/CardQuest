using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> clipList;
    private static AudioSourceManager instance = null;
    private AudioSource mainAudioSource;

    public static AudioSourceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioSourceManager>();
                if (instance == null)
                {
                    Debug.LogError("An instance of PersistentAudioSource is needed in the scene, but there is none.");
                }
            }
            return instance;
        }
    }

    public AudioSource MainAudioSource => mainAudioSource;

    private void Awake()
    {
        // Check if there's already an instance of this script as duplicates
        // can occur if the same scene is reloaded or something
        if (instance != null && instance != this)
        {
            Debug.Log("Found Duplicate Audio Script and deleted.");
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Debug.Log("New instance for Audio Script created.");
            // If no instance exists, set this as the instance and make it persist
            mainAudioSource = GetComponent<AudioSource>();
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            mainAudioSource.clip = clipList[Random.Range(0, clipList.Count)]; //this chooses a random song from the
                                                                              //list I specified in the inspector
            mainAudioSource.Play();
        }

        if (PlayerPrefs.HasKey("isSoundOn"))
        {
            bool isSoundOn = PlayerPrefs.GetInt("isSoundOn") == 1;
            if (mainAudioSource != null)
            {
                mainAudioSource.volume = isSoundOn ? 1.0f : 0.0f; //Depending on whether the user chose to mute or not
                                                                  //before, it will set the volume to 0 or 1. This info
                                                                  //is stored in playerpref.
            }
        }
        else
        {
            if (mainAudioSource != null)
            {
                mainAudioSource.volume = 1.0f;
            }
        }
    }
}
