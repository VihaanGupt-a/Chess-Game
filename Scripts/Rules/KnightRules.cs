using UnityEngine;

public static class KnightRules 
{
    public static bool IsValidMove(
        ChessPiece knight,
        int targetX,
        int targetZ,
        ChessPiece targetPiece
    )
    {
        int dx = Mathf.Abs(targetX - knight.BoardX);
        int dz = Mathf.Abs(targetZ - knight.BoardZ);

        bool validLShape = (dx == 1 && dz == 2) || (dx == 2 && dz == 1);

        if (!validLShape)
        {
            return false;
        }

        if (targetPiece != null && targetPiece.Color == knight.Color)
        {
            return false;
        }

        return true;
    }
}