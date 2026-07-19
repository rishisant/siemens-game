using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * @class UserData
 * @brief This is a dataclass used to work with the json utility in Unity.
 * @details The fields in this class are synced up with the fields in the json
 * response from the API. It is important to not change these names (i.e. to
 * camelcase) because they are serialized directly from the json response.
 */
[System.Serializable]
public class UserData
{
    public int user_id;
    public string user_name;
    public string employee_id;
    public int current_coins;
    public int total_coins;
    public List<int> items_owned;
    public List<int> items_equipped;
    public List<int> cards_owned;
    public List<int> achievements_complete;
    public Dictionary<string, int> achievements;
    public bool has_finished_cutscene;
    public int location_x;
    public int location_y;
    public string current_scene;
    public UserDataInteractions interactions;
}

/**
 * @class UserDataInteractions
 * @brief Dataclass that maps to a dictionary returned in login request
 * @details Since the json utility is very barebones in Unity, and since we
 * didn't want to introduce more external dependencies with something like
 * newtonsoft, we are using the JsonUtility built in with Unity. This has some
 * problems, for example, the existence of this class. The API response from
 * the login returns a dictionary for the npc interactions, which the
 * JsonUtility cannot implicitly convert to a dictionary. It instead looks like
 * more Json, so we have to parse it like so. This then needs to be converted
 * into a dictionary so that we can store it in playerData properly.
 */
[System.Serializable]
public class UserDataInteractions
{
    public int sensei;
    public int drunkard;
    public int labenter;
    public int deckmaster;
    public int shopkeeper;
    public int casinoenter;
    public int casino_owner;
}

/**
 * @class LoginResponse
 * @brief This is a dataclass used to work with the json utility in Unity.
 * @details The fields in this class are synced up with the fields in the json
 * response from the API. It is important to not change these names (i.e. to
 * camelcase) because they are serialized directly from the json response.
 * Becuase of the way that the login response is formed, this class mainly uses
 * the UserData class to hold the relevant information. This is effectively a
 * wrapper around the UserData dataclass.
 *
 * @see UserData
 */
[System.Serializable]
public class LoginResponse
{
    public string message;
    public UserData user;
}

// Rishi Santhanam
// Main menu manager script will be responsible for the functions that the
// buttons on the main menu are assigned to. It will cause certain modals to
// appear and disappear, and will also load the game scene when the play button
// is clicked.
/**
 * @class MainMenuManager
 * @brief This handles the login and entering into the introduction cutscene at
 * the start of the game
 * @details This class handles the display of the login modals as well as
 * sending the login api request.
 * - On new user creation, the username is checked against a bad word list seen
 *   below.
 * - Once the player is successfully logged in, we populate PlayerData with the
 *   values we get back from the API
 * - If the login is unsuccessful, error screens are shown
 * - If login is successful, then the player data is populated and the starting
 *   cutscene is loaded
 *
 * @see UserData
 * @see PlayerData
 */
public class MainMenuManager : MonoBehaviour
{
    // Serialize all panels in the game needed
    // in GameObject form (for simple act/deact)
    [SerializeField] private GameObject loginPanel; // Base login panel
    [SerializeField] private GameObject registerPanel; // Base register panel
    [SerializeField] private GameObject loginregAbstractPanel; // The panel responsible for abstracting the login and register panels

    // Serialize the modal blackOut
    [SerializeField] private GameObject blackOutModal;

    // Serialize the settings and credits panels
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [SerializeField] private TMP_InputField createAccountEmployeeId;
    [SerializeField] private TMP_InputField createAccountUsername;
    [SerializeField] private TMP_InputField createAccountPassword;

    [SerializeField] private TMP_InputField loginUsername;
    [SerializeField] private TMP_InputField loginPassword;

    [SerializeField] private Button createAccountButton;
    [SerializeField] private Button loginButton;

    [SerializeField] private GameObject errorScreen;
    [SerializeField] private TMP_Text errorMessageText;

    private bool isLoginSuccessful;

    private PlayerData playerData => PlayerData.Instance;

