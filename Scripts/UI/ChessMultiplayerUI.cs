using UnityEngine;
using TMPro;

public class ChessMultiplayerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roomCodeText;
    [SerializeField] private TMP_InputField joinRoomInput;

    private ChessMultiplayerManager manager;

    private void Start()
    {
        manager = FindAnyObjectByType<ChessMultiplayerManager>();

        if (manager != null)
        {
            manager.RoomCodeCreated += ShowRoomCode;
        }
        else
        {
            Debug.LogError("ChessMultiPlayerManager not found!");
        }
    }

    private void OnDestroy()
    {
        if (manager != null)
        {
            manager.RoomCodeCreated -= ShowRoomCode;
        }
    }
    
    public async void OnCreateRoomClicked()
    {
        if (manager == null)
        {
            Debug.LogError("ChessMultiplayerManager not found!");
            return;
        }

        roomCodeText.text = "Creating room...";
        await manager.CreateRoom();
    }

    public async void OnJoinRoomClicked()
    {
        if (manager == null || joinRoomInput == null)
            return;

        string code = joinRoomInput.text.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(code))
        {
            roomCodeText.text = "Enter a room code first."; 
            return;
        }

        roomCodeText.text = "Joining room...";
        await manager.JoinRoom(code);

        if (manager.IsInSession)
        {
            roomCodeText.text = "Joined Room: " + code;
        }
        else
        {
            roomCodeText.text = "Failed to join the room. Check console!";
        }
    }

    private void ShowRoomCode(string code)
    {
        if (roomCodeText != null)
            roomCodeText.text = "Room Code: " + code;
    }
}






