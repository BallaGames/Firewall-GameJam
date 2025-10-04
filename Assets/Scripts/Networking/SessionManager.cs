using Balla.Core;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class SessionManager : BallaNetScript
{
    public TMP_Text lobbyCodeDisplay;
    
    public override void OnNetworkSpawn()
    {
        lobbyCodeDisplay.text = $"Code: {ConnectionManager.lobbyCode}";
    }
    public void CopyCodeToClipboard()
    {
        GUIUtility.systemCopyBuffer = ConnectionManager.lobbyCode;
    }
}
