using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief This handles the dialogue in the Laboratory. This consists of
 * sensei's dialogue at the beginning as well as the deckmaster's dialogue
 *
 * @see DialogueManager
 */
public class DialogueManager_Lab : DialogueManagerBase
{
    // PlayerData
    private PlayerData playerData => PlayerData.Instance;
    // We are going to set to "shopopen" once the player talks with the shop guy
    // Keep in mind if the npc_interactions with shopkeeper is 0, the shopkeeper
    // will have a short explanation before he opens the shop

    // isTyping, typingSpeed, dialogueText and the TypeSentence coroutine
    // live in DialogueManagerBase

    // Charname
    public TMPro.TextMeshProUGUI charName;
    // image for the character
    public UnityEngine.UI.Image characterImage;
    // TTC text
    public TMPro.TextMeshProUGUI TTC_Text;

    // import Camera_Movement script
    [SerializeField] private CameraFollow cameraFollow;
    // import PlayerMovement script
    [SerializeField] private Character_Movement playerMovement;

    // Open the choice panel
    [SerializeField] private GameObject choicePanel;

    // Modal back
    [SerializeField] public GameObject backModal;

    // Load in the dialoguePanel
    // Look for UI-Panel Dialogue-Panel and assign it
    public GameObject dialoguePanel;

    // Text for the dialoguePanel
    // Text for the name of the NPC

    // public int dialogueindex
    private int dialogueIndex = 0;

    // DeckmasterChoice script
    [SerializeField] private DeckmasterChoice deckmasterChoice;

    // Card view panel
    [SerializeField] private GameObject cardViewPanel;
    // Add all the casino owner sprites
    [SerializeField] private Sprite[] deckmasterSprites; // 0->serious, 1-> serious with hands, 2->charismatic, 3->happy with hands

    // Add all the casino owner dialogues
    private string[] deckMasterInitial = {
        "Hey. I'm the Deckmaster. You want to play some cards?",
        "I'm the best card player in all of Byte City. I live, eat, and breathe cards.",
        "Well, not necessarily. I don't live, nor do I eat, nor do I breathe... But that's beside the point.",
        "Before you even hop into those games, you gotta speak with me. I'll give you a rundown of the rules.",
        "There are three types of cards in this game: heat cards, pressure cards, and electricity cards.",
        "Heat beats pressure, pressure beats electricity, and electricity beats heat.",
        "Each card has its own level of power. The higher the power, the rarer the card is, and therefore, the more useful.",
        "There are three power ranges that dictate card rarity. Cards with power from 3-6 are common. Cards with power from 7-11 are rare. And cards with power from 12+ are legendary.",
        "Even with legendary cards, you're not always guaranteed to win. It's all about strategy. If I had a 11 electricity, and you had a 3 pressure, even though my card is higher power, I lose.",
        "So, you ready to play? I'm going to give you a few cards from here. You can start playing with them. Bonne chance!"
    };

    // Deckmaster lines (non-initial), so randomized
    private string[] deckMasterRandom = {
        "Cards are like life. You never know what you're going to get. Well, I guess you're about to, since you want to buy some from me.",
        "I've been playing cards for years. I've seen it all. I've seen the best, and I've seen the worst. And you'll see it too.",
        "You know, I've been thinking about it. I should probably get a real name. Deckmaster is just... weird. Well, some people say it is.",
        "Do you like me? Do you hate me? Do you wish I was more charismatic? Well, I'm trying my best.",
        "I've never done flux before. Come to think of it, I've never done anything before. I'm just a card dealer.",
        "I used to be a big video game junkie back when I was... a human? Oh right, I used to be a human. What a life.",
        "Jobs are for people who don't make enough money to sit at home and play cards all day. Am I right?",
        "Hello, hello, hello... This is the Deckmaster speaking. I'm here to give you some cards. You want some cards? I want some cards. Let's trade.",
        "Give me money, I give you cards. Give me a middle finger, I give you a middle finger. It's all about the exchange.",
        "I am your father. Well, I probably was. Who knows? I could be anyone? I could be you. I could be me. I could be the cards I'm holding in my pocket.",
        "I'm the Deckmaster. I'm the best card player in all of Byte City. I live, eat, and breathe cards. And you are, an amateur. But that's okay. We all start somewhere.",
        "There used to be this thing called the alphabet. I used to know it. Now I can only count to seventeen, and I forgot the order."
    };

