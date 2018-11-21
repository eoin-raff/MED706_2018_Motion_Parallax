using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager GM;
    public MotionParallax motionParallax;
    public GameObject[] calibrationObjects;

    private bool _calibrating = true;
    private bool _isRunning = false;
    private bool _isPaused = false;
    private Vector3 _verticalOffset = Vector3.zero;
    private float _yOffset;
    [SerializeField][Range(0, 1)]
    private float _zMapFactor;
    [SerializeField][Range(0.05f, 0.1f)]
    private float _zMapStep;
    private List<float> _zMapData = new List<float>();

    private void Awake()
    {
        if (GM == null)
        {
            DontDestroyOnLoad(gameObject);
            GM = this;
        }
    }
	
	
    void Update () {

        if (_calibrating)
        {
            _isRunning = false;
            CalibrateVerticalPosition();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Calibration Done");
                ToggleObjects(calibrationObjects, true);
                _calibrating = false;
            }
        }
        if (_isRunning)
        {
            //run the game as normal
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Pause();
            }
            CalibrateZMapFactor(_zMapStep);
        }
	}

    private void CalibrateZMapFactor(float v)
    {
        /*
         * This should allow the zMap factor to be changed in the test, and store the changes in a list.
         * This list should be printed to a csv file for each participant so that the data can be graphed.
         */
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _zMapFactor += _zMapStep;
            _zMapData.Add(_zMapFactor);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _zMapFactor -= _zMapStep;
            _zMapData.Add(_zMapFactor);
        }
    }

    private void Pause()
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    private void CalibrateVerticalPosition()
    {
        Debug.Log("Calibrating");
        _yOffset = 0;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _yOffset = 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _yOffset = -0.1f;
        }
        _verticalOffset.y += _yOffset;
    }

    private void ToggleObjects(GameObject[] a, bool b)
    {
        for (int i = 0; i < a.Length-1; i++)
        {
            a[i].SetActive(b);
        }
    }

    public Vector3 verticalOffset
    {
        get { return _verticalOffset; }
        set { _verticalOffset = value; }
    }

    public bool isRunning
    {
        get { return _isRunning; }
        set { _isRunning = value; }
    }
}
