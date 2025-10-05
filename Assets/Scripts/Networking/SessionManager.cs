using Balla.Core;
using Eflatun.SceneReference;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SessionManager : BallaNetScript
{
    public TMP_Text lobbyCodeDisplay;
    public SceneReference gameScene;

    public CanvasGroup lobbyUI, readyUI;

    public TMP_Text playerCounter;

    public int readyPlayers;
    public CanvasGroup readyButton, readyPressedMessage;

    public CanvasGroup gameStartMessage;
    public TMP_Text gameStartCountdown;

    public NetworkObject playerCharacter;

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("Disconnect"))
        {
            NetworkManager.Shutdown();
        }
        GUILayout.EndVertical();
    }

    public void StartGame()
    {
        if (!IsServer)
        {
            //Cannot start game if we're not the server.
            return;
        }
        lobbyUI.SetGroupActive(false);
        SceneEventProgressStatus status = NetworkManager.SceneManager.LoadScene(gameScene.Name, LoadSceneMode.Single);
        if(status != SceneEventProgressStatus.Started)
        {
            //Something went wrong here!
            return;
        }
        readyUI.SetGroupActive(true);
        WaitForReady();
    }

    public void WaitForReady()
    {
        readyButton.SetGroupActive(true);
        readyPressedMessage.SetGroupActive(false);
    }

    public void SendReadyUp()
    {
        ReadyUp_RPC();
        readyButton.SetGroupActive(false);
        readyPressedMessage.SetGroupActive(true);
    }
    [Rpc(SendTo.Server)]
    public void ReadyUp_RPC(RpcParams @params = default)
    {
        Debug.Log($"{@params.Receive.SenderClientId} has readied up.");
        if(readyPlayers == NetworkManager.ConnectedClients.Count)
        {
            SendGameStart_RPC();
        }
    }
    [Rpc(SendTo.ClientsAndHost, DeferLocal = true)]
    public void SendGameStart_RPC()
    {
        StartCoroutine(BeginGameCountdown());
    }
    IEnumerator BeginGameCountdown()
    {
        int time = 0;
        gameStartMessage.SetGroupActive(true);
        while (time < 3)
        {
            gameStartCountdown.text = $"Game Starting in {3 - time}";
            time++;
            yield return new WaitForSeconds(1);
        }
        gameStartCountdown.text = "Game starting!";
        yield return new WaitForSeconds(1);
        SpawnPlayers();
    }
    public void SpawnPlayers()
    {
        if (playerCharacter != null)
        {
            for (int i = 0; i < readyPlayers; i++)
            {
                Debug.Log($"Spawning character for player {i}");
                NetworkManager.SpawnManager.InstantiateAndSpawn(playerCharacter, NetworkManager.ConnectedClientsIds[i], false, false, false, Vector3.zero, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("Should've spawned players, no prefab specified.");
        }
    }

    public void DisconnectFromGame()
    {
        ConnectionManager.Instance.Disconnect();
    }

    public override void OnNetworkSpawn()
    {
        lobbyCodeDisplay.text = $"Code: {ConnectionManager.lobbyCode}";

        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;

        NetworkManager.SceneManager.OnSceneEvent += SceneLoadevent;
    }

    private void SceneLoadevent(Unity.Netcode.SceneEvent sceneEvent)
    {
        switch (sceneEvent.SceneEventType)
        {
            case Unity.Netcode.SceneEventType.Load:
                Debug.Log("scene loading");
                break;
            case Unity.Netcode.SceneEventType.Unload:
                Debug.Log("scene unloading");
                break;
            case Unity.Netcode.SceneEventType.Synchronize:
                Debug.Log("scene synchronised");
                break;
            case Unity.Netcode.SceneEventType.ReSynchronize:
                Debug.Log("resynchronising scene");
                break;
            case Unity.Netcode.SceneEventType.LoadEventCompleted:
                Debug.Log("All players have loaded this scene");
                break;
            case Unity.Netcode.SceneEventType.UnloadEventCompleted:
                Debug.Log("Unload Event completed (?)");
                break;
            case Unity.Netcode.SceneEventType.LoadComplete:
                Debug.Log("Load completed (?)");
                break;
            case Unity.Netcode.SceneEventType.UnloadComplete:
                Debug.Log("Unload completed (?)");
                break;
            case Unity.Netcode.SceneEventType.SynchronizeComplete:
                Debug.Log("Completed scene sync");
                break;
            case Unity.Netcode.SceneEventType.ActiveSceneChanged:
                Debug.Log("Active scene changed");
                break;
            case Unity.Netcode.SceneEventType.ObjectSceneChanged:
                Debug.Log("Object Scene Changed (?)");
                break;
            default:
                break;
        }
    }

    private void OnClientDisconnected(ulong obj)
    {
        UpdatePlayerCounter();
    }

    private void OnClientConnected(ulong obj)
    {
        UpdatePlayerCounter();
    }
    public void UpdatePlayerCounter()
    {
        playerCounter.text = $"Players: {NetworkManager.ConnectedClients.Count}";
    }

    public void CopyCodeToClipboard()
    {
        GUIUtility.systemCopyBuffer = ConnectionManager.lobbyCode;
    }
}
