using UnityEngine;
using Unity.Netcode;

public class ChessLobbyUIVisibility : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject inGamePanel;

    [Header("Lobby Controls")]
    [SerializeField] private GameObject createRoomButton;
    [SerializeField] private GameObject joinRoomInput;
    [SerializeField] private GameObject joinRoomButton;
    [SerializeField] private GameObject roomCodeText;

    private ChessMultiplayerManager multiplayerManager;
    private bool waitingRoomConfigured;
    private bool gameUIShown;

    private void Start()
    {
        multiplayerManager = FindAnyObjectByType<ChessMultiplayerManager>();

        if (multiplayerManager == null)
        {
            Debug.LogError("ChessMultiplayerManager not found!");
            enabled = false;
            return;
        }

        if (inGamePanel != null)
            inGamePanel.SetActive(false);
    }

    private void Update()
    {
        if (gameUIShown || multiplayerManager == null)
            return;

        if (!multiplayerManager.IsInSession)
            return;

        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager == null)
            return;

        if (networkManager.IsServer)
        {
            if (networkManager.ConnectedClientsIds.Count < 2)
            {
                if (!waitingRoomConfigured)
                {
                    if (createRoomButton != null)
                        createRoomButton.SetActive(false);

                    if (joinRoomInput != null)
                        joinRoomInput.SetActive(false);

                    if (joinRoomButton != null)
                        joinRoomButton.SetActive(false);

                    if (roomCodeText != null)
                        roomCodeText.SetActive(true);

                    waitingRoomConfigured = true;

                    Debug.Log("Waiting for second player ...");
                }

                return;
            }
        }

        if (lobbyPanel != null)
            lobbyPanel.SetActive(false);

        if (inGamePanel != null)
            inGamePanel.SetActive(true);


        gameUIShown = true;

        Debug.Log("Both Players Connected. Lobby Hidden.");
    }
}