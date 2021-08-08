using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;
public class HeatMapGenerate : MonoBehaviour {

    [Header("Visual parameter")]

	[Range(1, 10)]
    [SerializeField] private int radiusInfluence;

    [SerializeField] private Size TexResolution = Size.low;

	private float[] pixel;
   
	private float maxPixel;
    
	private int u;
	private int v;
	private Vector2 pixelUV;

    [SerializeField] private GameObject sphere;

    [SerializeField] private Transform commandBridge;

    [SerializeField] private Gradient gradientFinal;

	private Texture2D tex;

    private DetectEnvironment detector;

    [Header("RayInfo")]
    [SerializeField] Camera cam;
    [SerializeField] float rayRadius = 0.01f;
    [SerializeField] float maxDistance = 10f;
    [SerializeField] LayerMask heatmapLayer;
    [SerializeField] private QueryTriggerInteraction collideToTriggers = QueryTriggerInteraction.Collide;

    RaycastHit hit;

    void Awake (){

        detector = GetComponent<DetectEnvironment>();

		switch (TexResolution){
			case Size.veryLow:
				tex = new Texture2D(32,16);
				break;
			case Size.low:
				tex = new Texture2D(64,32);
				break;
			case Size.medium:
				tex = new Texture2D(128,64);
				break;
			case Size.high:
				tex = new Texture2D(256,128);
				break;
			case Size.veryHigh:
				tex = new Texture2D(512,256);
				break;
            case Size.ultra:
                tex = new Texture2D(2048,1080);
                break;

            
		}

		sphere.GetComponent<MeshRenderer>().material.mainTexture = tex;

		//create pixel variable  depending on size texture
		pixel = new float[tex.width*tex.height];

	}


    void Start (){
        ResetValues();
	}

	void Update() {
        DrawListHeatmap();
	}

    public void ResetValues()
    {
        print("RESET");
        //value of pixels 0 
        for (int i = 0; i < tex.width * tex.height; i++)
        {
            pixel[i] = 0f;
        }
        maxPixel = 0f;
    }

    void DrawListHeatmap()
    {

        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                
                int i = x + tex.width * y;                
                //take the highest value of the pixels
                if (pixel[i] > maxPixel)
                {
                    maxPixel = pixel[i];
                }
                Color colorUpdate = gradientFinal.Evaluate(pixel[i] / maxPixel);
                tex.SetPixel(x, y, colorUpdate);              
            }
        }
   
        //raycast in the centerof viewport
        foreach(var point in detector.hitPoints)
        {
            RaycastHit hit;

            if(Physics.Linecast(commandBridge.TransformPoint( point), cam.transform.position, out hit, heatmapLayer, collideToTriggers))
            {
                Debug.DrawLine(cam.transform.position, point, Color.green);

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

                //choose radius of the circle
                float rSquared = radiusInfluence * radiusInfluence;

                //for each pixel of texture
                for (int u = (int)pixelUV.x - (int)radiusInfluence; u < (int)pixelUV.x + (int)radiusInfluence + 1; u++)
                    for (int v = (int)pixelUV.y - (int)radiusInfluence; v < (int)pixelUV.y + (int)radiusInfluence + 1; v++)

                        //create circle
                        if ((pixelUV.x - u) * (pixelUV.x - u) + (pixelUV.y - v) * (pixelUV.y - v) < rSquared)
                        {
                            //edit the value of the pixel, adding deltatime and making a gradient from the center
                            int PixCurrent = u + tex.width * v;
                            pixel[PixCurrent] += Time.deltaTime * (1f - ((pixelUV.x - u) * (pixelUV.x - u) + (pixelUV.y - v) * (pixelUV.y - v)) / radiusInfluence * 0.05f);
                        }
            }
            
        }
        //apply texture
        tex.Apply();
    }

    void DrawHeatmap()
    {
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                int i = x + tex.width * y;

                //take the highest value of the pixels
                if (pixel[i] > maxPixel)
                {
                    maxPixel = pixel[i];
                }
                Color colorUpdate = gradientFinal.Evaluate(pixel[i] / maxPixel);
                tex.SetPixel(x, y, colorUpdate);
            }
        }

        //raycast in the centerof viewport

        Ray ray = cam.ViewportPointToRay(new Vector3(1, 1, 0));
        
        Physics.Raycast(ray, out hit,heatmapLayer);

        Debug.Log(hit.collider.name);

        //apply hit to renderer
        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
            return;

        //take coordinates of hit
        pixelUV = hit.textureCoord;

        //coordinates in depending on height and width
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;

        //check position of pixel
        tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);

        //choose radius of the circle
        float rSquared = radiusInfluence * radiusInfluence;

        //for each pixel of texture
        for (int u = (int)pixelUV.x - (int)radiusInfluence; u < (int)pixelUV.x + (int)radiusInfluence + 1; u++)
            for (int v = (int)pixelUV.y - (int)radiusInfluence; v < (int)pixelUV.y + (int)radiusInfluence + 1; v++)

                //create circle
                if ((pixelUV.x - u) * (pixelUV.x - u) + (pixelUV.y - v) * (pixelUV.y - v) < rSquared)
                {
                    //edit the value of the pixel, adding deltatime and making a gradient from the center
                    int PixCurrent = u + tex.width * v;
                    pixel[PixCurrent] += Time.deltaTime * (1f - ((pixelUV.x - u) * (pixelUV.x - u) + (pixelUV.y - v) * (pixelUV.y - v)) / radiusInfluence * 0.05f);
                }

        //apply texture
        tex.Apply();
    }
}