using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateCamera : MonoBehaviour {

	public Camera mainCamera;
	public Camera referenceCamera;
	public float screenSizeInches;
	public float aspectRatioA;
	public float aspectRatioB;
	private float screenWidth;
	private float screenHeight;
	
	private GameObject eyes;
	private Vector3 trackedEyePosition;
	private Vector3 translationVector;
	private Vector3[] nearCorners;
	void Start()
	{
		mainCamera = Camera.main;
		Debug.Log("Starting camera translation script.");
		eyes = null;
		translationVector = Vector3.zero;
		trackedEyePosition = Vector3.zero;
		GetScreenDimension(screenSizeInches, (aspectRatioA/aspectRatioB));
	}

	void Update ()
	{
		if (eyes==null)
		{
			Debug.Log("Waiting for head position...");
			eyes = GameObject.FindGameObjectWithTag("HeadPosition");
			if (eyes!=null)
			{
				Debug.Log("Found user's head!");	
				trackedEyePosition = eyes.transform.position;	
			}
		}
		 else
		{
			trackedEyePosition = eyes.transform.position + new Vector3(0, screenHeight/2, 0);
			translationVector = CalculateTranslationVector();
		}
	}
	void LateUpdate()
	{
		FrustumDistortion(GetNearClipPlane(referenceCamera), translationVector);
		UpdateCameraPosition();
	}

	void UpdateCameraPosition(){
		//gameObject.transform.SetPositionAndRotation(gameObject.transform.position + translationVector, gameObject.transform.rotation);
		//Debug.Log("Translation Vector: " + translationVector);
		//transform.position = mainCamera.transform.position + translationVector;
		transform.position = CalculateTranslationVector();
		//transform.position = trackedEyePosition;
	}

	void GetScreenDimension(float inches, float aspectRatio)
	{
		float metres = inches * 0.0255f;
		screenWidth = metres * Mathf.Sin(Mathf.Atan(aspectRatio));
		screenHeight = metres * Mathf.Cos(Mathf.Atan(aspectRatio));
		Debug.Log("Screen Height: " + screenHeight + ", Screen Width: " + screenWidth);
	}

	Vector3 ScreenEyePosition(Vector3 trackedEyePosition, float screenHeight, float screenWidth)
	{
		//increment vector, presumably this means add value to all coordinates of V
		Vector3 worldEyePosition =new Vector3( trackedEyePosition.x + (screenHeight/2), trackedEyePosition.y + (screenHeight/2), trackedEyePosition.z + (screenHeight/2));
		Vector3 screenEyePosition = worldEyePosition / screenWidth;
		return screenEyePosition;
	}

	Vector3[] GetNearClipPlane(Camera cam)
	{
		Vector3[] corners = new Vector3[4];

		cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, corners);
		//Debug.Log("corners:\n " + corners[0]+" "+ corners[1]+" "+ corners[2]+" "+ corners[3]);
		return corners;
	}

	Vector3 VirtualEyePosition(){
		//FIXME
		nearCorners = GetNearClipPlane(referenceCamera);

		float fl = nearCorners[0].x;
		float fr = nearCorners[2].x;
		float ft = nearCorners[1].y;
		float fb = nearCorners[0].y;
		float fn = referenceCamera.nearClipPlane;

		float virtualScreenWidth = fr - fl;
		float virtualScreenHeight = ft - fb;
		
		Vector3 virtualEyeWorldCoordinates = new Vector3((virtualScreenWidth+fr+fl)/2, (virtualScreenHeight+ft+fb)/2, fn);
		Debug.Log("Vsw: " + virtualScreenWidth + "\nVsh: "+virtualScreenHeight + "\nVEwc: "+ virtualEyeWorldCoordinates + "\nVEsc: " + virtualEyeWorldCoordinates/virtualScreenWidth);
		return virtualEyeWorldCoordinates / virtualScreenWidth;
	}
	Vector3 CalculateTranslationVector()
	{
		float x, y, z;
		Vector3 REsc = ScreenEyePosition(trackedEyePosition, screenHeight, screenWidth);
		Vector3 VEsc = VirtualEyePosition();
		x = REsc.x - VEsc.x;
		y = REsc.y - VEsc.y;
		z = REsc.y - VEsc.z;
		Vector3 translation =  new Vector3(-x, Mathf.Clamp(y, -1, 1), z);
		Debug.Log(string.Format("REsc: {0}\nVEsc: {1}", REsc, VEsc));
		Debug.Log(string.Format("Translation: ({0}, {1}, {2})", translation.x, translation.y, translation.z));
		return translation;
	}

	void FrustumDistortion(Vector3[] frustumCorners, Vector3 translation)
	{
		//DF = F −Tvector
		//describe the new clip plane
		Vector3[] DF = new Vector3[4];
		for (int i = 0; i < frustumCorners.Length; i++)
		{
			DF[i] = frustumCorners[i] + translation.normalized;
		}
		Matrix4x4 p = Matrix4x4.Frustum(DF[0].x, DF[3].x, DF[0].y, DF[1].y, DF[0].z, referenceCamera.farClipPlane-translation.z);
		Debug.Log( "DF:\n"+DF[0]+","+DF[1]+","+DF[2]+","+DF[3]+","+ "Projeciton Matrix:\n" + p);
		mainCamera.projectionMatrix = p;
	}
}
