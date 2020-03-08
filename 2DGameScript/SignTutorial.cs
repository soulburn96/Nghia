using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignTutorial : MonoBehaviour
{
    public GameObject uiObject;
    // Start is called before the first frame update
    private void Start()
    {
        uiObject.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        var p = collider.gameObject.GetComponent<CharacterController>();
        uiObject.SetActive(true);
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        uiObject.SetActive(false);
    }
}
