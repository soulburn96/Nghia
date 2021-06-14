using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour,  IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private UnityEvent ButtonDown;
    [SerializeField] private UnityEvent ButtonUp;


    public void OnPointerDown(PointerEventData eventData)
    {
        ButtonDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ButtonUp.Invoke();
    }
    
}
