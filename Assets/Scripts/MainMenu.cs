using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private SessionHandler RoomBrowser;
    [SerializeField] private GameObject PlayerDetails;
    [SerializeField] private GameObject CreatePanel;
    [SerializeField] private GameObject StatusPanel;

    [Header("UI")]
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField sessionNameInput;

    private void Start()
    {
        username.onValueChanged.AddListener(delegate { UsernameTyped(); });
        HidePanels();
        RoomBrowser.gameObject.SetActive(true);
    }

    public void SetUsername(string username)
    {
        PlayerPrefs.SetString("username", username);
    }

    private void HidePanels() { 
        RoomBrowser.gameObject.SetActive(false);
        PlayerDetails.SetActive(false);
        CreatePanel.SetActive(false);
        StatusPanel.SetActive(false);
    }

    public void Browse()
    {
        SetUsername(username.text);
        //SceneManager.LoadScene("GameScene");

        NetworkRunnerHandler networkRunnerHandler = Object.FindFirstObjectByType<NetworkRunnerHandler>();
        networkRunnerHandler.OnJoinLobby();

        HidePanels();
        RoomBrowser.gameObject.SetActive(true);

        RoomBrowser.ClearList();
    }

    public void NewGame()
    {
        HidePanels();

        CreatePanel.SetActive(true);
    }

    public void ConfirmNewGame()
    {
        NetworkRunnerHandler networkRunnerHandler = FindFirstObjectByType<NetworkRunnerHandler>();
        networkRunnerHandler.CreateGame("");

        HidePanels();

        StatusPanel.SetActive(true);
    }

    private void UsernameTyped()
    {
        joinButton.interactable = CheckValidUsername(username.text);
    }

    private bool CheckValidUsername(string username)
    {
        // 4 characters minimum, 32 characters maximum, only letters and digits, at least one letter
        if (username.Length < 4 || username.Length > 32) return false;
        if (username.Any(c => !char.IsLetterOrDigit(c))) return false;
        if (!username.Any(c => char.IsLetter(c))) return false;

        return true;
    }
}
