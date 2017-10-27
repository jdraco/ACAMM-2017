using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//debug menu stuff
public class menuDebug : MonoBehaviour {
	bool debug = false;
	public GameObject debugMenu;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void enableOrDisableDebug(){
		debug = !debug;
		debugMenu.SetActive (debug);
	}


}
