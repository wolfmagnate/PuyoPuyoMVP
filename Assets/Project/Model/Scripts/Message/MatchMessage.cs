using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchMessage
{
    List<MatchPuyo> matchPuyos { get; set; } = new List<MatchPuyo>();

    internal void AddMatchPuyo(Vector2Int position)
    {
        matchPuyos.Add(new MatchPuyo(position));
    }

    public IEnumerable<MatchPuyo> MatchPuyos => matchPuyos;
}
