using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Fusion;

public class SessionItem: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sessionNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Button joinButton;

    SessionInfo sessionInfo;

    public event Action<SessionInfo> OnJoinSession;

    public void SetInformation(SessionInfo sessionInfo)
    {
        this.sessionInfo = sessionInfo;

        sessionNameText.text = sessionInfo.Name;
        playerCountText.text = $"{sessionInfo.PlayerCount}/{sessionInfo.MaxPlayers}";

        bool joinable = sessionInfo.PlayerCount < sessionInfo.MaxPlayers;

        joinButton.gameObject.SetActive(joinable);
        Debug.Log(joinable);
    }

    public void OnClick()
    {
        OnJoinSession?.Invoke(sessionInfo);
    }
}