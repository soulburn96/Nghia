using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class WaterCurtainFX : MonoBehaviour
{
    // Start is called before the first frame update
    private bool waterCurtainOn;
    private VisualEffect waterVFX;
    private Vector3 curtainVelocityA = new Vector3(-10,0,15);
    private Vector3 curtainVelocityB = new Vector3(10, 10, 25);
    private Vector3 sprayVelocityA = new Vector3(-0.6f, -0.6f, 8);
    private Vector3 sprayVelocityB = new Vector3(-0.6f, -0.6f, 12);
    void Start()
    {
        waterCurtainOn = false;
        waterVFX = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SwitchWaterCurtain()
    {
        waterCurtainOn = !waterCurtainOn;
        Debug.Log("Switch");
        switch (waterCurtainOn)
        {
            case true:
                waterVFX.SetFloat("Drag", 5f);
                waterVFX.SetVector3("VelocityRandomA", curtainVelocityA);
                waterVFX.SetVector3("VelocityRandomB", curtainVelocityB);
                    break;
            case false:
                waterVFX.SetFloat("Drag", 0);
                waterVFX.SetVector3("VelocityRandomA", sprayVelocityA);
                waterVFX.SetVector3("VelocityRandomB", sprayVelocityB);
                    break;            
        }
    }
}
