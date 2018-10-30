using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class TranslateCamera : MonoBehaviour {

	public Camera mainCamera;
	public Camera referenceCamera;
	public float screenSizeInches;
	public float aspectRatioA, aspectRatioB;
    public bool kinectOnTop, sizeInInches;

    private float aspectRatio;
    private float screenWidth; //= 720cm
	private float screenHeight; // = 80.9

    private int index = 0;
    private GameObject eyes;
    //    private List<GameObject> allEyes;
    private GameObject[] allEyes;
    private Vector3 trackedEyePosition;

	void Start()
	{
        mainCamera.layerCullSpherical = true;
		Debug.Log("Starting camera translation script.");
		eyes = null;
        //allEyes = new List<GameObject>();
        allEyes = null;
		trackedEyePosition = Vector3.zero;
		aspectRatio = aspectRatioA/aspectRatioB;

        if(sizeInInches)
        {
            GetScreenDimension(screenSizeInches, aspectRatio);
        }
        else
        {
            //assuming using panoramic screen in SMILE lab.
            //285 inches?
            screenHeight = 0.89f;
            screenWidth = 7.195f;
        }
        Thread findCamPos = new Thread(GetCameraPosition);
        //Thread projectionMatrix = new Thread(FixNearClipPlane(mainCamera, trackedEyePosition));
	}

	void Update ()
	{
		if (eyes==null)
		{
			Debug.Log("Waiting for head position...");
            //eyes = GameObject.FindGameObjectWithTag("HeadPosition");
            allEyes = GameObject.FindGameObjectsWithTag("HeadPosition");
            eyes = allEyes[0];
        }
        else
		{
            if (Input.GetKeyDown(KeyCode.I))
            {
                index++;
                eyes = allEyes[index%allEyes.Length];
            }
            if (kinectOnTop)
            {
                trackedEyePosition = (eyes.transform.position * 0.1f) - new Vector3(0, screenHeight / 2, 0); //consider the Kinect as the center of the screen;
            }
            else
            {
                trackedEyePosition = (eyes.transform.position * 0.1f) + new Vector3(0, screenHeight / 2, 0); //consider the Kinect as the center of the screen;
            }
		}
    }

    void LateUpdate()
	{
        if (eyes != null)
        {
            GetCameraPosition();

//            Vector3 localCamPos = transform.InverseTransformPoint(transform.position);
  //          referenceCamera.nearClipPlane = -localCamPos.z;
            //Debug.Log(localCamPos); 
            /*FIXME:
            worked nicely using trackedEyeposition, but had too strong of a dolly-zoom when moving on z axis.
            also worked with eyes.transform.position, but was a very small frustum.
            NOT WORKING:
                localCamPos
                translationVector
             */
            FixNearClipPlane(mainCamera, trackedEyePosition);
        }

	}

	void GetScreenDimension(float inches, float aspectRatio)
	{
		float metres = inches * 0.0255f;
		screenWidth = metres * Mathf.Sin(Mathf.Atan(aspectRatio));
		screenHeight = metres * Mathf.Cos(Mathf.Atan(aspectRatio));
        Debug.Log("width: " + screenWidth + "\nheight: " + screenHeight);
	}

	void GetCameraPosition(){
        mainCamera.transform.position = trackedEyePosition;
//        FixNearClipPlane(mainCamera, trackedEyePosition);

  //mainCamera.transform.position = eyes.transform.position;
  //FixNearClipPlane(mainCamera, eyes.transform.position);
        
    }

    void FixNearClipPlane(Camera cam, Vector3 perspectiveOffset)
	{
		float left = (((0.5f * aspectRatio)+perspectiveOffset.x)/perspectiveOffset.z) * cam.nearClipPlane;
		float right = (((-0.5f * aspectRatio)+perspectiveOffset.x)/perspectiveOffset.z) * cam.nearClipPlane;
		float top = ((0.5f + -perspectiveOffset.y)/-perspectiveOffset.z)* cam.nearClipPlane;
		float bottom = ((-0.5f + -perspectiveOffset.y)/-perspectiveOffset.z)* cam.nearClipPlane;

		cam.projectionMatrix = GetObliqueProjectionMatrix(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
	}

	Matrix4x4 GetObliqueProjectionMatrix(float left, float right, float bottom, float top, float near, float far)
	{
		Matrix4x4 m = Matrix4x4.identity;

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
