using UnityEngine;
using UnityEngine.InputSystem;

public class RotateJointSystem : MonoBehaviour
{
    public Collider DetectMouseZone;
    public string RotateOnAxis;
    enum rotationAxis { x, y, z };
    rotationAxis effectAxis;
    public RoverManager rovermanager;
    Vector3 StartingRotation;
    public float minRotation;
    public float maxRotation;
    public bool isLimited;
    public bool invert;
    int invertNum;
    private static readonly char[] CheckForCharacters = "xyz".ToCharArray();
    void OnValidate()
    {
        int indexOf = RotateOnAxis.IndexOfAny(CheckForCharacters);
        if (RotateOnAxis.ToLower().Length > 1 && indexOf != -1)
        {
            Debug.Log("Enter a valid rotation axis(x, y, or z)");
            return;
        }
        string input = RotateOnAxis.ToLower();
        switch (input)
        {
            case "x":
                effectAxis = rotationAxis.x;
                break;
            case "y":
                effectAxis = rotationAxis.y;
                break;
            case "z":
                effectAxis = rotationAxis.z;
                break;
        }
        Debug.Log("Rotation Axis has been set to: " + effectAxis.ToString());
    }
    void Start()
    {
        StartingRotation = transform.rotation.eulerAngles;
        if (invert) invertNum = -1;
        else invertNum = 1;
    }
    Vector3 CheckForLimits(Vector3 NewRotation)
    {
        Vector3 offset = StartingRotation - NewRotation;
        if (!isLimited) return NewRotation;

        if (minRotation > 0) minRotation += 360;
        switch (effectAxis)
        {
            case rotationAxis.x:
                NewRotation.x = Mathf.Clamp(NewRotation.x, minRotation, maxRotation);
                break;
            case rotationAxis.y:
                NewRotation.y = Mathf.Clamp(NewRotation.y, minRotation, maxRotation);
                break;
            case rotationAxis.z:
                NewRotation.z = Mathf.Clamp(NewRotation.z, minRotation, maxRotation);
                break;
        }
        return NewRotation;
    }
    void OnMouseDrag()
    {
        if (!rovermanager.ArmModeEnabled) return;
        Vector3 Rotation = transform.localEulerAngles;
        switch (effectAxis)
        {
            case rotationAxis.x:
                Rotation += Vector3.right * Mouse.current.delta.x.ReadValue() * invertNum;
                transform.localEulerAngles = (CheckForLimits(Rotation));
                break;
            case rotationAxis.y:
                Rotation += Vector3.up * Mouse.current.delta.x.ReadValue() * invertNum;
                transform.localEulerAngles = (CheckForLimits(Rotation));
                break;
            case rotationAxis.z:
                Rotation += Vector3.up * Mouse.current.delta.x.ReadValue() * invertNum;
                transform.localEulerAngles = (CheckForLimits(Rotation));
                break;
        }
    }
}
