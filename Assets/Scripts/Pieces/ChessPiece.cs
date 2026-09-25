using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public enum PieceType
    {
        King,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn
    }

    public enum PieceColor
    {
        White,
        Black
    }

    public PieceType Type {get; private set; }
    public PieceColor Color {get; private set; }
    public int BoardX {get; set;}
    public int BoardZ {get; set;}

    public void Initialize(PieceType type, PieceColor color, int x, int z)
    {
        Type = type;
        Color = color;

        BoardX = x;
        BoardZ = z;

        gameObject.name = $"{color}_{type}";
    }
}



