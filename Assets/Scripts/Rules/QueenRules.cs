using UnityEngine;

public static class QueenRules
{
    public static bool IsValidMove(
        ChessPiece queen,
        int targetX,
        int targetZ,
        ChessPiece targetPiece,
        System.Func<int, int, ChessPiece> findPieceAt
    )
    {
        int dx = targetX - queen.BoardX;
        int dz = targetZ - queen.BoardZ;

        bool isStraight = dx == 0 || dz == 0;
        bool isDiagonal = Mathf.Abs(dx) == Mathf.Abs(dz);

        if (!isStraight && !isDiagonal)
        {
            return false;
        }

        int stepX = dx == 0 ? 0 : (dx > 0 ? 1 : -1);
        int stepZ = dz == 0 ? 0 : (dz > 0 ? 1 : -1);

        int currentX = queen.BoardX + stepX;
        int currentZ = queen.BoardZ + stepZ;

        while (currentX != targetX || currentZ != targetZ)
        {
            if (findPieceAt(currentX, currentZ) != null)
            {
                return false;
            }

            currentX += stepX;
            currentZ += stepZ;
    
        }

        if (targetPiece != null && targetPiece.Color == queen.Color)
        {
            return false;
        }

        return true;
    }
}








