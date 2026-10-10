using UnityEngine;

public class ChessSquare : MonoBehaviour 
{
    private int boardX;
    private int boardZ;

    public void Initialize(int x, int z)
    {
        boardX = x;
        boardZ = z;
    }

    private void OnMouseDown()
    {
        Debug.Log($"Square clicked: ({boardX}, {boardZ})");

        if (ChessGameManager.Instance != null)
        {
            Debug.Log("Calling TryMoveSelected...");
            ChessGameManager.Instance.TryMoveSelected(boardX, boardZ);
        }
        else
        {
            Debug.LogError("ChessGameManager.Instance is NULL!");
        }
    }

}