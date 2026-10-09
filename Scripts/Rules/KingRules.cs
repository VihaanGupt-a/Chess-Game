using UnityEngine;

public static class KingRules 
{
    public static bool IsValidMove(
        ChessPiece king,
        int targetX,
        int targetZ,
        ChessPiece targetPiece
    )
    {
        int dx = Mathf.Abs(targetX - king.BoardX);
        int dz = Mathf.Abs(targetZ - king.BoardZ);

        if (dx > 1 || dz > 1)
        {
            return false;
        }

        if (targetPiece != null && targetPiece.Color == king.Color)
        {
            return false;
        }

        return true;

    }
}