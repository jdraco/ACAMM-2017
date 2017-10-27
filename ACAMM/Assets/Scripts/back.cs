using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//back function used for simple implimentation incase of lazy
public class back : MonoBehaviour {
	public string sceneName;
	// Use this for initialization
	void Start () {
		
	}
	
	void Update() {
		if (Input.GetKeyUp ("escape"))
			SceneManager.LoadSceneAsync (sceneName);
	}
}
