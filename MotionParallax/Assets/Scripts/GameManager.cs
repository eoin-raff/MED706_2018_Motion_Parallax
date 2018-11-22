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
    private int _scene;
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
    private static int _participant;
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
        _scene = 0;
        _verticalOffset = Vector3.zero;
        _participant = 0;
        _verticalOffset = Vector3.zero;
        _aValues = new List<float>();
        _bValues = new List<float>();
        _path = Application.dataPath + "/CSV/";
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("T");
            if (!_testing)
            {
                StartTest();
            }
            else
            {
                StopTest();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            CalibrateZMapFactor();
        }

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

    #region Z Map Test
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
            Mathf.Clamp(_zTemp, 0, 1);
            SwitchStaircase();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Zmap Down");

            _zTemp -= _zMapStep;
            Mathf.Clamp(_zTemp, 0, 1);

            SwitchStaircase();
        }
    }

    private void SwitchStaircase()
    {
        if (_staircaseTestA)
        {
            Debug.Log("add val to A");
            _zA = _zTemp;
            _aValues.Add(_zA);
        }
        else
        {
            Debug.Log("add val to B");
            _zB = _zTemp;
            _bValues.Add(_zB);
        }
        Debug.Log("Changing Staircase");
        _staircaseTestA = !_staircaseTestA;
    }


    private void StartTest()
    {
        Debug.Log("Start Test");
        _participant++;
        DateTime dt = DateTime.Now;
        String _date = dt.ToString("dd-MM");
        _filename = "Z_Factor_" + _date + "_Participant_" + _participant + ".csv";
        _zA = 1.0f;
        _zB = 0.0f;
        _staircaseTestA = true;
        _testing = true;
    }

    private void StopTest()
    {
        Debug.Log("End Test, printing .csv");
        PrintTestData();
        _testing = false;
        //load Post-Test-level
    }

    private void PrintTestData()
    {
        String _dataA = "";
        String _dataB = "";
        for (int i = 0; i < _aValues.Count; i++)
        {
            _dataA += _aValues[i].ToString("F2") + ",";
        }
        for (int i = 0; i < _bValues.Count; i++)
        {
            _dataB += _bValues[i].ToString("F2") + ",";
        }
        StreamWriter csvWriter = File.CreateText(_path + "/" + _filename);
        csvWriter.WriteLine(_dataA);
        csvWriter.WriteLine(_dataB);
        csvWriter.Close();
    }

    private void CheckForEndCondition()
    {
        SearchForFalse(_finishedA, _checkChangesA);
        SearchForFalse(_finishedB, _checkChangesB);
        if (_finishedA && _finishedB)
        {
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
        Debug.Log("no false values found");
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
            Debug.Log("Y up");
            _yOffset = 0.01f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("Y down");
            _yOffset = -0.01f;
        }
        _verticalOffset.y += _yOffset;
    }

    #endregion

    #region Scene Management Methods

    private void LoadNextScene()
    {
        _scene++;
        SceneManager.LoadScene(_scene);
        throw new NotImplementedException();
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
