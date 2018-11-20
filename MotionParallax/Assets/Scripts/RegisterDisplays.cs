using UnityEngine;
using System.Collections;

public class RegisterDisplays : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Debug.Log("Displays connected: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON.
        // Check if additional displays are available and activate each.
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }
}