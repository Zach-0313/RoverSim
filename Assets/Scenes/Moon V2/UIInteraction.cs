using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class UIInteraction : MonoBehaviour
{
    [Range(0,1)]
    public float MouseOverScaleChange;
    [Range(0, 1)]
    public float ClickScaleChange;
    Vector3 NormalScale;
    Vector3 MouseOverScale;
    Vector3 ClickedScale;
    public float Timeframe;

    // Start is called before the first frame update
    void Start()
    {
        NormalScale = transform.localScale;
        MouseOverScale = NormalScale * (1 + MouseOverScaleChange);
        ClickedScale = NormalScale * (1 + ClickScaleChange);
    }
    void OnMouseEnter()
    {
        transform.DOScale(MouseOverScale, Timeframe);
    }
    void OnMouseExit()
    {
        transform.DOScale(NormalScale, Timeframe);
    }
    void OnMouseDown()
    {
        Sequence ClickedTween = DOTween.Sequence();
        ClickedTween.Append(transform.DOScale(ClickedScale, Timeframe))
          .PrependInterval(Timeframe)
          .Append(transform.DOScale(MouseOverScale, Timeframe));
    }
}
