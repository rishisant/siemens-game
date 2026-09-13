using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/** @brief Exercises Sensei objectives and one-time duel rewards with real player data. */
public class PlayerJourneyTests
{
    private const BindingFlags Fields=BindingFlags.Instance|BindingFlags.NonPublic;
    [UnityTest] public IEnumerator SenseiRequiresFlagsAndInventoryBeforeFinishing()
    {
        SceneManager.LoadScene("MainMenu");yield return null;yield return null;
        if(GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
        GameManager.Instance.playerSpawnPosition=new Vector2(-14.89f,.23f);
        SceneManager.LoadScene("Tutorial");yield return null;yield return null;
        var tutor=Object.FindObjectOfType<TutorialManager>();var dialogue=Object.FindObjectOfType<DialogueManagerTutorial>();
        var player=Object.FindObjectOfType<Character_Movement>();
        Assert.IsFalse(player.CanMove);
        var flags=(GameObject[])typeof(TutorialManager).GetField("flags",Fields).GetValue(tutor);
        foreach(int index in new[]{5,7,9})
        {
            dialogue.dialogueIndex=index;yield return null;yield return null;
            Assert.IsTrue(player.CanMove);
            player.transform.position=flags[(index-5)/2].transform.position;
            yield return null;yield return null;
            Assert.AreEqual(index+1,dialogue.dialogueIndex);Assert.IsFalse(player.CanMove);
            // Let the camera return before starting the next objective.
            yield return new WaitForSeconds(2.1f);
        }
        dialogue.dialogueIndex=13;yield return null;yield return null;
        var inventory=Object.FindObjectOfType<InventoryUI_Tutorial>();
        inventory.OpenInventory();yield return null;Assert.IsFalse(inventory.inventoryClosed);
        inventory.CloseInventory();yield return null;yield return null;Assert.AreEqual(14,dialogue.dialogueIndex);
        dialogue.dialogueIndex=17;yield return null;yield return null;
        Assert.AreEqual(1,PlayerData.Instance.npc_interactions["sensei"]);
        tutor.EndTutorial();Assert.AreEqual(1,PlayerData.Instance.unlocked_achievements.Count(id=>id==0));
    }
    private class RewardRandom : System.Random { public override double NextDouble() { return 0; } }
    [UnityTest] public IEnumerator VictoryAwardsCoinsAndOneNewCardOnlyOnce()
    {
        SceneManager.LoadScene("MainMenu");yield return null;yield return null;
        var player=PlayerData.Instance;var oldCards=new List<int>(player.unlocked_cards);int coins=player.coins,wins=player.card_game_wins;
        try
        {
            player.unlocked_cards=new List<int>{0,4,9};
            var match=new DeckmasterMatch(player.unlocked_cards.Select(id=>player.cards[id]),new RewardRandom());
            while(!match.Complete)
            {
                int best=0;for(int i=0;i<match.Hand.Count;i++)if(DeckmasterMatch.Compare(match.Hand[i],match.Opponent)>0)best=i;
                match.Play(best);match.Deal();
            }
            Assert.IsTrue(match.Won);int card=match.ClaimReward(player);
            Assert.GreaterOrEqual(card,0);Assert.AreEqual(coins+80,player.coins);Assert.AreEqual(4,player.unlocked_cards.Count);
            Assert.AreEqual(-1,match.ClaimReward(player));Assert.AreEqual(coins+80,player.coins);Assert.AreEqual(wins+1,player.card_game_wins);
        }
        finally {player.unlocked_cards=oldCards;player.coins=coins;player.card_game_wins=wins;}
    }
}
