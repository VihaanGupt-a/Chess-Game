using Unity.Netcode;
using UnityEngine;

public class NetworkChessPiece : NetworkBehaviour
{
    public NetworkVariable<int> BoardX = new NetworkVariable<int>();
    public NetworkVariable<int> BoardZ = new NetworkVariable<int>();

    public NetworkVariable<int> PieceType = new NetworkVariable<int>();
    public NetworkVariable<int> PieceColor = new NetworkVariable<int>();

    public NetworkVariable<bool> IsCaptured = new NetworkVariable<bool>(false);

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

        IsCaptured.OnValueChanged += OnCapturedStateChanged;

        ApplyPieceData();
        ApplyPosition();
    }

    public override void OnNetworkDespawn()
    {
        BoardX.OnValueChanged -= OnPositionChanged;
        BoardZ.OnValueChanged -= OnPositionChanged;

        PieceType.OnValueChanged -= OnPieceDataChanged;
        PieceColor.OnValueChanged -= OnPieceDataChanged;

        IsCaptured.OnValueChanged -= OnCapturedStateChanged;

        if (chessPiece != null && ChessGameManager.Instance != null)
        {
            ChessGameManager.Instance.UnregisterPiece(chessPiece);
        }
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

    private void OnCapturedStateChanged(bool previous, bool current)
    {
        ApplyCapturedState(current);
    }

    private void ApplyCapturedState(bool captured)
    {
        foreach(Collider pieceCollider in GetComponentsInChildren<Collider>(true))
        {
            pieceCollider.enabled = !captured;
        }

        foreach (ChessPieceInteraction interaction in GetComponentsInChildren<ChessPieceInteraction>(true))
        {
            interaction.enabled = !captured;
        }

        if (captured && chessPiece != null && ChessGameManager.Instance != null)
        {
            ChessGameManager.Instance.UnregisterPiece(chessPiece);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestMoveServerRpc(int x, int z, ServerRpcParams rpcParams = default)
    {
        if (!IsServer)
            return;

        if (ChessGameManager.Instance == null)
        {
            Debug.LogError("ChessGameManager instance not found on Server.");
            return;
        }
        
        ulong senderClientId = rpcParams.Receive.SenderClientId;

        bool accepted = ChessGameManager.Instance.ProcessMoveOnServer(this, x, z, senderClientId);

        if (!accepted)
        {
            Debug.LogWarning($"Server rejected move request from client {senderClientId}.");
        }
    }

    public void CaptureOnServer(ChessPiece.PieceColor capturingPlayerColor)
    {
        if (!IsServer)
            return;

        if (chessPiece == null)
        {
            Debug.LogError("Cannot capture piece: ChessPiece is missing.");
            return;
        }

        if (CapturedPiecesHolder.Instance == null)
        {
            Debug.LogError("CapturedPieceHolder is missing.");
            return;
        }

        Vector2Int slot = CapturedPiecesHolder.Instance.GetCapturedSlot(capturingPlayerColor);

        BoardX.Value = slot.x;
        BoardZ.Value = slot.y;

        IsCaptured.Value = true;

        Debug.Log(
            $"Captured {chessPiece.Color} {chessPiece.Type};" + $"tray slot = ({slot.x}, {slot.y})."
        );
    }

    public void CaptureOnServer()
    {
        if (chessPiece == null)
            return;

        ChessPiece.PieceColor capturingPlayerColor = chessPiece.Color == ChessPiece.PieceColor.White
            ?ChessPiece.PieceColor.Black
            :ChessPiece.PieceColor.White;

        CaptureOnServer(capturingPlayerColor);
    }
}




