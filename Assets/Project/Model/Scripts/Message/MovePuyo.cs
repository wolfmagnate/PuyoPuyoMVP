using UnityEngine;

public class MovePuyo
{
    public Vector2Int From { get; private set; }
    public Vector2Int To { get; private set; }

    internal MovePuyo(Vector2Int from, Vector2Int to)
    {
        From = from;
        To = to;
    }
}
