using UnityEngine;

public class ChessPieceInteraction : MonoBehaviour
{
    private ChessPiece piece;
    private Renderer pieceRenderer;

    private Color originalColor;

    private void Awake()
    {
        piece = GetComponent<ChessPiece>();
        pieceRenderer = GetComponent<Renderer>();

        originalColor = pieceRenderer.material.color;
    }

    private void OnMouseDown()
    {
        if (piece == null)
            return;

        ChessGameManager.Instance.SelectPiece(this);
    }

    public void Select()
    {
        pieceRenderer.material.color = Color.yellow;

        Debug.Log(
            $"Selected {piece.Color} {piece.Type}"
        );
    }

    public void Deselect()
    {
        pieceRenderer.material.color = originalColor;
    }

    public ChessPiece GetPiece()
    {
        return piece;
    }
}





