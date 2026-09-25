using UnityEngine;

public class ChessPieceManager : MonoBehaviour
{
    [SerializeField] private ChessBoard board;

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
        GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        piece.transform.position = new Vector3(
            x-3.5f,
            0.3f,
            z-3.5f
        );

        piece.transform.localScale = new Vector3(
            0.6f,                                                                                    
            0.6f,
            0.6f
        );

        ChessPiece chessPiece = piece.AddComponent<ChessPiece>();

        piece.AddComponent<ChessPieceInteraction>();

        chessPiece.Initialize(type, color, x, z);

        ChessGameManager.Instance.RegisterPiece(chessPiece);

        Renderer renderer = piece.GetComponent<Renderer>();

        Material material = new Material(
            Shader.Find("Universal Render Pipeline/Lit")
        );

        material.color = color == ChessPiece.PieceColor.White
            ?Color.white
            :Color.black;

        renderer.material = material;
    }
}










