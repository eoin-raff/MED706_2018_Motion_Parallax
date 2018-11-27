using UnityEngine; using UnityEngine.UI; 
using System.Collections;

public class play_anim_on_ui_button : MonoBehaviour {
	public Button Text;
	public Animation anim;
	public Canvas yourcanvas;



	void Start () 
	{
		Text = Text.GetComponent<Button> ();
		anim = GetComponent<Animation>();
		yourcanvas.enabled = true;
	}


	public void Press() 

	{
		Text.enabled = true;
		GetComponent<Animation>().CrossFade ("LeftFrontDoorOpen");


	}
}