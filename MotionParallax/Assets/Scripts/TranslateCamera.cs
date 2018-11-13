using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateCamera : MonoBehaviour {

	public Camera mainCamera;
    public float screenSizeInches;
	public float aspectRatioA, aspectRatioB;
    public bool kinectOnTop, sizeInInches;

    private float aspectRatio;
    private float screenWidth; //= 720cm
	private float screenHeight; // = 80.9
    private bool lockZPosition = false;
    private float fixedZPosition;

    private int index = 0;
    private GameObject eyes;
    private GameObject[] allEyes;
    private Vector3 trackedEyePosition;
    private Vector3 verticalAdjustment;


	void Start()
	{
        mainCamera.layerCullSpherical = true;
		Debug.Log("Starting camera translation script.");
		eyes = null;
        allEyes = null;
		trackedEyePosition = Vector3.zero;
        verticalAdjustment = Vector3.zero;

		aspectRatio = aspectRatioA/aspectRatioB;

        GetScreenDimension(screenSizeInches, aspectRatio);
        
        /*
        //assuming using panoramic screen in SMILE lab.
        //285 inches diagonal?
        //
        screenHeight = 0.89f;
        screenWidth = 7.195f;
        */
	}


	void Update ()
    {
        GetEyePosition();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            lockZPosition = !lockZPosition;
            if (lockZPosition)
            {
                Debug.Log("switching Z-lock");
                fixedZPosition = trackedEyePosition.z;
            }
        }
    }


    void LateUpdate()
	{
        if (eyes != null)
        {
            GetCameraPosition();
        }
	}


	void GetScreenDimension(float inches, float aspectRatio)
	{
		float metres = inches * 0.0255f;
		screenWidth = metres * Mathf.Sin(Mathf.Atan(aspectRatio));
		screenHeight = metres * Mathf.Cos(Mathf.Atan(aspectRatio));
	}

    private void GetEyePosition()
    {
        if (eyes == null)
        {
            Debug.Log("Waiting for head position...");
            allEyes = GameObject.FindGameObjectsWithTag("HeadPosition");
            eyes = allEyes[0];

            verticalAdjustment.y = -(eyes.transform.position.y * 0.1f) - (screenHeight / 2);

            if (kinectOnTop)
            {
                verticalAdjustment.y *= -1;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                print("changing perspective");
                index++;
                eyes = allEyes[index % allEyes.Length];
            }
            trackedEyePosition = verticalAdjustment + eyes.transform.position * 0.1f;
            if (lockZPosition)
            {
                trackedEyePosition.z = fixedZPosition;
            }
        }
    }

    void GetCameraPosition(){
        mainCamera.transform.position = trackedEyePosition;
        mainCamera.ResetProjectionMatrix();
        GetParallaxValues(mainCamera, trackedEyePosition);
    }


    void GetParallaxValues(Camera cam, Vector3 perspectiveOffset)
	{
		float left = (((-0.5f * aspectRatio) - perspectiveOffset.x) / -perspectiveOffset.z) * cam.nearClipPlane;
		float right = (((0.5f * aspectRatio) - perspectiveOffset.x) / -perspectiveOffset.z) * cam.nearClipPlane;
		float top = ((0.5f - perspectiveOffset.y) / -perspectiveOffset.z) * cam.nearClipPlane;
		float bottom = ((-0.5f  - perspectiveOffset.y) / -perspectiveOffset.z) * cam.nearClipPlane;

        //cam.projectionMatrix = GetObliqueProjectionMatrix(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane + perspectiveOffset.z);
        cam.projectionMatrix = PerspectiveOffCenter(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane, perspectiveOffset.z);
    }


    Matrix4x4 GetObliqueProjectionMatrix(float left, float right, float bottom, float top, float near, float far)
	{
		Matrix4x4 m = Matrix4x4.identity;
        float x = (2.0f * near)/ (right-left);
		float y = (2.0f * near) / (top-bottom);
		float a = (right + left) / (right - left);
		float b = (top + bottom) / (top - bottom);
		float c = -(far+near)/(far-near);
		float d = (-2.0f * near * far) / (far - near);
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

    static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far, float z)
    {
        float x = (2.0f * near) / (right - left);
        float y = (2.0f * near) / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0f * far * near) / (far - near);
        float e = -1.0f;

        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x;    m[0, 1] = 0.0f;     m[0, 2] = a;    m[0, 3] = 0.0f;
        m[1, 0] = 0.0f; m[1, 1] = y;        m[1, 2] = b;    m[1, 3] = 0.0f;
        m[2, 0] = 0.0f; m[2, 1] = 0.0f;     m[2, 2] = c;    m[2, 3] = d;
        m[3, 0] = 0.0f; m[3, 1] = 0.0f;     m[3, 2] = e;    m[3, 3] = 0.0f;
        return m;
    }
}
