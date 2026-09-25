using UnityEngine;

public static class BishopRules
{
    public static bool IsValidMove(
        ChessPiece bishop,
        int targetX,
        int targetZ,
        ChessPiece targetPiece,
        System.Func<int, int, ChessPiece> findPieceAt
    )
    {
        int dx = targetX - bishop.BoardX;
        int dz = targetZ - bishop.BoardZ;

        if (Mathf.Abs(dx) != Mathf.Abs(dz))
        {
            return false;
        }

        int stepX = dx > 0 ? 1 : -1;
        int stepZ = dz > 0 ? 1 : -1;

        int currentX = bishop.BoardX + stepX;
        int currentZ = bishop.BoardZ + stepZ;

        while (currentX != targetX || currentZ != targetZ)
        {
            if (findPieceAt(currentX, currentZ) != null)
            {
                return false;
            }

            currentX += stepX;
            currentZ += stepZ;
        }

        if (targetPiece != null &&targetPiece.Color == bishop.Color)
        {
            return false;
        }

        return true;
    }
}