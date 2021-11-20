using System.Collections.Generic;

public class GenerateMessage
{
    List<NewGeneratedPuyo> generatedPuyos { get; set; } = new List<NewGeneratedPuyo>();

    internal void AddPuyo(int x, int y, PuyoBase puyo)
    {
        generatedPuyos.Add(new NewGeneratedPuyo(x, y, puyo));
    }

    public IEnumerable<NewGeneratedPuyo> GeneratedPuyos => generatedPuyos;
}
