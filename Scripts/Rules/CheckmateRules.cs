using System;
using System.Collections.Generic;

public static class CheckmateRules
{
    public static bool HasAnyLegalMove(
        ChessPiece.PieceColor color,
        List<ChessPiece> pieces,
        System.Func<int, int, ChessPiece> findPieceAt,
        System.Func<ChessPiece, int, int, bool> wouldBeInCheck
    )
    {
        foreach (ChessPiece piece in pieces)
        {
            if (piece.Color != color)
                continue;

            for (int targetX = 0; targetX < 8; targetX++)
            {
                for (int targetZ = 0; targetZ < 8; targetZ++)
                {
                    ChessPiece targetPiece = findPieceAt(targetX, targetZ);

                    if (IsBasicMoveValid(piece, targetX, targetZ, targetPiece, findPieceAt))
                    {
                        if (!wouldBeInCheck(piece, targetX, targetZ))
                        {
                            return true;
                        }
                    }
                }
            } 
        }

        return false;
    }

    private static bool IsBasicMoveValid(
        ChessPiece piece,
        int targetX,
        int targetZ,
        ChessPiece targetPiece,
        System.Func<int, int, ChessPiece> findPieceAt
    )
    {
        if (targetPiece != null && targetPiece.Color == piece.Color)
        {
            return false;
        }

        int dx = targetX - piece.BoardX;
        int dz = targetZ - piece.BoardZ;

        switch (piece.Type) 
        {
            case ChessPiece.PieceType.King:
                return Math.Abs(dx) <= 1 && Math.Abs(dz) <= 1;

            case ChessPiece.PieceType.Knight:
                return (Math.Abs(dx) == 1 && Math.Abs(dz) == 2) || (Math.Abs(dx) == 2 && Math.Abs(dz) == 1);

            case ChessPiece.PieceType.Rook:
                return IsStraightPathClear(piece, targetX, targetZ, findPieceAt);

            case ChessPiece.PieceType.Bishop:
                return IsDiagonalPathClear(piece, targetX, targetZ, findPieceAt);

            case ChessPiece.PieceType.Queen:
                return IsStraightPathClear(piece, targetX, targetZ, findPieceAt) || IsDiagonalPathClear(piece, targetX, targetZ, findPieceAt);

            case ChessPiece.PieceType.Pawn:
                return IsPawnMoveValid(piece, targetX, targetZ, targetPiece, findPieceAt);

        }

        return false;
    }

    private static bool IsStraightPathClear(ChessPiece piece, int targetX, int targetZ, System.Func<int, int, ChessPiece> findPieceAt)
    {
        int dx = targetX - piece.BoardX;
        int dz = targetZ - piece.BoardZ;

        if (dx != 0 && dz != 0)
            return false;

        if (dx == 0 && dz == 0)
            return false;

        int stepX = dx == 0 ? 0 : Math.Sign(dx);
        int stepZ = dz == 0 ? 0 : Math.Sign(dz);

        int x = piece.BoardX + stepX;
        int z = piece.BoardZ + stepZ;

        while (x != targetX || z != targetZ)
        {
            if (findPieceAt(x, z) != null)
                return false;

            x += stepX;
            z += stepZ;
        }

        return true;
    }


    private static bool IsDiagonalPathClear(ChessPiece piece, int targetX, int targetZ, System.Func<int, int, ChessPiece> findPieceAt)
    {
        int dx = targetX - piece.BoardX;
        int dz = targetZ - piece.BoardZ;

        if (Math.Abs(dx) != Math.Abs(dz) || dx == 0)
            return false;

        int stepX = Math.Sign(dx);
        int stepZ = Math.Sign(dz);

        int x = piece.BoardX + stepX;
        int z = piece.BoardZ + stepZ;

        while (x != targetX || z != targetZ)
        {
            if (findPieceAt(x, z) != null)
                return false;

            x += stepX;
            z += stepZ;
        }

        return true;
    }

    private static bool IsPawnMoveValid(ChessPiece pawn, int targetX, int targetZ, ChessPiece targetPiece, System.Func<int, int, ChessPiece> findPieceAt)
    {
        int dx = targetX - pawn.BoardX;
        int dz = targetZ - pawn.BoardZ;

        int direction = pawn.Color == ChessPiece.PieceColor.White
            ? 1
            : -1;

        if (dx == 0 && dz == direction && targetPiece == null)
            return true;

        if (dx == 0 && dz == direction * 2)
        {
            bool startingPosition = pawn.Color == ChessPiece.PieceColor.White
                ? pawn.BoardZ == 1
                : pawn.BoardZ == 6;

            ChessPiece middlePiece = findPieceAt(
                pawn.BoardX,
                pawn.BoardZ + direction
            );

            return startingPosition && middlePiece == null && targetPiece == null;
        }

        if (Math.Abs(dx) == 1 && dz == direction && targetPiece != null && targetPiece.Color != pawn.Color)
        {
            return true;
        }

        return false; 

    }
}



