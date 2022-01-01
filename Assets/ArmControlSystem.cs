using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ArmControlSystem : MonoBehaviour
{
    [SerializeField] Vector3 TargetPosition;
    [SerializeField] float RadiusToTarget;
    [SerializeField] float distanceFromArmToTarget;
    [SerializeField] float AngleToTarget;
    [SerializeField] float MaxDistance, MinDistance;
    public float YOffset;
    public bool ArmModeEnabled;
    public Transform EndOfArm;
    void OnEnable()
    {
        RoverManager roverManager = GetComponent<RoverManager>();
        roverManager.OnArmModeUpdated += OnArmModeUpdate;
    }
    void OnDisable()
    {
        RoverManager roverManager = GetComponent<RoverManager>();
        roverManager.OnArmModeUpdated -= OnArmModeUpdate;
    }
    void OnArmModeUpdate(object sender, RoverManager.OnArmModeChangedEventArgs eventArgs)
    {
        ArmModeEnabled = eventArgs.isActive;
        Debug.Log($"Arm Control active = {ArmModeEnabled}");
    }
    public void OnMouseClick()
    {
        if (!ArmModeEnabled) return;   //Only run code if the Arm Control Mode is enabled

        if (isOverUI())
        {
            Debug.Log("Clicked over UI");
            return;
        }
        Ray MouseToWorld = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(MouseToWorld, out RaycastHit hit);
        TargetPosition = hit.point;
        //RadiusToPoint = RadiusToTarget();
        //AngleToPoint = AngleToTargetPoint(EndOfArm.position.x, EndOfArm.position.z, TargetPosition.x, TargetPosition.z);
        AngleToTarget = AngleFromArmToTarget();
        distanceFromArmToTarget = DistanceFromArmToTarget();
    }
    float DistanceFromArmToTarget()
    {
        float toArm = Mathf.Abs(Vector3.Distance(EndOfArm.position, transform.position));
        float toTarget = Mathf.Abs(Vector3.Distance(TargetPosition, transform.position));
        return toTarget - toArm;
    }
    float AngleFromArmToTarget()
    {
        Vector3 targetPosition = TargetPosition;
        return Vector3.SignedAngle((transform.position - EndOfArm.position).normalized, (transform.position - targetPosition).normalized, Vector3.up);
    }
    private bool isOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, EndOfArm.position);
        Gizmos.color = Color.red;
        if (TargetPosition != null)
        {
            Gizmos.DrawLine(transform.position, TargetPosition);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(TargetPosition, EndOfArm.position);
        }
    }
}
