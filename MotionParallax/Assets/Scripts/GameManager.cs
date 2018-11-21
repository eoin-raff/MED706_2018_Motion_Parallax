using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    private bool _calibrating;
    private int _scene;
    private float _yOffset;
    private Vector3 _verticalOffset;

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

        _scene = 0;
        _calibrating = true;
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
		
	}

    private void LoadNextScene()
    {
        throw new NotImplementedException();
    }

    private void CalibrateVerticalPosition()
    {
        _yOffset = 0;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Debug.Log("Up");
            _yOffset = 0.01f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("Down");
            _yOffset = -0.01f;
        }
        _verticalOffset.y += _yOffset;
        Debug.Log(_verticalOffset);
    }

    public Vector3 VerticalOffset
    {
        get { return _verticalOffset; }
        set { _verticalOffset = value; }
    }
}
