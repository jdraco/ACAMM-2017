using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetName : MonoBehaviour {
	//public SaveData data;
	public string currName = "new user";
	public InputField input;
	// Use this for initialization
	void Start () {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 30;
		Debug.Log (PlayerPrefs.GetString ("name"));
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
