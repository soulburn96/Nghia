using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VarjoExample;

[System.Serializable]
// Point object to store infomation of the scanpath point
public class ScanpathPoint
{
    public string objectName;
    public VarjoGazeTarget targetObject;
    public Vector3 contactPoint;
    public float duration;
}

public class DetectEnvironment : MonoBehaviour
{
    // Start is called before the first frame update
    private ScanpathManager scan;

    [SerializeField] private TargetCollection collection;
    [SerializeField] private Transform commandBridge;

    [Header("Data")]
    public List<Vector3> hitPoints = new List<Vector3>();     
    public List<ScanpathPoint> scanpathPointsList = new List<ScanpathPoint>();

    [Header("Ray Info")]
    [SerializeField] Camera cam;
    [SerializeField] float rayRadius = 0.01f;
    [SerializeField] float maxDistance = 10f;
    [SerializeField] LayerMask environmentLayer;
    [SerializeField] private QueryTriggerInteraction collideToTriggers = QueryTriggerInteraction.Collide;
    [SerializeField] bool detecting = true;

    RaycastHit hit;

    public float maxDuration = 0;
    private float timer = 0;
    private Vector3 contactPoint = Vector3.zero;
    private VarjoGazeTarget currentTarget = null;

    private void Awake()
    {
        scan = GetComponent<ScanpathManager>();        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetectEnvironmentWithRayCast();
        DetectGazeTargetWithRayCast();
        Timer();
    }

    // Detect contact point in the environment
    void DetectEnvironmentWithRayCast()
    {
        if (!detecting)
            return;
        if (Physics.SphereCast(cam.transform.position, rayRadius, cam.transform.forward, out hit, maxDistance, environmentLayer.value, collideToTriggers))
        {
            hitPoints.Add(commandBridge.InverseTransformPoint(hit.point));            
        }
    }

    //Detect gaze target in the environment 
    void DetectGazeTargetWithRayCast()
    {
        if (!detecting)
            return;
        if (Physics.SphereCast(cam.transform.position, rayRadius, cam.transform.forward, out hit, maxDistance, environmentLayer.value, collideToTriggers))
        {
            VarjoGazeTarget hitTarget = hit.collider.GetComponent<VarjoGazeTarget>();
            if ( hitTarget == null)
                return;            

            if (currentTarget == null)
            {                
                timer = 0;
                currentTarget = hitTarget;
                contactPoint = commandBridge.InverseTransformPoint(hit.point);
            }
            // Add gaze target when player look to another gaze target
            if(hitTarget != currentTarget)
            {
                Add(currentTarget, timer, contactPoint);
                timer = 0;
                currentTarget = hitTarget;
                contactPoint = commandBridge.InverseTransformPoint(hit.point);
            }
        }
    }

    void Timer()
    {
        timer += Time.deltaTime;
    }

    void Add(VarjoGazeTarget target, float duration, Vector3 contactPoint)
    {
        ScanpathPoint currentPoint = new ScanpathPoint();
        currentPoint.objectName = target.gameObject.name;
        currentPoint.targetObject = target;
        currentPoint.duration = duration;
        currentPoint.contactPoint = contactPoint;
        scanpathPointsList.Add(currentPoint);
        if(duration> maxDuration)
        {
            maxDuration = duration;
        }
    }
    // Add list of scanpath point to a sequence to compared with other player
    public string CollectScanpathData()
    {
        string scanpathStringSequence = "";

        foreach(var point in scanpathPointsList)
        {
            scanpathStringSequence += point.objectName + "-";
        }
        return scanpathStringSequence;
    }
}
