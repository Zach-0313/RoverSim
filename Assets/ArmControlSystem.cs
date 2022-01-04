using DG.Tweening;
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
    public Transform ArmRotateAtBase;
    public Animator ArmAnimator;
    public float toArm;
    public Transform ArmTarget;
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
    void Update()
    {
        toArm = Mathf.Abs(Vector3.Distance(EndOfArm.position, transform.position));

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
        ArmTarget.transform.position = TargetPosition;
        //RadiusToPoint = RadiusToTarget();
        //AngleToPoint = AngleToTargetPoint(EndOfArm.position.x, EndOfArm.position.z, TargetPosition.x, TargetPosition.z);
        AngleToTarget = AngleFromArmToTarget();
        distanceFromArmToTarget = DistanceFromArmToTarget();
        Vector3 rotation = ArmRotateAtBase.transform.rotation.eulerAngles;
        rotation.y += AngleToTarget;
        ArmRotateAtBase.rotation = Quaternion.Euler(rotation);
        //ArmTarget.transform.forward = ArmRotateAtBase.transform.forward;
    }
    float AngleToDirection(Vector3 target)
    {
        return Vector3.SignedAngle(transform.forward, target.normalized, Vector3.up);
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
        return Vector3.Angle((EndOfArm.position-transform.position).normalized, (targetPosition - transform.position).normalized);
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
