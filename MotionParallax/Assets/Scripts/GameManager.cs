using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    private int _scene;
    private float _yOffset;
    private Vector3 _verticalOffset;
    [SerializeField]
    private GameObject[] _calibrationReference;
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
        _verticalOffset = Vector3.zero;
    }

    void Update () {
        SetObjectsActive(_calibrationReference, false);
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            SetObjectsActive(_calibrationReference, true);
            CalibrateVerticalPosition();
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Debug.Log("Can Change Zmap");
            CalibrateZMapFactor();
        }

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
            (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))&&
            Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }

    }

    private void SetObjectsActive(GameObject[] a, bool b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            a[i].SetActive(b);
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
        _scene++;
        SceneManager.LoadScene(_scene);
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
