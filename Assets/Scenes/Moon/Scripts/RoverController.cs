using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class RoverController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    float FR_Drive;
    Animator animator;
    public float CurrentSpeed;
    public float MaxSpeed = 3;
    Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void FixedUpdate()
    {
        CurrentSpeed = rb.velocity.sqrMagnitude;
        float motor = maxMotorTorque * FR_Drive;
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

        if (CurrentSpeed > MaxSpeed) rb.velocity = rb.velocity.normalized * MaxSpeed;
    }
    public void Drive(int direction)
    {
        FR_Drive = direction;
        animator.SetFloat("FR_Input", FR_Drive);
    }
    public void CancelDrive()
    {
        FR_Drive = 0;
        animator.SetFloat("FR_Input", FR_Drive);
    }
}