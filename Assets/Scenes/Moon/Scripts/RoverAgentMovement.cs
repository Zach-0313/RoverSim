using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RoverAgentMovement : MonoBehaviour
{
    float FR_Drive;
    Animator animator;
    public NavMeshAgent agent;
    public RoverUI ui;
    public float TurnSpeed;
    public float whichWay;
    Vector3 RaycastPoint;
    public float TurnThreshold;
    Vector3 targetDir;
    public bool ActivelyTurning;
    public bool DestinationSet;
    public RoverProfileSO RoverProfileSO;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        animator.SetFloat("ExcavateInput", 1);
        destination = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        TurnSpeed = RoverProfileSO.RoverTurnSpeed;
        TurnThreshold = RoverProfileSO.RoverTurnThreshold;
        agent.speed = RoverProfileSO.RoverForwardSpeed;

        if (Input.GetMouseButtonDown(0) && !isOverUI())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.ResetPath();
                destination = hit.point;
                //agent.SetDestination(destination);
                agent.updatePosition = true;
                DestinationSet = false;
            }
        }
        targetDir = destination - transform.position;
        targetDir = targetDir.normalized;
        whichWay = Vector3.Cross(transform.forward, targetDir.normalized).y;

        if (agent.nextPosition != null)
        {
            if (Mathf.Abs(whichWay) > TurnThreshold) ActivelyTurning = true;
            else ActivelyTurning = false;
        }
        if (agent.isStopped)
        {
            Debug.Log("Agent has Stopped");
            animator.SetFloat("DriveTurning", 0);
            animator.SetFloat("DriveInput", 0);
        }
        if (!DestinationSet && ActivelyTurning)
        {
            StartCoroutine(DoRotationAtTargetDirection(destination));
        }
        if (!DestinationSet && !ActivelyTurning)
        {
            //MoveToPoint(destination);
        }
        //conform Rover to surface normals
    }
    Vector3 destination;
    public void MoveToPoint(Vector3 destination)
    {
        DestinationSet = true;
        this.destination = destination;
        agent.updatePosition = true;
        Debug.Log(destination.ToString());
        agent.SetDestination(destination);
    }
    void LateUpdate()
    {
        
        animator.SetFloat("DriveInput", agent.velocity.magnitude / agent.speed);
        animator.SetFloat("DriveTurning", whichWay);
    }
    IEnumerator DoRotationAtTargetDirection(Vector3 destination)
    {
        Quaternion targetRotation = Quaternion.identity;
        do
        {
            Vector3 targetDirection = (destination-transform.position).normalized;
            targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, TurnSpeed * Time.deltaTime);

            yield return null;

        } while (Mathf.Abs(whichWay) > TurnThreshold);
        Debug.Log("Done Rotating");
        ActivelyTurning = false;
        MoveToPoint(destination);
    }
    private bool isOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}