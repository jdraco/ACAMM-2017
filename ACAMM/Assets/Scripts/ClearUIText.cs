using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearUIText : MonoBehaviour {
	public UnityEngine.UI.Text log;

	public void ClearLog(){
		log.text = "";
	}
}
