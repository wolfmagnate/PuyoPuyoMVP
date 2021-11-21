using UnityEngine;
using System;
using UniRx;

public class PuyoPresenter
{
    public ReactiveProperty<Vector2Int> position { get; private set; }
    public PuyoColor color { get; private set; }
    public float animationTime { get; internal set; }

    Subject<Unit> _matchPuyo;
    internal IObserver<Unit> MatchPuyoObserver => _matchPuyo;
    public IObservable<Unit> MatchPuyoObservable => _matchPuyo;
    Subject<Unit> _deletePuyo;
    internal IObserver<Unit> DeletePuyoObserver => _deletePuyo;
    public IObservable<Unit> DeletePuyoObservable => _deletePuyo;
    internal ReactiveProperty<bool> IsSelected;
    public IObservable<bool> IsSelectedObservable => IsSelected;

    public PuyoPresenter(int x, int y, PuyoColor color)
    {
        _matchPuyo = new Subject<Unit>();
        _deletePuyo = new Subject<Unit>();
        IsSelected = new ReactiveProperty<bool>();
        position = new ReactiveProperty<Vector2Int>();
        position.Value = new Vector2Int(x, y);
        this.color = color;
    }
}
