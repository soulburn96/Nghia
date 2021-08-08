using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Hand))]
public class HandInteractionScript : MonoBehaviour
{
    [SerializeField] private bool disableHoverSpheresOnInteraction;

    private Hand hand;
    private bool subscribed;

    private float defaultHoverSphereRadius;
    private float defaultControllerHoverRadius;
    private float defaultFingerJointHoverRadius;

    void Start()
    {
        hand = GetComponent<Hand>();

        defaultHoverSphereRadius = hand.hoverSphereRadius;
        defaultControllerHoverRadius = hand.controllerHoverRadius;
        defaultFingerJointHoverRadius = hand.fingerJointHoverRadius;
    }

    void Update()
    {
        if (disableHoverSpheresOnInteraction == false)
            return;

        //Detach
        if (hand.currentAttachedObject == null && subscribed == true)
        {
            Detached();
        }

        //Attach
        if (hand.currentAttachedObject != null && subscribed == false)
        {
            Attached();
        }

    }

    void Attached()
    {
        subscribed = true;
        ToggleHandHover(false);
    }

    void Detached()
    {
        subscribed = false;
        ToggleHandHover(true);
    }

    void ToggleHandHover(bool toggleValue)
    {
        Debug.Log("HAND: " + gameObject.name + " HOVER: " + toggleValue);

        hand.useHoverSphere = toggleValue;
        hand.useControllerHoverComponent = toggleValue;
        hand.useFingerJointHover = toggleValue;

        if (toggleValue == true)
        {
            hand.hoverSphereRadius = defaultHoverSphereRadius;
            hand.controllerHoverRadius = defaultControllerHoverRadius;
            hand.fingerJointHoverRadius = defaultFingerJointHoverRadius;
        }
        else
        {
            hand.hoverSphereRadius = 0f;
            hand.controllerHoverRadius = 0f;
            hand.fingerJointHoverRadius = 0f;
        }
    }
}
