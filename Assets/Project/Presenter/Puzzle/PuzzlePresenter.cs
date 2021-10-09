using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PuzzlePresenter
{

    public ReactiveCollection<PuyoPresenter> puyos { get; private set; }
    AnimationTimeManager timeManager;

    float PuyoDropSpeed = 0.5f;
    float MatchAnimationTime = 0.5f;
    float DeleteAnimationTime = 0.5f;

    public PuzzlePresenter()
    {
        Model.Board.DeletePuyo.Subscribe(DeletePuyo);
        Model.Board.GeneratePuyo.Subscribe(GeneratePuyo);
        Model.Board.MatchPuyo.Subscribe(MatchPuyo);
        Model.Board.MovePuyo.Subscribe(MovePuyo);
        Observable.EveryUpdate().Where(_ => Model.Board.CanNext && timeManager.IsWating).Subscribe(_ => Model.Board.Next());

        puyos = new ReactiveCollection<PuyoPresenter>();
        timeManager = new AnimationTimeManager();

        dragPositions = new List<Vector2Int>();
        dragBoardEvent = new Subject<Vector2Int>();
        dragBoardEvent
            .Where(_ => (!Model.Board.CanNext) && timeManager.IsWating)
            .Where(_ => dragPositions.Count < Model.MaxSelectPuyoCount)
            .Where(position => !dragPositions.Contains(position))
            .Where(position => dragPositions.Count == 0 || dragPositions.Any(x => IsNextTo(x, position)))
            .Subscribe(DragBoard);
        dragStoppedEvent = new Subject<Unit>();
        dragStoppedEvent
            .Where(_ => (!Model.Board.CanNext) && timeManager.IsWating)
            .Where(_ => dragPositions.Count != 0)
            .Subscribe(DragStopped);
    }

    void DeletePuyo(DeleteMessage message)
    {
        foreach (var deletePuyo in message.DeletePuyos)
        {
            PuyoPresenter target = null;
            target = puyos.First(x => x.position.Value == deletePuyo.Position);
            puyos.Remove(target);
            target.animationTime = DeleteAnimationTime;
            target.DeletePuyoObserver.OnNext(Unit.Default);
        }
        timeManager.WaitFor(DeleteAnimationTime);
    }

    void GeneratePuyo(GenerateMessage message)
    {
        float maxAnimationTime = 0;
        foreach(var generatedPuyo in message.GeneratedPuyos)
        {
            var puyo = new PuyoPresenter(generatedPuyo.X, generatedPuyo.Y + 6, generatedPuyo.Puyo.GetColor());
            puyos.Add(puyo);
            puyo.animationTime = PuyoDropSpeed * 6;
            if(puyo.animationTime > maxAnimationTime)
            {
                maxAnimationTime = puyo.animationTime;
            }
            puyo.position.Value = new Vector2Int(generatedPuyo.X, generatedPuyo.Y);
        }
        timeManager.WaitFor(maxAnimationTime);
    }

    void MatchPuyo(MatchMessage message)
    {
        foreach(var matchPuyo in message.MatchPuyos)
        {
            var target = puyos.First(x => x.position.Value == matchPuyo.Position);
            puyos.Remove(target);
            target.animationTime = MatchAnimationTime;
            target.MatchPuyoObserver.OnNext(Unit.Default);
        }
        timeManager.WaitFor(MatchAnimationTime);
    }

    void MovePuyo(MoveMessage message)
    {
        float maxAnimationTime = 0;
        foreach(var movePuyo in message.MovePuyos)
        {
            var target = puyos.First(x => x.position.Value == movePuyo.From);
            target.animationTime = PuyoDropSpeed * (movePuyo.From.y - movePuyo.To.y);
            if(target.animationTime > maxAnimationTime)
            {
                maxAnimationTime = target.animationTime;
            }
            target.position.Value = movePuyo.To;
        }
        timeManager.WaitFor(maxAnimationTime);
    }


    public Subject<Vector2Int> dragBoardEvent;
    public IObserver<Vector2Int> DragBoardEvent => dragBoardEvent;

    public Subject<Unit> dragStoppedEvent;
    public IObserver<Unit> DragStoppedEvent => dragStoppedEvent;

    List<Vector2Int> dragPositions;
    void DragBoard(Vector2Int position)
    {
        dragPositions.Add(position);
        foreach(var puyo in puyos)
        {
            if(puyo.position.Value == position)
            {
                puyo.IsSelected.Value = true;
            }
        }
    }

    void DragStopped(Unit unit)
    {
        Model.Board.ErasePuyo(dragPositions);
        foreach(var puyo in puyos)
        {
            if (dragPositions.Contains(puyo.position.Value))
            {
                puyo.IsSelected.Value = false;
            }
        }
        dragPositions.Clear();
    }

    bool IsNextTo(Vector2Int x, Vector2Int y)
    {
        return (x - y).magnitude == 1;
    }

}
