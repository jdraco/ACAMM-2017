	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Custom Child content size fitter.
/// allows to fit content size based on child or grandchild instead of direct content
/// </summary>
public class ChildContentSizeFitter : MonoBehaviour {
	public RectTransform thisObj;
	public RectTransform lastChild;

	public bool useGrandchild = false;
	public int grandChildParent = 0;
	public bool resizeAtStart = true;

	public float lWay = 50f;
	// Use this for initialization
	void Start () {
		if(resizeAtStart)
		reSize ();
	}
	
	public void reSize(){
		if (useGrandchild) {
			thisObj = this.transform.GetComponent<RectTransform> ();
			lastChild = this.transform.GetChild(grandChildParent).GetChild (this.transform.GetChild(grandChildParent).childCount - 1).GetComponent<RectTransform> ();
			thisObj.sizeDelta = new Vector2 (thisObj.sizeDelta.x, -lastChild.localPosition.y + (lastChild.rect.height/2*lastChild.localScale.y) + lWay);
		} else {
			thisObj = this.transform.GetComponent<RectTransform> ();
			lastChild = this.transform.GetChild (this.transform.childCount - 1).GetComponent<RectTransform> ();
			thisObj.sizeDelta = new Vector2 (thisObj.sizeDelta.x, -lastChild.localPosition.y + (lastChild.rect.height/2*lastChild.localScale.y) + lWay);
		}
		Debug.Log(-lastChild.localPosition.y +  "," + lastChild.sizeDelta.y);
	}
}
