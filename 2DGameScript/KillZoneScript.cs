using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZoneScript : MonoBehaviour
{
    public Transform spawnPoint;
    [SerializeField] private AudioClip restartSound;
    [SerializeField] private AudioSource audioSrc;


    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.transform.CompareTag("Player"))
            collision.transform.position = spawnPoint.position;
            audioSrc.PlayOneShot(restartSound);
    }
}
