using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class RoverUI : MonoBehaviour
{
    public enum UImode { movement, features };
    public UImode UIstate;
    public Animator animator;

    [SerializeField] private List<GameObject> MovementUI;
    [SerializeField] private List<GameObject> FeatureUI;
    public TextMeshProUGUI ModeReadout;

    // Start is called before the first frame update
    void Start()
    {
        UIstate = UImode.movement;
        SetMode(UIstate);
    }
    public void SwitchMode()
    {
        if(UIstate == UImode.features)
        {
            SetMode(UImode.movement);
            animator.SetFloat("ExcavateInput", 0);
            UIstate = UImode.movement;
        }
        else if(UIstate == UImode.movement)
        {
            SetMode(UImode.features);
            UIstate = UImode.features;
            animator.SetFloat("ExcavateInput", 1);
        }
    }
    public void SetMode(UImode mode)
    {
        if (UIstate != mode)
        {
            switch (UIstate)
            {
                case UImode.movement:
                    foreach (GameObject item in MovementUI)
                    {
                        item.SetActive(false);
                    }
                    break;
                case UImode.features:
                    foreach (GameObject item in FeatureUI)
                    {
                        item.SetActive(false);
                    }
                    break;
            }
            switch (mode)
            {
                case (UImode.movement):
                    ModeReadout.text = "Movement";
                    foreach (GameObject item in MovementUI)
                    {
                        item.SetActive(true);
                    }
                    break;
                case (UImode.features):
                    ModeReadout.text = "Features";
                    foreach (GameObject item in FeatureUI)
                    {
                        item.SetActive(true);
                    }
                    break;
            }
        }
        else return;
    }

}
