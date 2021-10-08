using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Model
{
    // �ݒ�t�@�C���ɂ����肾�������Ȃ�悤�ȓ��e�͂����ɂ����Ă���
    public static readonly int Width = 8;
    public static readonly int Height = 6;
    public static readonly int MinMatchPuyoCount = 4;
    public static readonly int MaxSelectPuyoCount = 5;

    // Model�̃Z�b�g�A�b�v��Model���ōs���BPresenter���ł���Ă͂Ȃ�Ȃ��B
    static Model()
    {
        Board = new PuyoBoard(Width, Height, MinMatchPuyoCount);
    }

    public static PuyoBoard Board { get; }
}
