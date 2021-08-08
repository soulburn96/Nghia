using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsCastEvents : MonoBehaviour
{
    public UnityEvent onPhysicsCastHit;

    public void InvokeHitEvent()
    {
        if (onPhysicsCastHit != null)
            onPhysicsCastHit.Invoke();
    }
}
