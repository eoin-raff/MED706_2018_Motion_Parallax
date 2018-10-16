using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetObliqueness : MonoBehaviour {

    public float horizObl, vertObl;
    private Camera _camera;
    private Matrix4x4 mat;

    void Start ()
    {
        _camera = GetComponent<Camera>();
        mat = _camera.projectionMatrix;
    }

    void Update()
    {
        mat[0, 2] = horizObl;
        mat[1, 2] = vertObl;
        _camera.projectionMatrix = mat;
    }
}
