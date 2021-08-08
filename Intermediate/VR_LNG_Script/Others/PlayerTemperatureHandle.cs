using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PlayerTemperatureHandle : MonoBehaviour
{

    [SerializeField]private float playerTemperature;
    [SerializeField]private Text playerTemperatureText;
    [SerializeField]private float rate = 0.5f;
    private bool insideFireRange;
    private void Start()
    {
        playerTemperature = 37f;
        insideFireRange = false;
        StartCoroutine(PlayerTemperatureRecover());
    }
    
    

    IEnumerator PlayerTemperatureRecover()
    {
        while (true)
        {
            if (!insideFireRange && playerTemperature > 37f)
            {
                playerTemperature -= Time.deltaTime * rate / 3;
                if (playerTemperature < 37f)
                {
                    playerTemperature = 37f;
                }
            }
            if (insideFireRange)
            {
                playerTemperature += Time.deltaTime * rate;
                Debug.Log("working");
            }
            playerTemperatureText.text = Convert.ToInt32(playerTemperature).ToString() + " C";
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            insideFireRange = true;
            Debug.Log(insideFireRange);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            insideFireRange = false;
            Debug.Log(insideFireRange);
        }
    }
}
