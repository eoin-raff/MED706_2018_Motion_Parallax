using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ChangeScenes
{
    //using this script for the buttons allows us to access the GM object in all scenes, instead of just the one in which it was instantated in.
   static GameManager GM = GameManager.instance;

    public static void ChangeScene(string sceneName) {
        GM.LoadScene(sceneName);
    }
}
