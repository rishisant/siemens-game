using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
#if !UNITY_WEBGL || UNITY_EDITOR
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
#else
using System.Runtime.InteropServices;
#endif

[Serializable] public class TownPlayerState
{
    public string id, name, scene, facing;
    public float x, y;
    public bool moving;
    public int[] equipped;
}
[Serializable] public class TownPacket
{
    public string type, id, room, name, message, scene, facing;
    public float x,y;
    public bool moving;
    public int[] equipped;
    public TownPlayerState[] players;
}

/** @brief Room-based cross-platform presence, using browser or native WebSockets. */
public class TownMultiplayer : MonoBehaviour
{
    public static TownMultiplayer Instance { get; private set; }
    public string ServerUrl = "ws://127.0.0.1:8090/socket";
    public string DisplayName = "Explorer", RoomCode = "", Status = "Create a room or join a friend.";
    public bool Connected { get; private set; }
    public bool Connecting { get; private set; }
    public int PlayerCount { get; private set; }
    public readonly List<string> Chat = new List<string>();
    private string selfId, pendingJoin;
    private float nextSend, retryAt;
    private bool autoJoin;
    private int generation;
    private Character_Movement local;
    private readonly ConcurrentQueue<string> inbox = new ConcurrentQueue<string>();
    private readonly Dictionary<string,TownRemoteAvatar> avatars = new Dictionary<string,TownRemoteAvatar>();
#if !UNITY_WEBGL || UNITY_EDITOR
    private ClientWebSocket socket;
    private CancellationTokenSource lifetime;
    private bool sending;
    private readonly Queue<string> pendingMessages=new Queue<string>();
#else
    [DllImport("__Internal")] private static extern void BC_Connect(string url,string receiver);
    [DllImport("__Internal")] private static extern void BC_Send(string message);
    [DllImport("__Internal")] private static extern void BC_Close();
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        if (Instance == null) DontDestroyOnLoad(new GameObject("Town multiplayer").AddComponent<TownMultiplayer>().gameObject);
    }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance=this;
        DisplayName=PlayerPrefs.GetString("ByteCity.DisplayName","Explorer");
        ServerUrl=PlayerPrefs.GetString("ByteCity.ServerUrl",ServerUrl);
        if (!string.IsNullOrEmpty(Application.absoluteURL))
        {
            Uri origin;
            if(Uri.TryCreate(Application.absoluteURL,UriKind.Absolute,out origin))ServerUrl=(origin.Scheme=="https"?"wss":"ws")+"://"+origin.Authority+"/socket";
        }
        SceneManager.sceneLoaded+=OnScene;
        gameObject.AddComponent<TownMultiplayerUI>();
    }
    private void OnScene(Scene scene,LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") Disconnect();
        local=null;
        foreach(var avatar in avatars.Values)if(avatar!=null)Destroy(avatar.gameObject);
        avatars.Clear();
    }
    public void ConnectToTown() { autoJoin = true; ConnectMode("auto"); }
    public void Connect(bool create) { autoJoin = false; ConnectMode(create ? "create" : "join"); }
    private void ConnectMode(string mode)
    {
        Disconnect(true);
        Chat.Clear();
        Uri uri;
        if(!Uri.TryCreate(ServerUrl.Trim(),UriKind.Absolute,out uri)||(uri.Scheme!="ws"&&uri.Scheme!="wss")) { Status="Enter a valid server address in connection settings.";return; }
        DisplayName=string.IsNullOrWhiteSpace(DisplayName)?"Explorer":DisplayName.Trim();
        if(DisplayName.Length>20)DisplayName=DisplayName.Substring(0,20);
        PlayerPrefs.SetString("ByteCity.DisplayName",DisplayName);
        PlayerPrefs.SetString("ByteCity.ServerUrl",ServerUrl.Trim());
        if(LocalPlaytest.IsActive && PlayerData.Instance!=null)PlayerData.Instance.username=DisplayName;
        pendingJoin=JsonUtility.ToJson(new TownPacket {type=mode,room=RoomCode.Trim().ToUpperInvariant(),name=DisplayName});
        Connecting=true;Status="Connecting...";
#if !UNITY_WEBGL || UNITY_EDITOR
        OpenNative(uri,++generation);
#else
        BC_Connect(uri.ToString(),gameObject.name);
#endif
    }
