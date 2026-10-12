using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public partial class ChessGameManager : NetworkBehaviour 
{
    public static ChessGameManager Instance {get; private set;}

    private ChessPieceInteraction selectedPiece;

    private NetworkVariable<int> currentTurn = new NetworkVariable<int>((int)ChessPiece.PieceColor.White);

    private const ulong UnassignedClientId = ulong.MaxValue;

    private NetworkVariable<ulong> whitePlayerClientId = new NetworkVariable<ulong> (UnassignedClientId);

    private NetworkVariable<ulong> blackPlayerClientId = new NetworkVariable<ulong> (UnassignedClientId);

    private List<ChessPiece> pieces = new List<ChessPiece>();

    private ChessMultiplayerManager multiplayerManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        multiplayerManager = FindAnyObjectByType<ChessMultiplayerManager>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer || NetworkManager.Singleton == null)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback += AssignPlayerColor;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            AssignPlayerColor(clientId);
        }

        Debug.Log("ChessGameManager spawned on server.");
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= AssignPlayerColor;
        }
    }

    private void AssignPlayerColor(ulong clientId)
    {
        if (!IsServer)
            return;

        if (whitePlayerClientId.Value == clientId || blackPlayerClientId.Value == clientId)
        {
            return;
        }

        if (whitePlayerClientId.Value == UnassignedClientId)
        {
            whitePlayerClientId.Value = clientId;
            Debug.Log($"Player {clientId} assigned WHITE.");
        }
        else if (blackPlayerClientId.Value == UnassignedClientId)
        {
            blackPlayerClientId.Value = clientId;
            Debug.Log($"Player {clientId} assigned BLACk.");
        }
        else
        {
            Debug.LogWarning($"No chess color available for client {clientId}");
        }
    }

    private bool TryGetLocalPlayerColor(out ChessPiece.PieceColor color)
    {
        color = default;

        if (NetworkManager.Singleton == null)
            return false;

        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        if (localClientId == whitePlayerClientId.Value)
        {
            color = ChessPiece.PieceColor.White;
            return true;
        }

        if (localClientId == blackPlayerClientId.Value)
        {
            color = ChessPiece.PieceColor.Black;
            return true;
        }

        return false;
    }

    public void AdvanceTurnOnServer()
    {
        if (!IsServer)
            return;

        currentTurn.Value = currentTurn.Value == (int)ChessPiece.PieceColor.White
                ?(int)ChessPiece.PieceColor.Black
                :(int)ChessPiece.PieceColor.White;

        Debug.Log($"Turn changed to {(ChessPiece.PieceColor)currentTurn.Value}");
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
        if (piece == null)
            return;

        ChessPiece chessPiece = piece.GetPiece();

        if (chessPiece == null)
            return;

        if (selectedPiece != null)
        {
            ChessPiece selectedChessPiece = selectedPiece.GetPiece();

            if (selectedChessPiece != null && selectedChessPiece != chessPiece && selectedChessPiece.Color != chessPiece.Color) 
            {
                TryMoveSelected(chessPiece.BoardX, chessPiece.BoardZ);
                return;
            }
        }

        if (!TryGetLocalPlayerColor(out ChessPiece.PieceColor localColor))
        {
            Debug.LogWarning("Your player color has not been assigned.");
            return;
        }

        if (chessPiece.Color != localColor)
        {
            Debug.Log($"You are {localColor}. You cannot select {chessPiece.Color} pieces.");
            return;
        }

        if ((int)chessPiece.Color != currentTurn.Value)
        {
            Debug.Log($"It is {(ChessPiece.PieceColor)currentTurn.Value}'s turn.");
            return;
        }

        if (selectedPiece != null)
            selectedPiece.Deselect();

        selectedPiece = piece;
        selectedPiece.Select();
    }

    public void TryMoveSelected(int targetX, int targetZ)
    {
        if (selectedPiece == null)
            return;

        ChessPiece piece = selectedPiece.GetPiece();

        if (piece == null)
        {
            return;
        }

        NetworkChessPiece networkPiece = piece.GetComponent<NetworkChessPiece>();

        if (networkPiece == null)
        {
            Debug.LogError("NetworkChessPiece component not found.");
            return;
        }

        networkPiece.RequestMoveServerRpc(targetX, targetZ);

        selectedPiece.Deselect();
        selectedPiece = null;

        Debug.Log($"Submitted move request: {piece.Color} {piece.Type} to ({targetX}, {targetZ}).");
    }

    private void MovePiece(ChessPiece piece , int targetX, int targetZ)
    {
        NetworkChessPiece networkPiece = piece.GetComponent<NetworkChessPiece>();

        if (networkPiece == null)
        {
            Debug.LogError("NetworkChessPiece component not found!");
            return;
        }

        networkPiece.RequestMoveServerRpc(targetX, targetZ);

        // piece.transform.position = new Vector3(
        //     targetX - 3.5f,
        //     0f,
        //     targetZ - 3.5f
        // );

        selectedPiece.Deselect();
        selectedPiece = null;

        Debug.Log(
            $"Moved {piece.Color} {piece.Type} to {targetX}, {targetZ}"
        );

        // if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        // {
            
        // }

        // Debug.Log($"Turn changed to {(ChessPiece.PieceColor)currentTurn.Value}");

        if (IsCheckMate((ChessPiece.PieceColor)currentTurn.Value))
        {
            Debug.Log($"CHECKMATE! {(ChessPiece.PieceColor)currentTurn.Value} has been checkmated");
        }
        else if (IsInCheck((ChessPiece.PieceColor)currentTurn.Value))
        {
            Debug.Log($"{currentTurn} is in CHECK!");
        }

    }

    private void CapturePiece(ChessPiece piece)
    {
        if (piece == null)
            return;

        NetworkChessPiece networkPiece = piece.GetComponent<NetworkChessPiece>();

        if (networkPiece == null)
        {
            Debug.LogError("NetworkChessPiece not found on captured piece!");
            return;
        }

        pieces.Remove(piece);
        networkPiece.CaptureOnServer();

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










