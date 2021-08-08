using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedActivator : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField][Range(0f, 20f)] private float activationDelay = 1f;

    [Header("Events")]
    public UnityEvent onActivation;

    private Coroutine activationProcess;

    public void StartActivationProcess()
    {
        if (activationProcess != null)
            StopCoroutine(activationProcess);

        activationProcess = StartCoroutine(ActivationProcess());
    }

    public void CancelActivationProcess()
    {
        if (activationProcess == null)
            return;

        StopCoroutine(activationProcess);
    }

    IEnumerator ActivationProcess()
    {
        yield return new WaitForSeconds(activationDelay);

        if (onActivation != null)
            onActivation.Invoke();
    }
}
