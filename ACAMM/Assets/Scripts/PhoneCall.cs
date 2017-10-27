using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneCall : MonoBehaviour {
	//public string phoneID;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MakeCall(string phoneID){
		Application.OpenURL ("tel://" + phoneID);
	}
}
