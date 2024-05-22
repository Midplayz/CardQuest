using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/* This is the main script that handles the entire gameplay including card interactions, winning & losing conditions,
 Game over screen displaying, UI changes etc. I could have made separate scripts for a neater presentation but I felt
it would overcomplicate the script linking network hence why I stuck with only one script for this.*/

public class CardManager : MonoBehaviour
{
    [Header("Main Game Stuff")]
    [SerializeField] private CardData[] cards;
    [SerializeField] private Image[] handCards;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private Transform droppedCardsArea;
    [SerializeField] private Image deckCard;

    [Header("Post Game Stuff")]
    [SerializeField] private Sprite winLogo;
    [SerializeField] private Sprite loseLogo;
    [SerializeField] private Image resultLogo;
    [SerializeField] private TextMeshProUGUI finalScore;
    [SerializeField] private GameObject gameOverScreen;
    
    private int currentHandValue;
    private int selectedCardIndex = -1;
    private Vector3 originalCardScale;

    private Dictionary<Image, CardData> handCardValues = new Dictionary<Image, CardData>();

    private bool isSwipe;
    private Vector3 originalCardPosition;
    private GameObject draggingCard;

    void Start()
    {
        InitializeHand(); 

        //This part checks whether the user enabled Swipe Input or not and changes the input method accordingly.
        isSwipe = PlayerPrefs.GetInt("isSwipe", 0) == 1;
        if (isSwipe)
        {
            foreach (var card in handCards)
            {
                Button button = card.GetComponent<Button>();
                if (button != null)
                {
                    button.enabled = false;
                }
                AddDragEvents(card.gameObject);
            }
            Debug.Log("Swipe Input");
        }
        else
        {
            Debug.Log("Tap Input");
        }
    }

    void InitializeHand()
    {
        // Selects three random cards
        currentHandValue = 0;
        scoreText.text = "0";
        originalCardScale = handCards[0].transform.localScale;

        for (int i = 0; i < handCards.Length; i++)
        {
            int randomIndex = Random.Range(0, cards.Length);
            handCards[i].sprite = cards[randomIndex].sprite;
            handCards[i].gameObject.SetActive(true);

            // Stores card values in dictionary for reference when dropping
            handCardValues[handCards[i]] = cards[randomIndex];
        }
        Debug.Log("Hand Initialized!");
    }

