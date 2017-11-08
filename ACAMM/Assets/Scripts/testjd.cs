using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testjd : MonoBehaviour {
	public Scrollbar sb;
	int direction = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		sb.value += 0.002f * direction;
		if (sb.value >= 1)
			direction = -1;
		else if (sb.value <= 0)
			direction = 1;
	}
}
