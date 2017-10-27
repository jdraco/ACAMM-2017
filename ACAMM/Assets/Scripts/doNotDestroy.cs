using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//yet another lazy script
public class doNotDestroy : MonoBehaviour {

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
}
