using System.Collections;
using System.Collections.Generic;
using System.Data; 
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//this is the places of interest manager
public class pdfManager : MonoBehaviour {
	public l_sceneManager lsm;
	public GameObject pdf_Content;
	public RectTransform pdf_PageRect;
	public GameObject pdf_Select;
	public GameObject Country_Select;
	public GameObject[] pdfSEL_LIST;
	public GameObject[] pdf_LIST;
	public string[] pdfSEL_HLIST;
	public string[] pdf_HLIST;
	public int selectedCountry = -1;
	public int selectedpdf = -1;
	public Text header;
	public ChildContentSizeFitter ccsf;
	public InitDB DB;
	public GameObject pTabPrefab;
	public List<GameObject> presentationList = new List<GameObject>();
	public float distBetweenPresentationTabs = 0;
	public Vector3 tabPosOffset = new Vector3(-10,-80,0);
	//countrySelect = selection of which country
	//pdfSelect = selection of which places of interest data to view
	//inpdf = currently viewing a place of interest
	enum pdfState{
		countrySelect,
		pdfSelect,
		inpdf
	}

	pdfState pState = pdfState.countrySelect;

	void Start () {

	}

	//calls back function if back button is pressed
	void Update() {
		if (Input.GetKeyUp ("escape"))
			back ();
	}

	//view place of interest
	public void enterpdf(int v)
	{
		foreach (dbTypes.Presentation presentation in DB.presentationList) {
			for(int i = 0; i < presentation.pages; i++)
			{
				
				WWW loadDB = new WWW(presentation.pageImageList[0][i+1]);
				//WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/PDF_Database.db"); 
				while(!loadDB.isDone) {
					Debug.Log("trying to load database");
				}
				if(loadDB.size != 0)
				{
					File.WriteAllBytes(Application.dataPath + "/PDF_Database.db", loadDB.bytes);
					Debug.Log("wrote file to database from server");
				}
					
			}
		}
		pdf_LIST [v].SetActive (true);
		pdf_Content.SetActive (true);
		pdfSEL_LIST [selectedCountry].SetActive (false);
		pdf_Select.SetActive (false);
		header.text = pdf_HLIST [v];
		pState = pdfState.inpdf;
		selectedpdf = v;
		ccsf.grandChildParent = v;
		ccsf.reSize ();

	}

	//stop view place of interest
	void exitpdf(){
		pdf_LIST [selectedpdf].SetActive (false);
		pdf_Content.SetActive (false);
		pdfSEL_LIST [selectedCountry].SetActive (true);
		pdf_Select.SetActive (true);
		header.text = pdfSEL_HLIST[selectedCountry];
		pState = pdfState.pdfSelect;
		selectedpdf = -1;
		Vector2 ctPos = pdf_PageRect.anchoredPosition;
		ctPos.y = 840;
		pdf_PageRect.anchoredPosition = ctPos;
	}

	//enter a places of interest selection screen based on which country is chosen
	public void enterpdfSel(int v)
	{
		GlobalValues.cp2 = (GlobalValues.CP)v;
		DB.initPresentation ();
//		foreach (dbTypes.Presentation presentation in DB.presentationList) {
//			for(int i = 0; i < presentation.pages; i++)
//			{
//				
//				WWW loadDB = new WWW(presentation.pageImageList[0][i+1]);
//				//WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/PDF_Database.db"); 
//				while(!loadDB.isDone) {
//					Debug.Log("trying to load database");
//				}
//				if(loadDB.size != 0)
//				{
//					File.WriteAllBytes(Application.dataPath + "/PDF_Database.db", loadDB.bytes);
//					Debug.Log("wrote file to database from server");
//				}
//					
//			}
//		}
		int j = 0;
		foreach(dbTypes.Presentation tPresentation in DB.presentationList)
		{
			Vector3 tabPos = pdfSEL_LIST [v].transform.position;
			tabPos = tabPos + tabPosOffset;
			tabPos.y += j * (-distBetweenPresentationTabs	);
			GameObject newObj = Instantiate(pTabPrefab, tabPos, Quaternion.identity);
			//newObj.transform.Rotate (0, 0, -90);
			newObj.GetComponent<pdfTab> ().presentation = tPresentation;
			newObj.GetComponent<pdfTab> ().pdfm = this;
			newObj.GetComponent<pdfTab> ().value = j;
			newObj.transform.parent = pdfSEL_LIST [v].transform;
			//newObj.transform.localPosition = defTabPos;
			presentationList.Add (newObj);
			j++;
		}
		pdfSEL_LIST [v].SetActive (true);
		pdf_Select.SetActive (true);
		Country_Select.SetActive (false);
		header.text = pdfSEL_HLIST [v];
		ccsf.grandChildParent = v;
		ccsf.reSize ();
		selectedCountry = v;
		pState = pdfState.pdfSelect;
	}

	//exit a places of interest selection screen based on which country is chosen
	void exitSel(){
		foreach (GameObject presentation in presentationList) {
			Destroy (presentation);
		}
		presentationList.Clear ();
		pdfSEL_LIST [selectedCountry].SetActive (false);
		pdf_Content.SetActive (false);
		pdf_Select.SetActive (false);
		Country_Select.SetActive (true);
		//Country_Select.SetActive (true);
		header.text = "Speaker's Notes";
		selectedCountry = -1;
		pState = pdfState.countrySelect;
	}

	//back function which navigates based on current state of pdfmanager
	public void back(){
		switch (pState) {
		case pdfState.countrySelect:
			#if UNITY_ANDROID
			lsm.changeScene ("mainmenu_and");
			#else
			lsm.changeScene ("mainmenu");
			#endif
			break;
		case pdfState.pdfSelect:
			exitSel ();
			break;
		case pdfState.inpdf:
			exitpdf ();
			break;
		default:
			break;
		}
		//		if (selectedCountry != -1)
		//			exitpdf ();
		//		else
		//			lsm.changeScene ("mainmenu");	
	}
}