    private int[] deckMasterRandomSprites = {
        1, 1, 0, 1, 2, 1, 1, 3, 1, 2, 3, 1
    };

    // Integer list of inital sprites
    private int[] deckMasterInitialSprites = {
        2, 1, 0, 1, 2, 3, 1, 2, 1, 3
    };

    // Make the deckmaster speak
    public void DeckMasterSpeak()
    {
        // Stop player
        playerMovement.StopPlayer();

        // Turn on the dialogue panel
        dialoguePanel.SetActive(true);

        // First, say the first dialogue, second dialogue, third dialogue, pan(0), fourth dialogue, pan(1), fifth dialogue, pan(2), sixth dialogue
        // If the casino owner has never been interacted with, do the initial coroutine
        // otherwise just the normal coroutine
        if (playerData.npc_interactions["deckmaster"] == 0)
        {
            StartCoroutine(DeckMasterSpeakInitial());
        }
        else
        {
            StartCoroutine(DeckMasterSpeakCoroutine());
        }

        // Increment the npc_interactions for deckmaster
        playerData.npc_interactions["deckmaster"] += 1;
    }

    public void DeckMasterInterrupt()
    {
        // Stop player
        playerMovement.StopPlayer();

        // Turn on the dialogue panel
        dialoguePanel.SetActive(true);

        // Now just start the coroutine for DeckMasterInterrupt()
        StartCoroutine(DeckMasterInterruptCoroutine());
        
        // That's all.
    }

    private IEnumerator DeckMasterInterruptCoroutine()
    {
        // Change TTC_Text to "Do Not Tap."
        TTC_Text.text = "Do Not Tap...";

        // Just one dialogue
        // If the coroutine is not null, stop the coroutine
        if (typeSentenceCoroutine != null)
        {
            StopCoroutine(typeSentenceCoroutine);
        }

        // Set the dialogueText to an empty string
        dialogueText.text = "";

        // Pick a random length
        int i = Random.Range(0, deckMasterHaventSpoken.Length);

        // Now set the characterImage to the appropriate sprite
        characterImage.sprite = deckmasterSprites[deckMasterHaventSpokenSprites[i]];

        // Set the charName to "Deckmaster"
        charName.text = "Deckmaster";

        // Start typing the sentence
        isTyping = true;
        typeSentenceCoroutine = StartCoroutine(TypeSentence(deckMasterHaventSpoken[i]));

        // Wait
        yield return new WaitForSeconds(deckMasterHaventSpoken[i].Length * typingSpeed + 1.25f);

        // Change TTC_Text to "Tap to Continue."
        TTC_Text.text = "Tap to Continue...";

        // Close the dialogue panel
        dialoguePanel.SetActive(false);

        // Let player move
        playerMovement.UnstopPlayer();
    }

    // Dialogues for trying to enter the card game without talking to deckmaster
    private string[] deckMasterHaventSpoken = {
        "Hey, hey, hey! You can't just walk in here and start playing cards. You gotta talk to me first.",
        "What you doing? You gotta have a convo with the deckmaster before you get to tossing them cards.",
        "Hola. You speak Spanish? Tu necesitas hablar conmigo antes de jugar cartas. That's a good Spanish accent, right?",
        "Ok. You're probably making an honest mistake. I'll forgive that. However, you gotta talk to me before you play cards. That's the rule."
    };

    private int[] deckMasterHaventSpokenSprites = {
        0, 0, 1, 0
    };

    // Coroutine for typing the sentence
    private Coroutine typeSentenceCoroutine;

