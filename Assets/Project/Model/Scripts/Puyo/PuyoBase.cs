public abstract class PuyoBase
{
    public abstract bool IsSameColor(PuyoBase other);
    public abstract PuyoColor GetColor();
}

public enum PuyoColor
{
    red, blue, yellow, green, purple
}