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
	void EarlyUpdate()
	{

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

			trackedEyePosition -= new Vector3(screenHeight/2,screenHeight/2,screenHeight/2);
			Vector3 t = CalculateTranslationVector();
			translationVector = trackedEyePosition - transform.position;
			//Debug.Log(t.x +", "+ t.y +", "+ t.z);
		}
	}
	void LateUpdate()
	{
		FrustumDistortion(GetNearClipPlane(referenceCamera), CalculateTranslationVector());
		UpdateCameraPosition();
	}

	void UpdateCameraPosition(){
		//gameObject.transform.SetPositionAndRotation(gameObject.transform.position + translationVector, gameObject.transform.rotation);
		//Debug.Log("Translation Vector: " + translationVector);
		transform.position = mainCamera.transform.position + translationVector;
	}

	void GetScreenDimension(float inches, float aspectRatio)
	{
		float metres = inches * 0.0255f;
		screenWidth = metres * Mathf.Sin(Mathf.Atan(aspectRatio));
		screenHeight = metres * Mathf.Cos(Mathf.Atan(aspectRatio));
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
		
		Vector3 virtualEyeWorldCoordinates = new Vector3((fr+fl), (ft+fb), fn);
		Debug.Log("Vsw: " + virtualScreenWidth + "\nVsh: "+virtualScreenHeight + "\nVEwc: "+ virtualEyeWorldCoordinates + "\nVEsc: " + virtualEyeWorldCoordinates/virtualScreenWidth);
		return virtualEyeWorldCoordinates / virtualScreenWidth;
	}
	Vector3 CalculateTranslationVector()
	{
		Vector3 REsc = ScreenEyePosition(trackedEyePosition, screenHeight, screenWidth);
		Vector3 VEsc = VirtualEyePosition();
		float x = REsc.x / VEsc.x;
		float y = REsc.y / VEsc.y;
		float z = REsc.y / VEsc.z;
		Vector3 translation =  new Vector3(x, y, z);
		string msg = string.Format("REsc: {0}\nVEsc: {1}\nTranslation: ({2}, {3}, {4})", REsc, VEsc, translation.x, translation.y, translation.z);
		Debug.Log(msg);
		return translation;
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
		Matrix4x4 p = Matrix4x4.Frustum(DF[0].x, DF[3].x, DF[0].y, DF[1].y, DF[0].z, mainCamera.farClipPlane-translation.z);
		//Debug.Log("F:\n" + frustumCorners[0]+","+ frustumCorners[1]+","+ frustumCorners[2] + "DF:\n"+DF+ "Projeciton Matrix:\n" + p);
		mainCamera.projectionMatrix = p;
	}
}
