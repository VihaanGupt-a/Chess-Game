using UnityEngine;

public static class ChessRules 
{
    public static bool IsInsideBoard(int x, int z)
    {
        return x >= 0 && x < 8 && z >= 0 && z < 8;
    }
}