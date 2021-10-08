using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGeneratedPuyo
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public PuyoBase Puyo { get; private set; }

    internal NewGeneratedPuyo(int x, int y, PuyoBase puyo)
    {
        X = x;
        Y = y;
        Puyo = puyo;
    }
}
