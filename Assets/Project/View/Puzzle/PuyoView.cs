using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class PuyoView : MonoBehaviour
{
    PuyoPresenter presenter;
    Sequence selectAnimation;
    public void Init(PuyoPresenter presenter)
    {
        this.presenter = presenter;
        GetComponent<Image>().sprite = PuzzleView.GetSprite(presenter.color);
        PuzzleView.GetScreenPosition(presenter.position.Value);

        presenter.position.Buffer(2, 1).Subscribe(Move).AddTo(gameObject);
        presenter.DeletePuyoObservable.Subscribe(Delete).AddTo(gameObject);
        presenter.MatchPuyoObservable.Subscribe(Match).AddTo(gameObject);
        presenter.IsSelectedObservable.Where(selected => selected).Subscribe(selected => StartSelectAnimation()).AddTo(gameObject);
        presenter.IsSelectedObservable.Where(selected => !selected).Subscribe(_ => StopSelectAnimation()).AddTo(gameObject);

        GetComponent<RectTransform>().localScale = Vector3.one;

        selectAnimation = DOTween.Sequence();
        selectAnimation.Append(GetComponent<RectTransform>().DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.3f));
        selectAnimation.Append(GetComponent<RectTransform>().DOScale(new Vector3(1f, 1f, 1f), 0.3f));
        selectAnimation.SetLoops(-1);
        selectAnimation.Pause();
    }

    private void Move(IList<Vector2Int> positions)
    {
        GetComponent<RectTransform>().anchoredPosition = PuzzleView.GetScreenPosition(positions[0]);
        GetComponent<RectTransform>().DOAnchorPos(PuzzleView.GetScreenPosition(positions[1]), presenter.animationTime);
    }

    private void Delete(Unit unit)
    {
        selectAnimation.Kill();
        StartCoroutine(deleteCoroutine());
    }

    IEnumerator deleteCoroutine()
    {
        GetComponent<Image>().DOFade(0, presenter.animationTime);
        yield return new WaitForSeconds(presenter.animationTime + 0.1f);
        Destroy(gameObject);
    }

    private void Match(Unit unit)
    {
        selectAnimation.Kill();
        StartCoroutine(matchCoroutine());
    }

    IEnumerator matchCoroutine()
    {
        GetComponent<RectTransform>().DOScale(2 * Vector3.one, presenter.animationTime);
        yield return new WaitForSeconds(presenter.animationTime + 0.1f);
        Destroy(gameObject);
    }

    private void StartSelectAnimation()
    {
        selectAnimation.Play();
    }

    private void StopSelectAnimation()
    {
        selectAnimation.Pause();
    }
}
