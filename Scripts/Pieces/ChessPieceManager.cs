using UnityEngine;

public class ChessPieceManager : MonoBehaviour
{
    [SerializeField] private ChessBoard board;

    [SerializeField] private GameObject pawnPrefab;
    [SerializeField] private GameObject rookPrefab;
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject bishopPrefab;
    [SerializeField] private GameObject queenPrefab;
    [SerializeField] private GameObject kingPrefab;

    private void Start()
    {
        SetupPieces();
    }

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

        GameObject piece = Instantiate(
            prefab,
            new Vector3(x-3.5f, 0f, z-3.5f),
            Quaternion.identity
        );

        piece.transform.localScale = Vector3.one * 0.3f;

        piece.name = $"{color}_{type}";

        if (piece.GetComponent<Collider>() == null)
        {
            piece.AddComponent<CapsuleCollider>();
        }

        Renderer[] renderers = piece.GetComponentsInChildren<Renderer>();

        Color pieceColor = color == ChessPiece.PieceColor.White
            ? new Color(0.92f, 0.92f, 0.92f)
            : new Color(0.12f, 0.12f, 0.12f);

        foreach (Renderer pieceRenderer in renderers)
        {
            Material[] materials = pieceRenderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = pieceColor;
            }

            pieceRenderer.materials = materials;
        }
        

        ChessPiece chessPiece = piece.GetComponent<ChessPiece>();

        if (chessPiece == null)
            chessPiece = piece.AddComponent<ChessPiece>();

        if (piece.GetComponent<ChessPieceInteraction>() == null)
            piece.AddComponent<ChessPieceInteraction>();

        chessPiece.Initialize(type, color, x, z);

        ChessGameManager.Instance.RegisterPiece(chessPiece);

    }
}










