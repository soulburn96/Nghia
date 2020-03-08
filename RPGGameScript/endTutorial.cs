using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endTutorial : MonoBehaviour
{
    public void OnDestroy()
    {
        UIManager.Instance.Restart(); 
    }
}
