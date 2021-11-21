using UnityEngine;

public class DeletePuyo
{
    public Vector2Int Position { get; private set; }
    internal DeletePuyo(Vector2Int position)
    {
        Position = position;
    }
}
