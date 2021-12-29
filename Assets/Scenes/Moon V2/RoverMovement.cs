using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using DG.Tweening;

public class RoverMovement : MonoBehaviour
{
    [SerializeField] RoverManager.ControlMode ControlMethod;
    Vector3 MousePositionInWorld;
    NavMeshAgent agent;
    public float whichWay;
    public float TurnThreshold;
    Vector3 targetDir;
    bool ActivelyTurning;
    bool DestinationSet;
    public float TurnSpeed;
    Vector3 destination;



    void OnEnable()
    {
        RoverManager roverManager = GetComponent<RoverManager>();
        roverManager.OnControlSchemeChanged += OnUpdateControlType;
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = true;
        agent.updateUpAxis = true;
        agent.updateRotation = true;
    }
    void OnUpdateControlType(object sender, RoverManager.OnControlSchemeChangedEventArgs eventArgs)
    {
        ControlMethod = eventArgs.newControlMode;
        Debug.Log($"Control Mode has been updated to: {ControlMethod.ToString()}");
    }
    void OnDisable()
    {
        RoverManager roverManager = GetComponent<RoverManager>();
        roverManager.OnControlSchemeChanged -= OnUpdateControlType;
    }
    public void OnMouseClick()
    {
        if (isOverUI())
        {
            Debug.Log("Clicked over UI");
            return;
        }
        if (ControlMethod != RoverManager.ControlMode.Auto)
        {
            return;
        }

        Ray MouseToWorld = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(MouseToWorld, out RaycastHit hit);
        MousePositionInWorld = hit.point;
        //agent.SetDestination(MousePositionInWorld);
        StartCoroutine(DoRotationAtTargetDirection(MousePositionInWorld));

    }
    public Vector3 MoveDirection;
    void Update()
    {
        switch (ControlMethod) 
        {
            case RoverManager.ControlMode.Manual:
                if(MoveDirection.magnitude > 0.1) agent.Move(MoveDirection * agent.speed * Time.fixedDeltaTime);
                break;
            case RoverManager.ControlMode.Auto:
                //if(!ActivelyTurning) StartCoroutine(DoRotationAtTargetDirection(MousePositionInWorld));
                break;
        }      
    }
    IEnumerator DoRotationAtTargetDirection(Vector3 destination)
    {
        Quaternion targetRotation = Quaternion.identity;
        do
        {
            targetDir = MousePositionInWorld - transform.position;
            whichWay = Vector3.Cross(transform.forward, targetDir.normalized).y;
            ActivelyTurning = true;
            Vector3 targetDirection = (destination - transform.position).normalized;
            targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, TurnSpeed * Time.deltaTime);

            yield return null;

        } while (Mathf.Abs(whichWay) > TurnThreshold && ControlMethod != RoverManager.ControlMode.Manual);
        if (ControlMethod == RoverManager.ControlMode.Manual) yield return null;
        Debug.Log("Done Rotating");
        ActivelyTurning = false;
        MoveToPoint(destination);
    }
    public void MoveToPoint(Vector3 destination)
    {
        DestinationSet = true;
        this.destination = destination;
        agent.updatePosition = true;
        Debug.Log(destination.ToString());
        agent.SetDestination(destination);
    }
    public void OnManualDrive(InputValue context)
    {
        if (ControlMethod != RoverManager.ControlMode.Manual) return;
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
        //Vector3 TargetRotation = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - TargetAngle, transform.rotation.eulerAngles.z);
        //transform.DORotate(TargetRotation, 0.5f, RotateMode.Fast);
        //transform.rotation = Quaternion.Euler(0, TargetAngle, 0);

        MoveDirection = Quaternion.Euler(0, TargetAngle, 0) * Vector3.forward;
    }



    private bool isOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
