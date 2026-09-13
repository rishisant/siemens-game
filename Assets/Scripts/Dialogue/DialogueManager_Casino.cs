using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief This is the DialogueManager for the Town Square. This holds the
 * dialogues for sensei in the casino, and the casino owner's dialogue
 *
 * @see DialogueManager
 */
public class DialogueManager_Casino : DialogueManagerBase
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

    // current flux value (start at 10)
    public int fluxCost = 10;
    // Text for flux cost
    public TMPro.TextMeshProUGUI fluxCostText;

    // import Camera_Movement script
    [SerializeField] private CameraFollow cameraFollow;
    // import PlayerMovement script
    [SerializeField] private Character_Movement playerMovement;

    // Modal back
    [SerializeField] public GameObject backModal;

    // Close choice panel
    public void CloseChoicePanel()
    {
        casinoOwnerChoicePanel.SetActive(false);
        backModal.SetActive(false);

        // Let player move
        playerMovement.UnstopPlayer();
    }

    // Load in the dialoguePanel
    // Look for UI-Panel Dialogue-Panel and assign it
    public GameObject dialoguePanel;

    // Text for the dialoguePanel
    // Text for the name of the NPC

    // public int dialogueindex
    private int dialogueIndex = 0;

    // Casino owner Choice Panel
    public GameObject casinoOwnerChoicePanel;
    // Horse panel choose
    public GameObject horsePanelChoose;

    // Add all the casino owner sprites
    [SerializeField] private Sprite[] casinoOwnerSprites; // 0->serious, 1-> serious with hands, 2->charismatic, 3->happy with hands

    // Add all the casino owner dialogues
    private string[] casinoOwnerInitial = {
        "What's up, hot stuff. You want to make some money?",
        "Ignore the first comment if you happen to be a male. If you're a girl, then pretend like it was even more charismatic.",
        "Anyways, there's really not much else to say. I offer a 2:1 payout on bets. You bet 10 coins, you can win 20 coins. Simple, right?",
        "Oh yeah, and you're probably wondering what that meter at the top of your screen is. That's the Neuroflux meter.",
        "Meaning that, the more \"fluxed up\" you are, the higher chance you have of winning. Or at least, that's what some people say.",
        "Something about \"being in the zone\". I don't know. I'm just here to take your money. And give you some back, I guess.",
    };

    // Integer list of inital sprites
    private int[] casinoOwnerInitialSprites = {
        2, 0, 1, 2, 3, 2
    };

    // Casino owner one liners dialogues
    // Make the first three if the player has more losses moneys than wins (we will fix this)
    private string[] casinoOwnerOneLinersLosing = {
        "For someone who likes to lose money, you're doing a great job.",
        "You're like a reverse Robin Hood. Stealing from the poor and giving to the rich. But, want to throw some more money on the table, hotshot?",
        "Psst. Flux makes you better. But even then, you still seem to suck! How's that for a motivational speech?", // these 3 play if losses > wins
        "You're on a roll! A really bad one. But hey, you're still here, so you must like losing money. Want to lose some more?",
        "Losing money isn't my favorite thing to do. But hey, one man's trash is another man's treasure. Too bad you don't seem to have either.",
        "They say that the more you lose, the more you win. But I don't think that applies to you. Want to try your luck again?" // now let's do wins > losses
    };

    // losing sprites
    private int[] casinoOwnerOneLinersLosingSprites = {
        3, 0, 2, 3, 1, 2
    };

    // Casino owner one liners dialogues (for winning)
    private string[] casinoOwnerOneLinersWinning = {
        "Stay away from this table. You're winning too much. I might have to make some calls if you keep this up.",
        "Hey, hotshot. You're doing pretty well. But remember, the house always wins. Want to try your luck again?",
        "I can't tell whether you're fluxed up or not.",
        "You must seem to like money. Or maybe you like winning. Or maybe you like both. Yeah, you probably like both.",
        "I know you're winning a lot, but trust me when I say, don't pick Thunderbyte. He might end up losing you some money.",
        "I heard Nano Mane just got a new upgrade. You might want to bet on him. And no, I'm not lying to you. I'm pretending to lie to you.",
        "You win a lot for such a loser. I mean, you're not a loser. You're a winner. But you're also a loser. You know what I mean.",
    };

    // winning sprites
    private int[] casinoOwnerOneLinersWinningSprites = {
        0, 2, 2, 2, 1, 1, 0
    };

    // Casino owner one liners for neutral
    private string[] casinoOwnerOneLinersNeutral = {
        "Alright, alright. Let's get this thing started, shall we? You want to bet on some robot horses? You want some flux?",
        "You're not doing too bad. But you're not doing too good either. You're just... doing.",
        "You're like a coin. You have two sides. One side is winning, the other side is losing. But you're still a coin.",
        "I'm not sure how else to motivate you to bet. But, I heard that the more you bet, the more you win. Or lose. But you're probably going to lose. Right, that didn't help.",
        "I just installed a golden toilet in the back. Only winners allowed. You want to be a winner, right? That's what I thought.",
    };

    // neutral sprites
    private int[] casinoOwnerOneLinersNeutralSprites = {
        2, 2, 2, 0, 3
    };

    // Make casino owner speak
    public void CasinoOwnerSpeak()
    {
        // Stop player
        playerMovement.StopPlayer();

        // Turn on the dialogue panel
        dialoguePanel.SetActive(true);

        // First, say the first dialogue, second dialogue, third dialogue, pan(0), fourth dialogue, pan(1), fifth dialogue, pan(2), sixth dialogue
        // If the casino owner has never been interacted with, do the initial coroutine
        // otherwise just the normal coroutine
        if (playerData.npc_interactions["casino_owner"] == 0)
        {
            StartCoroutine(CasinoOwnerSpeakInitial());
        }
        else
        {
            StartCoroutine(CasinoOwnerSpeakCoroutine());
        }

        // Increment the npc_interactions for casino_owner
        playerData.npc_interactions["casino_owner"] += 1;
    }

    private IEnumerator CasinoOwnerSpeakInitial()
    {
        // Change TTC_Text to "Do Not Tap."
        TTC_Text.text = "Do Not Tap...";

        for (int i = 0; i < casinoOwnerInitial.Length; i++)
        {
            // If the coroutine is not null, stop the coroutine
            if (typeSentenceCoroutine != null)
            {
                StopCoroutine(typeSentenceCoroutine);
            }

            // Set the dialogueText to an empty string
            dialogueText.text = "";

            // Set the characterImage to the appropriate sprite
            characterImage.sprite = casinoOwnerSprites[casinoOwnerInitialSprites[i]];

            // Set the charName to "Casino Owner"
            charName.text = "Casino Owner";

            // Start typing the sentence
            isTyping = true;
            typeSentenceCoroutine = StartCoroutine(TypeSentence(casinoOwnerInitial[i]));

            // Wait
            yield return WaitForLineAdvance();

            // Wait
            yield return null;
        }

        // Change TTC_Text to "Tap to Continue."
        TTC_Text.text = "Tap to Continue...";

        // Close the dialogue panel
        dialoguePanel.SetActive(false);

        // Let player move
        playerMovement.UnstopPlayer();
    }

    // Now, thanks for the flux lines
    public string[] casinoOwnerThanksForFlux = {
        "You went with the flux, huh? Good choice. You're going to need it.",
        "You want to be a winner, hey! I like that. Keep it up, champ.",
        "I see a winner in you now. I had my doubts, but you're proving me wrong.",
        "You're going to win big. I can feel it. Or maybe I'm just feeling the flux. Either way, you're going to win big.",
        "There's more where this came from. Keep betting, keep winning. You're going to be rich.",
    };

    // Now, thanks for the flux sprites
    public int[] casinoOwnerThanksForFluxSprites = {
        2, 2, 3, 2, 2
    };

    // Too poor to buy flux, so hate on the character
    public string[] casinoOwnerHate = {
        "Why are you gambling when you don't even want to win?",
        "You don't have money to buy flux, and you're still gambling? OK.",
        "... OK. You know what? I'm not even going to say anything. You're just... something else.",
        "Right... Why are you even here? You don't have money to buy flux, you don't have money to bet. What are you doing?",
    };

    // Flux is full
    public string[] casinoOwnerFluxFull = {
        "You want more flux? Dude. Your eyes are glowing. You're full. You're like a balloon. You're going to pop.",
        "Flux, flux, flux... That's what you really care about? You're not going to try to win? You're just going to keep buying flux?",
        "I've heard of addicts like you. You're not going to stop, are you? You're just going to keep buying flux. Sorry, but I can't assist you.",
        "You're not even a human. Why are you addicted like one?",
        "Monsieur Addict. Tu veux plus de flux? Tu es plein. Tu vas exploser. Did you know I spoke French?",
        "Idiot. Idiot. Idiot. You're full! You're going to combust at this rate."
    };

    public int[] casinoOwnerFluxFullSprites = {
        0, 1, 0, 0, 0, 1
    };

    // Hate sprites
    public int[] casinoOwnerHateSprites = {
        0, 0, 1, 0
    };

    // Buy flux function
    public void CasinoOwnerBuyFlux()
    {
        // Turn off the choice menu
        casinoOwnerChoicePanel.SetActive(false);
        // Turn off the modal
        backModal.SetActive(false);

        // Check if the player has >=10 coins
        // playerData.flux_meter += 25;
        if (playerData.coins >= fluxCost && playerData.neuroflux_meter < 100)
        {
            playerData.coins -= fluxCost;
            if (playerData.neuroflux_meter + 25 > 100)
            {
                playerData.neuroflux_meter = 100;
            }
            else
            {
                playerData.neuroflux_meter += 25;
            }
            CasinoOwnerThanksForFlux();
        } else
        {
            CasinoOwnerHateOnPlayer();
        }
    }

    // Open the horse panel
    public void CasinoOwnerOpenHorsePanel()
    {
        // Turn off the choice menu
        casinoOwnerChoicePanel.SetActive(false);
        // Turn on the horse panel
        horsePanelChoose.SetActive(true);
    }

    // Close horse panel
    public void CasinoOwnerCloseHorsePanel()
    {
        // Turn off the horse panel
        horsePanelChoose.SetActive(false);
        // Now turn off the backModal
        backModal.SetActive(false);

        // Let player move
        playerMovement.UnstopPlayer();
    }

    // Hate on the player for not buying flux
    public void CasinoOwnerHateOnPlayer()
    {
        // Stop player
        playerMovement.StopPlayer();

        // Turn on the dialogue panel
        dialoguePanel.SetActive(true);

        // First, say the first dialogue, second dialogue, third dialogue, pan(0), fourth dialogue, pan(1), fifth dialogue, pan(2), sixth dialogue
        // If the casino owner has never been interacted with, do the initial coroutine
        // otherwise just the normal coroutine
        StartCoroutine(CasinoOwnerHateOnPlayerCoroutine());

        // Increment the npc_interactions for casino_owner
        playerData.npc_interactions["casino_owner"] += 1;
    }

    private IEnumerator CasinoOwnerHateOnPlayerCoroutine()
    {
        // Change TTC_Text to "Do Not Tap."
        TTC_Text.text = "Do Not Tap...";

        // If the player is fluxed up (neuroflux_meter == 100) otherwise we do normal hate
        if (playerData.neuroflux_meter == 100)
        {
            // Choose a random hate dialogue
            int i = Random.Range(0, casinoOwnerFluxFull.Length);

            // Now choose the sprites for the hate dialogue
            // If the coroutine is not null, stop the coroutine
            if (typeSentenceCoroutine != null)
            {
                StopCoroutine(typeSentenceCoroutine);
            }

            // Set the dialogueText to an empty string
            dialogueText.text = "";

            // Set the characterImage to the appropriate sprite
            characterImage.sprite = casinoOwnerSprites[casinoOwnerFluxFullSprites[i]];

            // Set the charName to "Casino Owner"
            charName.text = "Casino Owner";

            // Start typing the sentence
            isTyping = true;
            typeSentenceCoroutine = StartCoroutine(TypeSentence(casinoOwnerFluxFull[i]));

            // Wait
            yield return WaitForLineAdvance();
        } else
        {
            // Choose a random hate dialogue
            int i = Random.Range(0, casinoOwnerHate.Length);

            // Now choose the sprites for the hate dialogue
            // If the coroutine is not null, stop the coroutine
            if (typeSentenceCoroutine != null)
            {
                StopCoroutine(typeSentenceCoroutine);
            }

            // Set the dialogueText to an empty string
            dialogueText.text = "";

            // Set the characterImage to the appropriate sprite
            characterImage.sprite = casinoOwnerSprites[casinoOwnerHateSprites[i]];

            // Set the charName to "Casino Owner"
            charName.text = "Casino Owner";

            // Start typing the sentence
            isTyping = true;
            typeSentenceCoroutine = StartCoroutine(TypeSentence(casinoOwnerHate[i]));

            // Wait
            yield return WaitForLineAdvance();
        }

        // Change TTC_Text to "Tap to Continue."
        TTC_Text.text = "Tap to Continue...";

        // Close the dialogue panel

        dialoguePanel.SetActive(false);

        // Let player move
        playerMovement.UnstopPlayer();
    }

    // A function to thank the player for using flux
    public void CasinoOwnerThanksForFlux()
    {
        // Stop player
        playerMovement.StopPlayer();

        // Turn on the dialogue panel
        dialoguePanel.SetActive(true);

        // First, say the first dialogue, second dialogue, third dialogue, pan(0), fourth dialogue, pan(1), fifth dialogue, pan(2), sixth dialogue
        // If the casino owner has never been interacted with, do the initial coroutine
        // otherwise just the normal coroutine
        StartCoroutine(CasinoOwnerThanksForFluxCoroutine());

        // Increment the npc_interactions for casino_owner
        playerData.npc_interactions["casino_owner"] += 1;
    }

    private IEnumerator CasinoOwnerThanksForFluxCoroutine()
    {
        // Change TTC_Text to "Do Not Tap."
        TTC_Text.text = "Do Not Tap...";

        // Choose a random thanks for flux dialogue
        int i = Random.Range(0, casinoOwnerThanksForFlux.Length);

        // Now choose the sprites for the thanks for flux dialogue
        // If the coroutine is not null, stop the coroutine

        if (typeSentenceCoroutine != null)
        {
            StopCoroutine(typeSentenceCoroutine);
        }

        // Set the dialogueText to an empty string
        dialogueText.text = "";

        // Set the characterImage to the appropriate sprite
        characterImage.sprite = casinoOwnerSprites[casinoOwnerThanksForFluxSprites[i]];

        // Set the charName to "Casino Owner"
        charName.text = "Casino Owner";

        // Start typing the sentence
        isTyping = true;

        typeSentenceCoroutine = StartCoroutine(TypeSentence(casinoOwnerThanksForFlux[i]));

        // Wait
        yield return WaitForLineAdvance();

        // Change TTC_Text to "Tap to Continue."
        TTC_Text.text = "Tap to Continue...";

        // Close the dialogue panel
        dialoguePanel.SetActive(false);

        // Let player move
        playerMovement.UnstopPlayer();
    }

    private IEnumerator CasinoOwnerSpeakCoroutine()
    {
        // Basically, we're going to go to playerData casino_winnings (int) and casino_losses (int) and get the ratio for that.
        // This only applies if we have interacted with the casino owner more than once.
        float ratio = (float)playerData.casino_winnings / (float)playerData.casino_losses;

        // Change TTC_Text to "Do Not Tap."
        TTC_Text.text = "Do Not Tap...";

        string[] selectedDialogues;
        int[] selectedSprites;

        // Determine which dialogues and sprites to use based on the ratio
        if (ratio < 0.95)
        {
            selectedDialogues = casinoOwnerOneLinersLosing;
            selectedSprites = casinoOwnerOneLinersLosingSprites;
        }
        else if (ratio >= 0.95 && ratio <= 1)
        {
            selectedDialogues = casinoOwnerOneLinersNeutral;
            selectedSprites = casinoOwnerOneLinersNeutralSprites;
        }
        else
        {
            selectedDialogues = casinoOwnerOneLinersWinning;
            selectedSprites = casinoOwnerOneLinersWinningSprites;
        }

        // If winnings and losings are both 0, then just do ratio = 1
        if (playerData.casino_winnings == 0 && playerData.casino_losses == 0)
        {
            selectedDialogues = casinoOwnerOneLinersNeutral;
            selectedSprites = casinoOwnerOneLinersNeutralSprites;
        }

        // Execute only one oneliner
        int i = Random.Range(0, selectedDialogues.Length);

        // If the coroutine is not null, stop the coroutine
        if (typeSentenceCoroutine != null)
        {
            StopCoroutine(typeSentenceCoroutine);
        }

        // Set the dialogueText to an empty string
        dialogueText.text = "";

        // Set the characterImage to the appropriate sprite
        characterImage.sprite = casinoOwnerSprites[selectedSprites[i]];

        // Set the charName to "Casino Owner"
        charName.text = "Casino Owner";

        // Start typing the sentence
        isTyping = true;
        typeSentenceCoroutine = StartCoroutine(TypeSentence(selectedDialogues[i]));

        // TTC
        TTC_Text.text = "Tap to Continue...";

        yield return WaitForLineAdvance();

        // Show the choice panel
        // Before we do choice panel, we need to set the fluxCostText
        // We will do it based on the current fluxCost
        // The text is only the number itself to a string
        // First, determine the fluxCost based on the current playerCoins
        // initialFluxCost is 10, so 10 + playerCoins*0.1

        // The max flux cost should never exceed 100
        fluxCost = 10 + (int)(playerData.coins * 0.05);
        if (fluxCost > 100)
        {
            fluxCost = 100;
        }
        fluxCostText.text = fluxCost.ToString();

        casinoOwnerChoicePanel.SetActive(true);

        // modal goes up, and also dialogue is out
        dialoguePanel.SetActive(false);
        backModal.SetActive(true);
    }



    // Coroutine for typing the sentence
    private Coroutine typeSentenceCoroutine;


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
        if (playerData.npc_interactions["casinoenter"] == 0)
        {
            // Sensei Tutorial
            SenseiTutorial();
        }
    }

    // GameObjects to pan the Camera to
    [SerializeField] private GameObject cameraPanTarget; // The area where the casino owner is.

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
        playerData.npc_interactions["casinoenter"] = 1;
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
            yield return WaitForLineAdvance();

            // if i = 2, pan to the casino owner
            if (i == 1)
            {
                cameraFollow.PanCamera(cameraPanTarget.transform);
            }

            // Wait
            yield return null;
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
        "Right... Welcome to the casino. You want money? Possibly? Well, you're in the right place.",
        "There's really only one thing to do here. Bet on robot horses. Yes, it sounds ridiculous, but it's the only way to make money. The table is over there.",
        "The casino owner offers a 2:1 payout on bets. You basically get twice the amount you bet, if you win. Sick, right? Well... There is a catch.",
        "There's something in this town called Neuroflux. You've probably already noticed the meter at the top of your screen. You can only see this meter in the casino.",
        "And when you're more \"fluxed up\", you have a higher chance of winning. But hey, flux isn't cheap, so you have to pay to play. Go ask the casino owner for more details.",
        "Winning big sounds fun, though. But it's much harder than it seems. Good luck, and remember, the house always wins."
    };

    private int[] senseiSpriteIndices = {
        2, 1, 0, 1, 1, 3
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
