using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCaster : MonoBehaviour
{
    [SerializeField] private Transform sphereCastSpawn;
    [SerializeField] private float sphereCastDistance = 2f;
    [SerializeField] private float sphereCastRadius = 0.5f;
    [SerializeField] private LayerMask triggeringLayers;
    [SerializeField] private QueryTriggerInteraction collideToTriggers = QueryTriggerInteraction.Ignore;

    private Coroutine castLoop;

    public void ToggleCaster(bool toggleValue)
    {
        if (castLoop != null)
            StopCoroutine(castLoop);

        if (toggleValue == true)
            castLoop = StartCoroutine(CastLoop());
    }

    IEnumerator CastLoop()
    {
        while (true)
        {
            CheckForContacts();
            yield return new WaitForFixedUpdate();
        }
    }

    void CheckForContacts()
    {
        RaycastHit sphereHit;
        if (Physics.SphereCast(sphereCastSpawn.position, sphereCastRadius, sphereCastSpawn.forward, out sphereHit, sphereCastDistance, triggeringLayers, collideToTriggers))
        {
            PhysicsCastEvents physicsCastEvents = sphereHit.collider.GetComponent<PhysicsCastEvents>();

            if (physicsCastEvents != null)
                physicsCastEvents.InvokeHitEvent();
        }
    }
}
