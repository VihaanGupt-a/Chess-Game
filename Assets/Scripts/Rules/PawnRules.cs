using UnityEngine;

public static class PawnRules
{
    public static bool IsValidMove(
        ChessPiece pawn,
        int targetX,
        int targetZ,
        ChessPiece targetPiece,
        ChessPiece middlePiece
    )
    {
        int dx = targetX - pawn.BoardX;
        int dz = targetZ - pawn.BoardZ;

        int direction = pawn.Color == ChessPiece.PieceColor.White
            ? 1
            : -1;

        if (dx == 0 && dz == direction )
        {
            return targetPiece == null;
        }

        if (dx == 0 && dz == direction * 2)
        {
            bool startingPosition = pawn.Color == ChessPiece.PieceColor.White
                ? pawn.BoardZ == 1
                : pawn.BoardZ == 6;

            return startingPosition && middlePiece == null && targetPiece == null;
        }

        if (Mathf.Abs(dx) == 1 && dz == direction)
        {
            return targetPiece != null && targetPiece.Color != pawn.Color;
        }

        return false;
    }
}


