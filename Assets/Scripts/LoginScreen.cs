using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/* This is a simple Mobile Number Login Page. There are many edge cases I did not involve due to the lack of time and
 to reduce the complexity of the code. For instance:
* There should be a back button to allow the user to change the number if needed.
* There should be a countdown to indicate when the OTP expires.
* There should be a button to resend the OTP.
* You should use a service like Firebase Login to integrate actual Mobile Number Login.
* There should be options to choose the country code for the number.

So, my version is just simulating how a very basic Mobile Number Login system would work. In all honesty, it can be 
simplified even more.*/

public class LoginScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI placeholderText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button submitButton;

    private int currentInputLength; // This is so that the Button's On Click Event knows what data is being submitted.
    private bool hasSubmittedNumber = false;
    private string phoneNumber;

    void Start()
    {
        if (PlayerPrefs.HasKey("PhoneNumber"))//This checks if the Phone Number is already saved then continues to dashboard.
        {
            phoneNumber = PlayerPrefs.GetString("PhoneNumber");
            Debug.Log("Phone number already saved: " + phoneNumber);
            LoadDashboard();
        }
        else//This is if it is a new user
        {
            submitButton.interactable = false;
            inputField.onValueChanged.AddListener(CheckInputLength);
            titleText.text = "Enter your mobile number:";
        }
    }

    void CheckInputLength(string input)
    {
        if (input.Length == 10 && !hasSubmittedNumber) // This is for Phone Number Submission
        {
            submitButton.interactable = true;
            currentInputLength = input.Length;
        }
        else if (input.Length == 4 && hasSubmittedNumber) // This is for OTP Submission
        {
            submitButton.interactable = true;
            currentInputLength = input.Length;
        }
        else
        {
            submitButton.interactable = false;
        }
    }

    public void OnClickSubmit()
    {
        if (currentInputLength == 10) // For Phone Number
        {
            phoneNumber = inputField.text;
            hasSubmittedNumber = true;
            inputField.text = string.Empty;
            submitButton.interactable = false;
            inputField.characterLimit = 4;
            placeholderText.text = "OTP is 0000";
            titleText.text = "Enter OTP:";
        }
        else if (currentInputLength == 4) // For OTP
        {
            if (inputField.text != "0000") // If OTP is incorrect
            {
                Debug.Log("Incorrect OTP!");

                Handheld.Vibrate();
                StartCoroutine(IncorrectOTP());
                inputField.text = string.Empty;
                submitButton.interactable = false;
            }
            else
            {
                Debug.Log("Correct OTP!");

                PlayerPrefs.SetString("PhoneNumber", phoneNumber);
                PlayerPrefs.Save();
                
                LoadDashboard();
            }
        }
    }

    IEnumerator IncorrectOTP() // This Coroutine changes the Placeholder text for one second. Just cosmetic.
    {
        placeholderText.text = "Incorrect OTP!";
        yield return new WaitForSeconds(1);
        placeholderText.text = "OTP is 0000";
    }

    void LoadDashboard()// Loads the next scene which is the dashboard.
    {
        Debug.Log("Loading dashboard...");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
    }
}