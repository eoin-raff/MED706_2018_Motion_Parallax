using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScenes : MonoBehaviour
{
	public int loadLevelIndex;
    public GameObject inputA;
    public GameObject inputB;
    public GameObject inputD;

    public void Change()
    {
		SceneManager.LoadScene(loadLevelIndex);     // Change for when implemented!  
    }

    public void ChangeSmile()
    {
        ApplicationModel.aspectA = 80;
        ApplicationModel.aspectB = 9;
        ApplicationModel.diagonal = 285.0f;

		SceneManager.LoadScene(loadLevelIndex);      // Change for when implemented!
    }

    public void ChangeAspectA()
    {
        string inputText = inputA.GetComponent<InputField>().text;
        ApplicationModel.aspectA = int.Parse(inputText);
    }

    public void ChangeAspectB()
    {
        string inputText = inputB.GetComponent<InputField>().text;
        ApplicationModel.aspectB = int.Parse(inputText);
    }

    public void ChangeAspectD()
    {
        string inputText = inputD.GetComponent<InputField>().text;
        ApplicationModel.diagonal = float.Parse(inputText);
    }
}

public class ApplicationModel
{
    static public int aspectA = 0;
    static public int aspectB = 0;
    static public float diagonal = 0;
}
