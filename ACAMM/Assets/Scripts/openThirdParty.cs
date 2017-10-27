using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Data; 
using System;
using System.IO;

public class openThirdParty : MonoBehaviour {
	//public string batLink = "D:\\Downloads\\testFile.bat";
	public string url = "https://s3-ap-southeast-1.amazonaws.com/acamm/testFile.bat";
	public string batPath = "/testFile.bat";
	// Use this for initialization
	void Start () {
		//downloadBatFile ();
	}

	// Update is called once per frame
	void Update () {

	}

	void downloadBatFile(){
		WWW loadDB = new WWW(url);
		//WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/Database.db"); 
		while(!loadDB.isDone) {
		}
		if(loadDB.size != 0)
		{
			File.WriteAllBytes(Application.dataPath + batPath, loadDB.bytes);
		}
		//string conn = "URI=file:" + Application.dataPath + "/Database.db"; //Path to database.
	}

	public void openLocalApp(){
		System.Diagnostics.Process.Start("app.exe");
	}

	public void openNotePad(){
		Process myProcess = new Process();
		myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
		myProcess.StartInfo.CreateNoWindow = true;
		//myProcess.StartInfo.UseShellExecute = false;
		myProcess.StartInfo.FileName = "C:\\Windows\\system32\\notepad.exe";
		myProcess.Start ();
	}

	public void openExistingNotePad(){
		Process[] myProcess;
		myProcess = Process.GetProcessesByName("notepad.exe");
		myProcess [0].Start ();
	}

	public void openBatFile(){
		Process myProcess = new Process();
		myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		myProcess.StartInfo.CreateNoWindow = true;
		myProcess.StartInfo.UseShellExecute = false;
		//myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
		myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
		string path = Application.dataPath + batPath;
		print(path);
		//string path = batLink;
		print(Application.dataPath);
		myProcess.StartInfo.Arguments = "/c " + path;
		myProcess.EnableRaisingEvents = true;
		myProcess.Start();
		//myProcess.WaitForExit();
		//int ExitCode = myProcess.ExitCode;
		//print(ExitCode);
	}
}
