using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Model
{
    // 設定ファイルにくくりだしたくなるような内容はここにおいておく
    public static readonly int Width = 8;
    public static readonly int Height = 6;
    public static readonly int MinMatchPuyoCount = 4;
    public static readonly int MaxSelectPuyoCount = 5;

    // ModelのセットアップもModel側で行う。Presenter側でやってはならない。
    static Model()
    {
        Board = new PuyoBoard(Width, Height, MinMatchPuyoCount);
    }

    public static PuyoBoard Board { get; }
}
