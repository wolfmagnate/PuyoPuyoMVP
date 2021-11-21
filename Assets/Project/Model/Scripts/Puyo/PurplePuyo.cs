public class PurplePuyo : PuyoBase
{
    public override bool IsSameColor(PuyoBase other)
    {
        return other is PurplePuyo;
    }

    public override PuyoColor GetColor()
    {
        return PuyoColor.purple;
    }
}
