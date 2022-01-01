using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DualCameraSystem : MonoBehaviour
{
    public CinemachineFreeLook Camera1;  //Static Camera
    public CinemachineFreeLook Camera2;    //Orbit Camera
    public CinemachineFreeLook ArmModeCam;   //Top-Down Camera

    [SerializeField] CinemachineFreeLook CurrentActiveCam;
    public RoverManager roverManager;
    void Start()
    {
        ArmModeCam.gameObject.SetActive(false);
        CurrentActiveCam = Camera2;
        Camera1.m_Orbits = Camera2.m_Orbits;
    }
    void OnEnable()
    {
        roverManager.OnArmModeUpdated += OnArmModeUpdate;
    }
    void OnDisable()
    {
        roverManager.OnArmModeUpdated -= OnArmModeUpdate;
    }
    void OnArmModeUpdate(object sender, RoverManager.OnArmModeChangedEventArgs eventArgs)
    {
        if (true == eventArgs.isActive)
        {
            ArmModeCam.gameObject.SetActive(true);
            CurrentActiveCam = ArmModeCam;
            Camera1.m_Orbits = ArmModeCam.m_Orbits;
        }
        else
        {
            ArmModeCam.gameObject.SetActive(false);
            CurrentActiveCam = Camera2;
            Camera1.m_Orbits = Camera2.m_Orbits;

        }
        Debug.Log($"Arm Control active = {eventArgs.isActive}");
    }
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            CurrentActiveCam.transform.gameObject.SetActive(true);
            CurrentActiveCam.transform.position = Camera1.transform.position;
            Camera1.gameObject.SetActive(false);
            //Camera1.gameObject.SetActive(false);
            //Camera2.transform.gameObject.SetActive(true);
        }
        else
        {
            ResetCamera1Position();
            Camera1.gameObject.SetActive(true);
            CurrentActiveCam.gameObject.SetActive(false);
        }
    }
    public void ResetCamera1Position()
    {
        Camera1.transform.position = CurrentActiveCam.transform.position;
        Camera1.transform.rotation = CurrentActiveCam.transform.rotation;
    }
}
