using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SessionHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;

    [SerializeField] private GameObject sessionItemPrefab;

    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    public void AddToList(SessionInfo sessionInfo)
    {
        SessionItem addedSessionItem = Instantiate(sessionItemPrefab, verticalLayoutGroup.transform).GetComponent<SessionItem>();

        addedSessionItem.SetInformation(sessionInfo);

        addedSessionItem.OnJoinSession += AddedSessionItem_OnJoinSession;
    }

    private void AddedSessionItem_OnJoinSession(SessionInfo obj)
    {
        SceneManager.LoadScene("GameScene");
    }

    private void OnNoSessionFound()
    {
        statusText.text = "No game session found";
        statusText.gameObject.SetActive(true);
    }

    private void OnLookingForGameSessions()
    {
        statusText.text = "Looking for game sessions";
        statusText.gameObject.SetActive(true);
    }

    public void ClearList()
    {
        foreach (Transform child in verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        statusText.gameObject.SetActive(false);
    }
}
