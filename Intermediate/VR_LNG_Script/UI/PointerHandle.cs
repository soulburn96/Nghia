using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class PointerHandle : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;

    void Awake()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
       if (e.target.tag == "Button")
       {
            Debug.Log("Button was clicked");
            e.target.GetComponent<Button>().onClick.Invoke();
       }
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        if(e.target.tag == "Button")
        {
            e.target.GetComponent<ButtonHover>().hoverChangeImage();
            Debug.Log("hover");
        }
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        if (e.target.tag == "Button")
        {
            e.target.GetComponent<ButtonHover>().normalChangeImage();
            Debug.Log("non-hover");
        }
    }
}