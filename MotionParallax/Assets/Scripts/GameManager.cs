using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    private static int _scene = 0;
    private float _yOffset;
    private Vector3 _verticalOffset;
    [SerializeField]
    private GameObject[] _calibrationReference;
    [SerializeField]
    [Range(0, 1)]
    private float _zMapFactor = 0.25f;



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
        Application.targetFrameRate = 30;
        //initalize variables
        _verticalOffset = Vector3.zero;
        _verticalOffset = Vector3.zero;
    }

    void Update()
    {
        #region Calibration


        if (_calibrationReference == null)
        {
            print("calib = null");
            _calibrationReference = GameObject.FindGameObjectsWithTag("Calibration");
        }
        SetObjectsActive(_calibrationReference, false);
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            SetObjectsActive(_calibrationReference, true);
            CalibrateVerticalPosition();
        }


        #endregion

        #region Level Select

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadScene("Start Screen");
        }

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
            (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
            Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }

        #endregion
    }

    #region Calibration Methods

    private void SetObjectsActive(GameObject[] a, bool b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            a[i].SetActive(b);
        }
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

    #endregion

    #region Scene Management Methods

    private void LoadNextScene()
    {
        _calibrationReference = null;
        _scene++;
        SceneManager.LoadScene(_scene);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log("Load Scene " + sceneName);
    }
    #endregion

    #region Set/Get

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

    #endregion

}