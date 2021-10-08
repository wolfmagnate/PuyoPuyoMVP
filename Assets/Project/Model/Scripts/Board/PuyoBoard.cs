using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System;

public class PuyoBoard
{
    /// <summary>
    /// ぷよが並ぶマス目を表す。
    /// 1次元目がx座標、2次元目がy座標を表す。
    /// 数学で用いるx軸、y軸の向きを採用する。
    /// </summary>
    PuyoBase[,] board { get; set; }

    int Width { get; }

    int Height { get; }

    int MinMatchPuyoCount { get; }

    public bool CanNext
    {
        get; private set;
    }

    Subject<DeleteMessage> deletePuyo { get; set; } = new Subject<DeleteMessage>();
    Subject<GenerateMessage> generatePuyo { get; set; } = new Subject<GenerateMessage>();
    Subject<MatchMessage> matchPuyo { get; set; } = new Subject<MatchMessage>();
    Subject<MoveMessage> movePuyo { get; set; } = new Subject<MoveMessage>();

    public IObservable<DeleteMessage> DeletePuyo => deletePuyo;
    public IObservable<GenerateMessage> GeneratePuyo => generatePuyo;
    public IObservable<MatchMessage> MatchPuyo => matchPuyo;
    public IObservable<MoveMessage> MovePuyo => movePuyo;

    static PuyoBase GetRandomPuyo()
    {
        var puyos = new PuyoBase[] { new RedPuyo(), new BluePuyo(), new YellowPuyo(), new GreenPuyo(), new PurplePuyo() };
        return puyos[UnityEngine.Random.Range(0, 5)];
    }

    internal PuyoBoard(int width, int height, int minMatchPuyoCount)
    {
        board = new PuyoBase[width, height];
        CanNext = true;
        Width = width;
        Height = height;
        MinMatchPuyoCount = minMatchPuyoCount;
    }

    public void ErasePuyo(List<Vector2Int> erasePositions)
    {
        var message = new DeleteMessage();
        foreach(var position in erasePositions)
        {
            board[position.x, position.y] = null;
            message.AddDelete(position);
            CanNext = true;
        }
        deletePuyo.OnNext(message);
    }

    public void Next()
    {
        if (CanDrop())
        {
            Drop();
        }
        else if (CanMatch())
        {
            Match();
        }
        else if (CanGenerate())
        {
            Generate();
        }
        else
        {
            CanNext = false;
        }
    }

    private bool CanGenerate()
    {
        bool hasNull = false;
        for(int i = 0;i < board.GetLength(0); i++)
        {
            for(int j = 0;j < board.GetLength(1); j++)
            {
                if(board[i,j] == null)
                {
                    hasNull = true;
                }
            }
        }
        return (!CanDrop()) && hasNull;
    }

    private void Generate()
    {
        GenerateMessage generateMessage = new GenerateMessage();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if(board[i,j] == null)
                {
                    var generated = GetRandomPuyo();
                    board[i, j] = generated;
                    generateMessage.AddPuyo(i, j, generated);
                }
            }
        }
        generatePuyo.OnNext(generateMessage);
    }

    private bool CanMatch()
    {
        var areas = GetSameColorAreas();
        foreach(var area in areas)
        {
            if (area.Count >= MinMatchPuyoCount)
            {
                return true;
            }
        }
        return false;
    }

    List<List<Vector2Int>> GetSameColorAreas()
    {
        List<Vector2Int> selectedPoints = new List<Vector2Int>();
        List<List<Vector2Int>> areas = new List<List<Vector2Int>>();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if(selectedPoints.Contains(new Vector2Int(i, j))) { continue; }
                areas.Add(GetSameColorArea(new List<Vector2Int>() { new Vector2Int(i, j) }));
                selectedPoints = selectedPoints.Concat(areas.Last()).ToList();
            }
        }
        return areas;
    }

    List<Vector2Int> GetSameColorArea(List<Vector2Int> selectedPositions)
    {
        var startPosition = selectedPositions.Last();

        // 端の判定
        var isLeft = startPosition.x == 0;
        var isRight = startPosition.x == Width - 1;
        var isBottom = startPosition.y == 0;
        var isTop = startPosition.y == Height - 1;

        if ((!isLeft) && (!selectedPositions.Contains(new Vector2Int(startPosition.x - 1, startPosition.y))) && (board[startPosition.x - 1, startPosition.y] != null))
        {
            if(board[startPosition.x - 1, startPosition.y].IsSameColor(board[startPosition.x, startPosition.y]))
            {
                selectedPositions.Add(new Vector2Int(startPosition.x - 1, startPosition.y));
                GetSameColorArea(selectedPositions);
            }
        }

        if ((!isRight) && (!selectedPositions.Contains(new Vector2Int(startPosition.x + 1, startPosition.y))) && (board[startPosition.x + 1, startPosition.y] != null))
        {
            if (board[startPosition.x + 1, startPosition.y].IsSameColor(board[startPosition.x, startPosition.y]))
            {
                selectedPositions.Add(new Vector2Int(startPosition.x + 1, startPosition.y));
                GetSameColorArea(selectedPositions);
            }
        }

        if ((!isBottom) && (!selectedPositions.Contains(new Vector2Int(startPosition.x, startPosition.y - 1))) && (board[startPosition.x, startPosition.y - 1] != null))
        {
            if (board[startPosition.x, startPosition.y - 1].IsSameColor(board[startPosition.x, startPosition.y]))
            {
                selectedPositions.Add(new Vector2Int(startPosition.x, startPosition.y - 1));
                GetSameColorArea(selectedPositions);
            }
        }

        if ((!isTop) && (!selectedPositions.Contains(new Vector2Int(startPosition.x, startPosition.y + 1))) && (board[startPosition.x, startPosition.y + 1] != null))
        {
            if (board[startPosition.x, startPosition.y + 1].IsSameColor(board[startPosition.x, startPosition.y]))
            {
                selectedPositions.Add(new Vector2Int(startPosition.x, startPosition.y + 1));
                GetSameColorArea(selectedPositions);
            }
        }
        return selectedPositions;
    }

    private void Match()
    {
        MatchMessage message = new MatchMessage();
        var areas = GetSameColorAreas();
        foreach(var area in areas)
        {
            if (area.Count >= MinMatchPuyoCount)
            {
                foreach(var point in area)
                {
                    message.AddMatchPuyo(point);
                    board[point.x, point.y] = null;
                }
            }
        }
        matchPuyo.OnNext(message);
    }

    private bool CanDrop()
    {
        bool[] canColumnDrop = Enumerable.Repeat(false, Width).ToArray();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            int minNullPosition = -1;
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if(board[i,j] == null)
                {
                    minNullPosition = j;
                }
                if(minNullPosition != -1)
                {
                    if(board[i,j] != null)
                    {
                        canColumnDrop[i] = true;
                    }
                }
            }
        }
        return canColumnDrop.Any(x => x == true);
    }

    void Drop()
    {
        var message = new MoveMessage();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if(board[i,j] != null)
                {
                    int minNull = -1;
                    for(int k = j;k >= 0; k--)
                    {
                        if(board[i,k] == null)
                        {
                            minNull = k;
                        }
                    }
                    if(minNull == -1)
                    {
                        continue;
                    }
                    board[i, minNull] = board[i, j];
                    board[i, j] = null;
                    message.AddMove(new Vector2Int(i, j), new Vector2Int(i, minNull));
                }
            }
        }
        movePuyo.OnNext(message);
    }

}
