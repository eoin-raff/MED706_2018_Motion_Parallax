using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayScript : MonoBehaviour {


	void Start () {
        /* Activates all cameras currently in the scene,
         * each rendering to a different display.
         */

        Debug.Log("Displays connected: " + (Display.displays.Length - 1));
        int i = 1;
        while(Display.displays.Length > i)
        {
            Display.displays[i].Activate();
            Debug.Log("Found additional display " + i);
            i++;
        }
	}
}
