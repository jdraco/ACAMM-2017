using System.Collections;
using System.Collections.Generic;
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
