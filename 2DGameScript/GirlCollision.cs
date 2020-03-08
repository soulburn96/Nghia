using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GirlCollision : MonoBehaviour
{
    public GameObject canvas;
    public GameObject scene;
    public int sceneNum;
    public GameObject Girl;

    // Start is called before the first frame update
    void Start()
    {
        canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        canvas.SetActive(true);
        FindObjectOfType<FadeInScript>().FadeIn();
        scene.SetActive(false);
        GameObject.Find("Girl").transform.localScale = new Vector3(0, 0, 5);
        StartCoroutine(WaitForSec());

        IEnumerator WaitForSec()
        {
            yield return new WaitForSeconds(13);
            SceneManager.LoadScene(sceneNum);
        }
    }
}
