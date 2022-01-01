using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RoverMovement : MonoBehaviour
{
    [SerializeField] RoverManager.ControlMode ControlMethod;
    RoverManager roverManager;
    Vector3 MousePositionInWorld;
    NavMeshAgent agent;
    public float whichWay;
    public float TurnThreshold;
    Vector3 targetDir;
    public float TimeToTurn360;

    void OnEnable()
    {
        roverManager = GetComponent<RoverManager>();
        roverManager.OnControlSchemeChanged += OnUpdateControlType;
    }
    void OnDisable()
    {
        roverManager = GetComponent<RoverManager>();
        roverManager.OnControlSchemeChanged -= OnUpdateControlType;
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void OnUpdateControlType(object sender, RoverManager.OnControlSchemeChangedEventArgs eventArgs)
    {
        ControlMethod = eventArgs.newControlMode;
        Debug.Log($"Control Mode has been updated to: {ControlMethod.ToString()}");
    }
    public void OnMouseClick()
    {
        if (isOverUI() || roverManager.ArmModeEnabled || ControlMethod != RoverManager.ControlMode.Auto)
        {
            return;
        }
        agent.ResetPath();
        Ray MouseToWorld = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(MouseToWorld, out RaycastHit hit);
        MousePositionInWorld = hit.point;
        targetDir = transform.position - MousePositionInWorld;
        whichWay = AngleToDirection(-targetDir.normalized);
        if (Mathf.Abs(whichWay) > TurnThreshold)
        {
            Tween rotateRover = transform.DORotate(TransformRotateToDirection(-targetDir.normalized), (Mathf.Abs(whichWay) / 360) * TimeToTurn360, RotateMode.FastBeyond360);
            rotateRover.OnComplete(() => SetAgentDestination());
        }
    }
    float AngleToDirection(Vector3 target)
    {
        return Vector3.SignedAngle(transform.forward, target.normalized, Vector3.up);
    }
    Vector3 TransformRotateToDirection(Vector3 target)
    {
        Vector3 rotation = transform.eulerAngles;
        rotation.y += AngleToDirection(target);
        return rotation;
    }
    public Vector3 MoveDirection;
    void Update()
    {
        if (ControlMethod == RoverManager.ControlMode.Manual)
        {
            if (MoveDirection.magnitude > 0.1) 
            {
                agent.Move(MoveDirection * agent.speed * Time.fixedDeltaTime);
                transform.DORotate(TransformRotateToDirection(MoveDirection.normalized), (Mathf.Abs(AngleToDirection(MoveDirection.normalized)) / 360) * TimeToTurn360, RotateMode.Fast);
            }
        }
    }
    private void SetAgentDestination()
    {
        agent.SetDestination(MousePositionInWorld);
        Debug.Log("Agent Destination has been set");
    }
    public void OnManualDrive(InputValue context)
    {
        if (ControlMethod != RoverManager.ControlMode.Manual || roverManager.ArmModeEnabled) return;
        agent.ResetPath();

        Vector2 MovementInput = -context.Get<Vector2>();
        float t = MovementInput.y;
        MovementInput.y = MovementInput.x;
        MovementInput.x = -t;
        if (MovementInput.magnitude < 0.1)
        {
            MoveDirection = Vector3.zero;
            return;
        }
        float TargetAngle = (Mathf.Atan2(MovementInput.x, MovementInput.y) * Mathf.Rad2Deg) + Camera.main.transform.eulerAngles.y - 90;
        MoveDirection = Quaternion.Euler(0, TargetAngle, 0) * Vector3.forward;
    }
    private bool isOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
