using UnityEngine;
using Unity.Netcode;

public class ChessPieceManager : NetworkBehaviour
{
    [SerializeField] private ChessBoard board;

    [SerializeField] private GameObject pawnPrefab;
    [SerializeField] private GameObject rookPrefab;
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject bishopPrefab;
    [SerializeField] private GameObject queenPrefab;
    [SerializeField] private GameObject kingPrefab;

    private bool piecesSpawned;

    public override void OnNetworkSpawn()
    {
        if (!IsServer || piecesSpawned)
            return;

        piecesSpawned = true;
        SetupPieces();
    }

    // private void Start()
    // {
    //     SetupPieces();
    // }

    private void SetupPieces()
    {
        CreatePiece(ChessPiece.PieceType.Rook, ChessPiece.PieceColor.White, 0, 0);
        CreatePiece(ChessPiece.PieceType.Knight, ChessPiece.PieceColor.White, 1, 0);
        CreatePiece(ChessPiece.PieceType.Bishop, ChessPiece.PieceColor.White, 2, 0);
        CreatePiece(ChessPiece.PieceType.Queen, ChessPiece.PieceColor.White, 3, 0);
        CreatePiece(ChessPiece.PieceType.King, ChessPiece.PieceColor.White, 4, 0);
        CreatePiece(ChessPiece.PieceType.Bishop, ChessPiece.PieceColor.White, 5, 0);
        CreatePiece(ChessPiece.PieceType.Knight, ChessPiece.PieceColor.White, 6, 0);
        CreatePiece(ChessPiece.PieceType.Rook, ChessPiece.PieceColor.White, 7, 0);


        for (int x = 0; x < 8; x++){
            CreatePiece(
                ChessPiece.PieceType.Pawn,
                ChessPiece.PieceColor.White,
                x,
                1
            );
        }

        CreatePiece(ChessPiece.PieceType.Rook, ChessPiece.PieceColor.Black, 0, 7);
        CreatePiece(ChessPiece.PieceType.Knight, ChessPiece.PieceColor.Black, 1, 7);
        CreatePiece(ChessPiece.PieceType.Bishop, ChessPiece.PieceColor.Black, 2, 7);
        CreatePiece(ChessPiece.PieceType.Queen, ChessPiece.PieceColor.Black, 3, 7);
        CreatePiece(ChessPiece.PieceType.King, ChessPiece.PieceColor.Black, 4, 7);
        CreatePiece(ChessPiece.PieceType.Bishop, ChessPiece.PieceColor.Black, 5, 7);
        CreatePiece(ChessPiece.PieceType.Knight, ChessPiece.PieceColor.Black, 6, 7);
        CreatePiece(ChessPiece.PieceType.Rook, ChessPiece.PieceColor.Black, 7, 7);


        for (int x = 0; x < 8; x++){
            CreatePiece(
                ChessPiece.PieceType.Pawn,
                ChessPiece.PieceColor.Black,
                x,
                6
            );
        }
    }

    private void CreatePiece(
        ChessPiece.PieceType type,
        ChessPiece.PieceColor color,
        int x,
        int z
    )
    {
        GameObject prefab = type switch
        {
            ChessPiece.PieceType.Pawn => pawnPrefab,
            ChessPiece.PieceType.Rook => rookPrefab,
            ChessPiece.PieceType.Knight => knightPrefab,
            ChessPiece.PieceType.Bishop => bishopPrefab,
            ChessPiece.PieceType.Queen => queenPrefab,
            ChessPiece.PieceType.King => kingPrefab,
            _ => null
        };

        if (prefab == null)
        {
            Debug.LogError($"Missing Prefab for {type}!");
            return;
        }

        GameObject pieceObject = Instantiate(
            prefab,
            new Vector3(x-3.5f, 0f, z-3.5f),
            Quaternion.identity
        );

        NetworkObject networkObject = pieceObject.GetComponent<NetworkObject>();
        NetworkChessPiece networkPiece = pieceObject.GetComponent<NetworkChessPiece>();

        if (networkObject == null || networkPiece == null)
        {
            Debug.LogError($"Network component missing on {prefab.name}!");
            Destroy(pieceObject);
            return;
        }

        networkPiece.BoardX.Value = x;
        networkPiece.BoardZ.Value = z;
        networkPiece.PieceType.Value = (int)type;
        networkPiece.PieceColor.Value = (int)color;

        pieceObject.name = $"{color}_{type}";

        networkObject.Spawn();
    }
}










