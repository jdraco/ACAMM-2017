using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//another lazy script that you can use
public class disableGameObject : MonoBehaviour {
	#if UNITY_ANDROID
	#else
	// Use this for initialization
	void Start () {
		this.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	#endif
}
