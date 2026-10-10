
using UnityEngine;

public class ChessPieceInteraction : MonoBehaviour
{
    private ChessPiece piece;
    private Renderer pieceRenderer;
    private Color originalColor;

    private void Awake()
    {
        piece = GetComponentInParent<ChessPiece>();

        if (piece == null)
        {
            Debug.LogError(
                $"ChessPiece missing on {gameObject.name} and its parents",
                gameObject
            );
        }

        pieceRenderer = GetComponentInChildren<Renderer>(true);

        if (pieceRenderer != null)
        {
            originalColor = pieceRenderer.material.color;
        }
        else
        {
            Debug.LogError(
                $"Renderer missing on {gameObject.name}",
                gameObject
            );
        }
    }

    private void OnMouseDown()
    {
        Debug.Log($"Clicked object: {gameObject.name}");

        if (piece == null)
        {
            Debug.LogError($"No ChessPiece component found for {gameObject.name}");
            return;
        }

        if (ChessGameManager.Instance == null)
        {
            Debug.LogError("ChessGameManager.Instance is null!");
            return;
        }

        ChessGameManager.Instance.SelectPiece(this);
    }

    
    
    public void Select()
    {
        SetPieceColor(Color.yellow);
        Debug.Log($"Selected {piece.Type}");
    }

    public void Deselect()
    {
        SetPieceColor(
            piece.Color == ChessPiece.PieceColor.White
                ? new Color(0.92f, 0.92f, 0.92f)
                : new Color(0.12f, 0.12f, 0.16f)
        );
    }

    private void SetPieceColor(Color color)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in renderers)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            r.GetPropertyBlock(block);

            block.SetColor("_BaseColor", color);
            block.SetColor("_Color", color);

            r.SetPropertyBlock(block);
        }
    }


    public ChessPiece GetPiece()
    {
        return piece;
    }
}



