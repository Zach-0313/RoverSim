using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivateOnCollision : MonoBehaviour
{
    public GameObject CollisonTarget;
    public UnityEvent CollidedWithTarget;

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.Equals(CollisonTarget))
        {
            CollidedWithTarget.Invoke();
            Debug.Log($"{gameObject.name} Collided with Target");
        }
    }
}
