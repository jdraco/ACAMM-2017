using UnityEngine;
using System.Collections;

//Scales stuff according to resolution
public class ScaleToScreen : MonoBehaviour {
	public float xRatio = 16;
	public float yRatio = 9;
	Vector2 pRes = new Vector2 (0, 0);
	Vector2 cRes = new Vector2 (0, 0);
	bool LandScape = true;
	public Vector2 originalRes = new Vector2(1920,1080);
	//public GameObject buttons;
	// Use this for initialization
	void Start () {
		if (Screen.width > Screen.height) {
			DoScaleX ();
		} 
	}
	// Update is called once per frame
	void Update () {
		cRes = new Vector2 (Screen.width, Screen.height);
		if (pRes != cRes) {
			if(LandScape)
				DoScaleX();
			else
				DoScaleY();
		}
	}

	void DoScaleX()
	{
		this.transform.localScale = new Vector3 (1,1,1);
		Vector2 ratio = AspectRatio.GetAspectRatio(Screen.width, Screen.height);
		this.transform.localScale = new Vector3 (ScaleXtoY(ScaleXequalsY(this.transform.localScale.x),ratio),this.transform.localScale.y,1);
		pRes = new Vector2 (Screen.width, Screen.height);
		cRes = new Vector2 (Screen.width, Screen.height);
	}

	void DoScaleY()
	{
		this.transform.localScale = new Vector3 (1,1,1);
		Vector2 ratio = AspectRatio.GetAspectRatio(Screen.height,Screen.width);
		Vector2 ratio2 = AspectRatio.GetAspectRatio(Screen.width,Screen.height);
		this.transform.localScale = new Vector3 (ScaleXtoY(this.transform.localScale.x,ratio2),ScaleXtoY(this.transform.localScale.y,ratio2),1);
		this.transform.localScale = new Vector3 (ScaleXtoY(ScaleXequalsY(this.transform.localScale.x),ratio),this.transform.localScale.y,1);
		pRes = new Vector2 (Screen.width, Screen.height);
		cRes = new Vector2 (Screen.width, Screen.height);
	}

	public float ScaleToHeight(float value)
	{
		return (float)((value / originalRes.y) * Screen.height);
	}
	
	public float ScaleToWidth(float value)
	{
		return (float)((value / originalRes.x) * Screen.width);
	}


	public float ScaleXequalsY(float value)
	{
		return (value / originalRes.x * originalRes.y);
	}

	public float ScaleYequalsX(float value)
	{
		return (value / originalRes.y * originalRes.x);
	}

	public float ScaleXtoY(float value, Vector2 ratio)
	{
		return (value *(ratio.x/ratio.y));
	}

	public float ScaleYtoX(float value, Vector2 ratio)
	{
		return (value *(ratio.y/ratio.x));
	}
	//get screen to 1:1
	//then get it to actual ratio
}