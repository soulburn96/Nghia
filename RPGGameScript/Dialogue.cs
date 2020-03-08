using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{
    //giving the inspector text input
    [TextArea(3,10)]
    public string[] sentences; 
}
