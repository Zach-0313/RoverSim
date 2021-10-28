using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DualCameraSystem : MonoBehaviour
{
    public CinemachineFreeLook Camera1;
    public CinemachineFreeLook Camera2;

    public void MouseDown()
    {
        Camera1.gameObject.SetActive(false);
        Camera2.transform.position = Camera1.transform.position;
        Camera2.transform.gameObject.SetActive(true);
    }
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Camera1.gameObject.SetActive(false);
            Camera2.transform.gameObject.SetActive(true);
        }
        else
        {
            ResetCamera1Position();
            Camera1.gameObject.SetActive(true);
            Camera2.gameObject.SetActive(false);
        }
    }
    public void ResetCamera1Position()
    {
        Camera1.transform.position = Camera2.transform.position;
        Camera1.transform.rotation = Camera2.transform.rotation;
    }
}
