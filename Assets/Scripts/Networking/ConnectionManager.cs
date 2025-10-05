using Eflatun.SceneReference;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    [Tooltip("Where client should be sent when server disconnects")]
    public SceneReference menuScene;
    
    UnityTransport transport;

    public Allocation hostAlloc;
    public JoinAllocation joinAlloc;
    public Lobby currentLobby;
    public bool isHost;

    public static string lobbyCode;
    public static string relayCode;

    public static string playerName = "FunnyPlayerName";

    public NetworkObject sessionManagerPrefab;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        GetTransport();
        NetworkManager.Singleton.OnClientStarted += Singleton_OnClientStarted;
        NetworkManager.Singleton.OnClientStopped += Singleton_OnClientStopped;

    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }

    private void Singleton_OnClientStopped(bool obj)
    {
        //If our client is stopped and we're NOT on the menu scene, we should try to go back to it.
        if(SceneManager.GetActiveScene().buildIndex != menuScene.BuildIndex)
        {
            SceneManager.LoadScene(menuScene.BuildIndex);
        }
        LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
        currentLobby = null;
    }

    private void Singleton_OnClientStarted()
    {

    }

    void GetTransport()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    }
    public void ConnectionStarted()
    {

    }

    public async void StartHost()
    {
        isHost = true;
        //Starts a relay host
        relayCode = await StartRelayHost();
        //Then starts the lobby if a relay code exists, since we need it in the lobby data.
        if(!string.IsNullOrWhiteSpace(relayCode))
        {
            await StartLobby();
        }
        else
        {
            isHost = false;
        }
        //Then our Network Objects will take over from us.
    }

    public async void TryJoinLobby(string lobbyCode)
    {
        currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
        ConnectionManager.lobbyCode = lobbyCode;
        await JoinRelay(currentLobby.Data["relaycode"].Value);

    }

    public async Task JoinRelay(string relayCode)
    {
        ConnectionManager.relayCode = relayCode;
        joinAlloc = await RelayService.Instance.JoinAllocationAsync(relayCode);
        if(transport == null)
        {
            GetTransport();
        }
        transport.SetRelayServerData(AllocationUtils.ToRelayServerData(joinAlloc, "udp"));
        NetworkManager.Singleton.StartClient();
    }

    public async Task StartLobby()
    {
        CreateLobbyOptions clo = new CreateLobbyOptions()
        {
            IsPrivate = true,
            Data = new Dictionary<string, DataObject>
            {
                { "relaycode", new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
            }
        };
        clo.IsPrivate = true;
        currentLobby = await LobbyService.Instance.CreateLobbyAsync($"NewLobby{Random.Range(0, 100000)}", 4, clo);
        //Assign the lobby code
        lobbyCode = currentLobby.LobbyCode;
        //then create our session manager prefab
        if (sessionManagerPrefab != null)
        {
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(sessionManagerPrefab);
        }
    }

    public async Task<string> StartRelayHost(int maxConnections = 4)
    {
        hostAlloc = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        if(transport == null)
        {
            GetTransport();
        }
        transport.SetRelayServerData(AllocationUtils.ToRelayServerData(hostAlloc, "udp"));
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAlloc.AllocationId);
        //And try to spawn the session manager prefab if it exists
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

}
