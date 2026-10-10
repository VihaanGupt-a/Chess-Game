using Unity.Netcode;
using UnityEngine;

public class NetworkChessPiece : NetworkBehaviour
{
    public NetworkVariable<int> BoardX = new NetworkVariable<int>();
    public NetworkVariable<int> BoardZ = new NetworkVariable<int>();

    public NetworkVariable<int> PieceType = new NetworkVariable<int>();
    public NetworkVariable<int> PieceColor = new NetworkVariable<int>();

    [Header("Piece Appearance")]
    [SerializeField, Range(0.1f, 1f)]
    private float pieceScale = 0.2f;

    [SerializeField] private Color whitePieceColor = new Color(0.92f, 0.92f, 0.92f);
    [SerializeField] private Color blackPieceColor = new Color(0.12f, 0.12f, 0.16f);

    private ChessPiece chessPiece;

    private void Awake()
    {
        chessPiece = GetComponent<ChessPiece>();

        if (chessPiece == null)
        {
            chessPiece = GetComponentInChildren<ChessPiece>(true);
        }
    }

    public override void OnNetworkSpawn()
    {
        BoardX.OnValueChanged += OnPositionChanged;
        BoardZ.OnValueChanged += OnPositionChanged;

        PieceType.OnValueChanged += OnPieceDataChanged;
        PieceColor.OnValueChanged += OnPieceDataChanged;

        ApplyPieceData();
        ApplyPosition();
    }

    public override void OnNetworkDespawn()
    {
        BoardX.OnValueChanged -= OnPositionChanged;
        BoardZ.OnValueChanged -= OnPositionChanged;

        PieceType.OnValueChanged -= OnPieceDataChanged;
        PieceColor.OnValueChanged -= OnPieceDataChanged;
    }

    private void OnPositionChanged(int previous, int current)
    {
        ApplyPosition();
    }

    private void OnPieceDataChanged(int previous, int current)
    {
        ApplyPieceData();
    }

    private void ApplyPieceData()
    {
        if (chessPiece == null)
        {
            Debug.LogError($"No ChessPiece component found on {gameObject.name} or its children!");
            return;
        }

        chessPiece.Initialize(
            (ChessPiece.PieceType)PieceType.Value,
            (ChessPiece.PieceColor)PieceColor.Value,
            BoardX.Value,
            BoardZ.Value
        );

        ApplyAppearance();

        if (ChessGameManager.Instance != null)
        {
            ChessGameManager.Instance.RegisterPiece(chessPiece);
        }
    }

    private void ApplyAppearance()
    {
        Debug.Log(
            $"Appearance applied: {gameObject.name}, " +
            $"Color={(ChessPiece.PieceColor)PieceColor.Value}, " +
            $"Renderers={GetComponentsInChildren<Renderer>(true).Length}"
        );

        transform.localScale = Vector3.one * pieceScale;

        Color targetColor =
            (ChessPiece.PieceColor)PieceColor.Value == ChessPiece.PieceColor.White
                ? Color.white
                : Color.black;

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();

            renderer.GetPropertyBlock(block);

            block.SetColor("_BaseColor", targetColor);
            block.SetColor("_Color", targetColor);

            renderer.SetPropertyBlock(block);
        }
    }

    private void ApplyPosition()
    {
        int x = BoardX.Value;
        int z = BoardZ.Value;

        transform.position = new Vector3(BoardX.Value - 3.5f, 0.3f, BoardZ.Value - 3.5f);

        if (chessPiece != null)
        {
            chessPiece.BoardX = x;
            chessPiece.BoardZ = z;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestMoveServerRpc(int x, int z)
    {
        if (!IsServer)
            return;

        if (x < 0 || x > 7 || z < 0 || z > 7)
            return;

        BoardX.Value = x;
        BoardZ.Value = z;
    }

    public void CaptureOnServer()
    {
        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn(true);
        }
    }
}




