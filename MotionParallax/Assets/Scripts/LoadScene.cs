﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{

    //This script may seem redundant, but it allows us to access the GM object in all scenes, instead of just the one in which it was instantated in.
    GameManager GM;
    
    void Start()
    {
        GM = GameManager.instance;
    }

    public void ChangeScene(string sceneName)
    {

        GM.LoadScene(sceneName);
    }
}
