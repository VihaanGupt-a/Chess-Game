using UnityEngine;

public static class ChessRules 
{
    public static bool IsInsideBoard(int x, int z)
    {
        return x >= 0 && x < 8 && z >= 0 && z < 8;
    }

    public static bool IsSquareUnderAttack(
        int targetX,
        int targetZ,
        ChessPiece.PieceColor attackingColor,
        System.Collections.Generic.List<ChessPiece> pieces
    )
    {
        foreach (ChessPiece piece in pieces)
        {
            if (piece.Color != attackingColor)
                continue;

            int dx = targetX - piece.BoardX;
            int dz = targetZ - piece.BoardZ;

            if (piece.Type == ChessPiece.PieceType.King)
            {
                if (Mathf.Abs(dx) <= 1 && Mathf.Abs(dz) <= 1)
                {
                    return true;
                }
            }

            if (piece.Type == ChessPiece.PieceType.Knight)
            {
                int absX = Mathf.Abs(dx);
                int absZ = Mathf.Abs(dz);

                if ((absX == 1 && absZ == 2) || (absX == 2 && absZ == 1))
                {
                    return true;
                }
            }

            if (piece.Type == ChessPiece.PieceType.Pawn)
            {
                int direction = piece.Color == ChessPiece.PieceColor.White
                    ? 1
                    : -1;

                if (Mathf.Abs(dx) == 1 && dz == direction)
                {
                    return true;
                }
            }

            if (piece.Type == ChessPiece.PieceType.Rook || piece.Type == ChessPiece.PieceType.Queen)
            {
                if (dx == 0 || dz == 0)
                {
                    if (IsPathClear(piece, targetX, targetZ, pieces))
                    {
                        return true;
                    }
                }
            }

            if (piece.Type == ChessPiece.PieceType.Bishop || piece.Type == ChessPiece.PieceType.Queen)
            {
                if (Mathf.Abs(dx) == Mathf.Abs(dz))
                {
                    if (IsPathClear(piece, targetX, targetZ, pieces))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool IsPathClear(
        ChessPiece piece,
        int targetX,
        int targetZ,
        System.Collections.Generic.List<ChessPiece> pieces
    )
    {
        int dx = targetX - piece.BoardX;
        int dz = targetZ - piece.BoardZ;

        int stepX = dx == 0 ? 0 : (dx > 0 ? 1 : -1);
        int stepZ = dz == 0 ? 0 : (dz > 0 ? 1 : -1);

        int currentX = piece.BoardX + stepX;
        int currentZ = piece.BoardZ + stepZ;

        while (currentX != targetX || currentZ != targetZ)
        {
            foreach (ChessPiece other in pieces)
            {
                if (other.BoardX == currentX && other.BoardZ == currentZ)
                {
                    return false;
                }
            }

            currentX += stepX;
            currentZ += stepZ;
        }

        return true;
    }
}











