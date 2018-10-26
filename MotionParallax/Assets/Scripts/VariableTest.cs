using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableTest : MonoBehaviour {

    public GameObject inA;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        int input = ApplicationModel.aspectA;
        inA.GetComponent<Text>().text = input.ToString();
	}
}
