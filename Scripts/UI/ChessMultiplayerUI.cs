using UnityEngine;
using TMPro;

public class ChessMultiplayerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roomCodeText;
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

    private void ShowRoomCode(string code)
    {
        roomCodeText.text = "Room Code: " + code;
    }
}






