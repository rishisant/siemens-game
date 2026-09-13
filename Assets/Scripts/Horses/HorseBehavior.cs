using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/**
 * @brief This contains the logic for running the horse game. This handles
 * moving the horses across the screen using a fixed update. It also tracks
 * which horse is in the lead, as well as awarding bets when finished
 *
 * @see TrailPrefab
 */
public class HorseBehavior : MonoBehaviour
{
    // Grab all HorseObjects (they're Unity UI Images)
    [SerializeField] private GameObject[] horseObjects;

    [SerializeField] private GameObject[] horseTrailPrefabs;

    private Vector2[] horseStartPositions;

    // Grab the PlayerData
    private PlayerData playerData => PlayerData.Instance;

    // Grab playerMomvent
    [SerializeField] private Character_Movement playerMovement;
    //Modal back
    [SerializeField] private GameObject modalBack;

    // Grab the flags gameObjects
    [SerializeField] private GameObject[] flags;

    [SerializeField] private TMP_Text horseInLead;

    private float[] slowSpeeds =
    {
        20f, 22.5f, 25f, 27.5f, 30f, 32.5f, 35f, 37.5f, 40f, 42.5f, 42.5f
    };

    private float[] slowChosenSpeeds =
    {
        20f, 22.5f, 25f, 27.5f, 30f, 32.5f, 35f, 37.5f, 40f, 42.5f, 42.5f, 45f, 60f, 70f, 20f, 22.5f, 30f, 32.5f, 40f, 45f, 100f, 120f, 70f, 50f, 35f, 35f, 23.5f, 40f, 40f, 22.5f
    };

    private float[] slowMediumSpeeds =
    {
        25f, 30f, 35f, 40f, 45f, 50f, 52.5f, 55.5f, 60f, 62.5f, 70f, 75f
    };

    private float[] mediumSpeeds =
    {
        45f, 46.25f, 47f, 50f, 55.5f, 60f, 62.5f, 65f, 70f, 72.5f, 45f, 50f, 75f, 70f, 55.5f, 60f, 65f, 80f, 60f, 65f, 70f, 50f, 50f, 70f, 65f
    };

    // slightmedium
    private float[] slightMediumSpeeds =
    {
        // 37.5f, 35f, 25f, 55.5f, 50f, 46.25f, 43f, 42.5f, 100f, 30f, 34.5f
        55f, 60f, 65f, 70f, 47.5f, 50f, 60f, 65f, 55f, 65f, 70f, 150f, 45f, 60f, 65f, 60f, 55f, 50f, 70f, 72.5f, 20f, 30f, 40f, 25f, 40f, 100f, 40f, 50f, 60f, 55f, 65f, 50f
    };

    private float[] fastSpeeds =
    {
        60f, 65f, 70f, 75f, 80f
    };

    public bool lostGame = false;

    private float trailLifetime = 0.1f;

    // Grab the players neuroflux level. It goes up to 100 (think of percent, but its an integer)
    private int neurofluxLevel;

    // The horse ids and display names, by lane index
    private static readonly string[] horseNames = { "blackhoof", "chromeblitz", "robotrotter", "nanomane", "thunderbyte" };
    private static readonly string[] horseDisplayNames = { "Blackhoof", "Chrome Blitz", "Robotrotter", "Nano Mane", "Thunderbyte" };

    // The player's chosen horse and bet are read straight from PlayerData
    private string chosen_horse_ => playerData.chosen_horse;
    private int betAmount => playerData.bet_amount;

    private bool horseCrossedFinishLine;

    private List<GameObject> horseTrails = new List<GameObject>();

    [SerializeField] Canvas GameScreen;
    [SerializeField] Canvas GameOverScreen;
    [SerializeField] TMP_Text horseWinnerText;
    [SerializeField] TMP_Text coinsLostOrGainedText;

    // RandomSpeed function
    private float RandomSpeed(bool isChosen)
    {
        int baseChance;
        float[] speedArray = slowSpeeds;

        neurofluxLevel = playerData.neuroflux_meter;

        if (isChosen)
        {
            // Base chance for slow speeds
            baseChance = Mathf.Clamp(70 - (neurofluxLevel / 2), 20, 70); // Giving range from 20% to 70%, decreasing as neuroflux increases

            int rand = Random.Range(0, 100);
            if (rand < baseChance)
            {
                speedArray = slowChosenSpeeds;
            }
            else if (rand < baseChance + 20)
            {
                speedArray = slowMediumSpeeds;
            }
            else
            {
                speedArray = slightMediumSpeeds;
            }
        }
        else
        {
            int baseNonChosen = 50; // Non-chosen horses have a fixed lower chance for faster speeds
            speedArray = Random.Range(0, 100) < baseNonChosen ? slowSpeeds : mediumSpeeds;
        }

        return speedArray[Random.Range(0, speedArray.Length)];
    }

    void Start()
    {
        neurofluxLevel = playerData.neuroflux_meter;
        horseCrossedFinishLine = false;

        horseStartPositions = new Vector2[horseObjects.Length];

        for (int i = 0; i < horseObjects.Length; i++)
        {
            horseStartPositions[i] = horseObjects[i].GetComponent<RectTransform>().anchoredPosition;
        }
    }

