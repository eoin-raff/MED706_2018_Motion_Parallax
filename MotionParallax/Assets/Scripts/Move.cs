using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    [Range(0, 10)]
    public float speed;

    private bool moving = true;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        Debug.Log(moving);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            moving = !moving;
        }

        if (moving)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }

        if (!moving)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
	}
}
