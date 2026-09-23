using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    [Header("Board Settings")]
    [SerializeField] private int boardSize = 8;
    [SerializeField] private float squareSize = 1f;

    [Header("Square Colors")]
    [SerializeField] private Color lightColor = new Color(0.9f, 0.9f, 0.9f);
    [SerializeField] private Color darkColor = new Color(0.25f, 0.25f, 0.25f);

    private GameObject[,] squares;

    private void Start(){
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        squares = new GameObject[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++){
            for (int z = 0; z < boardSize; z++){
                CreateSquare(x, z);
            }
        }
    }

    private void CreateSquare(int x, int z)
    {
        GameObject square = GameObject.CreatePrimitive(PrimitiveType.Cube);

        square.name = $"Square_{x}_{z}";

        square.transform.SetParent(transform);

        square.transform.position = new Vector3(
            (x-3.5f) * squareSize,
            0f,
            (z-3.5f) * squareSize
        );

        square.transform.localScale = new Vector3(
            squareSize,
            0.1f,
            squareSize
        );

        Renderer renderer = square.GetComponent<Renderer>();

        renderer.material = new Material(
            Shader.Find("Universal Render Pipeline/Lit")
        );

        renderer.material.color = 
            (x+z) % 2 == 0
            ? lightColor
            : darkColor;

        squares[x, z] = square;
    }
}