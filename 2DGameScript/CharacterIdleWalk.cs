using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIdleWalk : MonoBehaviour
{
    public float speed = 5f;
    public Animator idleAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Time.fixedDeltaTime * speed);
        idleAnimator.SetFloat("Speed", Mathf.Abs(speed));
    }
}
