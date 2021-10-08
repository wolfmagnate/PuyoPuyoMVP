using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMessage
{
    List<DeletePuyo> deletePuyos { get; set; } = new List<DeletePuyo>();
    internal void AddDelete(Vector2Int position)
    {
        deletePuyos.Add(new DeletePuyo(position));
    }

    public IEnumerable<DeletePuyo> DeletePuyos => deletePuyos;
}