    private string[] badwords =
    {
        "2g1c",
        "2 girls 1 cup",
        "acrotomophilia",
        "alabama hot pocket",
        "alaskan pipeline",
        "anal",
        "anilingus",
        "anus",
        "apeshit",
        "arsehole",
        "ass",
        "asshole",
        "assmunch",
        "auto erotic",
        "autoerotic",
        "babeland",
        "baby batter",
        "baby juice",
        "ball gag",
        "ball gravy",
        "ball kicking",
        "ball licking",
        "ball sack",
        "ball sucking",
        "bangbros",
        "bangbus",
        "bareback",
        "barely legal",
        "barenaked",
        "bastard",
        "bastardo",
        "bastinado",
        "bbw",
        "bdsm",
        "beaner",
        "beaners",
        "beaver cleaver",
        "beaver lips",
        "beastiality",
        "bestiality",
        "big black",
        "big breasts",
        "big knockers",
        "big tits",
        "bimbos",
        "birdlock",
        "bitch",
        "bitches",
        "black cock",
        "blonde action",
        "blonde on blonde action",
        "blowjob",
        "blow job",
        "blow your load",
        "blue waffle",
        "blumpkin",
        "bollocks",
        "bondage",
        "boner",
        "boob",
        "boobs",
        "booty call",
        "brown showers",
        "brunette action",
        "bukkake",
        "bulldyke",
        "bullet vibe",
        "bullshit",
        "bung hole",
        "bunghole",
        "busty",
        "butt",
        "buttcheeks",
        "butthole",
        "camel toe",
        "camgirl",
        "camslut",
        "camwhore",
        "carpet muncher",
        "carpetmuncher",
        "chocolate rosebuds",
        "cialis",
        "circlejerk",
        "cleveland steamer",
        "clit",
        "clitoris",
        "clover clamps",
        "clusterfuck",
        "cock",
        "cocks",
        "coprolagnia",
        "coprophilia",
        "cornhole",
        "coon",
        "coons",
        "creampie",
        "cum",
        "cumming",
        "cumshot",
        "cumshots",
        "cunnilingus",
        "cunt",
        "darkie",
        "date rape",
        "daterape",
        "deep throat",
        "deepthroat",
        "dendrophilia",
        "dick",
        "dildo",
        "dingleberry",
        "dingleberries",
        "dirty pillows",
        "dirty sanchez",
        "doggie style",
        "doggiestyle",
        "doggy style",
        "doggystyle",
        "dog style",
        "dolcett",
        "domination",
        "dominatrix",
        "dommes",
        "donkey punch",
        "double dong",
        "double penetration",
        "dp action",
        "dry hump",
        "dvda",
        "eat my ass",
        "ecchi",
        "ejaculation",
        "erotic",
        "erotism",
        "escort",
        "eunuch",
        "fag",
        "faggot",
        "fecal",
        "felch",
        "fellatio",
        "feltch",
        "female squirting",
        "femdom",
        "figging",
        "fingerbang",
        "fingering",
        "fisting",
        "foot fetish",
        "footjob",
        "frotting",
        "fuck",
        "fuck buttons",
        "fuckin",
        "fucking",
        "fucktards",
        "fudge packer",
        "fudgepacker",
        "futanari",
        "gangbang",
        "gang bang",
        "gay sex",
        "genitals",
        "giant cock",
        "girl on",
        "girl on top",
        "girls gone wild",
        "goatcx",
        "goatse",
        "god damn",
        "gokkun",
        "golden shower",
        "goodpoop",
        "goo girl",
        "goregasm",
        "grope",
        "group sex",
        "g-spot",
        "guro",
        "hand job",
        "handjob",
        "hard core",
        "hardcore",
        "hentai",
        "homoerotic",
        "honkey",
        "hooker",
        "horny",
        "hot carl",
        "hot chick",
        "how to kill",
        "how to murder",
        "huge fat",
        "humping",
        "incest",
        "intercourse",
        "jack off",
        "jail bait",
        "jailbait",
        "jelly donut",
        "jerk off",
        "jigaboo",
        "jiggaboo",
        "jiggerboo",
        "jizz",
        "juggs",
        "kike",
        "kinbaku",
        "kinkster",
        "kinky",
        "knobbing",
        "leather restraint",
        "leather straight jacket",
        "lemon party",
        "livesex",
        "lolita",
        "lovemaking",
        "make me come",
        "male squirting",
        "masturbate",
        "masturbating",
        "masturbation",
        "menage a trois",
        "milf",
        "missionary position",
        "mong",
        "motherfucker",
        "mound of venus",
        "mr hands",
        "muff diver",
        "muffdiving",
        "nambla",
        "nawashi",
        "negro",
        "neonazi",
        "nigga",
        "nigger",
        "nig nog",
        "nimphomania",
        "nipple",
        "nipples",
        "nsfw",
        "nsfw images",
        "nude",
        "nudity",
        "nutten",
        "nympho",
        "nymphomania",
        "octopussy",
        "omorashi",
        "one cup two girls",
        "one guy one jar",
        "orgasm",
        "orgy",
        "paedophile",
        "paki",
        "panties",
        "panty",
        "pedobear",
        "pedophile",
        "pegging",
        "penis",
        "phone sex",
        "piece of shit",
        "pikey",
        "pissing",
        "piss pig",
        "pisspig",
        "playboy",
        "pleasure chest",
        "pole smoker",
        "ponyplay",
        "poof",
        "poon",
        "poontang",
        "punany",
        "poop chute",
        "poopchute",
        "porn",
        "porno",
        "pornography",
        "prince albert piercing",
        "pthc",
        "pubes",
        "pussy",
        "queaf",
        "queef",
        "quim",
        "raghead",
        "raging boner",
        "rape",
        "raping",
        "rapist",
        "rectum",
        "reverse cowgirl",
        "rimjob",
        "rimming",
        "rosy palm",
        "rosy palm and her 5 sisters",
        "rusty trombone",
        "sadism",
        "santorum",
        "scat",
        "schlong",
        "scissoring",
        "semen",
        "sex",
        "sexcam",
        "sexo",
        "sexy",
        "sexual",
        "sexually",
        "sexuality",
        "shaved beaver",
        "shaved pussy",
        "shemale",
        "shibari",
        "shit",
        "shitblimp",
        "shitty",
        "shota",
        "shrimping",
        "skeet",
        "slanteye",
        "slut",
        "s&m",
        "smut",
        "snatch",
        "snowballing",
        "sodomize",
        "sodomy",
        "spastic",
        "spic",
        "splooge",
        "splooge moose",
        "spooge",
        "spread legs",
        "spunk",
        "strap on",
        "strapon",
        "strappado",
        "strip club",
        "style doggy",
        "suck",
        "sucks",
        "suicide girls",
        "sultry women",
        "swastika",
        "swinger",
        "tainted love",
        "taste my",
        "tea bagging",
        "threesome",
        "throating",
        "thumbzilla",
        "tied up",
        "tight white",
        "tit",
        "tits",
        "titties",
        "titty",
        "tongue in a",
        "topless",
        "tosser",
        "towelhead",
        "tranny",
        "tribadism",
        "tub girl",
        "tubgirl",
        "tushy",
        "twat",
        "twink",
        "twinkie",
        "two girls one cup",
        "undressing",
        "upskirt",
        "urethra play",
        "urophilia",
        "vagina",
        "venus mound",
        "viagra",
        "vibrator",
        "violet wand",
        "vorarephilia",
        "voyeur",
        "voyeurweb",
        "voyuer",
        "vulva",
        "wank",
        "wetback",
        "wet dream",
        "white power",
        "whore",
        "worldsex",
        "wrapping men",
        "wrinkled starfish",
        "xx",
        "xxx",
        "yaoi",
        "yellow showers",
        "yiffy",
        "zoophilia"
    };

