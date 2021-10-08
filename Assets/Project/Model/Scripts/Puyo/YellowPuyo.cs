using System.Collections;
using System.Collections.Generic;

public class YellowPuyo : PuyoBase
{
    public override bool IsSameColor(PuyoBase other)
    {
        return other is YellowPuyo;
    }

    public override PuyoColor GetColor()
    {
        return PuyoColor.yellow;
    }
}
