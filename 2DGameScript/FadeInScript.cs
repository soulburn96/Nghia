using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInScript : MonoBehaviour
{
    public Image whiteFade;
    // Start is called before the first frame update
    void Start()
    {
        whiteFade.canvasRenderer.SetAlpha(1.0f);
    }

    // Update is called once per frame
    public void FadeIn()
    {
        whiteFade.CrossFadeAlpha(0, 2, false);
    }
   
}
