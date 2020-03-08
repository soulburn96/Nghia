using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas canvas;
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            canvas.gameObject.SetActive(true);
            Debug.Log("Tab");
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            canvas.gameObject.SetActive(false);
        }
    }
}
