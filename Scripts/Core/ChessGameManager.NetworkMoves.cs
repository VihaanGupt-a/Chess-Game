using UnityEngine;

public partial class ChessGameManager
{
    public bool ProcessMoveOnServer(
        NetworkChessPiece networkPiece,
        int targetX,
        int targetZ,
        ulong senderClientId
    )
    {
        if (!IsServer || networkPiece == null || !networkPiece.IsSpawned)
            return false;

        if (targetX < 0 || targetX > 7 || targetZ < 0 || targetZ > 7)
            return false;

        ChessPiece piece = networkPiece.GetComponent<ChessPiece>();

        if (piece == null || !pieces.Contains(piece))
            return false;

        ChessPiece.PieceColor playerColor;

        if (senderClientId == whitePlayerClientId.Value)
        {
            playerColor = ChessPiece.PieceColor.White;
        }
        else if (senderClientId == blackPlayerClientId.Value)
        {
            playerColor = ChessPiece.PieceColor.Black;
        }
        else
        {
            Debug.LogWarning($"Rejected move from unassigned client {senderClientId}.");
            return false;
        }

        if (piece.Color != playerColor)
        {
            Debug.LogWarning($"{playerColor} cannot move a {piece.Color} piece.");
            return false;
        }

        if (currentTurn.Value != (int)playerColor)
        {
            Debug.LogWarning($"Rejected Move: It is not {playerColor}'s turn.");
            return false;
        }

        if (!IsValidPieceMove(piece, targetX, targetZ))
        {
            Debug.Log($"Server rejected invalid {piece.Type} move.");
            return false;
        }

        if (WouldBeInCheck(piece, targetX, targetZ))
        {
            Debug.Log("Server Rejected move because it leaves king in check.");
            return false;
        }

        ChessPiece capturedPiece = FindPieceAt(targetX, targetZ);

        if (capturedPiece != null)
        {
            if (!CaptureRules.CanCapture(piece, capturedPiece))
                return false;

            CapturePiece(capturedPiece);
        }

        piece.BoardX = targetX;
        piece.BoardZ = targetZ;

        networkPiece.BoardX.Value = targetX;
        networkPiece.BoardZ.Value = targetZ;

        currentTurn.Value = currentTurn.Value == (int)ChessPiece.PieceColor.White
            ?(int)ChessPiece.PieceColor.Black
            :(int)ChessPiece.PieceColor.White;

        Debug.Log($"Accepted {playerColor} {piece.Type} move to ({targetX}, {targetZ}).");
        Debug.Log($"Turn Change to {(ChessPiece.PieceColor)currentTurn.Value}");

        ChessPiece.PieceColor nextTurn = (ChessPiece.PieceColor)currentTurn.Value;

        if (IsCheckMate(nextTurn))
            Debug.Log($"CHECKMATE! {nextTurn} has been checkmated.");

        else if (IsInCheck(nextTurn))
            Debug.Log($"{nextTurn} is in CHECk.");

        return true;

    }

    private bool IsValidPieceMove(ChessPiece piece , int targetX, int targetZ)
    {
        ChessPiece targetPiece = FindPieceAt(targetX, targetZ);

        switch (piece.Type)
        {
            case ChessPiece.PieceType.Knight:
                return KnightRules.IsValidMove(piece, targetX, targetZ, targetPiece);

            case ChessPiece.PieceType.Pawn:
            {
                ChessPiece middlePiece = FindPieceAt(piece.BoardX, piece.BoardZ + (piece.Color == ChessPiece.PieceColor.White ? 1 : -1));
                return PawnRules.IsValidMove(piece, targetX, targetZ, targetPiece, middlePiece);
            }

            case ChessPiece.PieceType.Rook:
                return RookRules.IsValidMove(piece, targetX, targetZ, targetPiece, FindPieceAt);

            case ChessPiece.PieceType.Bishop:
                return BishopRules.IsValidMove(piece, targetX, targetZ, targetPiece,FindPieceAt);

            case ChessPiece.PieceType.Queen:
                return QueenRules.IsValidMove(piece, targetX, targetZ, targetPiece,FindPieceAt);

            case ChessPiece.PieceType.King:
                return KingRules.IsValidMove(piece, targetX, targetZ, targetPiece);

            default:
                return false;
        }
    }

    public void UnregisterPiece(ChessPiece piece)
    {
        if (piece != null)
            pieces.Remove(piece);
    }
}





