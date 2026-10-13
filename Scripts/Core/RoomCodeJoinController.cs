
using System;
using UnityEngine;
using UnityEngine.UI;

public class RoomCodeJoinController : MonoBehaviour
{
    [Header("Room Code UI")]
    [SerializeField] private RoomCodeDisplay roomCodeDisplay;
    [SerializeField] private Button joinRoomButton;

    [Header("Multiplayer")]
    [SerializeField] private ChessMultiplayerManager multiplayerManager;

    private void Awake()
    {
        if (joinRoomButton != null)
            joinRoomButton.onClick.AddListener(OnJoinClicked);
    }

    private void OnDestroy()
    {
        if (joinRoomButton != null)
            joinRoomButton.onClick.RemoveListener(OnJoinClicked);
    }

    private async void OnJoinClicked()
    {
        if (roomCodeDisplay == null)
            roomCodeDisplay = FindAnyObjectByType<RoomCodeDisplay>();

        if (multiplayerManager == null)
            multiplayerManager = FindAnyObjectByType<ChessMultiplayerManager>();

        if (roomCodeDisplay == null || multiplayerManager == null)
        {
            Debug.LogError(
                "RoomCodeJoinController: RoomCodeDisplay or ChessMultiplayerManager not found."
            );
            return;
        }

        string code = roomCodeDisplay.GetEnteredRoomCode();

        if (code.Length != 6)
        {
            Debug.LogWarning("Please enter all 6 characters of the room code.");
            return;
        }

        if (joinRoomButton != null)
            joinRoomButton.interactable = false;

        try
        {
            await multiplayerManager.JoinRoom(code);
        }
        catch (Exception e)
        {
            Debug.LogError($"Joining room failed: {e.Message}");
        }
        finally
        {
            if (joinRoomButton != null)
                joinRoomButton.interactable = true;
        }
    }
}





