using System;
using System.Linq;
using NUnit.Framework;

/** @brief Sprint timing and fair Card Jitsu rules, including collection-safe dealing. */
public class PlayerExperienceTests
{
    private PlayerData.Card Card(int id,int power,PlayerData.Card.Element element)
    { return new PlayerData.Card(id,1,power,element,"Card",null,0); }
    [Test] public void HoldingMovementSprintsAndReleasingResets()
    {
        var sprint=new SprintState();sprint.Tick(true,false,.5f);Assert.IsFalse(sprint.IsSprinting);
        sprint.Tick(true,false,.3f);Assert.IsTrue(sprint.IsSprinting);
        sprint.Tick(false,false,1);Assert.IsFalse(sprint.IsSprinting);
        sprint.Tick(true,false,.1f);Assert.IsFalse(sprint.IsSprinting);
        sprint.Tick(true,true,0);Assert.IsTrue(sprint.IsSprinting);
        sprint.Reset();Assert.IsFalse(sprint.IsSprinting);
    }
    [Test] public void ElementAdvantagesAndSameElementPowerMatchOriginalRules()
    {
        var heat=Card(0,3,PlayerData.Card.Element.Heat);var pressure=Card(1,13,PlayerData.Card.Element.Pressure);
        var electric=Card(2,13,PlayerData.Card.Element.Electrical);
        Assert.Greater(DeckmasterMatch.Compare(heat,pressure),0);
        Assert.Greater(DeckmasterMatch.Compare(pressure,electric),0);
        Assert.Greater(DeckmasterMatch.Compare(electric,heat),0);
        Assert.Less(DeckmasterMatch.Compare(heat,Card(3,5,heat.element)),0);
        Assert.AreEqual(0,DeckmasterMatch.Compare(heat,heat));
    }
    [Test] public void EveryOwnedCardRemainsSelectableAcrossRounds()
    {
        var owned=Enumerable.Range(0,18).Select(i=>Card(i,3+i%6,(PlayerData.Card.Element)(i%3))).ToArray();
        var match=new DeckmasterMatch(owned.Concat(owned.Take(2)),new Random(42));
        while(!match.Complete)
        {
            CollectionAssert.AreEquivalent(owned.Select(c=>c.id),match.Hand.Select(c=>c.id));
            match.Play(match.Hand.Count-1);match.Deal();
        }
        Assert.AreEqual(18,owned.Length);
    }
    [Test] public void OpponentNeverExceedsOwnedPoolAndCannotChangeAfterSeeingChoice()
    {
        var owned=new[]{Card(0,3,PlayerData.Card.Element.Heat),Card(1,4,PlayerData.Card.Element.Pressure),Card(2,5,PlayerData.Card.Element.Electrical)};
        for(int seed=0;seed<30;seed++)
        {
            var match=new DeckmasterMatch(owned,new Random(seed));
            while(!match.Complete)
            {
                var committed=match.Opponent;
                Assert.IsTrue(owned.Any(c=>c.id==committed.id));
                Assert.AreEqual(3,match.Hand.Count);Assert.AreEqual(3,match.Hand.Select(c=>c.id).Distinct().Count());
                match.Play(0);Assert.AreEqual(committed.id,match.Opponent.id);match.Deal();
            }
            Assert.LessOrEqual(match.Rounds,9);Assert.AreEqual(3,owned.Length);
        }
    }
}
