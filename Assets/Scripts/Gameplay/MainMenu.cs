using Unity.Netcode;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup hostUI, joinUI, settingsUI;
    public string joinCode;

    public CanvasGroup mainUI;

    private void Start()
    {
        NetworkManager.Singleton.OnClientStarted += Singleton_OnClientStarted;
        NetworkManager.Singleton.OnClientStopped += Singleton_OnClientStopped;
    }

    private void Singleton_OnClientStopped(bool obj)
    {
        if(mainUI != null)
        {
            mainUI.SetGroupActive(true);
        }
    }

    private void Singleton_OnClientStarted()
    {
        if (mainUI != null)
        {
            mainUI.SetGroupActive(false);
        }
    }

    public void GameQuit()
    {

    }
    public void StartHost()
    {
        ConnectionManager.Instance.StartHost();
    }
    public void JoinGame()
    {
        if (string.IsNullOrWhiteSpace(joinCode))
            return;
        ConnectionManager.Instance.TryJoinLobby(joinCode);
    }
    public void SetJoinCode(string code)
    {
        joinCode = code;
    }
    public void EnableUI(CanvasGroup group)
    {
        group.SetGroupActive(true);
    }
    public void DisableUI(CanvasGroup group)
    {
        group.SetGroupActive(false);
    }
}
