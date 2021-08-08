using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LockObject : MonoBehaviour
{
    public bool restoreTransformOnLock;
    public bool restoreTransformOnUnlock;

    public UnityEvent onObjectLocked;
    public UnityEvent onObjectUnlocked;

    private Vector3 startPosition;
    private Quaternion startRotaion;
    private bool locked;

    private Coroutine lockLoop;

    void Start()
    {
        startPosition = transform.localPosition;
        startRotaion = transform.localRotation;
    }

    public void Lock()
    {
        locked = true;

        if (restoreTransformOnLock == true)
        {
            transform.localPosition = startPosition;
            transform.localRotation = startRotaion;
        }

        if (lockLoop != null)
            StopCoroutine(lockLoop);

        lockLoop = StartCoroutine(LockLoop(transform.position, transform.rotation));
    }

    public void Unlock()
    {
        locked = false;

        if (restoreTransformOnUnlock == true)
        {
            transform.localPosition = startPosition;
            transform.localRotation = startRotaion;
        }
    }

    IEnumerator LockLoop(Vector3 lockedPosition, Quaternion lockedRotation)
    {
        while (locked == true)
        {
            transform.position = lockedPosition;
            transform.rotation = lockedRotation;

            yield return null;
        }
    }
}
