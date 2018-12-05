using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{

    //using this script for the buttons allows us to access the GM object in all scenes, instead of just the one in which it was instantated in.
    GameManager GM;
    
    void Start()
    {
        GM = GameManager.instance;
    }

    public void ChangeScene(string sceneName)
    {
        /*
        if (sceneName == "A" || sceneName == "B")
        {
            //center screen
            Camera.main.targetDisplay = 1;
            Screen.SetResolution(1920, 1080, true);
        }
        else if (sceneName == "C" || sceneName == "D")
        {
            //far left screen
            Camera.main.targetDisplay = 5;
            Screen.SetResolution(9600, 1080, false);
        }*/
        GM.LoadScene(sceneName);
    }
}
