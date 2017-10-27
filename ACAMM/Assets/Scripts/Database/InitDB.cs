using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite; 
using System.Data; 
using System;
using System.IO;

public class InitDB : MonoBehaviour {

	public List<dbTypes.Profile> profileList;
	public List<dbTypes.Schedule> scheduleList;
	public string url;
	public string profileToLoad;
	public string scheduleToLoad;

	//series of queries that can be called from database
	[System.Serializable]
	public class cQuery
	{
		public string SG = "SG_Profile";
		public string TH = "TH_Profile";
		public string VN = "VN_Profile";
		public string BN = "BN_Profile";
		public string CM = "CM_Profile";
		public string ID = "ID_Profile";
		public string LS = "LS_Profile";
		public string MY = "MY_Profile";
		public string MYR = "MYR_Profile";
		public string PH = "PH_Profile";

		public string ACAMM = "ACAMM_SCHEDULE";
		public string ASMAM = "ASMAM_SCHEDULE";
		public string ACAMMSP = "ACAMMSP_SCHEDULE";
	}

	//what type of data to load
	[System.Serializable]
	public enum dbToLoad{
		Profile,
		Schedule
	};

	public dbToLoad dbLoaded = dbToLoad.Profile;

	public cQuery cq = new cQuery ();

	bool dbInitDone = false;
	//start by loading either initprofile or initschedule
	void Start () {
		switch (dbLoaded) {
		case dbToLoad.Profile:
			initProfile ();
			break;
		case dbToLoad.Schedule:
			initSchedule ();
			break;
		default:
			initProfile ();
			break;
		}
		//loadDBGoogle (https://s3-ap-southeast-1.amazonaws.com/acamm/Database.db);
	}

	//init profile based on country selected
	void initProfile(){
		switch (GlobalValues.cp) {
		case GlobalValues.CP.SG:
			profileToLoad = cq.SG;
			break;
		case GlobalValues.CP.TH:
			profileToLoad = cq.TH;
			break;
		case GlobalValues.CP.VN:
			profileToLoad = cq.VN;
			break;
		case GlobalValues.CP.BN:
			profileToLoad = cq.BN;
			break;
		case GlobalValues.CP.CM:
			profileToLoad = cq.CM;
			break;
		case GlobalValues.CP.ID:
			profileToLoad = cq.ID;
			break;
		case GlobalValues.CP.LS:
			profileToLoad = cq.LS;
			break;
		case GlobalValues.CP.MY:
			profileToLoad = cq.MY;
			break;
		case GlobalValues.CP.MYR:
			profileToLoad = cq.MYR;
			break;
		case GlobalValues.CP.PH:
			profileToLoad = cq.PH;
			break;
		default:
			profileToLoad = cq.SG;
			break;
		}
		profileList = new List<dbTypes.Profile> ();
		loadDB ();

	}

	//init schedule based on faction selected
	public void initSchedule(){
		switch (GlobalValues.ss) {
		case GlobalValues.SS.ACAMM:
			scheduleToLoad = cq.ACAMM;
			break;
		case GlobalValues.SS.ASMAM:
			scheduleToLoad = cq.ASMAM;
			break;
		case GlobalValues.SS.ACAMMSP:
			scheduleToLoad = cq.ACAMMSP;
			break;
		default:
			scheduleToLoad = cq.ACAMM;
			break;
		}
		scheduleList = new List<dbTypes.Schedule> ();
		loadDBSchedule ();
	}
	// Update is called once per frame
	void Update () {
		
	}


	//loads the profile database
	void loadDB()
	{
		#if UNITY_EDITOR
		WWW loadDB = new WWW(url);
		//WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/Database.db"); 
		while(!loadDB.isDone) {
			Debug.Log("trying to load database");
		}
		if(loadDB.size != 0)
		{
			File.WriteAllBytes(Application.dataPath + "/Database.db", loadDB.bytes);
			Debug.Log("wrote file to database from server");
		}
		string conn = "URI=file:" + Application.dataPath + "/Database.db"; //Path to database.
		Debug.Log("reading database windows");
		#elif UNITY_ANDROID
		WWW loadDB = new WWW(url);
		//WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/Database.db"); 
		while(!loadDB.isDone) {
		Debug.Log("trying to load database");
		}
		if(loadDB.size != 0)
		{
			File.WriteAllBytes(Application.persistentDataPath + "/Database.db", loadDB.bytes);
			Debug.Log("wrote file to database from server");
		}
		else //if(!File.Exists(Application.persistentDataPath + "/Database.db"))
		{
			Debug.Log("loading from back up");
			//WWW loadDB = new WWW(url);
			loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/Database.db"); 

			while(!loadDB.isDone) {
				Debug.Log("trying to load database");
			}
			File.WriteAllBytes(Application.persistentDataPath + "/Database.db", loadDB.bytes);
			Debug.Log("wrote file to database");
		}
		string conn = "URI=file:" + Application.persistentDataPath + "/Database.db"; //Path to database.

		#else
		if(!dbInitDone)
		{
			WWW loadDB = new WWW(url);
			//WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/Database.db"); 
			while(!loadDB.isDone) {
			Debug.Log("trying to load database");
			}
			if(loadDB.size != 0)
			{
			File.WriteAllBytes(Application.dataPath + "/Database.db", loadDB.bytes);
			Debug.Log("wrote file to database from server");
			}
			dbInitDone = true;
		}
		string conn = "URI=file:" + Application.dataPath + "/Database.db"; //Path to database.
		Debug.Log("reading database windows");
		#endif
		IDbConnection dbconn;
		dbconn = (IDbConnection) new SqliteConnection(conn);
		dbconn.Open(); //Open connection to the database.
		IDbCommand dbcmd = dbconn.CreateCommand();
		string sqlQuery = "SELECT VALUE,ROLE,NAME,DOB,COUNTRY,RANK,COMMENT,PICTURE " + "FROM " + profileToLoad;//query to load
		dbcmd.CommandText = sqlQuery;
		IDataReader reader = dbcmd.ExecuteReader();
		while (reader.Read())//read and load query
		{
			int value = reader.GetInt32(0);
			string role = reader.GetString(1);
			string name = reader.GetString(2);
			string dob = reader.GetString(3);
			string country = reader.GetString(4);;
			string rank = reader.GetString(5);
			string comment = reader.GetString(6);
			string picture = reader.GetString(7);

			dbTypes.Profile tProfile = new dbTypes.Profile();
			tProfile = returnProfile (value,role, name, dob, country, rank, comment, picture);
			loadToDB (tProfile);
			Debug.Log( "value= "+value+"  name ="+name+"  dob="+dob+"  country="+country+"  rank="+rank+"  comment="+comment+"  picture="+picture );
		}
		reader.Close();//clear connection
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close();
		dbconn = null;
	}

