using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class ArcadePresentationTests
{
    private const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
    [UnityTest] public IEnumerator RemoteRobotsBlockMovementWithoutActivatingWorldTriggers()
    {
        SceneManager.LoadScene("MainMenu");yield return null;yield return null;
        if(GameManager.Instance==null)new GameObject("GameManager").AddComponent<GameManager>();
        SceneManager.LoadScene("Town_Square");yield return null;yield return null;
        var local=Object.FindObjectOfType<Character_Movement>();local.enabled=false;
        var body=local.GetComponent<Rigidbody2D>();body.position=new Vector2(100,100);Physics2D.SyncTransforms();
        var sensor=new GameObject("Test door sensor").AddComponent<BoxCollider2D>();sensor.isTrigger=true;sensor.transform.position=new Vector3(102,100,0);
        var visitor=new GameObject("Test visitor").AddComponent<TownRemoteAvatar>();
        visitor.Initialize(local,new TownPlayerState{id="collision-test",name="Visitor",x=102,y=100,facing="Down",equipped=new int[0]});
        var blocker=visitor.GetComponent<Collider2D>();
        Assert.IsNotNull(blocker);Assert.IsFalse(blocker.isTrigger);
        Assert.IsTrue(Physics2D.GetIgnoreCollision(blocker,sensor));
        Assert.IsFalse(Physics2D.GetIgnoreCollision(blocker,local.GetComponent<Collider2D>()));
        Assert.IsNotNull(local.GetComponent<UnityEngine.Rendering.SortingGroup>());
        Assert.IsNotNull(visitor.GetComponent<UnityEngine.Rendering.SortingGroup>());
        for(int step=0;step<40;step++){body.velocity=Vector2.right*4;yield return new WaitForFixedUpdate();}
        Assert.Less(body.position.x,101.4f,"The local robot must stop at the visitor.");
        Object.Destroy(visitor.gameObject);Object.Destroy(sensor.gameObject);yield return null;
        for(int step=0;step<20;step++){body.velocity=Vector2.right*4;yield return new WaitForFixedUpdate();}
        Assert.Greater(body.position.x,102f,"A disconnected visitor must stop blocking the path.");
        body.velocity=Vector2.zero;
    }
    [UnityTest] public IEnumerator DuelWaitsBeforeScoringAndRestoresLabMusicWhenClosed()
    {
        SceneManager.LoadScene("MainMenu");yield return null;yield return null;
        if(GameManager.Instance==null)new GameObject("GameManager").AddComponent<GameManager>();
        SceneManager.LoadScene("Laboratory_Main");yield return null;yield return null;
        PlayerData.Instance.unlocked_cards=PlayerData.Instance.cards.Keys.ToList();
        var playing=Object.FindObjectsOfType<AudioSource>().Where(s=>s.loop && s.isPlaying).ToArray();
        DeckmasterDuel.Show();yield return null;yield return null;
        var duel=Object.FindObjectOfType<DeckmasterDuel>();
        var match=(DeckmasterMatch)typeof(DeckmasterDuel).GetField("match",Private).GetValue(duel);
        Assert.AreEqual(PlayerData.Instance.unlocked_cards.Count,match.Hand.Count);
        Assert.AreEqual(ByteCityTheme.Current.cardMusic,duel.GetComponents<AudioSource>().Single(s=>s.loop).clip);
        foreach(var source in playing)Assert.IsFalse(source.isPlaying);
        var more=duel.GetComponentsInChildren<Button>().Single(b=>b.name=="More cards");more.onClick.Invoke();yield return null;
        var hand=(RectTransform)typeof(DeckmasterDuel).GetField("handRoot",Private).GetValue(duel);
        Assert.Greater(hand.childCount,0);Assert.LessOrEqual(hand.childCount,7);
        duel.Play(7);duel.Play(7);
        Assert.IsTrue(duel.Resolving);Assert.AreEqual(0,match.Rounds);
        yield return new WaitForSecondsRealtime(1.05f);
        Assert.AreEqual(0,match.Rounds,"No score before the opponent suspense and reveal finish.");
        float end=Time.realtimeSinceStartup+5;
        while(duel.Resolving && Time.realtimeSinceStartup<end)yield return null;
        Assert.IsFalse(duel.Resolving);Assert.AreEqual(1,match.Rounds);
        Object.Destroy(duel.gameObject);yield return null;
        foreach(var source in playing)if(source!=null)Assert.IsTrue(source.isPlaying);
    }
    [UnityTest] public IEnumerator LeavingDuringRevealCancelsTheRoundAndMinimapCleansUpOnSceneChange()
    {
        SceneManager.LoadScene("MainMenu");yield return null;yield return null;
        if(GameManager.Instance==null)new GameObject("GameManager").AddComponent<GameManager>();
        SceneManager.LoadScene("Town_Square");yield return null;yield return null;yield return null;
        var map=Object.FindObjectOfType<TownMinimap>();Assert.IsNotNull(map);
        Assert.GreaterOrEqual(map.LandmarkCount,3);Assert.IsNull(map.GetComponentInChildren<Camera>());
        Assert.IsNull(map.GetComponentInChildren<RawImage>());Assert.GreaterOrEqual(map.GetComponentsInChildren<MapMarkerGraphic>().Length,4);
        DeckmasterDuel.Show();yield return null;yield return null;
        var duel=Object.FindObjectOfType<DeckmasterDuel>();var match=(DeckmasterMatch)typeof(DeckmasterDuel).GetField("match",Private).GetValue(duel);
        duel.Play(0);Object.Destroy(duel.gameObject);yield return null;
        yield return new WaitForSecondsRealtime(1.5f);Assert.AreEqual(0,match.Rounds);
        SceneManager.LoadScene("PipeGame");yield return null;yield return null;
        Assert.IsNull(Object.FindObjectOfType<TownMinimap>());
        var background=Object.FindObjectsOfType<Image>().Single(i=>i.name=="Background");
        Assert.AreEqual(ByteCityTheme.Current.pipeBackground,background.sprite);Assert.AreEqual(Color.white,background.color);Assert.IsFalse(background.raycastTarget);
    }
}
