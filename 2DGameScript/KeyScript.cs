using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    [SerializeField] private AudioSource audioSrc;
    [SerializeField] private AudioClip pickupKeySound;
    // Start is called before the first frame update
    void Start()
    {
        KeyScore.keyAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        audioSrc.PlayOneShot(pickupKeySound);
        KeyScore.keyAmount += 1;
        Destroy(gameObject);
    }
}
