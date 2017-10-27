using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// L scene manager.
/// manages scenes, should make global when have time
/// </summary>
public class l_sceneManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public void changeScene(string sceneName){
		SceneManager.LoadSceneAsync (sceneName);
	}
	public void profileScene(int country){
		GlobalValues.cp = (GlobalValues.CP)country;

		#if UNITY_ANDROID
			SceneManager.LoadSceneAsync ("profile_and");
		#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
		//SceneManager.LoadSceneAsync ("profile_pc");
			SceneManager.LoadSceneAsync ("profile");
		#endif
	}
}
