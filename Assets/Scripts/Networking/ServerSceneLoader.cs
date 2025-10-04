using Eflatun.SceneReference;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Should be spawned when all players are ready to start.
/// </summary>
public class ServerSceneLoader : NetworkBehaviour
{
    public SceneReference sceneRef;

    public override void OnNetworkSpawn()
    {
        if(IsServer && sceneRef != null)
        {
            var status = NetworkManager.SceneManager.LoadScene(sceneRef.Name, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            if(status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Scene {sceneRef.Name} load failed - Reason:{status}");
            }
        }

    }
}
