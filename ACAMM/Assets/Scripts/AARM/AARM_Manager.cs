using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class AARM_Manager : MonoBehaviour {
	public l_sceneManager lsm;
	public RectTransform pdf_PageRect;
	public GameObject[] pdf_LIST = null;
	public int selectedpdf = -1;
	public Text header;
	public ChildContentSizeFitter ccsf;
	public InitDB DB;
	public GameObject presentationPrefab;
	public GameObject pagePrefab;
	public List<GameObject> presentationList = new List<GameObject>();
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
	// Use this for initialization
	void Start () {
		enterpdf (0);
	}
	
	// Update is called once per frame
	void Update () {
		
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

	public void enterpdf(int v)
	{
		GlobalValues.cp2 = (GlobalValues.CPre)10;
		DB.initPresentation ();

		foreach (dbTypes.Presentation presentation in DB.presentationList) {
			string directoryPath = Application.dataPath + "/Resources/Images/PDF/" + presentation.country + "/" + presentation.title;
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
				//string resLoadPath = "Images/PDF/" + DB.presentationList [v].country + "/" + DB.presentationList [v].title + "/Page" + (i + 1);// + ".png";
                string resLoadPath = Application.dataPath + "/Resources/Images/PDF/" + DB.presentationList[v].country + "/" + DB.presentationList[v].title + "/Page" + (i + 1) + ".png";
                preObj.GetComponent<loadPdfImage>().ImageLocation[i] = resLoadPath;
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
				} else {
					pdf_LIST [k].SetActive (false);
				}
				pdf_LIST[k].gameObject.transform.SetSiblingIndex (j + csOffset);
				j++;
			}
		}
		pdf_LIST [v].SetActive (true);
		header.text = DB.presentationList [v].title;
		selectedpdf = v;
		ccsf.grandChildParent = ccPos;
		ccsf.reSize ();

	}
		
}