    // Close button functions for login,register,abstract,settings,credits
    public void CloseLoginPanel()
    {
        loginPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    public void CloseRegisterPanel()
    {
        registerPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    public void CloseLoginRegAbstractPanel()
    {
        loginregAbstractPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    public void CloseCreditsPanel()
    {
        creditsPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    public void CloseErrorScreen()
    {
        errorScreen.SetActive(false);
        blackOutModal.SetActive(false);
    }

    // Open button functions for login,register,abstract,settings,credits
    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        blackOutModal.SetActive(true);

        // Make the abstract panel disappear
        loginregAbstractPanel.SetActive(false);
    }

    public void OpenRegisterPanel()
    {
        registerPanel.SetActive(true);
        blackOutModal.SetActive(true);

        // Make the abstract panel disappear
        loginregAbstractPanel.SetActive(false);
    }

    public void OpenLoginRegAbstractPanel()
    {
        loginregAbstractPanel.SetActive(true);
        blackOutModal.SetActive(true);
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
        blackOutModal.SetActive(true);
    }

    public void OpenCreditsPanel()
    {
        creditsPanel.SetActive(true);
        blackOutModal.SetActive(true);
    }

    public void CollectLoginInput()
    {
        string username = loginUsername.text;
        string password = loginPassword.text;

        SendLoginRequest(username, password);
    }

    public void SetupErrorScreen(string errorMessage)
    {
        errorScreen.SetActive(true);
        // Close other
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        loginregAbstractPanel.SetActive(false);
        settingsPanel.SetActive(false);
        errorMessageText.text = errorMessage;
    }

    public void CollectCreateAccountInput()
    {
        string employeeId = createAccountEmployeeId.text;
        string username = createAccountUsername.text;
        string password = createAccountPassword.text;

        for (int i = 0; i < badwords.Length; i++)
        {
            if (username.Contains(badwords[i]))
            {
                SetupErrorScreen($"Error: Your username contains {badwords[i]}, an inappropriate word. Please change it");
                return;
            }
        }

        SendCreateAccountRequest(employeeId, username, password);
    }

    // Request payloads are serialized with JsonUtility so quotes or
    // backslashes in user input can't corrupt the JSON
    [System.Serializable]
    private struct LoginRequest
    {
        public string user_name;
        public string user_password;
    }

    [System.Serializable]
    private struct CreateAccountRequest
    {
        public string user_name;
        public string user_password;
        public long employee_id;
    }

    public void SendLoginRequest(string username, string password)
    {
        Debug.Log($"sending request to login for {username}");

        string url = "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/Login";
        string jsonData = JsonUtility.ToJson(new LoginRequest
        {
            user_name = username,
            user_password = password
        });

        WebRequestUtility.SendWebRequest(
                this,
                url,
                jsonData,
                (string response) => {
                    Debug.Log(response);
                    LoginResponse data = JsonUtility.FromJson<LoginResponse>(response);
                    playerData.username = data.user.user_name;
                    playerData.userId = data.user.user_id;
                    playerData.coins = data.user.current_coins;
                    playerData.unlocked_items = data.user.items_owned;
                    playerData.equipped_items = data.user.items_equipped;
                    playerData.NotifyEquippedItemsChanged();
                    playerData.unlocked_cards = data.user.cards_owned;
                    playerData.unlocked_achievements = data.user.achievements_complete;
                    // Due to the way that JsonUtility parses Json, we need to
                    // manually set all keys in the npc_interactors since we
                    // cannot parse dictionaries with JsonUtility
                    playerData.npc_interactions["deckmaster"] = data.user.interactions.deckmaster;
                    playerData.npc_interactions["casino_owner"] = data.user.interactions.casino_owner;
                    playerData.npc_interactions["shopkeeper"] = data.user.interactions.shopkeeper;
                    playerData.npc_interactions["drunkard"] = data.user.interactions.drunkard;
                    playerData.npc_interactions["sensei"] = data.user.interactions.sensei;
                    playerData.npc_interactions["casinoenter"] = data.user.interactions.casinoenter;
                    playerData.npc_interactions["labenter"] = data.user.interactions.labenter;

                    if (playerData.npc_interactions["sensei"] > 0)
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Town_Square");

                    }
                    else
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene("Starting-Cutscene");
                    }
                },
                (string response) => {
                    SetupErrorScreen("Error: The request was invalid. Are you sure you have the right username and password?");
                }
        );
    }

    public void SendCreateAccountRequest(string employeeId, string username, string password)
    {
        Debug.Log($"sending request to create account for {employeeId} {username}");

        // The employee id must be numeric; a non-numeric id can never match
        // an employee badge, so reject it before hitting the API
        if (!long.TryParse(employeeId, out long employeeIdNumber))
        {
            SetupErrorScreen("Error: The request was invalid. Are you sure you have the right employee ID? If so, please contact your supervisor");
            return;
        }

        string url = "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/Login";
        string jsonData = JsonUtility.ToJson(new CreateAccountRequest
        {
            user_name = username,
            user_password = password,
            employee_id = employeeIdNumber
        });

        WebRequestUtility.SendWebRequest(
                this,
                url,
                jsonData,
                (string response) => {
                    LoginResponse data = JsonUtility.FromJson<LoginResponse>(response);
                    playerData.username = data.user.user_name;
                    playerData.userId = data.user.user_id;
                    playerData.coins = data.user.current_coins;
                    playerData.unlocked_items = data.user.items_owned;
                    playerData.equipped_items = data.user.items_equipped;
                    playerData.NotifyEquippedItemsChanged();
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Starting-Cutscene");
                },
                (string response) => {
                    SetupErrorScreen("Error: The request was invalid. Are you sure you have the right employee ID? If so, please contact your supervisor");
                }

        );
    }

    // Start is called before the first frame update
    void Start()
    {
        isLoginSuccessful = false;
        createAccountButton.onClick.AddListener(CollectCreateAccountInput);
        loginButton.onClick.AddListener(CollectLoginInput);
    }
}
