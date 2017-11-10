using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pdfTab : MonoBehaviour {
	public pdfManager pdfm;
	public dbTypes.Presentation presentation;
	public Dictionary<int, string> pageImage;
	public Button btn;
	public Text title;
	public int value = 0;
	// Use this for initialization
	void Start () {
		this.transform.localScale = new Vector3 (1, 1, 1);
		this.name = presentation.title;
		title.text = presentation.title;
		btn.onClick.AddListener (() => pdfm.enterpdf (value));
	}

}
