using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        print("triggered");
        if (other.gameObject.tag == "Destroy")
        {
            print("BOOM");
            Destroy(other.gameObject);
        }
        
    }
}
