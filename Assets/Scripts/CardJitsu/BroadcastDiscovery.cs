using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BroadcastDiscovery : MonoBehaviour
{
    private const int broadcastPort = 47777;
    private const string broadcastMessage = "HostAvailable";
    private UdpClient udpClient;
    private bool isBroadcasting;

    public static BroadcastDiscovery Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void StartBroadcasting()
    {
    try
        {
            if (udpClient == null)
            {
                udpClient = new UdpClient
                {
                    EnableBroadcast = true
                };
            }

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);

            Debug.Log("Starting to broadcast host availability...");
            isBroadcasting = true;
            while (isBroadcasting)
            {
                if (string.IsNullOrEmpty(broadcastMessage))
                {
                    Debug.LogError("BroadcastMessage is null or empty!");
                    break;
                }

                byte[] data = Encoding.UTF8.GetBytes(broadcastMessage);
                await udpClient.SendAsync(data, data.Length, endPoint);
                Debug.Log("Broadcasting host availability...");

                await Task.Delay(1000); // Broadcast every second
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Broadcasting failed: {ex.Message}");
        }
        finally
        {
            udpClient?.Close();
            udpClient = null;
            Debug.Log("Broadcasting stopped.");
        }
    }

    public async Task<string> DiscoverHost()
    {
        try
        {
            using (UdpClient discoveryClient = new UdpClient(AddressFamily.InterNetwork))
            {
                discoveryClient.EnableBroadcast = true;
                discoveryClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, broadcastPort);
                Debug.Log($"Binding to endpoint: {endPoint}");
                discoveryClient.Client.Bind(endPoint);

                Debug.Log("Listening for host broadcast...");
                while (true)
                {
                    Task<UdpReceiveResult> receiveTask = discoveryClient.ReceiveAsync();
                    while (!receiveTask.IsCompleted)
                    {
                        await Task.Yield();
                    }

                    UdpReceiveResult result = await receiveTask;
                    string receivedMessage = Encoding.UTF8.GetString(result.Buffer);

                    if (receivedMessage == broadcastMessage)
                    {
                        string hostIp = result.RemoteEndPoint.Address.ToString();
                        Debug.Log($"Discovered host at {hostIp}");
                        return hostIp;
                    }
                }
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Host discovery failed: {ex.Message}");
        }

        return null;
    }

    public void StopBroadcasting()
    {
        isBroadcasting = false;
    }

    private void OnDestroy()
    {
        // Ends the StartBroadcasting loop; without this it would keep
        // running after the object is destroyed (and after exiting play
        // mode in the editor)
        isBroadcasting = false;

        if (udpClient != null)
        {
            Debug.Log("Stopping broadcasting...");
            udpClient.Close();
            udpClient = null;
        }
    }
}