    private IEnumerator DeckMasterSpeakInitial()
    {
        // Change TTC_Text to "Do Not Tap."
        TTC_Text.text = "Do Not Tap...";

        for (int i = 0; i < deckMasterInitial.Length; i++)
        {
            // If the coroutine is not null, stop the coroutine
            if (typeSentenceCoroutine != null)
            {
                StopCoroutine(typeSentenceCoroutine);
            }

            // Set the dialogueText to an empty string
            dialogueText.text = "";

            // Set the characterImage to the appropriate sprite
            characterImage.sprite = deckmasterSprites[deckMasterInitialSprites[i]];

            // Set the charName to "Deckmaster"
            charName.text = "Deckmaster";

            // Start typing the sentence
            isTyping = true;
            typeSentenceCoroutine = StartCoroutine(TypeSentence(deckMasterInitial[i]));

            // Wait
            yield return new WaitForSeconds(deckMasterInitial[i].Length * typingSpeed + 1f);

            // Wait
            yield return new WaitForSeconds(2);
        }

        // Change TTC_Text to "Tap to Continue."
        TTC_Text.text = "Tap to Continue...";

        // Close the dialogue panel
        dialoguePanel.SetActive(false);

        // Let player move
        playerMovement.UnstopPlayer();

        // Now call DeckmasterChoice script's buyStarting()
        deckmasterChoice.buyStarting();

        // Increment the npc_interactions for deckmaster
        playerData.npc_interactions["deckmaster"] += 1;
    }

