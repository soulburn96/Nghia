using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadOnClick : MonoBehaviour
{
    public GameObject loadingImage;
    public int loadlevel;
    public void ChooseLevel()
    {
        SceneManager.LoadScene("MainMenu");
    }
    // Start is called before the first frame update
    public void LoadScene(int level)
    {
        
        SceneManager.LoadScene(level);
    }
}
