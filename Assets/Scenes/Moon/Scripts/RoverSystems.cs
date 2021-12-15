using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoverSystems : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private float Depth;
    [SerializeField] private float Angle;

    public enum ControlMode { Manual, Auto }
    public ControlMode ControlScheme;
    public TextMeshProUGUI DepthDisplay;
    public TextMeshProUGUI AngleDisplay;
    public float ExcavatorSpeed;
    bool Digging;
    public List<ParticleSystem> DigParticles;
    public Transform RaycastFrom;
    public UnityEvent DigTrench;
    public NavMeshAgent agent;
    public Toggle DrumToggle;
    public float MovingExcavationDepth;
    public float StationaryExcavationDepth;
    public RoverProfileSO RoverProfileSO;
    Vector3 Destination;
    // Start is called before the first frame update
    void Start()
    {
        SetDrums();
        StopRover();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        ExcavatorSpeed = RoverProfileSO.UpDownExcavatorSpeed;
        MovingExcavationDepth = RoverProfileSO.ExcavatorTrenchDepth;
        StationaryExcavationDepth = RoverProfileSO.ExcacatorBuryDepth;
        animator.SetFloat("_WheelSpeed", RoverProfileSO.WheelTurnSpeed);
        animator.SetFloat("_DrumSpeed", RoverProfileSO.DrumTurnSpeed);

        GetDepth();
        if (Digging)
        {
            Dig();
            ActivateParticles();
        }
        if (Lower) LowerSides();
        if (Raise) RaiseSides();

        if (animator.GetBool("DrumToggleLock") == false) return;

        if (!agent.hasPath && (animator.GetFloat("ExcavateInput") <= RoverProfileSO.ExcavationThreshold) && Lower)
        {

            RaycastFrom.GetComponent<Excavator>().ExcavationMaxDepth = StationaryExcavationDepth;
            RaycastFrom.GetComponent<Excavator>().Excavate();
        }
        else
        {
            RaycastFrom.GetComponent<Excavator>().ExcavationMaxDepth = MovingExcavationDepth;
        }
        if (agent.hasPath && (animator.GetFloat("ExcavateInput") <= RoverProfileSO.ExcavationThreshold))
        {
            RaycastFrom.GetComponent<Excavator>().Excavate();
        }
    }
    public void SetDrums()
    {
        bool a = DrumToggle.isOn;
        animator.SetBool("DrumToggleLock", a);
        animator.SetBool("DrumsActive", a);
    }
    void LateUpdate()
    {
        DepthDisplay.text = Depth.ToString();
        AngleDisplay.text = Angle.ToString();
    }
    public void StopRover()
    {
        agent.ResetPath();
    }

    public void RaiseSides()
    {

        Debug.Log("Raised sides");
        float currentValue = animator.GetFloat("ExcavateInput");
        float newValue = currentValue + ExcavatorSpeed * Time.deltaTime;
        newValue = Mathf.Clamp(newValue, 0, 1);
        animator.SetFloat("ExcavateInput", newValue);
    }
    public void LowerSides()
    {
        Debug.Log("Lowered sides");

        float currentValue = animator.GetFloat("ExcavateInput");
        if (animator.GetBool("DrumToggleLock") == false && currentValue <= 0.7f) return;
        float newValue = currentValue - ExcavatorSpeed * Time.deltaTime;
        newValue = Mathf.Clamp(newValue, 0, 1);
        animator.SetFloat("ExcavateInput", newValue);
    }

    public bool Raise, Lower;
    public void LowerSidesToggle(bool a)
    {
        Lower = a;
    }
    public void RaiseSidesToggle(bool a)
    {
        Raise = a;
    }

    public void GetDepth()
    {
        Ray DrillRay = new Ray(RaycastFrom.position, Vector3.down);
        RaycastHit DrillResult;
        if (Physics.Raycast(DrillRay, out DrillResult, 1000f))
        {
            Depth = DrillResult.distance;

            Angle = Quaternion.FromToRotation(-RaycastFrom.up, DrillResult.normal).z;
            Angle *= Mathf.Rad2Deg;
        }
    }
    public void ToggleDig(bool value)
    {
        Digging = value;
    }
    public float ExcavatorPosition()
    {
        return animator.GetFloat("ExcavateInput");
    }
    public void ActivateParticles()
    {
        foreach (ParticleSystem a in DigParticles)
        {
            a.gameObject.SetActive(true);
            a.Play();
        }
    }
    public void Dig()
    {
        Debug.Log("Trying to Dig");
        Ray ray = new Ray(RaycastFrom.position, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction);
        if (Physics.Raycast(ray, out RaycastHit hit, 15f))
        {
            if (hit.collider.GetComponent<Terrain>())
            {
                RaycastFrom.GetComponent<Excavator>().Excavate();
                float currentValue = animator.GetFloat("ExcavateInput");
                float newValue = currentValue - ExcavatorSpeed * Time.deltaTime;
                newValue = Mathf.Clamp(newValue, 0, 1);
                animator.SetFloat("ExcavateInput", newValue);
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(Destination, 1);
    }
}