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
				trackedEyePosition = eyes.transform.position * 0.1f; //times 0.1 to convert to metres
			}
		}
		 else
		{
			trackedEyePosition = (eyes.transform.position*0.1f) - new Vector3(0, screenHeight/2, 0); //consider the Kinect as the center of the screen;

		//	Debug.Log(string.Format("Eyes: ({0}, {1}, {2})\nAdjusted Eyes: ({3}, {4}, {5})",eyes.transform.position.x,eyes.transform.position.y,eyes.transform.position.z, trackedEyePosition.x, trackedEyePosition.y, trackedEyePosition.z));
			translationVector = CalculateTranslationVector();
		}
	}
	void LateUpdate()
	{
		FrustumDistortion(GetCorners(referenceCamera, referenceCamera.nearClipPlane), translationVector);
		UpdateCameraPosition();
	}

	void UpdateCameraPosition(){
		//TODO: Figure out which is best for camera movement.

		//transform.position = referenceCamera.transform.position + CalculateTranslationVector();
		//transform.position = trackedEyePosition;
		transform.position = eyes.transform.position;
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

	Vector3[] GetCorners(Camera cam, float cameraClipPlane)
	{
		Vector3[] corners = new Vector3[4];

		cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cameraClipPlane, Camera.MonoOrStereoscopicEye.Mono, corners);
		return corners;
	}

	Vector3 VirtualEyePosition(){
		//FIXME
		nearCorners = GetCorners(referenceCamera, referenceCamera.nearClipPlane);

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
		x = REsc.x / VEsc.x;
		y = REsc.y / VEsc.y;
		z = REsc.y / VEsc.z;
		Vector3 translation = new Vector3(x, y, -z);
		//Debug.Log(string.Format("REsc: {0}\nVEsc: {1}", REsc, VEsc));
		//Debug.Log(string.Format("Translation: ({0}, {1}, {2})", translation.x, translation.y, translation.z));
		return translation;
	}

	void FrustumDistortion(Vector3[] frustum, Vector3 translation)
	{
		//DF = F −Tvector
		//describe the new clip plane
		Vector3[] DF = new Vector3[4];
		for (int i = 0; i < frustum.Length; i++)
		{
			DF[i] = frustum[i] - translation;
		}
	//	Matrix4x4 p = Matrix4x4.Frustum(DF[0].x, DF[3].x, DF[0].y, DF[1].y, DF[0].z, referenceCamera.farClipPlane - translation.z);
		float l = -screenWidth/2;//frustum[0].x - translation.x;
		float r = screenWidth/2;//frustum[2].x - translation.x;
		float b =  -screenHeight/2;//frustum[0].y - translation.y;
		float t = screenHeight/2;//frustum[1].y - translation.y;
		float n = frustum[0].z - translation.z;
		float f = referenceCamera.farClipPlane - translation.z;
		Matrix4x4 p = GetProjectionMatrix(l, r, b, t, n, f);
		//Debug.Log( "DF:\n"+DF[0]+","+DF[1]+","+DF[2]+","+DF[3]+","+ "Projeciton Matrix:\n" + p);
		mainCamera.projectionMatrix = p;
	}

	Matrix4x4 GetProjectionMatrix(float left, float right, float bottom, float top, float near, float far)
	{
		Matrix4x4 m = Matrix4x4.identity;
		float x = 2.0f * near / (right - left);
		float y = 2.0f * near / (top-bottom);
		float a = (right + left) / (right - left);
		float b = (top + bottom) / (top - bottom);
		float c = -(far + near) / (far - near);
		float d = -(2.0f * far * near) / (far - near);
		float e = -1.0f;

		m[0, 0] = x;
		m[0, 2] = a;
		m[1, 1] = y;
		m[1, 2] = b;
		m[2, 2] = c;
		m[2, 3] = d;
		m[3, 2] = e;
		m[3, 3] = 0;

		return m;
	}
}
