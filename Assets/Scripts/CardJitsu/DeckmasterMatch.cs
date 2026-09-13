using System;
using System.Collections.Generic;
using System.Linq;

/** @brief A fair opponent commits before your choice and uses your collection's power pool. */
public sealed class DeckmasterMatch
{
    public const int WinCoins = 80;
    public const double CardDropChance = .05;
    private readonly System.Random random;
    private readonly List<PlayerData.Card> pool;
    private readonly List<PlayerData.Card> hand = new List<PlayerData.Card>();
    public IReadOnlyList<PlayerData.Card> Hand { get { return hand; } }
    public PlayerData.Card Opponent { get; private set; }
    public int PlayerWins { get; private set; }
    public int OpponentWins { get; private set; }
    public int Rounds { get; private set; }
    public bool Complete { get { return PlayerWins >= 3 || OpponentWins >= 3 || Rounds >= 9; } }
    public bool Won { get { return Complete && PlayerWins > OpponentWins; } }
    private bool rewardClaimed;
    public DeckmasterMatch(IEnumerable<PlayerData.Card> cards, System.Random rng)
    {
        random = rng;pool = cards.GroupBy(c=>c.id).Select(g=>g.First()).ToList();
        if(pool.Count == 0) throw new ArgumentException("A match needs at least one card.");
        Deal();
    }
    public void Deal()
    {
        if(Complete) return;
        // Commit independently before showing the player's hand. No reading the player's selection.
        Opponent = pool[random.Next(pool.Count)];
        hand.Clear();
        hand.AddRange(pool.OrderBy(card => card.element).ThenBy(card => card.power).ThenBy(card => card.id));
    }
    public int Play(int index)
    {
        if(Complete || index < 0 || index >= hand.Count) throw new InvalidOperationException("No playable card.");
        int result = Compare(hand[index],Opponent);Rounds++;
        if(result > 0) PlayerWins++;else if(result < 0) OpponentWins++;
        return result;
    }
    public static int Compare(PlayerData.Card a, PlayerData.Card b)
    {
        int ap=a.power*(Beats(a.element,b.element)?10:1),bp=b.power*(Beats(b.element,a.element)?10:1);
        return ap.CompareTo(bp);
    }
    private static bool Beats(PlayerData.Card.Element a,PlayerData.Card.Element b)
    {
        return (a==PlayerData.Card.Element.Heat && b==PlayerData.Card.Element.Pressure) ||
            (a==PlayerData.Card.Element.Pressure && b==PlayerData.Card.Element.Electrical) ||
            (a==PlayerData.Card.Element.Electrical && b==PlayerData.Card.Element.Heat);
    }
    public int ClaimReward(PlayerData player)
    {
        if(!Won || rewardClaimed) return -1;
        rewardClaimed=true;player.coins+=WinCoins;player.card_game_wins++;
        var missing=player.cards.Values.Where(c=>!player.unlocked_cards.Contains(c.id)).ToList();
        if(random.NextDouble() >= CardDropChance || missing.Count == 0) return -1;
        int id=missing[random.Next(missing.Count)].id;player.unlocked_cards.Add(id);return id;
    }
}
