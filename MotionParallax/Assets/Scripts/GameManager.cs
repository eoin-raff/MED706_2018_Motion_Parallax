using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    private bool _calibrating;
    private bool _accessZMapFActor;
    private int _scene;
    private float _yOffset;
    private Vector3 _verticalOffset;
    [SerializeField]
    private float _zMapStep = 0.05f;
    [SerializeField][Range(0, 1)]
    private float _zMapFactor = 0.5f;

    private void Awake()
    {
        //Singleton
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //initalize variables
        _scene = 0;
        _calibrating = true;
        _accessZMapFActor = false;
        _verticalOffset = Vector3.zero;
    }

    void Update () {

        if (_calibrating)
        {
            CalibrateVerticalPosition();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _calibrating = false;
                Debug.Log("Done Calibrating, load next scene");
                LoadNextScene();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Debug.Log("Can Change Zmap");
            CalibrateZMapFactor();
        }
		
	}

    private void CalibrateZMapFactor()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Zmap Up");
            _zMapFactor += _zMapStep;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Zmap Down");

            _zMapFactor -= _zMapStep;
        }
        Mathf.Clamp(_zMapStep, 0, 1);

    }



    private void CalibrateVerticalPosition()
    {
        _yOffset = 0;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _yOffset = 0.01f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            _yOffset = -0.01f;
        }
        _verticalOffset.y += _yOffset;
    }

    private void LoadNextScene()
    {
        throw new NotImplementedException();
    }

    public Vector3 VerticalOffset
    {
        get { return _verticalOffset; }
        set { _verticalOffset = value; }
    }

    public float ZMapFactor
    {
        get { return _zMapFactor; }
        set { _zMapFactor = value; }
    }
}
