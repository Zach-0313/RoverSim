using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RoverProfile", order = 1)]
public class RoverProfileSO : ScriptableObject
{
    [Header("Excavation Tweaks")]
    public float UpDownExcavatorSpeed = 0.2f;  //RoverSystems
    [Range(0, 5)]
    public float ExcavatorTrenchDepth = 2f;    //RoverSystems
    [Range(5, 25)]
    public float ExcacatorBuryDepth = 10f;     //RoverSystems
    public float ExcavationSizeX = 15f; //Excavator
    public float ExcavatoreSizeY = 25f; //Excavator
    [Range(0,3)]
    public float ExcavationSpeed = 2f;  //Excavator
    public float ExcavationThreshold = 0.5f;

    [Header("Movement Tweaks")]
    [Range(0, 10)]
    public float RoverForwardSpeed = 1f; //AgentMovement
    [Range(0, 5)]
    public float RoverTurnSpeed = 1f;    //AgentMovement
    public float RoverTurnThreshold = 0.1f; //AgentMovement
    [Header("Animation Tweaks")]
    public float WheelTurnSpeed = 0.25f;
    public float DrumTurnSpeed = 1f;


}