using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateCamera : MonoBehaviour {

	public Camera mainCamera;
	public Camera referenceCamera;
	public float screenWidth;
	public float screenHeight;

	
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
			trackedEyePosition	 = eyes.transform.position;
			translationVector = CalculateTranslationVector();
		}
		UpdateCameraPosition();
	}

	void UpdateCameraPosition(){
		mainCamera.transform.SetPositionAndRotation(mainCamera.transform.position + translationVector, mainCamera.transform.rotation);
		FrustumDistortion(GetNearClipPlane(), translationVector);
	}

	Vector3 ScreenEyePosition(Vector3 trackedEyePosition, float screenHeight, float screenWidth)
	{
		//increment vector, presumably this means add value to all coordinates of V
		Vector3 worldEyePosition =new Vector3( trackedEyePosition.x + (screenHeight/2), trackedEyePosition.y + (screenHeight/2), trackedEyePosition.z + (screenHeight/2));
		Vector3 screenEyePosition = worldEyePosition / screenWidth;
		return screenEyePosition;
	}

	Vector3[] GetNearClipPlane()
	{
		Vector3[] corners = new Vector3[4];

		referenceCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), mainCamera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, corners);

		return corners;
	}

	Vector3 VirtualEyePosition(){
		nearCorners = GetNearClipPlane();

		referenceCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), mainCamera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearCorners);

		float fl = nearCorners[0].x;
		float fr = nearCorners[1].x;
		float ft = nearCorners[0].y;
		float fb = nearCorners[2].y;
		float fn = nearCorners[0].z;

		float virtualScreenWidth = fr - fl;
		float virtualScreenHeight = ft - fb;
		
		Vector3 virtualEyeWorldCoordinates = new Vector3((fr+fl)/2, (ft+fb)/2, fn);
		return virtualEyeWorldCoordinates / virtualScreenWidth;
	}
	Vector3 CalculateTranslationVector()
	{
		Vector3 REsc = ScreenEyePosition(trackedEyePosition, screenHeight, screenWidth);
		Vector3 VEsc = VirtualEyePosition();
		float x = REsc.x / VEsc.x;
		float y = REsc.y / VEsc.z;
		float z = REsc.y / VEsc.z;
		return new Vector3(x, y, z);
	}

	void FrustumDistortion(Vector3[] frustumCorners, Vector3 translation)
	{
		//DF = F −Tvector
		//describe the new clip plane
		Vector3[] DF = new Vector3[4];
		for (int i = 0; i < frustumCorners.Length; i++)
		{
			DF[i] = frustumCorners[i] - translation;
		}
		//Matrix4x4 p = mainCamera.CalculateObliqueMatrix(distortedFrustumCorners); //v4
		Matrix4x4 p = Matrix4x4.Frustum(DF[0].x, DF[1].x, DF[2].y, DF[0].y, DF[0].z, referenceCamera.farClipPlane-translation.z);
		mainCamera.projectionMatrix = p;
	}
}
