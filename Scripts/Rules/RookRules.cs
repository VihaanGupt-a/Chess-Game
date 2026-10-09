using UnityEngine;

public static class RookRules
{
    public static bool IsValidMove(
        ChessPiece rook,
        int targetX, 
        int targetZ,
        ChessPiece targetPiece,
        System.Func<int, int, ChessPiece> FindPieceAt
    )
    {
        int dx = targetX - rook.BoardX;
        int dz = targetZ - rook.BoardZ;

        if (dx != 0 && dz != 0)
        {
            return false;
        }

        int stepX = dx == 0 ? 0 : (dx > 0 ? 1 : -1);
        int stepZ = dz == 0 ? 0 : (dz > 0 ? 1 : -1);

        int currentX = rook.BoardX + stepX;
        int currentZ = rook.BoardZ + stepZ;

        while (currentX != targetX || currentZ != targetZ)
        {
            if (FindPieceAt(currentX , currentZ ) != null)
            {
                return false;
            }

            currentX += stepX;
            currentZ += stepZ;
        }

        if (targetPiece != null && targetPiece.Color == rook.Color)
        {
            return false;
        }

        return true;
    }
}



