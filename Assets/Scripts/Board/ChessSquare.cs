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

    public void OnMouseDown()
    {
        if (ChessGameManager.Instance != null)
        {
            ChessGameManager.Instance.TryMoveSelected(boardX, boardZ);
        }
    }

}