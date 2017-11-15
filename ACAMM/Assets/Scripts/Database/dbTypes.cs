using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dbTypes {

	[System.Serializable]
	public class Profile
	{
		public int value = 0;
		public string role = "";
		public string name = "";
		public string dob = "";
		public string country = "";
		public string rank = "";
		public string comment = "";
		public string picture = "";
	}

	[System.Serializable]
	public class Presentation
	{
		public int value = 0;
		public string country = "";
		public string title = "";
		public int pages = 0;
        public string link = "";
        public int version = 0;
		//public List<string> pageImageList = new List<string>();
	}

	[System.Serializable]
	public class Schedule
	{
		public int value = 0;
		public int day = 0;
		public string date = "";
		public string time = "";
		public string event_ = "";
		public string location = "";
	}

	[System.Serializable]
	public class Login
	{
		int uID = 0;
		int userLevel = 0;
		string userName = "";
		string userRank = "";
	}
		
}