	public void loadDBSchedule()
	{
		#if UNITY_EDITOR
		//		WWW loadDB = new WWW(url);
		//		//WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/Database.db"); 
		//		while(!loadDB.isDone) {
		//			Debug.Log("trying to load database");
		//		}
		//		if(loadDB.size != 0)
		//		{
		//			File.WriteAllBytes(Application.dataPath + "/Database.db", loadDB.bytes);
		//			Debug.Log("wrote file to database from server");
		//		}
		string conn = "URI=file:" + Application.dataPath + "/Database.db"; //Path to database.
		Debug.Log("reading database windows");
		#elif UNITY_ANDROID
		WWW loadDB = new WWW(url);
		//WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/Database.db"); 
		while(!loadDB.isDone) {
		Debug.Log("trying to load database");
		}
		if(loadDB.size != 0)
		{
		File.WriteAllBytes(Application.persistentDataPath + "/Database.db", loadDB.bytes);
		Debug.Log("wrote file to database from server");
		}
		else //if(!File.Exists(Application.persistentDataPath + "/Database.db"))
		{
		Debug.Log("loading from back up");
		//WWW loadDB = new WWW(url);
		loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/Database.db"); 

		while(!loadDB.isDone) {
		Debug.Log("trying to load database");
		}
		File.WriteAllBytes(Application.persistentDataPath + "/Database.db", loadDB.bytes);
		Debug.Log("wrote file to database");
		}
		string conn = "URI=file:" + Application.persistentDataPath + "/Database.db"; //Path to database.

		#else
		WWW loadDB = new WWW(url);
		//WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/Database.db"); 
		while(!loadDB.isDone) {
		Debug.Log("trying to load database");
		}
		if(loadDB.size != 0)
		{
		File.WriteAllBytes(Application.dataPath + "/Database.db", loadDB.bytes);
		Debug.Log("wrote file to database from server");
		}
		string conn = "URI=file:" + Application.dataPath + "/Database.db"; //Path to database.
		Debug.Log("reading database windows");
		#endif
		IDbConnection dbconn;
		dbconn = (IDbConnection) new SqliteConnection(conn);
		dbconn.Open(); //Open connection to the database.
		IDbCommand dbcmd = dbconn.CreateCommand();
		string sqlQuery = "SELECT VALUE,DAY,DATE,TIME,EVENT,LOCATION " + "FROM " + scheduleToLoad;//query too load
		dbcmd.CommandText = sqlQuery;
		IDataReader reader = dbcmd.ExecuteReader();
		while (reader.Read())//read and load query
		{
		int value = reader.GetInt32(0);
		int day = reader.GetInt32(1);
		string date = reader.GetString(2);
		string time = reader.GetString(3);
		string event_ = reader.GetString(4);;
		string location = reader.GetString (5);

		dbTypes.Schedule tSchedule = new dbTypes.Schedule();
		tSchedule = returnSchedule (value,day,date,time,event_,location);
		loadToDBSchedule(tSchedule);

		}
		reader.Close();//clear query
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close();
		dbconn = null;
	}

	public dbTypes.Profile returnProfile(int value,string role,string name,string dob,string country,string rank,string comment, string picture)//return requested profile to be loaded
	{
		dbTypes.Profile tProfile = new dbTypes.Profile ();
		tProfile.value = value;
		tProfile.role = role;
		tProfile.name = name;
		tProfile.dob = dob;
		tProfile.country = country;
		tProfile.rank = rank;
		tProfile.comment = comment;
		tProfile.picture = picture;

		return tProfile;
	}

	public dbTypes.Schedule returnSchedule(int value,int day,string date,string time,string event_,string location)//return requested schedule to be loaded
	{
		dbTypes.Schedule tSchedule = new dbTypes.Schedule ();
		tSchedule.value = value;
		tSchedule.day = day;
		tSchedule.date = date;
		tSchedule.time = time;
		tSchedule.event_ = event_;
		tSchedule.location = location;

		return tSchedule;
	}

	void loadToDB(dbTypes.Profile data)//add profile to list
	{
		profileList.Add (data);
	}

	void loadToDBSchedule(dbTypes.Schedule data)//add schedule to list
	{
		scheduleList.Add (data);
	}

	public void resetDBSchedule()//reset schedule list so that another one can be loaded cleanly
	{
		scheduleList.Clear ();
	}
}
