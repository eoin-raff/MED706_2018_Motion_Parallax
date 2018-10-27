using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour {

    public float speed = 5.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(transform.right * Time.deltaTime * speed);
        transform.Rotate(Vector3.up * Time.deltaTime * speed, Space.World);
    }
}
