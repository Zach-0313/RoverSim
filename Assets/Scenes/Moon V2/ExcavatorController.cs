using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ExcavatorController : MonoBehaviour
{
    [SerializeField] Transform FrontExcavator;
    [SerializeField] Transform RearExcavator;
    [SerializeField] float MaxRotation;
    [SerializeField] float MinRotation;
    public float RotationSpeed;

    [SerializeField] UnityEvent Excavate;

    public bool Lowering;
    public bool Raising;
    public Vector3 FrontRotation;
    public Vector3 RearRotation;
    bool R1, R2, L1, L2;
    public void Raise()
    {
        Raising = true;
        FrontRotation = FrontExcavator.localEulerAngles;
        if (Raise1(FrontRotation) || Raise2(FrontRotation))
        {
            FrontRotation.x = FrontRotation.x - (RotationSpeed);
            FrontExcavator.DOLocalRotate(FrontRotation, 1 / RotationSpeed, RotateMode.Fast);
        }
        RearRotation = RearExcavator.localEulerAngles;
        if (Raise1(RearRotation) || Raise2(RearRotation))
        {
            RearRotation.x = RearRotation.x - (RotationSpeed);
            RearExcavator.DOLocalRotate(RearRotation, 1 / RotationSpeed, RotateMode.Fast);
        }
        Raising = false;
    }

    private bool Raise1(Vector3 rot)
    {
        return (rot.x < MinRotation + RotationSpeed && MinRotation + RotationSpeed - rot.x > 0);  //When below the midline
    }
    private bool Raise2(Vector3 rot)
    {
        return (rot.x >= MinRotation && rot.x >= 360 - MaxRotation);  //When above the midline
    }
    private bool Lower1(Vector3 rot)
    {
        return (rot.x <= MinRotation && MinRotation - rot.x > 0);  //When below the midline
    }
    private bool Lower2(Vector3 rot)
    {
        return (rot.x > MinRotation - RotationSpeed && rot.x >= 360 - MaxRotation - RotationSpeed);  //When above the midline
    }
    public void Lower()
    {
        Lowering = true;
        FrontRotation = FrontExcavator.localEulerAngles;
        if (Lower1(FrontRotation) || Lower2(FrontRotation))
        {
            FrontRotation.x = FrontRotation.x + (RotationSpeed);
            FrontExcavator.DOLocalRotate(FrontRotation, 1 / RotationSpeed, RotateMode.Fast);
        }
        RearRotation = RearExcavator.localEulerAngles;
        if (Lower1(RearRotation) || Lower2(RearRotation))
        {
            RearRotation.x = RearRotation.x + (RotationSpeed);
            RearExcavator.DOLocalRotate(RearRotation, 1 / RotationSpeed, RotateMode.Fast);
        }
        Lowering = false;

    }
}
