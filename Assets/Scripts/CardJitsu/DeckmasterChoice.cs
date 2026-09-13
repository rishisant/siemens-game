using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckmasterChoice : MonoBehaviour
{

    // Grab playerData
    private PlayerData playerData => PlayerData.Instance;

    // Grab the backModal object
    [SerializeField] private GameObject backModal;

    // Grab the choices panel
    [SerializeField] private GameObject choicesPanel;

    // Grab the ViewCards object
    [SerializeField] private GameObject viewCards;

    // Grab the dialogueManager
    [SerializeField] private DialogueManager_Lab dialogueManager;

    private void AddChallengeButton()
    {
        var button = TownMultiplayerUI.MakeButton(choicesPanel.transform,"Challenge Deckmaster",new Vector2(-240,12),new Vector2(480,54),()=>{closeChoiceManager();DeckmasterDuel.Show();});
        var rect=button.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=new Vector2(.5f,0);
    }

    // First choice
    public void viewCardOwned()
    {
        closeChoiceManager();viewCards.SetActive(false);
        CardGallery.Show("Your collection",playerData.unlocked_cards,ShowChoices);
    }
    public void ShowChoices() {closeChoiceManager();DeckmasterHub.Show(this);}
    private void ShowUnlockedCards()
    {
        cardUnlockingPanel.SetActive(false);closeChoiceManager();
        string title=TextofCardPackRarity.text=="Beginning"?"Starter cards":TextofCardPackRarity.text+" card pack";
        CardGallery.Show(title,cardsUnlockedOnBuy,ShowChoices);
    }

    // CloseChoiceManager
    public void closeChoiceManager()
    {
        // Set the choicesPanel to false
        choicesPanel.SetActive(false);
        // set modal false
        backModal.SetActive(false);
    }

    // Grab the cardUnlockingPanel
    [SerializeField] private GameObject cardUnlockingPanel;
    // leftButton of cardUnlock
    [SerializeField] private UnityEngine.UI.Button leftButton;
    // rightButton of cardUnlock
    [SerializeField] private UnityEngine.UI.Button rightButton;
    // TextofCardPackRarity
    [SerializeField] private TMPro.TMP_Text TextofCardPackRarity;

    // For card unlocking panel, grab
    // the cardImage
    [SerializeField] private UnityEngine.UI.Image cardImage;
    // the cardName
    [SerializeField] private TMPro.TMP_Text cardName;
    // the cardRarity
    [SerializeField] private TMPro.TMP_Text cardRarity;
    // cardType
    [SerializeField] private TMPro.TMP_Text cardType;
    // cardPower
    [SerializeField] private TMPro.TMP_Text cardPower;

    // Second choice is buy a normal card pack
    // third choice is buy a rare card pack

    // For normal card pack
    // You should unlock mostly commons, maybe rare, and very rarely ultra-rare,
    // with a super low chance of legendary (say 0.01%)

    // For rare card pack,
    // you should unlock 1 common at least, 1 rare at least, and 33% chance of ultra-rare,
    // with a 2% chance of legendary

    // Let's make functions to handle these
    // keep in mind 1=common, 2=rare, 3=ultra-rare, 4=legendary

    // We want to have all the normal card ids, the rare card ids, and the ultra-rare card ids, legendary separated
    public List<int> normalCardIds = new List<int> { 0, 4, 6, 13, 16, 9 };
    public List<int> rareCardIds = new List<int> { 1, 2, 11, 12, 14 };
    public List<int> ultraRareCardIds = new List<int> { 7, 8, 10, 15, 17 };
    public List<int> legendaryCardIds = new List<int> { 3, 5, 12 };

    // List of ints for the cards that were unlocked (this will always be reset)
    // after buying a new set of cards
    private List<int> cardsUnlockedOnBuy = new List<int>();
    public int currentCardUnlocked = 0;

    // Now we'll make the buying function and add all the things to cardsUnlockedOnBuy
    // Buy normal card pack

    // Current card
    private int currentCard = 0;

    // Function to fade in the unlocked panel
    // alphas
    public IEnumerator FadeInUnlockedPanel()
    {
        // Set the alpha to 0
        float alpha = 0;
        // Set the alpha of the cardUnlockingPanel to 0
        cardUnlockingPanel.GetComponent<CanvasGroup>().alpha = 0;
        // While the alpha is less than 1
        while (alpha < 1)
        {
            // Increment the alpha
            alpha += Time.deltaTime;
            // Set the alpha of the cardUnlockingPanel to alpha
            cardUnlockingPanel.GetComponent<CanvasGroup>().alpha = alpha;
            // Wait for the end of frame
            yield return new WaitForEndOfFrame();
        }
    }

    // buy StartingCards
    public void buyStarting()
    {
        // Just give 3 common, 2 rare, 1 ultra rare or 4 common 2 rare
        // mostly on the latter side
        // remove unlocked cards
        cardsUnlockedOnBuy.Clear();

        // The normal IDS that I will give are
        // id=4, id=0, id=9 , id=1, id=8
        // or possibly id=6, id=4, id=0, id=13 id=11, id=1

        // yo uhave 50% chance of getting either
        int random = Random.Range(0, 2);

        if (random == 0)
        {
            cardsUnlockedOnBuy.Add(4);
            cardsUnlockedOnBuy.Add(0);
            cardsUnlockedOnBuy.Add(9);
            cardsUnlockedOnBuy.Add(1);
            cardsUnlockedOnBuy.Add(8);
        } else
        {
            cardsUnlockedOnBuy.Add(6);
            cardsUnlockedOnBuy.Add(4);
            cardsUnlockedOnBuy.Add(0);
            cardsUnlockedOnBuy.Add(13);
            cardsUnlockedOnBuy.Add(11);
            cardsUnlockedOnBuy.Add(1);
        }

        // Purge cards unlocked on buy of duplicates
        cardsUnlockedOnBuy = new List<int>(new HashSet<int>(cardsUnlockedOnBuy));

        // now loop through cards unlocked on buy
        // and add to playerData.unlocked cards if not already there
        foreach (int card in cardsUnlockedOnBuy)
        {
            if (!playerData.unlocked_cards.Contains(card))
            {
                playerData.unlocked_cards.Add(card);
            }
        }


        // Now set the currentCard to 0
        currentCard = 0;

        // text of card pack rarity
        TextofCardPackRarity.text = "Beginning";

        // Now populate the card
        PopulateCard();

        // fade in the unlocked panel


        // Set the cardUnlockingPanel to active
        ShowUnlockedCards();
    }

    // Buy normal
    public void buyNormal()
    {
        // remove unlocked cards
        cardsUnlockedOnBuy.Clear();

        // Costs 125
        if (playerData.coins < 125)
        {
            // Not enough coins
            // use the dialogue manager to show a message

            return;
        } else {
            // Subtract 125 coins
            playerData.coins -= 125;
            // Let's make sure we give a highest chance for common cards
            // and a very low chance for legendary cards etc
            // and essentially what we're going to do is generate a random
            // number between 0 and 100
            // and check that number against the rarity
            // if it's less than 95 it's common, if it's less than 98 it's rare
            // if it's less than 99 it's ultra-rare, if it's less than 100 it's legendary
            // and then we'll add that card to the cardsUnlockedOnBuy list
            for (int i = 0; i < 5; i++)
            {
                int random = Random.Range(0, 100);
                if (random < 85)
                {
                    // Common
                    int randomCard = Random.Range(0, normalCardIds.Count);
                    // cardsUnlockedOnBuy.Add(normalCardIds[randomCard]);

                    // if this card already is in the unlocked cards, just don't add
                    // to cardsUnlockedOnBuy and add back 5 coins
                    if (playerData.unlocked_cards.Contains(normalCardIds[randomCard]))
                    {
                        playerData.coins += 5;
                    } else
                    {
                        cardsUnlockedOnBuy.Add(normalCardIds[randomCard]);
                    }
                } else if (random < 95)
                {
                    // Rare
                    int randomCard = Random.Range(0, rareCardIds.Count);
                    // cardsUnlockedOnBuy.Add(rareCardIds[randomCard]);
                    if (playerData.unlocked_cards.Contains(rareCardIds[randomCard]))
                    {
                        playerData.coins += 15;
                    } else
                    {
                        cardsUnlockedOnBuy.Add(rareCardIds[randomCard]);
                    }
                } else if (random < 99)
                {
                    // Ultra-Rare
                    int randomCard = Random.Range(0, ultraRareCardIds.Count);
                    // cardsUnlockedOnBuy.Add(ultraRareCardIds[randomCard]);
                    if (playerData.unlocked_cards.Contains(ultraRareCardIds[randomCard]))
                    {
                        playerData.coins += 50;
                    } else
                    {
                        cardsUnlockedOnBuy.Add(ultraRareCardIds[randomCard]);
                    }
                } else
                {
                    // Legendary
                    int randomCard = Random.Range(0, legendaryCardIds.Count);
                    // cardsUnlockedOnBuy.Add(legendaryCardIds[randomCard]);
                    if (playerData.unlocked_cards.Contains(legendaryCardIds[randomCard]))
                    {
                        playerData.coins += 85;
                    } else
                    {
                        cardsUnlockedOnBuy.Add(legendaryCardIds[randomCard]);
                    }
                }
            }

            // Purge cards unlocked on buy of duplicates
            cardsUnlockedOnBuy = new List<int>(new HashSet<int>(cardsUnlockedOnBuy));

            // now loop through cards unlocked on buy
            // and add to playerData.unlocked cards if not already there
            foreach (int card in cardsUnlockedOnBuy)
            {
                if (!playerData.unlocked_cards.Contains(card))
                {
                    playerData.unlocked_cards.Add(card);
                }
            }

            // Now set the currentCard to 0
            currentCard = 0;

            // text of card pack rarity
            TextofCardPackRarity.text = "Common";

            // Now populate the card
            PopulateCard();

            // fade in the unlocked panel


            // Set the cardUnlockingPanel to active
            ShowUnlockedCards();
        }
    }

    // Buy a rare card pack
    public void buyRare()
    {
        // Unlcokedcards should be unfilled
        cardsUnlockedOnBuy.Clear();

        // Costs 250
        if (playerData.coins < 500)
        {
            // Not enough coins
            Debug.Log("Not enough coins");
            return;
        } else {
            // Subtract 250 coins
            playerData.coins -= 500;
            // Let's make sure we give a highest chance for common cards
            // and a very low chance for legendary cards etc
            // and essentially what we're going to do is generate a random
            // number between 0 and 100
            // and check that number against the rarity
            // if it's less than 85 it's common, if it's less than 95 it's rare
            // if it's less than 99 it's ultra-rare, if it's less than 100 it's legendary
            // and then we'll add that card to the cardsUnlockedOnBuy list
            for (int i = 0; i < 5; i++)
            {
                int random = Random.Range(0, 100);
                if (random < 55)
                {
                    // Common
                    int randomCard = Random.Range(0, normalCardIds.Count);
                    // cardsUnlockedOnBuy.Add(normalCardIds[randomCard]);
                    if (playerData.unlocked_cards.Contains(normalCardIds[randomCard]))
                    {
                        playerData.coins += 5;
                    } else
                    {
                        cardsUnlockedOnBuy.Add(normalCardIds[randomCard]);
                    }
                } else if (random < 85)
                {
                    // Rare
                    int randomCard = Random.Range(0, rareCardIds.Count);
                    // cardsUnlockedOnBuy.Add(rareCardIds[randomCard]);
                    if (playerData.unlocked_cards.Contains(rareCardIds[randomCard]))
                    {
                        playerData.coins += 15;
                    } else
                    {
                        cardsUnlockedOnBuy.Add(rareCardIds[randomCard]);
                    }
                } else if (random < 98)
                {
                    // Ultra-Rare
                    int randomCard = Random.Range(0, ultraRareCardIds.Count);
                    // cardsUnlockedOnBuy.Add(ultraRareCardIds[randomCard]);
                    if (playerData.unlocked_cards.Contains(ultraRareCardIds[randomCard]))
                    {
                        playerData.coins += 50;
                    } else
                    {
                        cardsUnlockedOnBuy.Add(ultraRareCardIds[randomCard]);
                    }
                } else
                {
                    // Legendary
                    int randomCard = Random.Range(0, legendaryCardIds.Count);
                    // cardsUnlockedOnBuy.Add(legendaryCardIds[randomCard]);
                    if (playerData.unlocked_cards.Contains(legendaryCardIds[randomCard]))
                    {
                        playerData.coins += 85;
                    } else
                    {
                        cardsUnlockedOnBuy.Add(legendaryCardIds[randomCard]);
                    }
                }
            }

            // Purge cards unlocked on buy of duplicates
            cardsUnlockedOnBuy = new List<int>(new HashSet<int>(cardsUnlockedOnBuy));

            // now loop through cards unlocked on buy
            // and add to playerData.unlocked cards if not already there
            foreach (int card in cardsUnlockedOnBuy)
            {
                if (!playerData.unlocked_cards.Contains(card))
                {
                    playerData.unlocked_cards.Add(card);
                }
            }

            // Now set the currentCard to 0
            currentCard = 0;

            // text of card pack rarity
            TextofCardPackRarity.text = "Rare";

            // Now populate the card
            PopulateCard();

            // fade in the unlocked panel


            // Set the cardUnlockingPanel to active
            ShowUnlockedCards();

        }
    }


    // Rarity Checker
    public string RarityChecker(int rarity)
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

    // PopulateCard
    public void PopulateCard()
    {
        // // DEBUG
        // // what is the sized of cardsUnlockedOnBuy
        // Debug.Log("Size of cardsUnlockedOnBuy: " + cardsUnlockedOnBuy.Count);

        // These are for the unlocked cards during the first buy
        // if the currentCard is less than the length of the cardsUnlockedOnBuy
        if (currentCard < cardsUnlockedOnBuy.Count)
        {
            // Set the cardImage to the image of the card
            cardImage.sprite = playerData.cards[cardsUnlockedOnBuy[currentCard]].image;
            // Set the cardName to the name of the card
            cardName.text = playerData.cards[cardsUnlockedOnBuy[currentCard]].name;
            // Set the cardRarity to the rarity of the card
            cardRarity.text = RarityChecker(playerData.cards[cardsUnlockedOnBuy[currentCard]].rarity);
            // Set the cardType to the type of the card
            cardType.text = playerData.cards[cardsUnlockedOnBuy[currentCard]].element.ToString();
            // Set the cardPower to the power of the card
            cardPower.text = playerData.cards[cardsUnlockedOnBuy[currentCard]].power.ToString();
        }
    }

    // Left or right button
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


    // Close the panel of unlockedCards
    public void closePanel()
    {
        // Set the viewCards object to active
        viewCards.SetActive(false);
        // set modal false
        backModal.SetActive(false);

        // Close the choicepanel
        choicesPanel.SetActive(false);
    }

    // Close the unlockedPanel
    public void closeUnlockedPanel()
    {
        // Set the cardUnlockingPanel to false
        cardUnlockingPanel.SetActive(false);
        // set modal false
        backModal.SetActive(false);

        // Close the choicepanel
        choicesPanel.SetActive(false);
    }

    // Awake
    private void Awake()
    {
        // Add a canvas group to the cardUnlockingPanel
        cardUnlockingPanel.AddComponent<CanvasGroup>();

        // Set the alpha of the cardUnlockingPanel to 0
        cardUnlockingPanel.GetComponent<CanvasGroup>().alpha = 0;
    }

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        // if the cardUnlockingPanel is active, check if the currentCard is 0
        // if it is, disable the left button
        if (cardUnlockingPanel.activeSelf)
        {
            if (currentCard == 0)
            {
                leftButton.interactable = false;
            }
            else
            {
                leftButton.interactable = true;
            }

            // if the currentCard is the last card, disable the right button
            if (currentCard == cardsUnlockedOnBuy.Count - 1)
            {
                rightButton.interactable = false;
            }
            else
            {
                rightButton.interactable = true;
            }
        }

    }
}