    void AddDragEvents(GameObject card) //Adds the Swipe and Drag functionality to the cards if the user chose this input method
        //I did not know how else to implement touch controls so I used this method.
    {
        EventTrigger trigger = card.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = card.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entryBeginDrag = new EventTrigger.Entry();
        entryBeginDrag.eventID = EventTriggerType.BeginDrag;
        entryBeginDrag.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });
        trigger.triggers.Add(entryBeginDrag);

        EventTrigger.Entry entryDrag = new EventTrigger.Entry();
        entryDrag.eventID = EventTriggerType.Drag;
        entryDrag.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        trigger.triggers.Add(entryDrag);

        EventTrigger.Entry entryEndDrag = new EventTrigger.Entry();
        entryEndDrag.eventID = EventTriggerType.EndDrag;
        entryEndDrag.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });
        trigger.triggers.Add(entryEndDrag);
    }

    public void OnCardClick(int cardSlotIndex) //Selects or Drops the card that has been clicked on if the user disabled Swipe Input
    {
        if (!isSwipe)
        {
            if (selectedCardIndex == cardSlotIndex)
            {
                DropCard(cardSlotIndex);
            }
            else
            {
                if (selectedCardIndex != -1)
                {
                    DeselectCard(selectedCardIndex);
                }
                SelectCard(cardSlotIndex);
            }
        }
    }

    void SelectCard(int cardSlotIndex) //Basically, saves what card is selected and also enlarges it a bit so you can see that it's selected right now.
    {
        selectedCardIndex = cardSlotIndex;
        handCards[cardSlotIndex].transform.localScale = originalCardScale * 1.2f;
    }

    void DeselectCard(int cardSlotIndex)// As the name suggests, it deselects the selected card.
    {
        handCards[cardSlotIndex].transform.localScale = originalCardScale;
    }

    void DropCard(int cardSlotIndex) //This drops the selected card and instantiates a card in he dropped card area
                                     //as well as disable the card in hand, clearing the spot allowing you to
                                     //draw a card from the deck! It also adds to the score and checks if you won or lost.
    {
        DeselectCard(cardSlotIndex);
        selectedCardIndex = -1;

        GameObject newCard = Instantiate(cardPrefab, cardParent);
        Image newCardImage = newCard.GetComponent<Image>();
        newCardImage.sprite = handCards[cardSlotIndex].sprite;
        newCard.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));

        handCards[cardSlotIndex].gameObject.SetActive(false);
        currentHandValue += handCardValues[handCards[cardSlotIndex]].value;
        scoreText.text = currentHandValue.ToString();
        CheckWinCondition();
    }

    public void OnDeckClick() //When you click on the deck, it will add a card to your hand if there is an empty slot.
    {
        if (CountActiveCards() < 3)
        {
            int randomIndex = Random.Range(0, cards.Length);
            for (int i = 0; i < handCards.Length; i++)
            {
                if (!handCards[i].gameObject.activeSelf)
                {
                    handCards[i].sprite = cards[randomIndex].sprite;
                    handCards[i].gameObject.SetActive(true);

                    handCardValues[handCards[i]] = cards[randomIndex];
                    break;
                }
            }
        }
    }

    int CountActiveCards()//Checks how many cards you currently have in hand and returns that integer.
    {
        int count = 0;
        foreach (var card in handCards)
        {
            if (card.gameObject.activeSelf)
            {
                count++;
            }
        }
        return count;
    }

    private void CheckWinCondition() //Checks whether you won or lost and calls the Show End Screen function.
    {
        if (currentHandValue == 26)
        {
            Debug.Log("You win!");
            ShowEndScreen(true);
        }
        else if (currentHandValue > 26)
        {
            Debug.Log("You Lost!");
            ShowEndScreen(false);
        }
    }

    private void ShowEndScreen(bool hasWon) //Shows the end screen and updates the UI Elements depending on whether you won or lost.
    {
        Debug.Log("Showing End Screen...");
        UpdatePlayerStats(hasWon);
        gameOverScreen.SetActive(true);
        Sprite spriteToUse = hasWon ? winLogo : loseLogo;
        resultLogo.sprite = spriteToUse;
        finalScore.text = "Final Score: " + currentHandValue.ToString();
    }

    public void UpdatePlayerStats(bool hasWon) //This updates the Wins and Losses values in Player prefs.
    {
        string key = hasWon ? "playerWins" : "playerLosses";
        int currentValue = PlayerPrefs.GetInt(key, 0);
        PlayerPrefs.SetInt(key, currentValue + 1);
        PlayerPrefs.Save();
        Debug.Log("Updated Player Stats!");
    }

    public void ReplayGame() //Allows you to re-play the game
    {
        Debug.Log("Reloading Scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToHome() //Loads the Dashboard Screen.
    {
        Debug.Log("Loading Dashboard...");
        SceneManager.LoadScene("LoginScreen");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggingCard = eventData.pointerDrag;
        originalCardPosition = draggingCard.transform.position;
        Debug.Log("Begin Drag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingCard != null)
        {
            draggingCard.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        if (draggingCard == null) return;

        //checks whetehr you dropped the card over the Dropped Cards area and marks it as dropped
        if (RectTransformUtility.RectangleContainsScreenPoint(droppedCardsArea as RectTransform, eventData.position))
        {
            Image newCardImage = Instantiate(cardPrefab, cardParent).GetComponent<Image>();
            newCardImage.sprite = draggingCard.GetComponent<Image>().sprite;
            newCardImage.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));

            currentHandValue += handCardValues[draggingCard.GetComponent<Image>()].value;
            scoreText.text = currentHandValue.ToString();
            CheckWinCondition();

            draggingCard.SetActive(false);
        }
        else if (CountActiveCards() < 3 && draggingCard.transform.parent == cardParent)
        {
            // Handles adding a card from deck to hand if empty slot exists
            for (int i = 0; i < handCards.Length; i++)
            {
                if (!handCards[i].gameObject.activeSelf)
                {
                    handCards[i].sprite = draggingCard.GetComponent<Image>().sprite;
                    handCards[i].gameObject.SetActive(true);

                    handCardValues[handCards[i]] = handCardValues[draggingCard.GetComponent<Image>()];
                    break;
                }
            }
        }

        // Returns the card to original position
        draggingCard.transform.position = originalCardPosition;
        draggingCard = null;
    }
}