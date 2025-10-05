using Balla.Core;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class SessionManager : BallaNetScript
{
    public TMP_Text lobbyCodeDisplay;

    private void OnGUI()
    {
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * (Screen.width / 1280));
        if (GUILayout.Button("Disconnect"))
        {
            NetworkManager.Shutdown();
        }
    }



    public override void OnNetworkSpawn()
    {
        lobbyCodeDisplay.text = $"Code: {ConnectionManager.lobbyCode}";
    }
    public void CopyCodeToClipboard()
    {
        GUIUtility.systemCopyBuffer = ConnectionManager.lobbyCode;
    }
}
