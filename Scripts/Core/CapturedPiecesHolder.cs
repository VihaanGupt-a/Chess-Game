using UnityEngine;

public class CapturedPiecesHolder : MonoBehaviour
{
    public static CapturedPiecesHolder Instance {get; private set;}

    [Header("Tray Appearance")]
    [SerializeField] private Vector3 traySize = new Vector3(1.9f, 0.1f, 8.2f);

    [SerializeField] private Color whiteTrayColor = new Color(0.48f, 0.32f, 0.19f);
    [SerializeField] private Color blackTrayColor = new Color(0.20f, 0.18f, 0.17f);

    private int whiteCapturedCount;
    private int blackCapturedCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CreateTray(
            "Captured white pieces tray",
            new Vector3(-5f, 0f, 0f),
            whiteTrayColor
        );

        CreateTray(
            "Captured black pieces tray",
            new Vector3(5f, 0f, 0f),
            blackTrayColor
        );
    }

    private void CreateTray(string trayName, Vector3 position, Color trayColor)
    {
        GameObject tray = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tray.name = trayName;
        tray.transform.SetParent(transform, true);
        tray.transform.position = position;
        tray.transform.localScale = traySize;

        Collider trayCollider = tray.GetComponent<Collider>();

        if (trayCollider != null)
            trayCollider.enabled = false;

        Renderer trayRenderer = tray.GetComponent<Renderer>();

        Shader shader = Shader.Find("Universal Render Pipeline/UnLit.");
        if (shader == null)
            shader = Shader.Find("Sprites/Default");

        if (trayRenderer != null && shader != null)
        {
            Material trayMaterial = new Material(shader);
            if (trayMaterial.HasProperty("_BaseColor"))
                trayMaterial.SetColor("_BaseColor", trayColor);

            else if (trayMaterial.HasProperty("_Color"))
                trayMaterial.SetColor("_Color", trayColor);

            trayRenderer.material = trayMaterial;
        }
        else
        {
            Debug.LogError("Cannot find a compatible tray shader.");
        }

    }

    public Vector2Int GetCapturedSlot(ChessPiece.PieceColor pieceColor)
    {
        int index;
        int column;
        int row = 0;
        int x = 0;

        if (pieceColor == ChessPiece.PieceColor.White)
        {
            index = whiteCapturedCount++;
            column = index / 8;
            row = index % 8;

            x = -1 - column;
        }
        else
        {
            index = blackCapturedCount++;
            column = index / 8;
            row = 7 - (index % 8);

            x = 8 + column;
        }

        return new Vector2Int(x, row);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}