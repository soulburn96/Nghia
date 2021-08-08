using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleComponents : MonoBehaviour
{
    [SerializeField] private bool startEnabled;
    [SerializeField] private List<Component> components = new List<Component>();

    void Start()
    {
        Toggle(startEnabled);    
    }

    public void Toggle(bool toggleValue)
    {
        foreach (Component component in components)
        {
            Transform comTransform = component as Transform;
            if (comTransform != null)
                continue;

            Collider comCollider = component as Collider;
            if (comCollider != null)
            {
                comCollider.enabled = toggleValue;
                continue;
            }

            Renderer comRenderer = component as Renderer;
            if (comRenderer != null)
            {
                comRenderer.enabled = toggleValue;
                continue;
            }
        }
    }
}
