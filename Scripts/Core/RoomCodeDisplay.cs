using TMPro;
using UnityEngine;

public class RoomCodeDisplay : MonoBehaviour 
{
    [Header("Six room code input fiels")]
    [SerializeField] private TMP_InputField[] inputFields = new TMP_InputField[6];

    [Header("Multiplayer")]
    [SerializeField] private ChessMultiplayerManager multiplayerManager;

    private void Start()
    {
        if (multiplayerManager == null)
        {
            multiplayerManager = FindAnyObjectByType<ChessMultiplayerManager>();
        }

        if (multiplayerManager == null)
        {
            Debug.LogError("RoomCodeDisplay: ChessMultiPlayerManager not found!");
            return;
        }

        multiplayerManager.RoomCodeCreated += DisplayRoomCode;
        multiplayerManager.RoomLeft += ClearCode;
    }

    private void OnDestroy()
    {
        if (multiplayerManager != null)
        {
            multiplayerManager.RoomCodeCreated -= DisplayRoomCode;
            multiplayerManager.RoomLeft -= ClearCode;
        }
    }

    public void DisplayRoomCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            ClearCode();
            return;
        }

        code = code.ToUpperInvariant();

        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i] == null)
                continue;

            inputFields[i].SetTextWithoutNotify( i < code.Length ? code[i].ToString() : "");
        }

        Debug.Log($"Room code displayed: {code}");
    }

    public void ClearCode()
    {
        foreach (TMP_InputField inputField in inputFields)
        {
            if (inputField != null)
            {
                inputField.SetTextWithoutNotify("");
            }
        }
    }

    public string GetEnteredRoomCode()
    {
        string code = "";

        foreach (TMP_InputField inputField in inputFields)
        {
            if (inputField != null)
                code += inputField.text.Trim();
        }

        return code.ToUpperInvariant();
    }
}












