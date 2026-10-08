using System.Collections.Generic;
using UnityEngine;

public class ChessGameManager : MonoBehaviour 
{
    public static ChessGameManager Instance {get; private set;}

    private ChessPieceInteraction selectedPiece;

    private ChessPiece.PieceColor currentTurn = ChessPiece.PieceColor.White;

    private List<ChessPiece> pieces = new List<ChessPiece>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterPiece(ChessPiece piece)
    {
        if (!pieces.Contains(piece))
        {
            pieces.Add(piece);
        }
    }

    public void SelectPiece(ChessPieceInteraction piece)
    {
        ChessPiece chessPiece = piece.GetPiece();

        if (chessPiece.Color != currentTurn)
        {
            Debug.Log("It's not your turn!");
            return;
        }

        if (selectedPiece != null)
        {
            selectedPiece.Deselect();
        }

        selectedPiece = piece;
        selectedPiece.Select();
    }

    public void TryMoveSelected(int targetX, int targetZ)
    {
        if (selectedPiece == null)
            return;

        ChessPiece piece = selectedPiece.GetPiece();

        bool validMove = false;

        if (piece.Type == ChessPiece.PieceType.Knight)
        {
            ChessPiece targetPiece = FindPieceAt(targetX, targetZ);

            validMove = KnightRules.IsValidMove(
                piece,
                targetX,
                targetZ,
                targetPiece
            );

        }

        if (piece.Type == ChessPiece.PieceType.Pawn)
        {
            ChessPiece targetPiece = FindPieceAt(targetX, targetZ);
    
            ChessPiece middlePiece = FindPieceAt(piece.BoardX, piece.BoardZ + (piece.Color == ChessPiece.PieceColor.White ? 1 : -1));

            validMove = PawnRules.IsValidMove(
                piece,
                targetX,
                targetZ,
                targetPiece,
                middlePiece
            );

        }

        if (piece.Type == ChessPiece.PieceType.Rook)
        {
            ChessPiece targetPiece = FindPieceAt(targetX, targetZ);
    
            validMove = RookRules.IsValidMove(
                piece,
                targetX,
                targetZ,
                targetPiece,
                FindPieceAt
            );

        }

        if (piece.Type == ChessPiece.PieceType.Bishop)
        {
            ChessPiece targetPiece = FindPieceAt(targetX, targetZ);
    
            validMove = BishopRules.IsValidMove(
                piece,
                targetX,
                targetZ,
                targetPiece,
                FindPieceAt
            );

        }

        if (piece.Type == ChessPiece.PieceType.Queen)
        {
            ChessPiece targetPiece = FindPieceAt(targetX, targetZ);
    
            validMove = QueenRules.IsValidMove(
                piece,
                targetX,
                targetZ,
                targetPiece,
                FindPieceAt
            );

        }

        if (piece.Type == ChessPiece.PieceType.King)
        {
            ChessPiece targetPiece = FindPieceAt(targetX, targetZ);
    
            validMove = KingRules.IsValidMove(
                piece,
                targetX,
                targetZ,
                targetPiece
            );

        }

        if (!validMove)
        {
            Debug.Log($"Invalid {piece.Type} Move!");
            return;
        }

        if (WouldBeInCheck(piece, targetX, targetZ))
        {
            Debug.Log("Illegal move! Your king would be in check.");
            return;
        }

        ChessPiece capturedPiece = FindPieceAt(targetX, targetZ);

        if (capturedPiece != null)
        {
            if (!CaptureRules.CanCapture(piece, capturedPiece))
            {
                Debug.Log("This piece can't be captured!");
                return;
            }
            
            CapturePiece(capturedPiece);
        }

        MovePiece(piece, targetX, targetZ);
    }

    private void MovePiece(ChessPiece piece , int targetX, int targetZ)
    {
        piece.BoardX = targetX;
        piece.BoardZ = targetZ;

        piece.transform.position = new Vector3(
            targetX - 3.5f,
            0.3f,
            targetZ - 3.5f
        );

        selectedPiece.Deselect();
        selectedPiece = null;

        Debug.Log(
            $"Moved {piece.Color} {piece.Type} to {targetX}, {targetZ}"
        );

        currentTurn = currentTurn == ChessPiece.PieceColor.White
            ? ChessPiece.PieceColor.Black
            : ChessPiece.PieceColor.White;

        Debug.Log($"Turn changed to {currentTurn}");

        if (IsCheckMate(currentTurn))
        {
            Debug.Log($"CHECKMATE! {currentTurn} has been checkmated");
        }
        else if (IsInCheck(currentTurn))
        {
            Debug.Log($"{currentTurn} is in CHECK!");
        }

    }

    private void CapturePiece(ChessPiece piece)
    {
        if (piece == null)
            return;

        pieces.Remove(piece);
        Destroy(piece.gameObject);

        Debug.Log($"{piece.Color} {piece.Type} was captured!");
    }

    private ChessPiece FindPieceAt(int x, int z)
    {
        foreach (ChessPiece piece in pieces)
        {
            if (piece.BoardX == x && piece.BoardZ == z)
            {
                return piece;
            }
        }
        return null;
    }

    private ChessPiece FindKing(
        ChessPiece.PieceColor color
    )
    {
        foreach (ChessPiece piece in pieces)
        {
            if (piece.Type == ChessPiece.PieceType.King && piece.Color == color)
            {
                return piece;
            }
        }

        return null;
    }

    private bool IsInCheck(ChessPiece.PieceColor color)
    {
        ChessPiece king = FindKing(color);

        if (king == null)
            return false;

        ChessPiece.PieceColor enemyColor = color == ChessPiece.PieceColor.White
            ? ChessPiece.PieceColor.Black
            : ChessPiece.PieceColor.White;

        return ChessRules.IsSquareUnderAttack(
            king.BoardX,
            king.BoardZ,
            enemyColor,
            pieces
        );
    }

    private bool IsCheckMate(ChessPiece.PieceColor color)
    {
        if (!IsInCheck(color))
        {
            return false;
        }

        return !CheckmateRules.HasAnyLegalMove(color, pieces, FindPieceAt, WouldBeInCheck);
    }

    private bool WouldBeInCheck(ChessPiece piece, int targetX, int targetZ)
    {
        int originalX = piece.BoardX;
        int originalZ = piece.BoardZ;

        ChessPiece capturedPiece = FindPieceAt(targetX, targetZ);

        piece.BoardX = targetX;
        piece.BoardZ = targetZ;

        if (capturedPiece != null)
        {
            pieces.Remove(capturedPiece);
        }

        bool inCheck = IsInCheck(piece.Color);

        if (capturedPiece != null)
        {
            pieces.Add(capturedPiece);
        }

        piece.BoardX = originalX;
        piece.BoardZ = originalZ;

        return inCheck;
    }
}