    // Update to check if the horse has reached the finish line
    void Update()
    {
        if (horseCrossedFinishLine)
        {
            return;
        }

        // Check if the horse has reached the finish line
        for (int i = 0; i < horseObjects.Length; i++)
        {
            if (horseObjects[i].transform.position.x >= flags[i].transform.position.x)
            {
                horseCrossedFinishLine = true;
                EndGame(GetHorseName(i));
            }
        }
    }

    public void UpdateLeadHorse()
    {
        int horseInLeadIdx = 0;
        for (int i = 0; i < horseObjects.Length; i++)
        {
            if (horseObjects[i].transform.position.x > horseObjects[horseInLeadIdx].transform.position.x)
            {
                horseInLeadIdx = i;
            }
        }

        horseInLead.text = horseDisplayNames[horseInLeadIdx] + " is in the lead!";
    }

    private int frameCount = 0;
    private void FixedUpdate()
    {
        if (horseCrossedFinishLine) return;

        frameCount++;
        if (frameCount >= 25 / horseObjects.Length) // Each horse gets updated in round robin every second
        {
            MoveHorses();
            UpdateLeadHorse();
            frameCount = 0;
        }
    }

    public void MoveHorses()
    {
        float deltaTime = Time.deltaTime * 25; // Adjusting for 25 frames per second

        for (int i = 0; i < horseObjects.Length; i++)
        {
            // we need to change the position after the fact, since we are in canvas coordinates, not game coordinates
            GameObject horseTrail = Instantiate(horseTrailPrefabs[i], Vector3.zero, Quaternion.identity, horseObjects[i].transform.parent.GetComponent<RectTransform>());
            horseTrail.GetComponent<RectTransform>().anchoredPosition = horseObjects[i].GetComponent<RectTransform>().anchoredPosition - new Vector2(15f, 0f);
            horseTrails.Add(horseTrail);


            bool isChosen = (GetHorseName(i) == chosen_horse_);

            float speed = RandomSpeed(isChosen);

            horseObjects[i].transform.position += Vector3.right * speed * deltaTime;

            StartCoroutine(DestroyAfterDelay(horseTrail));
        }
    }

    private IEnumerator DestroyAfterDelay(GameObject trail)
    {
        yield return new WaitForSeconds(trailLifetime);
        horseTrails.Remove(trail);
        Destroy(trail);
    }

    private string GetHorseName(int index)
    {
        return (index >= 0 && index < horseNames.Length) ? horseNames[index] : "";
    }

    public void EndGame(string winningHorse)
    {
        GameScreen.gameObject.SetActive(false);
        GameOverScreen.gameObject.SetActive(true);
        horseWinnerText.text = $"{winningHorse} has won!";
        if (winningHorse == chosen_horse_)
        {
            int coinsGained = 3 * betAmount;
            lostGame = false;
            playerData.coins += coinsGained;
            coinsLostOrGainedText.text = $"You gained {coinsGained} coins!";//achid=13,14

            // add coinsGained to playerData.casino_wins
            playerData.casino_winnings += coinsGained;

            if (playerData.casino_winnings >= 500 && !playerData.unlocked_achievements.Contains(13))
            {
                // unlock achievement 13
                playerData.UnlockAchievement(13);
            }
            if (playerData.casino_winnings >= 1000 && !playerData.unlocked_achievements.Contains(14))
            {
                // unlock achievement 14
                playerData.UnlockAchievement(14);
            }

        }
        else
        {
            lostGame = true;
            coinsLostOrGainedText.text = $"You lost {betAmount} coins!";//achid=12

            // add betAmount to playerData.casino_losses
            playerData.casino_losses += betAmount;

            if (playerData.casino_losses >= 100 && !playerData.unlocked_achievements.Contains(12))
            {
                // unlock achievement 12
                playerData.UnlockAchievement(12);
            }
        }

        // clean up trails
        for (int i = 0; i < horseTrails.Count; i++)
        {
            Destroy(horseTrails[i]);
        }
        horseTrails.Clear();

        // reset horse positions
        for (int i = 0; i < horseObjects.Length; i++)
        {
            horseObjects[i].GetComponent<RectTransform>().anchoredPosition = horseStartPositions[i];
        }
    }

    public void ExitButton()
    {
        GameScreen.gameObject.SetActive(false);
        GameOverScreen.gameObject.SetActive(false);
        horseCrossedFinishLine = false;

        // If the player has won, subtract less from neuroflux
        if (!lostGame)
        {
            if (playerData.neuroflux_meter < 25)
            {
                playerData.neuroflux_meter = 0;
            }
            else
            {
                playerData.neuroflux_meter -= 25;
            }

        }
        else
        {
            // if the player has lost, subtract more from neuroflux
            // I.E. Their high was too high, and they lost a lot of money
            if (playerData.neuroflux_meter < 50)
            {
                playerData.neuroflux_meter = 0;
            }
            else
            {
                playerData.neuroflux_meter -= 50;
            }
        }

        // Unstop
        playerMovement.UnstopPlayer();
        modalBack.SetActive(false);
    }
}
