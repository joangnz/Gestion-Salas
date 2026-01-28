using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    SessionHandler _sessionHandler;

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();

    private InputAction moveAction;

    private void Awake()
    {
        _sessionHandler = FindFirstObjectByType<SessionHandler>(FindObjectsInactive.Include);
    }

    async void StartGame(GameMode mode, string sessionName)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner.ProvideInput = true;

        moveAction = InputSystem.actions.FindAction("Move");
        moveAction.Enable();

        // Create the NetworkSceneInfo from the game scene
        var scene = SceneRef.FromIndex(1);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            CustomLobbyName = "MyLobby",
            PlayerCount = 2,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player Joined the Scene");
        if (runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = new(0, 5, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            runner.Despawn(_spawnedCharacters[player]);
            _spawnedCharacters.Remove(player);
        }
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        data.direction = new(moveValue.x, 0f, moveValue.y);

        input.Set(data);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("UpdatedList");
        if (_sessionHandler == null) return;

        if (sessionList.Count == 0) _sessionHandler.OnNoSessionFound();
        else
        {
            _sessionHandler.ClearList();

            foreach (SessionInfo sessionInfo in sessionList)
            {
                _sessionHandler.AddToList(sessionInfo);
            }
        }
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnJoinLobby()
    {
        _runner = gameObject.AddComponent<NetworkRunner>();

        var clientTask = JoinLobby();
    }

    private async Task JoinLobby()
    {
        string lobbyId = "MyLobby";

        var result = await _runner.JoinSessionLobby(SessionLobby.Custom, lobbyId);

        if (!result.Ok)
        {
            Debug.LogError("Unable to join.");
        }
        else
        {
            Debug.Log("Joined");
        }
    }

    public void CreateGame(string sessionName)
    {
        PlayerPrefs.SetString("mode", "Host");
        PlayerPrefs.SetString("session", sessionName);
        StartGame(GameMode.Host, sessionName);
    }

    public void JoinGame(SessionInfo sessionInfo)
    {
        PlayerPrefs.SetString("session", sessionInfo.Name);
        StartGame(GameMode.Client, sessionInfo.Name);
    }
}