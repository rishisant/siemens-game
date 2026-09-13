using System.Collections;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class DialogueInputProbe : DialogueManagerBase
{
    public bool advanced;
    public IEnumerator Run()
    {
        isTyping=true;
        StartCoroutine(TypeSentence("A long sentence that should stay visible until the reader chooses to continue."));
        yield return WaitForLineAdvance();
        advanced=true;
    }
}

public class WorldUiRegressionTests
{
    [UnityTest] public IEnumerator RevealingNeedsAnotherInputBeforeAdvancing()
    {
        var go=new GameObject("Dialogue input test");
        var probe=go.AddComponent<DialogueInputProbe>();
        probe.dialogueText=new GameObject("Text",typeof(RectTransform)).AddComponent<TextMeshProUGUI>();
        probe.dialogueText.transform.SetParent(go.transform);
        probe.StartCoroutine(probe.Run());yield return null;yield return null;
        Assert.IsTrue(probe.IsRevealing);
        probe.RequestAdvance();probe.RequestAdvance();yield return null;yield return null;
        Assert.IsFalse(probe.IsRevealing);Assert.IsFalse(probe.advanced);
        yield return null;Assert.IsFalse(probe.advanced);
        probe.RequestAdvance();yield return null;yield return null;
        Assert.IsTrue(probe.advanced);Object.Destroy(go);
    }
    [UnityTest] public IEnumerator DeckmasterConversationRevealsCardsThenReturnsToPlay()
    {
        SceneManager.LoadScene("MainMenu");yield return null;yield return null;
        if(GameManager.Instance==null)new GameObject("GameManager").AddComponent<GameManager>();
        PlayerData.Instance.npc_interactions["labenter"]=1;
        PlayerData.Instance.npc_interactions["deckmaster"]=0;
        SceneManager.LoadScene("Laboratory_Main");yield return null;yield return null;
        var dialogue=Object.FindObjectOfType<DialogueManager_Lab>();
        dialogue.StopAllCoroutines();dialogue.dialoguePanel.SetActive(false);dialogue.DeckMasterSpeak();
        yield return null;yield return null;
        var player=Object.FindObjectOfType<Character_Movement>();Assert.IsFalse(player.CanMove);
        var surface=dialogue.dialoguePanel.GetComponent<DialogueSurface>();Assert.IsNotNull(surface);
        var background=surface.GetComponentsInChildren<UiSurface>().Single(g=>g.name=="Conversation background");
        Assert.IsNotNull(background.GetComponent<CanvasRenderer>());
        Assert.AreEqual(ByteCityTheme.Current.dialoguePanel.texture,background.sprite.texture);
        Assert.AreEqual(ByteCityTheme.Font,dialogue.dialogueText.font);
        for(int i=0;i<100 && Object.FindObjectOfType<CardGallery>()==null;i++)
        {
            dialogue.RequestAdvance();yield return new WaitForSeconds(.04f);
        }
        var gallery=Object.FindObjectOfType<CardGallery>();Assert.IsNotNull(gallery);Assert.IsFalse(dialogue.dialoguePanel.activeSelf);
        Assert.AreEqual(1,PlayerData.Instance.npc_interactions["deckmaster"]);
        Assert.IsFalse(player.CanMove);
        gallery.GetComponentsInChildren<Button>().Single(b=>b.name=="Done").onClick.Invoke();yield return null;yield return null;
        var hub=Object.FindObjectOfType<DeckmasterHub>();Assert.IsNotNull(hub);Assert.IsFalse(player.CanMove);
        hub.GetComponentsInChildren<Button>().Single(b=>b.name=="Close").onClick.Invoke();yield return null;yield return null;
        Assert.IsTrue(player.CanMove);
        SceneManager.LoadScene("MainMenu");yield return null;
    }
    [UnityTest] public IEnumerator CollectionNavigatesAndReleasesWorldFocus()
    {
        SceneManager.LoadScene("MainMenu");yield return null;yield return null;
        CardGallery.Show("Your collection",new[]{0,4,9});yield return null;yield return new WaitForSeconds(.2f);
        var gallery=Object.FindObjectOfType<CardGallery>();Assert.IsTrue(WorldUiFocus.Blocked);
        Assert.AreEqual(ByteCityTheme.Current.cardFrame.texture,gallery.GetComponentsInChildren<UiSurface>().Single(g=>g.name=="Original card book").sprite.texture);
        var buttons=gallery.GetComponentsInChildren<Button>();
        var previous=buttons.Single(b=>b.name=="Previous card");var next=buttons.Single(b=>b.name=="Next card");
        Assert.IsFalse(previous.interactable);Assert.IsTrue(next.interactable);
        next.onClick.Invoke();next.onClick.Invoke();Assert.IsFalse(next.interactable);Assert.IsTrue(previous.interactable);
        Assert.AreEqual("3 / 3",gallery.GetComponentsInChildren<TMP_Text>().Single(t=>t.name=="Card count").text);
        foreach(var surface in gallery.GetComponentsInChildren<UiSurface>())Assert.IsNotNull(surface.GetComponent<CanvasRenderer>());
        var safe=(RectTransform)gallery.transform.Find("Safe area");var panel=(RectTransform)safe.Find("Card details");
        Assert.LessOrEqual(panel.rect.width,safe.rect.width);Assert.LessOrEqual(panel.rect.height,safe.rect.height);
        buttons.Single(b=>b.name=="Done").onClick.Invoke();yield return null;yield return null;
        Assert.IsNull(Object.FindObjectOfType<CardGallery>());Assert.IsFalse(WorldUiFocus.Blocked);
        CardGallery.Show("Common card pack",new int[0]);yield return null;yield return null;
        gallery=Object.FindObjectOfType<CardGallery>();Assert.AreEqual("0 cards",gallery.GetComponentsInChildren<TMP_Text>().Single(t=>t.name=="Card count").text);
        Assert.IsTrue(gallery.GetComponentsInChildren<Button>().Where(b=>b.name=="Previous card"||b.name=="Next card").All(b=>!b.interactable));
        Object.Destroy(gallery.gameObject);yield return null;
    }
}
