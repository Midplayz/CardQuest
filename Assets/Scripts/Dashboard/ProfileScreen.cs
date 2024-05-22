using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileScreen : MonoBehaviour
{
    [Header("Profile Text")]
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI phoneNumberText;
    [SerializeField] private TextMeshProUGUI winsText;
    [SerializeField] private TextMeshProUGUI lossesText;

    [Header("Popup Related")]
    [SerializeField] private Button editButton;
    [SerializeField] private GameObject editPopup;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_InputField usernameEditText;
    [SerializeField] private TMP_InputField phoneNumberEditText;

    private string playerUsername = "";
    private string playerPhoneNumber = "";
    private int playerWins = 0;
    private int playerLosses= 0;

    private void OnEnable()
    {
        editPopup.SetActive(false);
    }
    void Start()
    {
        //These will check if they are saved or not
        if (PlayerPrefs.HasKey("playerUsername"))
        {
            playerUsername = PlayerPrefs.GetString("playerUsername");
        }

        if (PlayerPrefs.HasKey("playerPhoneNumber"))
        {
            playerPhoneNumber = PlayerPrefs.GetString("playerPhoneNumber");
        }

        if (PlayerPrefs.HasKey("playerWins"))
        {
            playerWins = PlayerPrefs.GetInt("playerWins");
        }

        if (PlayerPrefs.HasKey("playerLosses"))
        {
            playerLosses = PlayerPrefs.GetInt("playerLosses");
        }

        //This is for setting the initial values for the Input Fields for the Editing Pop up
        usernameEditText.text = playerUsername;
        phoneNumberEditText.text = playerPhoneNumber;

        //If they don't exist, it will use the default values that I assigned when initializing
        usernameText.text = playerUsername;
        phoneNumberText.text = playerPhoneNumber;
        winsText.text = playerWins.ToString();
        lossesText.text = playerLosses.ToString();

        //Adds necessary function listeners to buttons. This shows that i can approach button on-click function in script itself.
        saveButton.onClick.AddListener(SaveChanges);
        editButton.onClick.AddListener(onEditClicked);
        closeButton.onClick.AddListener(OnCloseEditPopupClicked);
    }

    private void onEditClicked() //When you click on Edit, it sets the pop up as true and fills the text with the current values in the input fields
    {
        editPopup.SetActive(true);
        usernameEditText.onValueChanged.AddListener(OnInputValueChanged);
        phoneNumberEditText.onValueChanged.AddListener(OnInputValueChanged);
    }

    void OnInputValueChanged(string value) //Enables the save button if the user actually changes the text in the input field
    {
        saveButton.interactable = usernameEditText.text != playerUsername || phoneNumberEditText.text != playerPhoneNumber;
    }

    void SaveChanges() //Saves the changes and updates the value on all necessary areas.
    {
        if (usernameEditText.text != playerUsername)
        {
            playerUsername = usernameEditText.text;
            usernameText.text = playerUsername;
            PlayerPrefs.SetString("playerUsername", playerUsername);
        }

        if (phoneNumberEditText.text != playerPhoneNumber)
        {
            playerPhoneNumber = phoneNumberEditText.text;
            phoneNumberText.text = playerPhoneNumber;
            PlayerPrefs.SetString("playerPhoneNumber", playerPhoneNumber);
        }

        PlayerPrefs.Save();

        saveButton.interactable = false;
    }

    void OnCloseEditPopupClicked() //Closes and Resets the Edit Popup
    {
        editPopup.SetActive(false);
        saveButton.interactable = false;
        usernameEditText.text = playerUsername;
        phoneNumberEditText.text = playerPhoneNumber;
    }
}
