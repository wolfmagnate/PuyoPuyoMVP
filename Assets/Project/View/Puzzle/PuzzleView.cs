using UniRx;
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

    public static Vector2 GetScreenPosition(Vector2Int value)
    {
        return 100 * value;
    }

    public static bool IsInBoard(Vector2 value)
    {
        int x = Mathf.FloorToInt((value.x - 560) / 100);
        int y = Mathf.FloorToInt((value.y - 240) / 100);
        return (x >= 0 && x <= Model.Width - 1 && y <= Model.Height - 1 && y >= 0);
    }

    public static Vector2Int FromScreenToBoard(Vector2 value)
    {
        int x = Mathf.FloorToInt((value.x - 560) / 100);
        int y = Mathf.FloorToInt((value.y - 240) / 100);
        return new Vector2Int(x, y);
    }

}
