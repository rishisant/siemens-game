using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewManager : MonoBehaviour
{

    // First grab the playerData singleton
    private PlayerData playerData => PlayerData.Instance;

    // Now, we have access to the playerData script (see below)
    // We want to grab the cards that are unlocked
    // and map them to the images, etc

    // We will need to SerializeField the UI image in Unity,
    // the card image text
    // the card name text
    // the card rarity text
    [SerializeField] private UnityEngine.UI.Image cardImage;
    [SerializeField] private TMPro.TMP_Text cardName;
    [SerializeField] private TMPro.TMP_Text cardRarity;

    // Now cardType
    // CardPower
    [SerializeField] private TMPro.TMP_Text cardType;
    [SerializeField] private TMPro.TMP_Text cardPower;

    // We will grab the ButtonRight and Left
    // to cycle through the cards
    [SerializeField] private UnityEngine.UI.Button buttonRight;
    [SerializeField] private UnityEngine.UI.Button buttonLeft;

    // Grab the viewCards gameObject
    [SerializeField] private GameObject viewCards;
    // modalBack
    [SerializeField] private GameObject modalBack;

    // Make a pointer currentCard to track
    // what card we're in in unlocked
    // this goes from 0 -> unlockedCardsLength
    private int currentCard = 0;

    // Get the length of the unlocked cards list
    private int unlockedCardsLength => playerData.unlocked_cards.Count;

    // On Awake
    private void Awake()
    {
        // If there is no instance of PlayerData, set it to this
        if (playerData == null)
        {
            Debug.LogError("PlayerData is null");
        }
    }

    // Rarity Checker
    private string RarityChecker(int rarity)
    {
        // Check the rarity
        switch (rarity)
        {
            case 1:
                return "Common";
            case 2:
                return "Rare";
            case 3:
                return "Ultra-Rare";
            case 4:
                return "Legendary";
            default:
                return "Unknown";
        }
    }

    // OnRight
    public void OnRight()
    {
        // Increment currentCard
        currentCard++;

        // Populate the card
        PopulateCard();
    }

    // OnLeft
    public void OnLeft()
    {
        // Decrement currentCard
        currentCard--;

        // Populate the card
        PopulateCard();
    }


    // PopulateCard
    private void PopulateCard()
    {
        if(unlockedCardsLength==0){cardImage.enabled=false;cardName.text="No cards yet";cardRarity.text=cardType.text=cardPower.text="";buttonLeft.interactable=buttonRight.interactable=false;return;}
        currentCard=Mathf.Clamp(currentCard,0,unlockedCardsLength-1);cardImage.enabled=true;
        // Grab the cardID
        int cardID = playerData.unlocked_cards[currentCard];

        // Grab the card from the dictionary
        PlayerData.Card card = playerData.cards[cardID];

        // Set the card image
        cardImage.sprite = card.image;

        // Set the card name
        // Card name is name of card
        // card power is number of power
        // card type is element
        cardName.text = card.name;
        cardPower.text = card.power.ToString();
        cardType.text = card.element.ToString();

        // Set the card rarity
        cardRarity.text = RarityChecker(card.rarity);
    }

    // ClosePanel
    public void ClosePanel()
    {
        // Close the panel
        viewCards.SetActive(false);

        // set modalBack to false
        modalBack.SetActive(false);
    }

    private void PageCheck()
    {
        // Just runs on Update, checking
        // if we need to disable the buttons
        // or enable them

        // Disable left button if currentCard==0
        if (currentCard == 0)
        {
            buttonLeft.interactable = false;
        }
        else
        {
            buttonLeft.interactable = true;
        }

        // Disable right button if we are on the last card
        if (currentCard == unlockedCardsLength - 1)
        {
            buttonRight.interactable = false;
        }
        else
        {
            buttonRight.interactable = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Populate the card
        PopulateCard();

        // Check if we need to disable the buttons
        PageCheck();
    }

    // Update is called once per frame
    void Update()
    {
        PageCheck(); // Check if we need to disable the buttons
        
    }
}
