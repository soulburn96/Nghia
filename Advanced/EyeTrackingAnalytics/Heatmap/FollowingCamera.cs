using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;

    void Update()
    {
        this.transform.position = cam.transform.position;
    }
}
