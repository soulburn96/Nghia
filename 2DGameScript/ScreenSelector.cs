﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSelector : MonoBehaviour
{
    public Button[] levelButtons;
    // Start is called before the first frame update
    void Start()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if((i + 1) > levelReached)
            {
                levelButtons[i].interactable = false;
                levelButtons[i].GetComponent<Image>().color = Color.gray;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
