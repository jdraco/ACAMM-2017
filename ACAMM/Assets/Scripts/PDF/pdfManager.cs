using System.Collections;
using System.Collections.Generic;
using System.Data; 
using System;
using System.IO;
using System.Text;
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
	public GameObject[] pdf_LIST = null;
	public string[] pdfSEL_HLIST;
	public string[] pdf_HLIST;
	public int selectedCountry = -1;
	public int selectedpdf = -1;
	public Text header;
	public ChildContentSizeFitter ccsf;
	public InitDB DB;
	public GameObject presentationPrefab;
	public GameObject pagePrefab;
	public GameObject pTabPrefab;
	public List<GameObject> presentationList = new List<GameObject>();
	public float distBetweenPresentationTabs = 0;
	public float distBetweenPages = 0;
	public Transform reference;
	public Vector3 tabPosOffset = new Vector3(-10,-80,0);
	public int csOffset = 1;
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
		//int j = 0;
		int ccPos = 0;
		if (pdf_LIST [v] == null) {
			Vector3 tabPos = reference.transform.position;
			GameObject preObj = Instantiate (presentationPrefab, tabPos, Quaternion.identity);
			preObj.name = DB.presentationList [v].title;
            //foreach (dbTypes.Presentation presentation in DB.presentationList) {
            preObj.GetComponent<loadPdfImage>().ImageLocation = new string[DB.presentationList[v].pages];
            preObj.GetComponent<loadPdfImage>().Pages = new GameObject[DB.presentationList[v].pages];
            for (int i = 0; i < DB.presentationList [v].pages; i++) {
				Vector3 tabPos2 = reference.transform.position;
				tabPos2.y = tabPos2.y + (distBetweenPages * i) + (distBetweenPages * 0.5f);
				GameObject pageObj = Instantiate (pagePrefab, tabPos2, Quaternion.identity);
				string resLoadPath = Application.dataPath + "/Resource/Images/PDF/" + DB.presentationList [v].country + "/" + DB.presentationList [v].title + "/Page" + (i + 1) + ".png";

                //Texture2D temp = new Texture2D(0, 0);
                //WWW www = new WWW(resLoadPath);
                //while (!www.isDone)
                //{
                //    Debug.Log("trying to load image");
                //}

                //temp = www.texture;
                //Sprite sprite = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), new Vector2(0.5f, 0.5f));

                //var loadedImg = Resource.Load<Sprite> (resLoadPath);
                //Image pageImg = pageObj.GetComponent<Image> ();
                //pageImg.sprite = sprite;
                //preObj.GetComponent<loadPdfImage>().ImageLocation[i] = resLoadPath;
                preObj.GetComponent<loadPdfImage>().SetImage(resLoadPath, pageObj, i);
                pageObj.name = "Page " + (i + 1);
				pageObj.transform.parent = preObj.transform;
			}
			preObj.transform.parent = pdf_PageRect.transform;
			preObj.transform.localScale = new Vector3 (1, 1, 1);
			//j++;
			//}

			pdf_LIST [v] = preObj;


		}
		int j = 0;
		for (int k = 0; k < pdf_LIST.Length; k++) {
			
			if (pdf_LIST [k] != null) {
				if (pdf_LIST [v] == pdf_LIST [k]) {
					ccPos = j + csOffset;
				}
				pdf_LIST[k].gameObject.transform.SetSiblingIndex (j + csOffset);
				j++;
			}
		}
		pdf_LIST [v].SetActive (true);
		pdf_Content.SetActive (true);
		pdfSEL_LIST [selectedCountry].SetActive (false);
		pdf_Select.SetActive (false);
		header.text = DB.presentationList [v].title;
		pState = pdfState.inpdf;
		selectedpdf = v;
		ccsf.grandChildParent = ccPos;
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

    private int loadVersion(string fileName)
    {
        if (!File.Exists(fileName))
            return 0;
        string line;

        StreamReader theReader = new StreamReader(fileName, Encoding.Default);

        using (theReader)
        {
            // While there's lines left in the text file, do this:
            do
            {
                line = theReader.ReadLine();
                if (line != null)
                {
                    theReader.Close();
                    return int.Parse(line);
                }
            }
            while (line != null);

            theReader.Close();
            return int.Parse(line);
        }
    }

    private void setVersion(string fileName, string version)
    {
        File.WriteAllText(@fileName, version);
    }

    //enter a places of interest selection screen based on which country is chosen
    public void enterpdfSel(int v)
	{
		GlobalValues.cp2 = (GlobalValues.CPre)v;
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
		foreach (dbTypes.Presentation presentation in DB.presentationList) {
            string directoryPath = Application.dataPath + "/Resource/Images/PDF/" + presentation.country + "/" + presentation.title;
			if (presentation.version > loadVersion(directoryPath + "/Version.txt"))// || !File.Exists(directoryPath + "/Page" + (i + 1) + ".png"))
            {
                for (int i = 0; i < presentation.pages; i++)
                {
                    
                   // if (!File.Exists(directoryPath + "/Page" + (i + 1) + ".png"))
                   // {
                        string dlLink = presentation.link + "-" + (i) + ".png";
                        WWW loadIMG = new WWW(dlLink);
                        //WWW loadIMG = new WWW (presentation.pageImageList [i]);
                        //WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/PDF_Database.db"); 
                        while (!loadIMG.isDone)
                        {
                            Debug.Log("trying to load image");
                        }


                        if (loadIMG.size != 0)
                        {
                            if (!Directory.Exists(directoryPath))
                            {
                                //if it doesn't, create it
                                Directory.CreateDirectory(directoryPath);

                            }
                            File.WriteAllBytes(directoryPath + "/Page" + (i + 1) + ".png", loadIMG.bytes);
                            Debug.Log("wrote file to local from server");
                        }
                   // }
                   // else
                        //Debug.Log("skip downloading " + presentation.title + " Page" + (i + 1) + " as file already exist");
                }
                setVersion(directoryPath + "/Version.txt", presentation.version.ToString());
                //File.WriteAllBytes(directoryPath + "/Version" + (i + 1) + ".png", loadIMG.bytes);
            }
        }
		pdf_LIST = new GameObject[DB.presentationList.Count];
		pdfSEL_LIST [v].SetActive (true);
		pdf_Select.SetActive (true);
		Country_Select.SetActive (false);
		header.text = pdfSEL_HLIST [v];
		ccsf.grandChildParent = 0;
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

		for (int i = 0; i < pdf_LIST.Length; i++) {
			if (pdf_LIST [i] != null)
				Destroy (pdf_LIST [i]);
		}
		pdf_LIST = null;
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
