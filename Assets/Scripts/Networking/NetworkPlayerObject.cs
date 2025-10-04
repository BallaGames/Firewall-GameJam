using Balla.Core;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerObject : BallaNetScript
{
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>("PlayerName_001", 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField, ReadOnly] protected string nameValue;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            PlayerName.Value = new FixedString32Bytes(ConnectionManager.playerName);        
        }
        nameValue = PlayerName.Value.ToString();
    }
}
