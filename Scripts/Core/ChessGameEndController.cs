using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChessGameEndController : NetworkBehaviour
{
    public static ChessGameEndController Instance { get; private set; }
    private const ulong NoOffer = ulong.MaxValue;

    private const int Playing = 0;
    private const int DrawOffered = 1;
    private const int DrawAgreed = 2;
    private const int WhiteWon = 3;
    private const int BlackWon = 4;

    private const int NoReason = 0;
    private const int Checkmate = 1;
    private const int Resignation = 2;

    [Header("Panels")]
    [SerializeField] private GameObject gameControlsPanel;
    [SerializeField] private GameObject drawOfferPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Buttons")]
    [SerializeField] private Button offerDrawButton;
    [SerializeField] private Button resignButton;
    [SerializeField] private Button acceptDrawButton;
    [SerializeField] private Button declineDrawButton;
    [SerializeField] private Button playAgainButton;

    [Header("Text")]
    [SerializeField] private TMP_Text drawOfferText;
    [SerializeField] private TMP_Text gameResultText;

    private readonly NetworkVariable<int> gameState = new NetworkVariable<int>(Playing);

    private readonly NetworkVariable<ulong> drawOfferFrom = new NetworkVariable<ulong>(NoOffer);

    private readonly NetworkVariable<int> resultReason = new NetworkVariable<int>(NoReason);


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this ;
    }

    public override void OnNetworkSpawn()
    {
        // if (IsServer)
        // {
        //     gameState.Value = Playing;
        //     drawOfferFrom.Value = NoOffer;
        //     resultReason.Value = NoReason;
        // }

        gameState.OnValueChanged += OnGameStateChanged;

        UpdateUI();
    }

    public override void OnNetworkDespawn()
    {
        gameState.OnValueChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(int previousValue, int newValue)
    {
        Debug.Log($"[DRAW DEBUG] Game state changed: {previousValue} -> {newValue}. " +
              $"OfferFrom={drawOfferFrom.Value}, IsServer={IsServer}, " +
              $"IsClient={IsClient}, LocalClient={NetworkManager.Singleton?.LocalClientId}");

        UpdateUI();
    }

    private void Start()
    {
        if (offerDrawButton != null)
            offerDrawButton.onClick.AddListener(OfferDraw);

        if (resignButton != null)
            resignButton.onClick.AddListener(Resign);

        if (acceptDrawButton != null)
            acceptDrawButton.onClick.AddListener(AcceptDraw);

        if (declineDrawButton != null)
            declineDrawButton.onClick.AddListener(DeclineDraw);

        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);

        UpdateUI();
    }

    private void Update()
    {
        if (IsSpawned)
            UpdateUI();
    }

    private void OnDestroy()
    {
        if (offerDrawButton != null)
            offerDrawButton.onClick.RemoveListener(OfferDraw);

        if (resignButton != null)
            resignButton.onClick.RemoveListener(Resign);

        if (acceptDrawButton != null)
            acceptDrawButton.onClick.RemoveListener(AcceptDraw);

        if (declineDrawButton != null)
            declineDrawButton.onClick.RemoveListener(DeclineDraw);

        if (playAgainButton != null)
            playAgainButton.onClick.RemoveListener(PlayAgain);

        if (Instance == this)
            Instance = null;
    }

    private void OfferDraw()
    {
        if (IsSpawned)
            OfferDrawServerRpc();
    }

    private void AcceptDraw()
    {
        if (IsSpawned)
            RespondToDrawServerRpc(true);
    }

    private void DeclineDraw()
    {
        if (IsSpawned)
            RespondToDrawServerRpc(false);
    }

    private void Resign()
    {
        if (IsSpawned)
            ResignServerRpc();
    }

    private void PlayAgain()
    {
        if (IsSpawned)
            RestartGameServerRpc();
    }


    
    [ServerRpc(RequireOwnership = false)]
    private void OfferDrawServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong sender = rpcParams.Receive.SenderClientId;

        bool assigned = IsAssignedPlayer(sender);
        int connectedCount = NetworkManager.Singleton != null
            ? NetworkManager.Singleton.ConnectedClientsIds.Count
            : 0;

        Debug.Log(
            $"[DRAW TRACE] RPC received | Sender={sender} | " +
            $"Assigned={assigned} | Connected={connectedCount} | " +
            $"State={gameState.Value} | IsServer={IsServer}"
        );

        if (!IsServer)
        {
            Debug.LogWarning("[DRAW TRACE] Rejected: not server.");
            return;
        }

        if (gameState.Value != Playing)
        {
            Debug.LogWarning(
                $"[DRAW TRACE] Rejected: state is {gameState.Value}, not Playing."
            );
            return;
        }

        if (!assigned)
        {
            Debug.LogWarning(
                $"[DRAW TRACE] Rejected: player {sender} has no assigned color."
            );
            return;
        }

        if (connectedCount < 2)
        {
            Debug.LogWarning(
                $"[DRAW TRACE] Rejected: only {connectedCount} client(s) connected."
            );
            return;
        }

        drawOfferFrom.Value = sender;
        gameState.Value = DrawOffered;

        Debug.Log(
            $"[DRAW TRACE] SUCCESS | State={gameState.Value} | " +
            $"OfferFrom={drawOfferFrom.Value}"
        );
    }


    [ServerRpc(RequireOwnership = false)]
    private void RespondToDrawServerRpc(bool accept, ServerRpcParams rpcParams = default)
    {
        if (!IsServer || gameState.Value != DrawOffered)
            return;

        ulong sender = rpcParams.Receive.SenderClientId;

        if (!IsAssignedPlayer(sender) || sender == drawOfferFrom.Value)
            return;

        if (accept)
        {
            resultReason.Value = NoReason;
            gameState.Value = DrawAgreed;
            Debug.Log("Draw Accepted!");
        }
        else
        {
            drawOfferFrom.Value = NoOffer;
            gameState.Value = Playing;
            Debug.Log("Draw Declined!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResignServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!IsServer || IsGameOver())
            return;

        ulong sender = rpcParams.Receive.SenderClientId;

        if (!TryGetPlayerColor(sender, out ChessPiece.PieceColor playerColor))
            return;

        DeclareWinner(
            playerColor == ChessPiece.PieceColor.White
                ?ChessPiece.PieceColor.Black
                :ChessPiece.PieceColor.White,
            Resignation
        );

        Debug.Log($"{playerColor} resigned!");
    }

    [ServerRpc(RequireOwnership = false)]
    private void RestartGameServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!IsServer || !IsGameOver())
            return;

        if (!IsAssignedPlayer(rpcParams.Receive.SenderClientId))
            return;

        if (NetworkManager.Singleton == null || NetworkManager.Singleton.SceneManager == null)
        {
            Debug.Log("Network scene management is unavailable!");
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;

        var status = NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogError($"Could not restart the match. Status: {status}.");
        }
    }



    public void DeclareCheckmateServer(ChessPiece.PieceColor winner)    
    {
        if (!IsServer || IsGameOver())
            return;

        DeclareWinner(winner, Checkmate);
    }

    private void DeclareWinner(ChessPiece.PieceColor winner, int reason)
    {
        if (!IsServer || IsGameOver())
            return;

        resultReason.Value = reason;

        gameState.Value = winner == ChessPiece.PieceColor.White 
            ?WhiteWon
            :BlackWon;
    }

    private bool TryGetPlayerColor(ulong clientId, out ChessPiece.PieceColor color)
    {
        color = default;

        ChessGameManager manager = ChessGameManager.Instance;

        return manager != null && manager.TryGetAssignedPlayerColor(clientId, out color);
    }

    private bool IsAssignedPlayer(ulong clientId)
    {
        return TryGetPlayerColor(clientId, out _);
    }

    private bool IsGameOver()
    {
        return gameState.Value == DrawAgreed || gameState.Value == WhiteWon || gameState.Value == BlackWon;
    }

    private void UpdateUI()
    {

        Debug.Log(
            $"[DRAW UI] IsServer={IsServer}, IsClient={IsClient}, " +
            $"IsSpawned={IsSpawned}, State={gameState.Value}, " +
            $"OfferFrom={drawOfferFrom.Value}, " +
            $"LocalClient={NetworkManager.Singleton?.LocalClientId}"
        );

        ulong localClientId = NetworkManager.Singleton != null
            ? NetworkManager.Singleton.LocalClientId
            : NoOffer;

        bool playing = gameState.Value == Playing;
        bool offering = gameState.Value == DrawOffered;
        bool ended = IsGameOver();

        SetActiveIfNeeded(gameControlsPanel, playing);

        bool isOfferer = offering && drawOfferFrom.Value == localClientId;
        bool isOpponent = offering && drawOfferFrom.Value != localClientId;

        SetActiveIfNeeded(drawOfferPanel, isOfferer || isOpponent);

        SetActiveIfNeeded(gameOverPanel, ended);

        if (offering && drawOfferText != null)
        {
            drawOfferText.text = drawOfferFrom.Value == localClientId
                ? "Draw offered. Waiting for opponent ..."
                : "Opponent offers a draw...";
        }

        bool canRespond = offering && drawOfferFrom.Value != localClientId;

        if (acceptDrawButton != null)
            acceptDrawButton.interactable = canRespond;

        if (declineDrawButton != null)
            declineDrawButton.interactable = canRespond;

        if (ended && gameResultText != null)
        {
            if (gameState.Value == DrawAgreed)
            {
                gameResultText.text = "Draw agreed!";
            }

            else
            {
                string winner = gameState.Value == WhiteWon
                    ? "White"
                    : "Black";

                string reason = resultReason.Value == Checkmate
                    ? "by Checkmate"
                    : "by resignation";

                gameResultText.text = $"{winner} wins {reason}!";
            }
        }

    }


    
    private void SetActiveIfNeeded(GameObject target, bool shouldBeActive)
    {
        if (target != null && target.activeSelf != shouldBeActive)
            target.SetActive(shouldBeActive);
    }


}














