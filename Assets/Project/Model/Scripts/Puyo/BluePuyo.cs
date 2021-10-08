using System.Collections;
using System.Collections.Generic;

public class BluePuyo : PuyoBase
{
    public override bool IsSameColor(PuyoBase other)
    {
        return other is BluePuyo;
    }

    public override PuyoColor GetColor()
    {
        return PuyoColor.blue;
    }
}
