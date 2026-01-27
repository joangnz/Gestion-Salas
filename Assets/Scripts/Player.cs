using Fusion;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    // Components
    [SerializeField] private NetworkCharacterController cc;

    // Child Components
    [SerializeField] private TextMeshPro tmp;

    // Networked Properties
    [Networked] private string Username { get; set; }

    public override void Spawned()
    {
        // If I'm spawning in my client, set my username
        if (Object.HasInputAuthority)
        {
            Username = PlayerPrefs.GetString("username");
            SetUsername_Rpc(Username);
        } else
        {
            tmp.text = Username;
        }
        // Else, if I'm being spawned in other clients, render my username
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            cc.Move(5*Runner.DeltaTime*data.direction);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void SetUsername_Rpc(string username)
    {
        Username = username;
        tmp.text = Username;
    }
}