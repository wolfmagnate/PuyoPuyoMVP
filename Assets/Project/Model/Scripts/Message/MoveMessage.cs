using System.Collections.Generic;
using UnityEngine;

public class MoveMessage
{
    List<MovePuyo> movePuyos { get; set; } = new List<MovePuyo>();

    internal void AddMove(Vector2Int from, Vector2Int to)
    {
        movePuyos.Add(new MovePuyo(from, to));
    }

    public IEnumerable<MovePuyo> MovePuyos => movePuyos;
}
