using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite; 
using System.Data; 
using System;
using System.IO;
using System.Text;

public class InitDB : MonoBehaviour {

	public List<dbTypes.Profile> profileList;
	public List<dbTypes.Presentation> presentationList;
	public List<dbTypes.Schedule> scheduleList;
	public string url;
	public string pdfurl;
	public string profileToLoad;
	public string scheduleToLoad;
	public string presentationToLoad;

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

		public string SGP = "SG";
		public string THP = "TH";
		public string VNP = "VN";
		public string BNP = "BN";
		public string CMP = "CM";
		public string IDP = "ID";
		public string LSP = "LS";
		public string MYP = "MY";
		public string MYRP = "MYR";
		public string PHP = "PH";
		public string AARM = "AARM";
	}

	//what type of data to load
	[System.Serializable]
	public enum dbToLoad{
		Profile,
		Schedule,
		Presentation
	};

	public dbToLoad dbLoaded = dbToLoad.Profile;

	public cQuery cq = new cQuery ();

	bool dbInitDone = false;
	//start by loading either initprofile or initschedule
	void Start () {
        url = LoadLinkFromFile(Application.dataPath + "/dblink.cfg");
        pdfurl = LoadLinkFromFile(Application.dataPath + "/dbpdflink.cfg");
        switch (dbLoaded) {
		case dbToLoad.Profile:
			initProfile ();
			break;
		case dbToLoad.Schedule:
			initSchedule ();
			break;
		case dbToLoad.Presentation:
			//initPresentation ();
			break;
		default:
			initProfile ();
			break;
		}
		//loadDBGoogle (https://s3-ap-southeast-1.amazonaws.com/acamm/Database.db);
	}

    private string LoadLinkFromFile(string fileName)
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
                    theReader.Close();
                    return line;
                }
            }
            while (line != null);

            theReader.Close();
            return line;
        }
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

	public void initPresentation(){
		switch (GlobalValues.cp2) {
		case GlobalValues.CPre.SG:
			presentationToLoad = cq.SGP;
			break;
		case GlobalValues.CPre.TH:
			presentationToLoad = cq.THP;
			break;
		case GlobalValues.CPre.VN:
			presentationToLoad = cq.VNP;
			break;
		case GlobalValues.CPre.BN:
			presentationToLoad = cq.BNP;
			break;
		case GlobalValues.CPre.CM:
			presentationToLoad = cq.CMP;
			break;
		case GlobalValues.CPre.ID:
			presentationToLoad = cq.IDP;
			break;
		case GlobalValues.CPre.LS:
			presentationToLoad = cq.LSP;
			break;
		case GlobalValues.CPre.MY:
			presentationToLoad = cq.MYP;
			break;
		case GlobalValues.CPre.MYR:
			presentationToLoad = cq.MYRP;
			break;
		case GlobalValues.CPre.PH:
			presentationToLoad = cq.PHP;
			break;
		case GlobalValues.CPre.AARM:
			presentationToLoad = cq.AARM;
			break;
		default:
			presentationToLoad = cq.SGP;
			break;
		}
		presentationList = new List<dbTypes.Presentation> ();
		loadDBPresentation ();

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
		string sqlQuery = "SELECT VALUE,ROLE,NAME,DOB,COUNTRY,RANK,COMMENT,PICTURE " + "FROM " + profileToLoad + " ORDER BY VALUE";//query to load
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

	public void loadDBPresentation()
	{
		#if UNITY_EDITOR
		WWW loadDB = new WWW(pdfurl);
		//WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/PDF_Database.db"); 
		while(!loadDB.isDone) {
		Debug.Log("trying to load database");
		}
		if(loadDB.size != 0)
		{
		File.WriteAllBytes(Application.dataPath + "/PDF_Database.db", loadDB.bytes);
		Debug.Log("wrote file to database from server");
		}
		string conn = "URI=file:" + Application.dataPath + "/PDF_Database.db"; //Path to database.
		Debug.Log("reading database windows");
		#elif UNITY_ANDROID
		WWW loadDB = new WWW(url);
		//WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/PDF_Database.db"); 
		while(!loadDB.isDone) {
		Debug.Log("trying to load database");
		}
		if(loadDB.size != 0)
		{
		File.WriteAllBytes(Application.persistentDataPath + "/PDF_Database.db", loadDB.bytes);
		Debug.Log("wrote file to database from server");
		}
		else //if(!File.Exists(Application.persistentDataPath + "/PDF_Database.db"))
		{
		Debug.Log("loading from back up");
		//WWW loadDB = new WWW(url);
		loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/PDF_Database.db"); 

		while(!loadDB.isDone) {
		Debug.Log("trying to load database");
		}
		File.WriteAllBytes(Application.persistentDataPath + "/PDF_Database.db", loadDB.bytes);
		Debug.Log("wrote file to database");
		}
		string conn = "URI=file:" + Application.persistentDataPath + "/PDF_Database.db"; //Path to database.
		#else

		WWW loadDB = new WWW(pdfurl);
		//WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/PDF_Database.db"); 
		while(!loadDB.isDone) {
		Debug.Log("trying to load database");
		}
		if(loadDB.size != 0)
		{
		File.WriteAllBytes(Application.dataPath + "/PDF_Database.db", loadDB.bytes);
		Debug.Log("wrote file to database from server");
		}

		string conn = "URI=file:" + Application.dataPath + "/PDF_Database.db"; //Path to database.
		Debug.Log("reading database windows");
		#endif
		IDbConnection dbconn;
		dbconn = (IDbConnection) new SqliteConnection(conn);
		dbconn.Open(); //Open connection to the database.
		IDbCommand dbcmd = dbconn.CreateCommand();
		string sqlQuery = "SELECT Title,PAGES,LINK,VERSION " + "FROM " + presentationToLoad;//query to load
//		for(int i = 1; i < 50; i++)
//		{
//			sqlQuery = sqlQuery + "," + (i + 1);
//		}
//		sqlQuery = sqlQuery + " FROM " + presentationToLoad;//query to load
		Debug.Log (sqlQuery);
		dbcmd.CommandText = sqlQuery;
		IDataReader reader = dbcmd.ExecuteReader();
		string Title = "";
		int Pages = 0;
		string country = "";
        string Link = "";
        int Version = 0;
		//List<string> pageImageList = new List<string>();
		while (reader.Read())//read and load query
		{
			Title = reader.GetString(0);
			Pages = reader.GetInt32(1);
            Link = reader.GetString(2);
            Version = reader.GetInt32(3);
            //Debug.Log(reader.GetString(2));
            country = presentationToLoad;
            //List<string> pageImage = new List<string>();
            //	IDbConnection dbconn2;
            //	dbconn2 = (IDbConnection) new SqliteConnection(conn);
            //	dbconn2.Open();
            //	IDbCommand dbcmd2 = dbconn2.CreateCommand();
            //	string sqlQuery2 = "SELECT P1";//Title,Pages " + "FROM " + profileToLoad;//query to load
            //	for(int i = 2; i <= Pages; i++)
            //	{
            //		sqlQuery2 = sqlQuery2 + ",P" + (i);
            //	}
            //	sqlQuery2 = sqlQuery2 + " FROM " + presentationToLoad + " WHERE Title = '" + Title + "'";
            //	dbcmd2.CommandText = sqlQuery2;
            //	Debug.Log (sqlQuery2);
            //	IDataReader reader2 = dbcmd2.ExecuteReader();
            //	while (reader2.Read ()) {//read and load query
            //		List<string> pageImageList = new List<string>();
            //		for (int i = 0; i < Pages; i++) {
            //			Debug.Log (reader2.GetString (i));
            //			pageImageList.Add (reader2.GetString (i));

            //		}
            //		dbTypes.Presentation tPresentation = new dbTypes.Presentation();
            //		tPresentation = returnPresentation (Title, Pages, country, pageImageList);
            //		loadToDBPresentation (tPresentation);
            //	}
            //	reader2.Close();//clear connection
            //	reader2 = null;
            //	dbcmd2.Dispose();
            //	dbconn2.Close();
            //	dbconn2 = null;
            dbTypes.Presentation tPresentation = new dbTypes.Presentation();
            tPresentation = returnPresentation (Title, Pages, country, Link, Version);
            loadToDBPresentation (tPresentation);
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

    //public dbTypes.Presentation returnPresentation(string title,int pages,string country, List<string> pageImageList)//return requested presentation to be loaded
    //{
    //	dbTypes.Presentation tPresentation = new dbTypes.Presentation ();
    //	tPresentation.title = title;
    //	tPresentation.pages = pages;
    //	tPresentation.country = country;
    //	tPresentation.pageImageList = pageImageList;


    //	return tPresentation;
    //}

    public dbTypes.Presentation returnPresentation(string title, int pages, string country, string link, int version)//return requested presentation to be loaded
    {
        dbTypes.Presentation tPresentation = new dbTypes.Presentation();
        tPresentation.title = title;
        tPresentation.pages = pages;
        tPresentation.country = country;
        tPresentation.link = link;
        tPresentation.version = version;
        //tPresentation.pageImageList = pageImageList;


        return tPresentation;
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

	void loadToDBPresentation(dbTypes.Presentation data)//add schedule to list
	{
		presentationList.Add (data);
	}

	void loadToDBSchedule(dbTypes.Schedule data)//add schedule to list
	{
		scheduleList.Add (data);
	}

	public void resetDBPresentation()//reset schedule list so that another one can be loaded cleanly
	{
		presentationList.Clear ();
	}

	public void resetDBSchedule()//reset schedule list so that another one can be loaded cleanly
	{
		scheduleList.Clear ();
	}
}
