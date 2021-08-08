using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
public class BarrelTempeture : MonoBehaviour
{
    [SerializeField] private Text protectionBarrelTemperature;    
    [SerializeField] private LayerMask m_LayerMask;
    [SerializeField] private float temperature;
    [SerializeField] private float defaultTemperature =20f;
    [SerializeField] private float temperatureRate = 10f;

    public bool onFire;
    void Start()
    {
        onFire = false;
        temperature = defaultTemperature;
        StartCoroutine(IncreaseTemperature());
    }

    void FixedUpdate()
    {
        MyCollision();
    }

    void MyCollision()
    {

        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, m_LayerMask);
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        if (hitColliders.Length > 0)
        {
            onFire = true;
        }
        if (hitColliders.Length == 0)
        {
            onFire = false;
        }
        
    }
    IEnumerator IncreaseTemperature()
    {
        while (true)
        {
            if (onFire)
            {
                temperature += temperatureRate * Time.deltaTime;
            }
            if(!onFire && temperature> defaultTemperature)
            {
                temperature -= temperatureRate * Time.deltaTime;
            }
            protectionBarrelTemperature.text =  Convert.ToInt32(temperature).ToString()+ " C";
            yield return null;

        }
    }
}
