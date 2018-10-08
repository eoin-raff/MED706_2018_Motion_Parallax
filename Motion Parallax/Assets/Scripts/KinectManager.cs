using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class KinectManager : MonoBehaviour {

	private KinectSensor _sensor;
	private BodyFrameReader _bodyFrameReader;
	private Body[] _bodies = null;

	public GameObject kinectAvailableText;

	public static KinectManager instance = null;

	public Body[] GetBodies()
	{
		return _bodies;
	}

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}else if (instance != this)
		{
			Destroy(gameObject);	
		}
	}

	void Start()
	{

	}
	
	void Update () 
	{
		
	}
}
