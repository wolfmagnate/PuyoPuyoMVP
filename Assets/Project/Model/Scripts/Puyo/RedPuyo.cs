using System.Collections;
using System.Collections.Generic;

public class RedPuyo : PuyoBase
{
    public override bool IsSameColor(PuyoBase other)
    {
        return other is RedPuyo;
    }

    public override PuyoColor GetColor()
    {
        return PuyoColor.red;
    }
}
