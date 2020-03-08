using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ElementChoosing : MonoBehaviour
{
    // Start is called before the first frame update
    public SpawnerManager spawnerManager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ElementName()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        spawnerManager.element = EventSystem.current.currentSelectedGameObject.name;
		spawnerManager.CreatePlayerObject();
    }
}
