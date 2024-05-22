using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DashboardUIController : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject helpScreen;
    [SerializeField] private GameObject profileScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject defaultScreen;

    private Dictionary<string, GameObject> screens;

    [Header("Watermark")]
    [SerializeField] private WatermarkTextData watermarkTextData;
    [SerializeField] private TextMeshProUGUI watermarkText;

    private int currentIndex = 0;


    private void Start()
    {   //Here, I disable all other screens except the default screen which is the main screen.
        //It's just a fail-safe just in case I set another screen as enabled before publishing the game.
        screens = new Dictionary<string, GameObject>
        {
            { "help", helpScreen },
            { "profile", profileScreen },
            { "settings", settingsScreen },
            { "default", defaultScreen }
        };

        foreach (var screen in screens)
        {
            screen.Value.SetActive(false);
        }

        defaultScreen.SetActive(true);

        if (watermarkTextData != null && watermarkTextData.texts.Length > 0)//Starts the Watermark text cycling coroutine using the scriptable object i made
        {
            StartCoroutine(UpdateWatermarkText());
        }
    }

    public void ToggleScreen(string screenName) //Enabled or disabled a screen based on the name
    {
        if (screens.ContainsKey(screenName))
        {
            screens[screenName].SetActive(!screens[screenName].activeSelf);
            defaultScreen.SetActive(!defaultScreen.activeSelf);
        }
        else
        {
            Debug.LogWarning("Screen not found: " + screenName);
        }
    }

    public void PlayGame() //Goes to the Game Scene
    {
        Debug.Log("Game Starting....");
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScreen");
    }

    public void ExitGame() //As the name suggests, it runs the Application.Quit() function
    {
        Debug.Log("Quitting Game....");
        Application.Quit();
    }
    private IEnumerator UpdateWatermarkText() //Changes the text of the watermark in order every 1 second and loops
    {
        while (true)
        {
            watermarkText.text = watermarkTextData.texts[currentIndex];
            currentIndex = (currentIndex + 1) % watermarkTextData.texts.Length;
            yield return new WaitForSeconds(1f);
        }
    }
}
