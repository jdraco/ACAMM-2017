using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this is the places of interest manager
public class poiManager : MonoBehaviour {
	public l_sceneManager lsm;
	public GameObject POI_Content;
	public GameObject POI_Select;
	public GameObject Day_Select;
	public GameObject[] POISEL_LIST;
	public GameObject[] POI_LIST;
	public string[] POISEL_HLIST;
	public string[] POI_HLIST;
	public int selectedDay = -1;
	public int selectedPOI = -1;
	public Text header;
	public ChildContentSizeFitter ccsf;
	//daySelect = selection of which day
	//poiSelect = selection of which places of interest data to view
	//inPoi = currently viewing a place of interest
	enum poiState{
		daySelect,
		poiSelect,
		inPoi
	}

	poiState pState = poiState.daySelect;

	void Start () {
		
	}

	//calls back function if back button is pressed
	void Update() {
		if (Input.GetKeyUp ("escape"))
			back ();
	}

	//view place of interest
	public void enterPOI(int v)
	{
		POI_LIST [v].SetActive (true);
		POI_Content.SetActive (true);
		POISEL_LIST [selectedDay].SetActive (false);
		POI_Select.SetActive (false);
		header.text = POI_HLIST [v];
		ccsf.grandChildParent = v;
		ccsf.reSize ();
		pState = poiState.inPoi;
		selectedPOI = v;
	}

	//stop view place of interest
	void exitPOI(){
		POI_LIST [selectedPOI].SetActive (false);
		Vector2 ctPos = ccsf.GetComponent<RectTransform> ().anchoredPosition;
		ctPos.y = 840;
		ccsf.GetComponent<RectTransform> ().anchoredPosition = ctPos;
		POI_Content.SetActive (false);
		POISEL_LIST [selectedDay].SetActive (true);
		POI_Select.SetActive (true);
		header.text = "Places of Interest";
		pState = poiState.poiSelect;
		selectedPOI = -1;
	}

	//enter a places of interest selection screen based on which day is chosen
	public void enterPOISel(int v)
	{
		POISEL_LIST [v].SetActive (true);
		POI_Select.SetActive (true);
		Day_Select.SetActive (false);
		header.text = POISEL_HLIST [v];
		ccsf.grandChildParent = v;
		ccsf.reSize ();
		selectedDay = v;
		pState = poiState.poiSelect;
	}

	//exit a places of interest selection screen based on which day is chosen
	void exitSel(){
		POISEL_LIST [selectedDay].SetActive (false);
		POI_Content.SetActive (false);
		POI_Select.SetActive (false);
		Day_Select.SetActive (true);
		//Day_Select.SetActive (true);
		header.text = "Places of Interest";
		selectedDay = -1;
		pState = poiState.daySelect;
	}

	//back function which navigates based on current state of poimanager
	public void back(){
		switch (pState) {
		case poiState.daySelect:
			#if UNITY_ANDROID
			lsm.changeScene ("mainmenu_and");
			#else
			lsm.changeScene ("mainmenu");
			#endif
			break;
		case poiState.poiSelect:
			exitSel ();
			break;
		case poiState.inPoi:
			exitPOI ();
			break;
		default:
			break;
		}
//		if (selectedDay != -1)
//			exitPOI ();
//		else
//			lsm.changeScene ("mainmenu");	
	}
}
