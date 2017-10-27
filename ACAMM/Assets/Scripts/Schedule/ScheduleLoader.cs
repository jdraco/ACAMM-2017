using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//loads schedule, schedule manager
public class ScheduleLoader : MonoBehaviour {
	public InitDB DB;
	public GameObject sTabPrefab;
	public List<GameObject> scheduleList = new List<GameObject>();
	public GameObject sTabContainer;
	public l_sceneManager lsm;
	public float distBetweenProfileTabs = 0;
	public float defScreenWidth = 1280;
	public float widthRatio = 1;
	public ChildContentSizeFitter ccsf;
	public GameObject eventList;


	public enum state{
		ACAMM,
		ASMAM,
		ACAMMSP
	}

	public state currentSelected = state.ACAMM;
	// Use this for initialization
	void Start () {
		initSchedule ();
	}

	//changes which faction to view and reloads from database
	public void changeState(int state){
		currentSelected = (state)state;
		DB.resetDBSchedule ();
		for (int i = 0; i < sTabContainer.transform.childCount; i++) {
			Destroy (sTabContainer.transform.GetChild (i).gameObject);
		}
		GlobalValues.ss = (GlobalValues.SS)currentSelected;
		DB.initSchedule ();
		initSchedule ();
	}

	//initschedule and display on screen
	void initSchedule(){
		int i = 0;
		widthRatio = Screen.width / defScreenWidth;
		foreach(dbTypes.Schedule tSchedule in DB.scheduleList)
		{
			Vector3 tabPos = sTabContainer.transform.position;
			tabPos.y += ((float)i+0.5f) * (-distBetweenProfileTabs);
			GameObject newObj = Instantiate(sTabPrefab, tabPos, Quaternion.identity);
			//newObj.transform.Rotate (0, 0, -90);
			//newObj.transform.localPosition = new Vector3 (0, ((float)i + 0.5f) * (-distBetweenProfileTabs * heightRatio) - 10 * heightRatio, 0);
			newObj.GetComponent<sTab> ().schedule = tSchedule;
			newObj.GetComponent<sTab> ().sLoader = this;
			newObj.transform.SetParent (sTabContainer.transform);
			newObj.transform.localScale = Vector3.one;

			scheduleList.Add (newObj);
			i++;
		}
		eventList.SetActive (true);

		//ccsf.reSize ();
	}
		


	void Update() {
		if (Input.GetKeyUp ("escape"))
			back ();
	}

	public void back(){
		#if UNITY_ANDROID
		lsm.changeScene ("mainmenu_and");
		#else
		lsm.changeScene ("mainmenu");	
		#endif
	}
}
