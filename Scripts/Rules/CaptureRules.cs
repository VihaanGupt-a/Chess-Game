public static class CaptureRules
{
    public static bool CanCapture(
        ChessPiece attacker,
        ChessPiece target
    )
    {
        if (target == null)
            return false;

        if (attacker.Color == target.Color)
            return false;

        if (target.Type == ChessPiece.PieceType.King)
            return false;

        return true;
    }
}