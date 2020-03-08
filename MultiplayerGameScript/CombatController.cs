using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof (PlayerStat))]
public class CombatController : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerStat myStat;
    private void Start()
    {
        myStat = GetComponent<PlayerStat>();
    }

    
}