#if !UNITY_WEBGL || UNITY_EDITOR
    private async void OpenNative(Uri uri,int attempt)
    {
        var connection=new ClientWebSocket();socket=connection;
        var tokenSource=new CancellationTokenSource();lifetime=tokenSource;
        try
        {
            using(var timeout=CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token))
            {
                timeout.CancelAfter(10000);await connection.ConnectAsync(uri,timeout.Token);
            }
            if(attempt!=generation)return;
            Send(pendingJoin);
            await ReceiveNative(connection,attempt,tokenSource.Token);
        }
        catch(Exception) { if(attempt==generation)OnSocketClosed("Could not connect. Check the server address and try again."); }
    }
    private async Task ReceiveNative(ClientWebSocket connection,int attempt,CancellationToken token)
    {
        var bytes=new byte[4096];
        while(connection.State==WebSocketState.Open && !token.IsCancellationRequested)
        {
            using(var stream=new System.IO.MemoryStream())
            {
                WebSocketReceiveResult received;
                do
                {
                    received=await connection.ReceiveAsync(new ArraySegment<byte>(bytes),token);
                    if(received.MessageType==WebSocketMessageType.Close) { if(attempt==generation)OnSocketClosed("Disconnected from the room.");return; }
                    stream.Write(bytes,0,received.Count);
                    if(stream.Length>16384)throw new InvalidOperationException("Message too large");
                } while(!received.EndOfMessage);
                if(attempt==generation)OnSocketMessage(Encoding.UTF8.GetString(stream.ToArray()));
            }
        }
    }
    private async void Send(string data)
    {
        var connection=socket;
        if(connection==null||connection.State!=WebSocketState.Open)return;
        if(sending)
        {
            // Old movement samples can be skipped; chat and room messages must wait their turn.
            if(!data.Contains("\"type\":\"state\"")&&pendingMessages.Count<16)pendingMessages.Enqueue(data);
            return;
        }
        sending=true;
        try
        {
            do
            {
                var bytes=Encoding.UTF8.GetBytes(data);
                await connection.SendAsync(new ArraySegment<byte>(bytes),WebSocketMessageType.Text,true,lifetime.Token);
                if(connection!=socket)return;
                data=pendingMessages.Count>0?pendingMessages.Dequeue():null;
            } while(data!=null);
        }
        catch(Exception) { if(connection==socket)OnSocketClosed("Connection lost. You can rejoin your room."); }
        finally { if(connection==socket)sending=false; }
    }
#else
    private void Send(string data) { BC_Send(data); }
#endif
    public void OnSocketOpen(string unused) { Send(pendingJoin); }
    public void OnSocketMessage(string data) { if(inbox.Count<64)inbox.Enqueue(data); }
    public void OnSocketClosed(string reason)
    {
        retryAt = Time.unscaledTime + 5f;
        Connected=false;Connecting=false;PlayerCount=0;Status=string.IsNullOrEmpty(reason)?"Disconnected. Join again when ready.":reason;
        foreach(var avatar in avatars.Values)if(avatar!=null)Destroy(avatar.gameObject);
        avatars.Clear();
    }
    public void Disconnect(bool preserveAuto = false)
    {
        if (!preserveAuto) autoJoin = false;
        generation++;
#if !UNITY_WEBGL || UNITY_EDITOR
        if(lifetime!=null) { lifetime.Cancel();lifetime.Dispose();lifetime=null; }
        if(socket!=null) { socket.Abort();socket.Dispose();socket=null; }
        sending=false;pendingMessages.Clear();
#else
        BC_Close();
#endif
        string ignored;while(inbox.TryDequeue(out ignored)){}
        OnSocketClosed("Create a room or join a friend.");
    }
    public void SendChat(string message)
    {
        if(!Connected || string.IsNullOrWhiteSpace(message))return;
        Send(JsonUtility.ToJson(new TownPacket {type="chat",message=message.Substring(0,Mathf.Min(140,message.Length))}));
    }
    private void Update()
    {
        if (autoJoin && !Connected && !Connecting && Time.unscaledTime >= retryAt) ConnectMode("auto");
        string json;
        while(inbox.TryDequeue(out json))
        {
            TownPacket packet;
            try { packet=JsonUtility.FromJson<TownPacket>(json); } catch { continue; }
            if(packet==null)continue;
            if(packet.type=="welcome") { Connected=true;Connecting=false;selfId=packet.id;RoomCode=packet.room;Status="Connected to town";GameToast.Show("Welcome to Byte City", "You are online  ·  Open Chat to say hello"); }
            if(packet.type=="error") { Status=packet.message;Connecting=false; }
            if(packet.type=="chat") { Chat.Add(packet.name+": "+packet.message);if(Chat.Count>8)Chat.RemoveAt(0); }
            if(packet.type=="snapshot")ApplySnapshot(packet.players);
        }
        if(!Connected || Time.unscaledTime<nextSend)return;
        nextSend=Time.unscaledTime+0.1f;
        if(local==null)local=FindObjectOfType<Character_Movement>();
        var pos=local!=null?local.transform.position:Vector3.zero;
        Send(JsonUtility.ToJson(new TownPacket {type="state",scene=SceneManager.GetActiveScene().name,x=pos.x,y=pos.y,facing=local!=null?local.NetworkFacing:"Down",moving=local!=null&&local.NetworkMoving,equipped=PlayerData.Instance!=null?PlayerData.Instance.equipped_items.ToArray():new int[0]}));
    }
    private void ApplySnapshot(TownPlayerState[] players)
    {
        if(!Connected||players==null)return;
        PlayerCount=players.Length;
        if(local==null)local=FindObjectOfType<Character_Movement>();
        var present=new HashSet<string>();
        foreach(var player in players)
        {
            if(player.id==selfId||local==null||player.scene!=SceneManager.GetActiveScene().name)continue;
            present.Add(player.id);
            TownRemoteAvatar avatar;
            if(!avatars.TryGetValue(player.id,out avatar)||avatar==null)
            {
                avatar=new GameObject("Visitor "+player.name).AddComponent<TownRemoteAvatar>();
                avatar.Initialize(local,player);avatars[player.id]=avatar;
            }
            avatar.Apply(player);
        }
        var removed=new List<string>();foreach(var pair in avatars)if(!present.Contains(pair.Key))removed.Add(pair.Key);
        foreach(var id in removed) { if(avatars[id]!=null)Destroy(avatars[id].gameObject);avatars.Remove(id); }
    }
    private void OnDestroy() { if(Instance!=this)return;SceneManager.sceneLoaded-=OnScene;Disconnect();Instance=null; }
}
