using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inherit_Camera : MonoBehaviour {

    public int number_of_cameras;
    public float shiftLeft, shiftRight;
    [Range(-2, 2)] public float shiftL, shiftR;

    private bool isDefined;
    public float originalZ;
    public float difference;

    //The multiple cameras
    Camera this_cam;

    //The parallax parent camera
    GameObject parent;
    Camera parent_cam;
    MotionParallax parent_mp;

	// Use this for initialization
	void Start () {
        isDefined = false;
        this_cam = GetComponent<Camera>();

        parent = GameObject.Find("ParallaxCamera");         //Parent parallax cam
        parent_cam = parent.GetComponent<Camera>();         //Its camera component
        parent_mp = parent.GetComponent<MotionParallax>();  //The parallax script from parent cam
    }
	
	// Update is called once per frame
	void Update () {
        this_cam.fieldOfView = parent_cam.fieldOfView;
        this_cam.aspect = (parent_mp.aspectRatioA / number_of_cameras) / parent_mp.aspectRatioB;

        /*Get initial z value
        if (!isDefined && parent_mp.trackedEyePosition.z != 0)
        {
            originalZ = parent_mp.trackedEyePosition.z;
            isDefined = true;
        }*/
	}

    void LateUpdate()
    {
        this_cam.ResetProjectionMatrix();
        GetWindowPosition(parent_cam, parent_mp.trackedEyePosition);
        this_cam.projectionMatrix = parent_cam.projectionMatrix;
    }

    void GetWindowPosition(Camera cam, Vector3 perspectiveOffset)
    {
        shiftRight = .5f;
        shiftLeft = -0.5f;

        float left = cam.nearClipPlane * (shiftLeft * cam.aspect - perspectiveOffset.x) / Mathf.Abs(perspectiveOffset.z);
        float right = cam.nearClipPlane * (shiftRight * cam.aspect - perspectiveOffset.x) / Mathf.Abs(perspectiveOffset.z);
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
        m[0, 0] = x; m[0, 1] = 0.0f; m[0, 2] = a; m[0, 3] = 0.0f;
        m[1, 0] = 0.0f; m[1, 1] = y; m[1, 2] = b; m[1, 3] = 0.0f;
        m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = c; m[2, 3] = d;
        m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = e; m[3, 3] = 0.0f;

        return m;
    }

}
