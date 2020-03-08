using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreatScript : MonoBehaviour
{
    // Start is called before the first frame update
    public TimeRun timeRun;
    [SerializeField] private AudioSource audioSrc;
    [SerializeField] private AudioClip pickupTreatSound;

    void Start()
    {
        timeRun = FindObjectOfType<TimeRun>();

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        audioSrc.PlayOneShot(pickupTreatSound);
        timeRun.time += 5;
        timeRun.UpdateTime();
        Destroy(gameObject);
    }
}