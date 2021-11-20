using UnityEngine;

public class MatchPuyo
{
    public Vector2Int Position { get; private set; }

    internal MatchPuyo(Vector2Int position)
    {
        Position = position;
    }
}
