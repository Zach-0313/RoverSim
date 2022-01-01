using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

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
    public UnityEvent MouseDown;
    public bool ClickEffect;
    public bool ScaleParent;
    public bool ChildScaler;
    public bool RepeatWhileHeld;
    UIInteraction parentUI;
    // Start is called before the first frame update
    void Start()
    {
        DOTween.defaultRecyclable = true;
        NormalScale = transform.localScale;
        MouseOverScale = NormalScale * (1 + MouseOverScaleChange);
        ClickedScale = NormalScale * (1 + ClickScaleChange);
        if (ScaleParent)
        {
            if (transform.parent.GetComponent<UIInteraction>())
            {
                parentUI = transform.parent.GetComponent<UIInteraction>();
            }
        }
    }
    void OnMouseEnter()
    {
        if (ScaleParent)
        {
            transform.parent.DOScale(parentUI.MouseOverScale, Timeframe);
            parentUI.ChildScaler = true;
        }
        transform.DOScale(MouseOverScale, Timeframe);
    }
    void OnMouseExit()
    {
        if(!ChildScaler)transform.DOScale(NormalScale, Timeframe);

        if (ScaleParent)
        {
            transform.parent.DOScale(parentUI.NormalScale, Timeframe);
            parentUI.ChildScaler = false;
        }
    }
    void OnMouseDown()
    {
        if (!ClickEffect) return;
        transform.DOScale(ClickedScale, Timeframe);
        MouseDown?.Invoke();
    }
    void OnMouseDrag()
    {
        if (RepeatWhileHeld) MouseDown?.Invoke();
    }
    void OnMouseUp()
    {
        transform.DOScale(MouseOverScale, Timeframe);
    }
}
