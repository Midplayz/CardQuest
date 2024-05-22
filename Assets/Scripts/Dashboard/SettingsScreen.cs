using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle inputToggle;
    [SerializeField] private Button logoutButton;
    [SerializeField] private GameObject logoutConfirm;
    [SerializeField] private Button confirmLogout;

    private void OnEnable()
    {
        logoutConfirm.SetActive(false);
    }
    void Start()
    {
        if (soundToggle != null) //Sets the sound toggle value based on the saved playerpref value if it exists, if not, it will be true by default.
        {
            soundToggle.onValueChanged.AddListener(OnSoundCheckboxValueChanged);

            if (PlayerPrefs.HasKey("isSoundOn"))
            {
                bool isSoundOn = PlayerPrefs.GetInt("isSoundOn") == 1;
                soundToggle.isOn = isSoundOn;
            }
            else
            {
                soundToggle.isOn = true;
            }
        }
        if (inputToggle != null) //Does the same for Swipe Input Toggle
        {
            inputToggle.onValueChanged.AddListener(OnInputCheckboxValueChanged);

            if (PlayerPrefs.HasKey("isSwipe"))
            {
                bool isSwipe = PlayerPrefs.GetInt("isSwipe") == 1;
                inputToggle.isOn = isSwipe;
            }
            else
            {
                inputToggle.isOn = false;
                PlayerPrefs.SetInt("isSwipe", 0);
                PlayerPrefs.Save();
            }
        }

        //Adds listeners to buttons with their respective functions
        confirmLogout.onClick.AddListener(ConfirmLogout);
        logoutButton.onClick.AddListener(onLogoutButtonClicked);
    }

    void OnSoundCheckboxValueChanged(bool isOn) //If the sound toggle value is changed, it will change the saved value and the Audio Source's volume accordingly.
    {
        AudioSource mainAudioSource = AudioSourceManager.Instance.MainAudioSource;
        if (mainAudioSource != null)
        {
            mainAudioSource.volume = isOn ? 1.0f : 0.0f;
        }
        PlayerPrefs.SetInt("isSoundOn", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    void OnInputCheckboxValueChanged(bool isOn)//if the input toggle value is changed, it will change the saved value accordingly.
    {
        PlayerPrefs.SetInt("isSwipe", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    void onLogoutButtonClicked()//Enabled the logout confirmation popup
    {
        logoutConfirm.SetActive(true);
    }

    void ConfirmLogout()//Deletes the Player Prefs Data and then goes back to the login screen
    {
        Debug.Log("Logging out now...");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene("LoginScreen");
    }
}
