using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VarjoExample;

public enum Size
{
    veryLow,
    low,
    medium,
    high,
    veryHigh,
    ultra
};

public class ScanpathManager : MonoBehaviour
{
    [Header("Visual parameter")]

    [SerializeField] private float radiusCircleMin;
    [SerializeField] private float radiusCircleMax;
    [SerializeField] private float radiusLine;
   
    [SerializeField] private Size TexResolution = Size.low;

    private float[] pixel;

    private int u;
    private int v;
    private Vector2 pixelUV;
    private Vector2 previousPixelUV ;

    [SerializeField] private GameObject sphere;

    [SerializeField] private Transform commandBridge;

    [Header("Render Color")]
    [SerializeField] private Color color;
    [SerializeField] private Color firstObjectColor;
    [SerializeField] private Color lineColor;
    [SerializeField] private Color defautColor; 
    private Texture2D tex;

    private DetectEnvironment detector;
    private ScanpathDataLog scanpathDataLog;

    [Header("RayInfo")]
    [SerializeField] Camera cam;
    [SerializeField] float rayRadius = 0.01f;
    [SerializeField] float maxDistance = 10f;
    [SerializeField] LayerMask heatmapLayer;
    [SerializeField] private QueryTriggerInteraction collideToTriggers = QueryTriggerInteraction.Collide;

    RaycastHit hit;

    void Awake()
    {

        detector = GetComponent<DetectEnvironment>();
        scanpathDataLog = GetComponent<ScanpathDataLog>();

        switch (TexResolution)
        {
            case Size.veryLow:
                tex = new Texture2D(32, 16);
                break;
            case Size.low:
                tex = new Texture2D(64, 32);
                break;
            case Size.medium:
                tex = new Texture2D(128, 64);
                break;
            case Size.high:
                tex = new Texture2D(256, 128);
                break;
            case Size.veryHigh:
                tex = new Texture2D(512, 256);
                break;
            case Size.ultra:
                tex = new Texture2D(2048, 1080);
                break;


        }

        sphere.GetComponent<MeshRenderer>().material.mainTexture = tex;

        //create pixel variable  depending on size texture
        pixel = new float[tex.width * tex.height];

    }
    private void Start()
    {
        ResetValues();        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ResetValues();
            RenderScanpath();
            scanpathDataLog.Log(detector.CollectScanpathData());
        }
    }

    // cast Ray from contact point in environment to the sphere and render scanpath by sphere texture
    void RenderScanpath()
    {

        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {

                int i = x + tex.width * y;
                tex.SetPixel(x, y, defautColor);
            }
        }        
        

        Vector3 previousContactPoint = Vector3.zero;
        //raycast in the centerof viewport
        foreach (var point in detector.scanpathPointsList)
        {
            
            RaycastHit hit;

            if (Physics.Linecast(commandBridge.TransformPoint(point.contactPoint), cam.transform.position, out hit, heatmapLayer, collideToTriggers))
            {

                Renderer rend = hit.collider.GetComponent<Renderer>();
                MeshCollider meshCollider = hit.collider as MeshCollider;

                if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                {
                    continue;
                }

                pixelUV = hit.textureCoord;

                //coordinates in depending on height and width
                pixelUV.x *= tex.width;
                pixelUV.y *= tex.height;

                //check position of pixel
                tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);

                float radiusRatio = point.duration / detector.maxDuration;

                float radiusCirle = radiusRatio * (radiusCircleMax - radiusCircleMin) + radiusCircleMin;
                //choose radius of the circle
                float rSquared = radiusCirle * radiusCirle;

                //for each pixel of texture
                
                for (int u = (int)pixelUV.x - (int)radiusCirle; u < (int)pixelUV.x + (int)radiusCirle + 1; u++)
                    for (int v = (int)pixelUV.y - (int)radiusCirle; v < (int)pixelUV.y + (int)radiusCirle + 1; v++)

                        //create circle
                        if ((pixelUV.x - u) * (pixelUV.x - u) + (pixelUV.y - v) * (pixelUV.y - v) < rSquared)
                        {
                            int PixCurrent = u + tex.width * v;

                            if (previousContactPoint == Vector3.zero)
                                tex.SetPixel(u, v, firstObjectColor);
                            else
                                tex.SetPixel(u, v, color);
                        }

                // Render line between 2 points
                if (previousContactPoint != Vector3.zero)
                {
                    Vector3 lineVector = commandBridge.TransformVector(point.contactPoint - previousContactPoint);

                    float distance = Vector3.Distance(point.contactPoint, previousContactPoint);

                    for (float i = 0; i < distance; i+= 0.001f * distance)
                    {
                        Vector3 p =i * Vector3.Normalize(lineVector) + commandBridge.TransformPoint(previousContactPoint);

                        if (Physics.Linecast(p, cam.transform.position, out hit, heatmapLayer, collideToTriggers))
                        {

                            rend = hit.collider.GetComponent<Renderer>();
                            meshCollider = hit.collider as MeshCollider;

                            if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                            {
                                continue;
                            }

                            pixelUV = hit.textureCoord;

                            //coordinates in depending on height and width
                            pixelUV.x *= tex.width;
                            pixelUV.y *= tex.height;

                            //check position of pixel
                            tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);

                            //choose radius of the circle
                            float rSquaredLine = radiusLine * radiusLine;

                            //for each pixel of texture

                            for (int u = (int)pixelUV.x - (int)radiusLine; u < (int)pixelUV.x + (int)radiusLine + 1; u++)
                                for (int v = (int)pixelUV.y - (int)radiusLine; v < (int)pixelUV.y + (int)radiusLine + 1; v++)

                                    //create circle
                                    if ((pixelUV.x - u) * (pixelUV.x - u) + (pixelUV.y - v) * (pixelUV.y - v) < rSquared)
                                    {
                                        int PixCurrent = u + tex.width * v;
                                        tex.SetPixel(u, v, lineColor);
                                    }

                        }

                    }

                }
                previousContactPoint = point.contactPoint;
            }
            //apply texture
            tex.Apply();
        }        
    }


    private Vector2[] PointsBetween2Coordinate(Vector2 point1, Vector2 point2)
    {
        int amount = (int)Mathf.Pow(point1.x - point2.x, 2) + (int)Mathf.Pow(point1.y - point2.y,2);
        Vector2[] points = new Vector2[amount];
        float yDiff = point2.y - point1.y;
        float xDiff = point2.x - point1.x;
        float s = (point2.y - point1.y) / (point2.x - point1.x);
        float newX, newY;
        --amount;
 
        for(float i = 0; i < amount; i++)
        {
            newY = s == 0 ? 0 : yDiff * (i / amount);
            newX = s == 0 ? xDiff * (i / amount) : yDiff / s;
            points[(int)i] = new Vector2((int)Mathf.Round(newX) + point1.x, (int)Mathf.Round(newY) + point1.y);
        }
        points[amount] = point2;

        return points;

    }


    public void ResetValues()
    {
        for (int i = 0; i < tex.width * tex.height; i++)
        {
            pixel[i] = 0f;
        }

        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {

                int i = x + tex.width * y;
                tex.SetPixel(x, y, defautColor);
            }
        }
        tex.Apply();
    }
}