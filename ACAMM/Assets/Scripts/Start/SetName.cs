using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;
using System.IO;

public class SetName : MonoBehaviour {
	//public SaveData data;
	public string currName = "new user";
	public InputField input;
	// Use this for initialization
	void Start () {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
		Debug.Log (PlayerPrefs.GetString ("name"));
		LoadNamefromFile(Application.dataPath + "/serverid.cfg");
		if (PlayerPrefs.GetString ("name") != "") {
			input.text = PlayerPrefs.GetString ("name");
			#if UNITY_ANDROID
			SceneManager.LoadSceneAsync ("mainmenu_and");
			#else
			SceneManager.LoadSceneAsync ("mainmenu");	
			#endif
		} else {
			input.gameObject.SetActive (true);
			input.text = currName;
		}
	}

	private bool LoadNamefromFile(string fileName)
	{

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
					currName = line;
					PlayerPrefs.SetString ("name", currName);
				}
			}
			while (line != null);
  
			theReader.Close();
			return true;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void NewName(string name){
		currName = name;
	}

	public void SaveName(){
		currName = input.text;
		PlayerPrefs.SetString ("name", currName);
		#if UNITY_ANDROID
		SceneManager.LoadSceneAsync ("mainmenu_and");
		#else
		SceneManager.LoadSceneAsync ("mainmenu");	
		#endif
	}
}
