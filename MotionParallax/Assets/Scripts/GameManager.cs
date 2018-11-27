using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    private static int _scene = 0;
    private float _yOffset;
    private Vector3 _verticalOffset;
    [SerializeField]
    private GameObject[] _calibrationReference;
    [SerializeField]
    private float _zMapStep = 0.05f;
    [SerializeField]
    [Range(0, 1)]
    private float _zMapFactor = 0.5f;

    #region Z Test Variables
    private string _path;
    private string _filename;
    private bool _testing;
    private static int _participant = 0;
    private bool _staircaseTestA = true;
    private float _zA = 1.0f;
    private float _zB = 0.0f;
    private float _zTemp;
    private List<float> _aValues;
    private List<float> _bValues;
    [SerializeField]
    private int _checkedIterations = 3;
    private bool _changedDirection;
    private bool _finishedA;
    private bool _finishedB;
    private bool[] _checkChangesA;
    private bool[] _checkChangesB;

    #endregion


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
        _verticalOffset = Vector3.zero;
        _verticalOffset = Vector3.zero;
        _aValues = new List<float>();
        _bValues = new List<float>();
        _path = "D:\\Projects\\MED706_2018_Motion_Parallax\\Data"; // Path.Combine(Application.persistentDataPath, "CSV");
        _testing = false;
        _changedDirection = false;
        _finishedA = false;
        _finishedB = false;
        _checkChangesA = new bool[_checkedIterations * 2];
        _checkChangesB = new bool[_checkedIterations * 2];
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

        #region Staircase Test
        //TODO: 
        /* DONEAutomatically detect test end
            * Automatically detect if direction was changed
            * Test script
            */
        print("test");
        if (Input.GetKeyDown(KeyCode.T))
        {
            StopTest();
            /*
            Debug.Log("T");
            if (!_testing)
            {
                StartTest();
            }
            else
            {
                StopTest();
            }
            */
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            CalibrateZMapFactor();
        }

        print("Zmap: " + _zMapFactor + " \nA: " + _zA + "\nB: " + _zB);

        CheckForEndCondition();
        #endregion

        #region Level Select

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
            (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
            Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }

        #endregion
    }

    #region Staircase Test Methods
    private void CalibrateZMapFactor()
    {
        if (_staircaseTestA)
            _zTemp = _zA;
        else
            _zTemp = _zB;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Zmap Up");
            _zTemp += _zMapStep;
            SwitchStaircase();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Zmap Down");

            _zTemp -= _zMapStep;
            SwitchStaircase();
        }
        Mathf.Clamp(_zMapStep, 0, 1);
        _zMapFactor = _zTemp;
    }

    private void SwitchStaircase()
    {
        if (_staircaseTestA)
        {
            _zA = _zTemp;
            _aValues.Add(_zA);
        }
        else
        {
            _zB = _zTemp;
            _bValues.Add(_zB);
        }
        _staircaseTestA = !_staircaseTestA;
    }


    private void StartTest()
    {
        print("Start Test");
        _participant++;
        DateTime dt = DateTime.Now;
        string _date = dt.ToString("hh-dd-MM");
        _filename = "Z_Factor_" + _date + "_Participant_" + _participant + ".csv";
        _zA = 1.0f;
        _zB = 0.0f;
        _staircaseTestA = true;
        _testing = true;
    }

    private void StopTest()
    {
        _testing = false;
        PrintTestData();
        //load Post-Test-level
        _scene = 0;
        _filename = "";
        SceneManager.LoadScene(_scene);

    }

    private void PrintTestData()
    {
        string _dataA = "1.0";
        string _dataB = "0.0";
        for (int i = 0; i < _aValues.Count; i++)
        {
            _dataA += "," + _aValues[i].ToString("F3");
        }
        for (int i = 0; i < _bValues.Count; i++)
        {
            _dataB += "," + _bValues[i].ToString("F3");
        }
        StreamWriter csvWriter = File.CreateText(Path.Combine(_path, _filename));
        csvWriter.WriteLine(_dataA);
        csvWriter.WriteLine(_dataB);
        csvWriter.Close();
        Debug.Log("Printing to " + _path);
    }

    private void CheckForEndCondition()
    {
        SearchForFalse(_finishedA, _checkChangesA);
        SearchForFalse(_finishedB, _checkChangesB);
        if (_finishedA && _finishedB)
        {
            _testing = false;
            StopTest();
        }
    }

    private void SearchForFalse(bool a, bool[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == false)
                return;
        }
        a = true;
    }
    #endregion

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
        StartTest();
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