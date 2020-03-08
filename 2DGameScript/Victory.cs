using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Victory : MonoBehaviour
{
    public GameObject uiObject;
    public Text score;
    public int scenes;
    public Animator doorAnimator;
    public int nextLevel;
    // Start is called before the first frame update
    private void Start()
    {
        uiObject.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        var p = collider.gameObject.GetComponent<CharacterController>();

        
         if(score.text == "1")
         {

            uiObject.SetActive(true);
            PlayerPrefs.SetInt("levelReached",nextLevel);
            StartCoroutine(WaitForSec());
         }
    


        IEnumerator WaitForSec()
        {
            doorAnimator.SetBool("Open", true);
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(scenes);
            Destroy(uiObject);
        }
    }
}