    // Deckmaster coroutine
    private IEnumerator DeckMasterSpeakCoroutine()
    {
        // If the coroutine is not null, stop the coroutine
        if (typeSentenceCoroutine != null)
        {
            StopCoroutine(typeSentenceCoroutine);
        }

        // Set the dialogueText to an empty string
        dialogueText.text = "";

        // Set the characterImage to the appropriate sprite
        int i = Random.Range(0, deckMasterRandom.Length);
        characterImage.sprite = deckmasterSprites[deckMasterRandomSprites[i]];

        // Set the charName to "Deckmaster"
        charName.text = "Deckmaster";

        // Start typing the sentence
        isTyping = true;
        typeSentenceCoroutine = StartCoroutine(TypeSentence(deckMasterRandom[i]));

        // TTC
        TTC_Text.text = "Tap to Continue...";

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Began)));

        // Close the dialogue panel
        dialoguePanel.SetActive(false);

        // Open the choice panel
        choicePanel.SetActive(true);

        // Let player move
        playerMovement.UnstopPlayer();
    }

    // We're going to make a function for NotEnoughCoins to buy a pack for Deckmaster
    // We're going to make another function for Can'tViewCards to view the cards






    // Public integer array for each drunkGuySprites[n] to correspond to drunkGuyDialogues[n]

    // We're going to have a set of dialogues on first interact with the shop owner
    // Start is called before the first frame update
    void Start()
    {
        // Debug to test how sensei interactions are stored
        // Debug.Log("sensei interactions: " + playerData.npc_interactions["sensei"]);

        // Assign the dialoguePanel
        if (dialoguePanel == null)
        {
            dialoguePanel = GameObject.Find("Dialogue-Panel");
        }
        if (dialogueText == null)
        {
            dialogueText = GameObject.Find("Current-Text").GetComponent<TMPro.TextMeshProUGUI>();
        }
        if (charName == null)
        {
            charName = GameObject.Find("Char-Name").GetComponent<TMPro.TextMeshProUGUI>();
        }
        if (characterImage == null)
        {
            characterImage = GameObject.Find("Character-Image").GetComponent<UnityEngine.UI.Image>();
        }

        // if casinoenter=0, start the sensei tutorial
        if (playerData.npc_interactions["labenter"] == 0)
        {
            // Sensei Tutorial
            SenseiTutorial();
        }
    }

    // GameObjects to pan the Camera to
    [SerializeField] private GameObject[] cameraPanTargets; // Four targets, leaderboards,datacentercomp1,datacentercomp2,deckmaster

    // Sensei Town Square tutorial
    public void SenseiTutorial()
    {
        // Stop player
        playerMovement.StopPlayer();

        // Turn on the dialogue panel
        dialoguePanel.SetActive(true);

        // First, say the first dialogue, second dialogue, third dialogue, pan(0), fourth dialogue, pan(1), fifth dialogue, pan(2), sixth dialogue
        StartCoroutine(SenseiTutorialCoroutine());

        // Increment the npc_interactions for casinoenter
        playerData.npc_interactions["labenter"] = 1;
    }

    private IEnumerator SenseiTutorialCoroutine()
    {
        // Change TTC_Text to "Do Not Tap."
        TTC_Text.text = "Do Not Tap...";

        for (int i = 0; i < senseiDialogues.Length; i++)
        {
            // If the coroutine is not null, stop the coroutine
            if (typeSentenceCoroutine != null)
            {
                StopCoroutine(typeSentenceCoroutine);
            }

            // Set the dialogueText to an empty string
            dialogueText.text = "";

            // Set the characterImage to the appropriate sprite
            characterImage.sprite = senseiSprites[senseiSpriteIndices[i]];

            // Set the charName to "Sensei"
            charName.text = "Sensei";

            // Start typing the sentence
            isTyping = true;
            typeSentenceCoroutine = StartCoroutine(TypeSentence(senseiDialogues[i]));

            // Wait
            yield return new WaitForSeconds(senseiDialogues[i].Length * typingSpeed + 1.25f);

            // if i = 2, pan to the casino owner
            if (i==4)
            {
                cameraFollow.PanCamera(cameraPanTargets[0].transform);
            } else if (i==5)
            {
                cameraFollow.PanCamera(cameraPanTargets[1].transform);
            } else if (i==6)
            {
                cameraFollow.PanCamera(cameraPanTargets[2].transform);
            } else if (i==7)
            {
                cameraFollow.PanCamera(cameraPanTargets[3].transform);
            }

            // Wait
            yield return new WaitForSeconds(1.5f);
        }

        // Change TTC_Text to "Tap to Continue."
        TTC_Text.text = "Tap to Continue...";

        // Close the dialogue panel
        dialoguePanel.SetActive(false);

        // Let player move
        playerMovement.UnstopPlayer();
    }

    // Sensei sprites
    [SerializeField] private Sprite[] senseiSprites; // series no hands, serious with hands, happy with hands, quite happy with hands
    // 0=serious no hands, 1=serious with hands, 2=happy with hands, 3=quite happy with hands

    // Multiple dialogues for the sensei
    private string[] senseiDialogues = {
        "You found the lab! That's nice. Welcome to the lab. Or whatever they say.",
        "Legend says this is where Rishi and his team worked on Protocol Asce–",
        "Right... I forgot that word was prohibited within Byte City. My apologies. But right, the laboratory...",
        "You like being competitive? Well, there are minigames here. You won't make any money, but you can be on the leaderboards...",
        "You might wonder... where are the leaderboards? Well, for all three of the games, you'll find the leaderboards near the entrance, over here.",
        "You like orienting pipes? There's a nice game for that. Check it out near the data center.",
        "How about putting wires together? There's a game for that too. You'll find it right about... here.",
        "And then, for those who enjoy card games, you can talk to the \"Deckmaster\" over there.",
        "Deckmaster is such a weird name. I don't know why he calls himself that. But he's a nice guy.",
        "Anyways... Toodles! Or whatever people say in this day and age."
    };

    private int[] senseiSpriteIndices = {
        3, 2, 1, 2, 2, 3, 2, 1, 0, 3
    };

    void Update()
    {

        // If player is talking to sensei, no matter if they click or tap, Do NOT close the dialogue
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && playerData.interactable == "sensei")
        {
            // Do nothing
            return;
        }


        // If player is talking to drunkard, one-liner dialogue
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && playerData.interactable == "drunkard")
        {
            dialoguePanel.SetActive(false);
        }

    }
}
