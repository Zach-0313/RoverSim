using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class FadeInOutUI : MonoBehaviour
{
    public CanvasGroup uiGroup;
    public float UIFadeTime;

    public void OnOpenUI(InputValue value)
    {
        if (value.Get<float>() == 1)
        {
            uiGroup.DOFade(1, UIFadeTime);
        }
        if (value.Get<float>() == 0f)
        {
            uiGroup.DOFade(0, UIFadeTime);
        }
    }
}
