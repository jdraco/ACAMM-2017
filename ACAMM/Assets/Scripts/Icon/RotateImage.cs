using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateImage : MonoBehaviour {
	RectTransform rectTransform;
	public float maxRotationValue = 7f;
	public float side = 1;
	public float rotationSpeed = 10f;
	// Use this for initialization
	float transformvalue = 0;
	void Start () {
		rectTransform = GetComponent<RectTransform>();
	}

	
	// Update is called once per frame
	void Update () {
		if (rectTransform.rotation.eulerAngles.z <= 180) {
			transformvalue = rectTransform.rotation.eulerAngles.z;
		}
		else if (rectTransform.rotation.eulerAngles.z > 180) {
			transformvalue = -(360-rectTransform.rotation.eulerAngles.z);
		}

		if (transformvalue > maxRotationValue)
			side = -1;
		else if (transformvalue < -maxRotationValue)
		{
			side = 1;
		}
		rectTransform.Rotate( new Vector3( 0, 0, rotationSpeed*Time.deltaTime*side ) );
	}
}
