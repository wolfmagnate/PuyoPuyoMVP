using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using UnityEngine;

public class PuzzleView : MonoBehaviour
{
    private void Awake()
    {
        RedPuyoSprite = redPuyoSprite;
        BluePuyoSprite = bluePuyoSprite;
        YellowPuyoSprite = yellowPuyoSprite;
        GreenPuyoSprite = greenPuyoSprite;
        PurplePuyoSprite = purplePuyoSprite;
    }

    PuzzlePresenter presenter;
    public GameObject PuyoPrefab;
    void Start()
    {
        presenter = new PuzzlePresenter();
        
        presenter.puyos.ObserveAdd().Subscribe((x) =>
        {
            var puyo = Instantiate(PuyoPrefab);
            puyo.GetComponent<RectTransform>().SetParent(transform.Find("Board").GetComponent<RectTransform>());
            puyo.GetComponent<PuyoView>().Init(x.Value);
        });

        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton(0))
            .Select(_ => Input.mousePosition)
            .Where(position => IsInBoard(position))
            .Select(position => FromScreenToBoard(position))
            .DistinctUntilChanged()
            .Subscribe(position =>
            {
                presenter.DragBoardEvent.OnNext(position);
            });
        this.UpdateAsObservable().Where(_ => Input.GetMouseButtonUp(0)).Subscribe(_ =>
        {
            presenter.DragStoppedEvent.OnNext(Unit.Default);
        });
    }

    [SerializeField]
    Sprite redPuyoSprite;
    static Sprite RedPuyoSprite;
    [SerializeField]
    Sprite bluePuyoSprite;
    static Sprite BluePuyoSprite;
    [SerializeField]
    Sprite greenPuyoSprite;
    static Sprite GreenPuyoSprite;
    [SerializeField]
    Sprite yellowPuyoSprite;
    static Sprite YellowPuyoSprite;
    [SerializeField]
    Sprite purplePuyoSprite;
    static Sprite PurplePuyoSprite;

    public static Sprite GetSprite(PuyoColor color)
    {
        return color switch
        {
            PuyoColor.red => RedPuyoSprite,
            PuyoColor.blue => BluePuyoSprite,
            PuyoColor.yellow => YellowPuyoSprite,
            PuyoColor.green => GreenPuyoSprite,
            PuyoColor.purple => PurplePuyoSprite,
            _ => null
        };
    }
    static readonly int PuyoWidth = 100;
    static readonly int PuyoHeight = 100;
    static readonly Vector2Int ScreenResolution = new Vector2Int(1920, 1080);
    public static Vector2 GetScreenPosition(Vector2Int value)
    {
        return new Vector2(value.x * PuyoWidth, value.y * PuyoHeight);
    }
    public static bool IsInBoard(Vector2 value)
    {
        int x = Mathf.FloorToInt((value.x - (ScreenResolution.x / 2 - Model.Width / 2 * PuyoWidth)) / 100);
        int y = Mathf.FloorToInt((value.y - (ScreenResolution.y / 2 - Model.Height / 2 * PuyoHeight)) / 100);
        return (x >= 0 && x <= Model.Width - 1 && y <= Model.Height - 1 && y >= 0);
    }
    public static Vector2Int FromScreenToBoard(Vector2 value)
    {
        int x = Mathf.FloorToInt((value.x - (ScreenResolution.x / 2 - Model.Width / 2 * PuyoWidth)) / 100);
        int y = Mathf.FloorToInt((value.y - (ScreenResolution.y / 2 - Model.Height / 2 * PuyoHeight)) / 100);
        return new Vector2Int(x, y);
    }

}
