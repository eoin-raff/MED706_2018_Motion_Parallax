using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionParallax : MonoBehaviour {
    public float Z_MapFactor;
    public Camera mainCamera;
    public float screenSizeInches;
	public float aspectRatioA, aspectRatioB;
    public bool kinectOnTop, sizeInInches;

    //private float screenSizeInches;           // For use with startscreen
    //private int aspectRatioA, aspectRatioB;   // For use with startscreen
    private float aspectRatio;
    private float screenWidth; //= 720cm
	private float screenHeight; // = 80.9
    private bool lockZPosition = false;
    private bool mapZPosition = true;
    private float fixedZPosition;
    private float mappedZPosition;

    private int index = 0;
    private GameObject eyes;
    private GameObject[] allEyes;
    private Vector3 trackedEyePosition;
    private Vector3 verticalOffset;
    private float yOffset = 0;
    private Vector3 initialPosition = Vector3.zero;

    //FIXME: Objects in background clip weirdly and are extremely zoomed in

    void Start()
	{
        //mainCamera.layerCullSpherical = true;
		Debug.Log("Starting camera translation script.");
		eyes = null;
        allEyes = null;
		trackedEyePosition = Vector3.zero;
        verticalOffset = Vector3.zero;

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
        /*#region Input
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            yOffset+=0.01f;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            yOffset-=0.01f;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            MapZPos();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            LockZPos();
        }
        #endregion
        */
        ChangeCameraView();
    }



    void LateUpdate()
	{
        if (eyes != null)
        {
            mainCamera.ResetProjectionMatrix();
            GetWindowPosition(mainCamera, trackedEyePosition);
        }
    }


    void GetScreenDimension(float inches, float aspectRatio)
	{
		float metres = inches * 0.0255f;
		screenWidth = metres * Mathf.Sin(Mathf.Atan(aspectRatio));
		screenHeight = metres * Mathf.Cos(Mathf.Atan(aspectRatio));
	}
    private void MapZPos()
    {
        lockZPosition = false;
        mapZPosition = !mapZPosition;
        Debug.Log("switching to mapped Zpos");
    }

    private void LockZPos()
    {
        mapZPosition = false;
        lockZPosition = !lockZPosition;
        if (lockZPosition)
        {
            Debug.Log("switching Z-lock");
            fixedZPosition = trackedEyePosition.z;
        }
    }

    private void ChangeCameraView()
    {
        if (eyes == null)
        {
            InitalizeEyeObject();
        }
        else
        {
            GetUserEyePosition();

            MapEyePosition();

            mainCamera.transform.localPosition = trackedEyePosition;

        }
    }

    private void MapEyePosition()
    {
        trackedEyePosition = verticalOffset + (eyes.transform.position * 0.1f);

        float diff = trackedEyePosition.z - initialPosition.z;
        mappedZPosition = initialPosition.z + diff * Z_MapFactor;
        //print("tracked: " + trackedEyePosition.z + " inital: " + initialPosition.z + ", diff: " + diff);
        trackedEyePosition.z = mappedZPosition;
    }

    private void GetUserEyePosition()
    {
        if (initialPosition == Vector3.zero)
        {
            initialPosition = eyes.transform.position * .1f;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            print("changing perspective");
            index++;
            eyes = allEyes[index % allEyes.Length];
            initialPosition = eyes.transform.position * .1f;
        }
        verticalOffset = GameManager.instance.VerticalOffset;
    }

    private void InitalizeEyeObject()
    {
        Debug.Log("Waiting for head position...");
        allEyes = GameObject.FindGameObjectsWithTag("HeadPosition");
        eyes = allEyes[0];
        /*verticalOffset.y = -(eyes.transform.position.y * 0.1f) - (screenHeight / 2);

        if (kinectOnTop)
        {
            verticalOffset.y *= -1;
        }*/
        verticalOffset = GameManager.instance.VerticalOffset;
    }

    void GetWindowPosition(Camera cam, Vector3 perspectiveOffset)
	{      
        float left = cam.nearClipPlane * (-.5f * aspectRatio - perspectiveOffset.x) / Mathf.Abs(perspectiveOffset.z);
        float right = cam.nearClipPlane * (.5f * aspectRatio - perspectiveOffset.x) / Mathf.Abs(perspectiveOffset.z);
        float bottom = cam.nearClipPlane * (-.5f - perspectiveOffset.y) / Mathf.Abs(perspectiveOffset.z);
        float top = cam.nearClipPlane * (.5f - perspectiveOffset.y) / Mathf.Abs(perspectiveOffset.z);
        cam.projectionMatrix = CustomProjectionMatrix(left, right, bottom, top, cam.nearClipPlane, 100);
    }

    static Matrix4x4 CustomProjectionMatrix(float left, float right, float bottom, float top, float near, float far)
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
