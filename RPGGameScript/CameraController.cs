using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform target;
     private Vector3 targetOffset = new Vector3(0,20,-20);
     private float movementSpeed =5f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }
    void MoveCamera()
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + targetOffset, movementSpeed * Time.deltaTime);
        }
    }
}
