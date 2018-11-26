using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndersParallax : MonoBehaviour {

    //Screen dimensions
    public int aspect_width, aspect_height, real_screen_inches;
    private int aspect_ratio;

    //Kinect
    private GameObject[] heads;
    private GameObject head;
    private Vector3 head_position;

    //Camera
    public Camera cam;
    private Matrix4x4 m;
    private float left, right, top, bottom, near, far;


	void Start () {
        #region Screen Dimensions
        //Calculate aspect ratio
        aspect_ratio = aspect_width / aspect_height;
        #endregion

        #region Kinect
        //Reset kinect bodies
        heads = null;
        head = null;
        #endregion

        #region Camera
        //Create static values for near and far
        near = cam.nearClipPlane;
        far = cam.farClipPlane;
        #endregion
    }

    void Update()
    {
        #region Kinect
        //Find kinect bodies
        heads = GameObject.FindGameObjectsWithTag("HeadPosition");
        head = heads[0];

        //Get the position of the tracked head joint
        head_position = head.transform.position;
        #endregion

        #region Camera
        //Move camera to virtual eyes
        cam.transform.position = head_position;

        cam.nearClipPlane = head_position.z;
        cam.farClipPlane = far;
        #endregion
    }

    // Update is called once per frame
    void LateUpdate () {
        //Start by resetting the projection matrix
        cam.ResetProjectionMatrix();

        //Get the frustum dimensions
        Get_Frustum_Dimensions(cam, aspect_ratio, head_position, cam.nearClipPlane);

        //Get new projection matrix based on custom coordinates
        cam.projectionMatrix = Get_Custom_Projection_Matrix(cam.nearClipPlane);
    }

    // Calculate the boundaries of the view frustum
    void Get_Frustum_Dimensions(Camera cam, int _aspect_ratio, Vector3 head, float _near)
    {
        left = ( (-0.5f * _aspect_ratio + head.x) / head.z ) * _near;
        right = ( (0.5f * _aspect_ratio + head.x) / head.z ) * _near;
        top = ( (0.5f * + head.y) / head.z ) * _near;
        bottom = ( (-0.5f * head.y) / head.z ) * _near;
    }


    // The oblique projection matrix object
    Matrix4x4 Get_Custom_Projection_Matrix(float _near)
    {

        float x = (2.0f * _near) / (right - left);
        float y = (2.0f * _near) / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = (_near + far) / (_near - far);
        float d = (2.0f * _near * far) / (_near - far);
        float e = -1.0f;

        m[0, 0] = x; m[0, 1] = 0.0f; m[0, 2] = a; m[0, 3] = 0.0f;
        m[1, 0] = 0.0f; m[1, 1] = y; m[1, 2] = b; m[1, 3] = 0.0f;
        m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = c; m[2, 3] = d;
        m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = e; m[3, 3] = 0.0f;

        return m;
    }
}
