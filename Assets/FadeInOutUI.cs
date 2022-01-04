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
            uiGroup.transform.GetChild(0).gameObject.SetActive(true);
            uiGroup.enabled = true;
            uiGroup.DOFade(1, UIFadeTime);
            uiGroup.blocksRaycasts = true;

        }
        else
        {
            uiGroup.DOFade(0, UIFadeTime);
            uiGroup.transform.GetChild(0).gameObject.SetActive(false);
            uiGroup.blocksRaycasts = false;
        }
    }
}
