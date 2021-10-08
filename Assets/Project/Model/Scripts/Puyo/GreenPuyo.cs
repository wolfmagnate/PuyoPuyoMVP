using System.Collections;
using System.Collections.Generic;

public class GreenPuyo : PuyoBase
{
    public override bool IsSameColor(PuyoBase other)
    {
        return other is GreenPuyo;
    }

    public override PuyoColor GetColor()
    {
        return PuyoColor.green;
    }
}
