using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class RoverManager : MonoBehaviour
{
    public event EventHandler<OnControlSchemeChangedEventArgs> OnControlSchemeChanged;
    public event EventHandler<OnArmModeChangedEventArgs> OnArmModeUpdated;

    public class OnControlSchemeChangedEventArgs : EventArgs { public ControlMode newControlMode; }
    public class OnArmModeChangedEventArgs : EventArgs { public bool isActive; }

    public event EventHandler OnRotateExcavators;

    public enum ControlMode { Manual, Auto }
    ControlMode ControlScheme { get; set; }

    [SerializeField] RoverProfileSO roverProfile;

    NavMeshAgent agent;
    public bool ArmModeEnabled;
    public void ToggleArmMode()
    {
        if (ArmModeEnabled) ArmModeEnabled = false;
        else ArmModeEnabled = true;

        OnArmModeUpdated?.Invoke(this, new OnArmModeChangedEventArgs{isActive = ArmModeEnabled});
    }
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetControlScheme(1);
    }
    public void SetControlScheme(int newMode)
    {
        if (newMode == 1 && ControlScheme != ControlMode.Auto)
        {
            ControlScheme = ControlMode.Auto;
            OnControlSchemeChanged?.Invoke(this, new OnControlSchemeChangedEventArgs { newControlMode = ControlMode.Auto });
        }
        else if (newMode == 0 && ControlScheme != ControlMode.Manual)
        {
            ControlScheme = ControlMode.Manual;
            OnControlSchemeChanged?.Invoke(this, new OnControlSchemeChangedEventArgs { newControlMode = ControlMode.Manual });
        }
    }
    private bool isOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
