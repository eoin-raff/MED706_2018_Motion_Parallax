using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateCamera : MonoBehaviour {

	public GameObject cam;

	private GameObject userHead;
	private Vector3 userHeadPos;
	private Vector3 translationVector;

	void Start()
	{
		Debug.Log("Starting camera translation script.");
		userHead = null;
		translationVector = Vector3.zero;
		userHeadPos = Vector3.zero;
	}
	void Update ()
	{
		if (userHead==null)
		{
			Debug.Log("Waiting for head position...");
			userHead = GameObject.FindGameObjectWithTag("HeadPosition");
			if (userHead!=null)
			{
				Debug.Log("Found user's head!");	
				userHeadPos = userHead.transform.position;	
			}
		}
		 else
		{
			userHeadPos = userHead.transform.position;	
			translationVector = transform.position - userHeadPos;
		}
		UpdateCameraPosition();
		
	}

	void UpdateCameraPosition(){
		cam.transform.position=userHeadPos;
	}
	public Vector3 GetTranlationVector(){
		return translationVector;
	}
}
