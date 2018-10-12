using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateCamera : MonoBehaviour {

	/*TODO:
		lock near clip plane on X and Y axes
	 */
	public Camera mainCamera;
	public Camera referenceCamera;
	public float screenSizeInches;
	public float aspectRatio, aspectRatioA, aspectRatioB;
	private float screenWidth;
	private float screenHeight;
	private Matrix4x4 m;
	private float windowHeight, windowWidth;

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
		aspectRatio = aspectRatioA/aspectRatioB;
		GetScreenDimension(screenSizeInches,aspectRatio);

		m = referenceCamera.projectionMatrix;
		windowWidth = 2*referenceCamera.nearClipPlane/m[0, 0];
		windowHeight = 2*referenceCamera.nearClipPlane/m[1, 1];
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
				trackedEyePosition = eyes.transform.position * 0.1f;
			}
		}
		 else
		{
			trackedEyePosition = (eyes.transform.position*0.1f) - new Vector3(0, screenHeight/2, 0); //consider the Kinect as the center of the screen;
			translationVector = CalculateTranslationVector();
		}
	}

	void LateUpdate()
	{
		UpdateCameraPosition();

		Vector3 localCamPos = transform.InverseTransformPoint(transform.position);
		referenceCamera.nearClipPlane = -localCamPos.z;
		//Debug.Log(localCamPos); 
		/*FIXME:
		worked nicely using trackedEyeposition, but had too strong of a dolly-zoom when moving on z axis.
		also worked with eyes.transform.position, but was a very small frustum.
		NOT WORKING:
			localCamPos
			translationVector
		 */
		FixNearClipPlane(mainCamera, trackedEyePosition);

		//FixNearClipPlane(GetCorners(referenceCamera, referenceCamera.nearClipPlane), eyes.transform.position);
	}

	void GetScreenDimension(float inches, float aspectRatio)
	{
		float metres = inches * 0.0255f;
		screenWidth = metres * Mathf.Sin(Mathf.Atan(aspectRatio));
		screenHeight = metres * Mathf.Cos(Mathf.Atan(aspectRatio));
	}

	void UpdateCameraPosition(){
		mainCamera.transform.position = eyes.transform.position;
		//mainCamera.transform.position = trackedEyePosition;
		//DONT USE THIS CODE
		//mainCamera.nearClipPlane = -mainCamera.transform.position.z;
		//mainCamera.farClipPlane = referenceCamera.farClipPlane - mainCamera.transform.position.z ;
	}

	Vector3 ScreenEyePosition(Vector3 trackedEyePosition, float screenHeight, float screenWidth)
	{
		Vector3 worldEyePosition =new Vector3(trackedEyePosition.x, trackedEyePosition.y - (screenHeight/2), trackedEyePosition.z);
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
		nearCorners = GetCorners(referenceCamera, referenceCamera.nearClipPlane);

		float fl = nearCorners[0].x;
		float fr = nearCorners[2].x;
		float ft = nearCorners[1].y;
		float fb = nearCorners[0].y;
		float fn = referenceCamera.nearClipPlane;

		float virtualScreenWidth = fr - fl;
		float virtualScreenHeight = ft - fb;
		
		Vector3 virtualEyeWorldCoordinates = new Vector3((virtualScreenWidth+fr+fl)/2, (virtualScreenHeight+ft+fb)/2, fn);
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
		Vector3 translation = new Vector3(x, y, z);
		return translation;
	}
	void FixNearClipPlane(Camera cam, Vector3 perspectiveOffset)
	{
		//TODO: lock x y values of
		//based on Javascript code from Unity Forum
		/*/
		float left = -windowWidth/2 - perspectiveOffset.x;
		float right = left+windowWidth;
		float bottom = -windowHeight/2 - perspectiveOffset.y;
		float top = bottom+windowHeight;
		Debug.Log(string.Format("l: {0}, r:{1}, b:{2}, t: {3}\nww: {4}, wh: {5}", left, right, bottom, top, windowWidth, windowHeight));
		*/
		float left = (((0.5f * aspectRatio)+perspectiveOffset.x)/perspectiveOffset.z) * cam.nearClipPlane;
		float right = (((-0.5f * aspectRatio)+perspectiveOffset.x)/perspectiveOffset.z) * cam.nearClipPlane;
		float top = ((0.5f + -perspectiveOffset.y)/-perspectiveOffset.z)* cam.nearClipPlane;
		float bottom = ((-0.5f + -perspectiveOffset.y)/-perspectiveOffset.z)* cam.nearClipPlane;
		cam.projectionMatrix = GetObliqueProjectionMatrix(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
	}
	void FixNearClipPlane(Vector3[] frustum, Vector3 translation)
	{
		mainCamera.nearClipPlane = -mainCamera.transform.position.z;
		Vector3[] DF = new Vector3[4];

		float l = -screenWidth/2 - translation.x;
		float r = l + screenWidth;//screenWidth/2;//frustum[2].x - translation.x;
		float b =  -screenHeight/2;//frustum[0].y - translation.y;
		float t = b + screenHeight - translation.y;//screenHeight/2;//frustum[1].y - translation.y;
		float n = Mathf.Abs(mainCamera.transform.position.z);// - translation.z;
		float f = referenceCamera.farClipPlane - Mathf.Abs(mainCamera.transform.position.z);
		Matrix4x4 p = GetObliqueProjectionMatrix(l, r, b, t, n, f);
		mainCamera.projectionMatrix = p;
	}

	Matrix4x4 GetObliqueProjectionMatrix(float left, float right, float bottom, float top, float near, float far)
	{
		Matrix4x4 m = Matrix4x4.identity;
		//float x = 1.0f / (screenWidth/near);
		//float y = (aspectRatioA/aspectRatioB)/ (screenHeight/near);
		float x = (2.0f * near)/ (right-left);
		float y = (2.0f * near) / (top-bottom);
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
		m[3, 3] = 0.0f;

		return m;
	}
}
