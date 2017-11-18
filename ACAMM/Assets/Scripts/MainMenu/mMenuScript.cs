using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
//controls the main menu screen 
public class mMenuScript : MonoBehaviour {
	
	public string chatIPaddr = "172.20.202.70";
	public authentication_Manager cManager;
	public GameObject serverButton;
	public bool serverButtonActive = false;
	public string url;
	float timeOut = 2.5f;
//	public RectTransform chatBox;
//	public float chatEnterSpeed = 1;
//	public Vector3 dChatPos = new Vector3(144,0,0), eChatPos = new Vector3(-127,0,0);
//	bool paused = false;
//	public ClearUIText cut;
	// Use this for initialization
//	enum chatBoxState{
//		closed,
//		opened,
//		closing,
//		opening
//	}
//
//	chatBoxState cbState = chatBoxState.closed;

	void Awake(){
		
	}

	//reconnects to chat server after alt tab in mobile
//	void OnApplicationFocus(bool hasFocus)
//	{
//		if (!hasFocus)
//			paused = true;
//		else if (hasFocus && paused) {
//			cManager.QuitLocalGame ();
//			//cut.ClearLog ();
//			cManager.JoinLocalGame ();
//			paused = false;
//		}
//	}

	//reconnects to chat server after alt tab in mobile
//	void OnApplicationPause(bool pauseStatus)
//	{
//		if (pauseStatus)
//			paused = true;
//		if (!pauseStatus && paused) {
//			cManager.QuitLocalGame ();
//			//cut.ClearLog ();
//			cManager.JoinLocalGame ();
//			paused = false;
//		}
//	}

	//connect to chat server when app init
	void Start () {
        url = LoadLinkFromFile(Application.dataPath + "/iplink.cfg");
        updateIpCfgfromWeb(url);
		LoadIPfromFile(Application.dataPath + "/serverip.cfg");
		cManager.UpdateJoinAddress (chatIPaddr);
//		//cManager.InitNetworkTransport ();
		cManager.JoinLocalGame ();
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

    private void updateIpCfgfromWeb(string url)
	{
		WWW loadIP = new WWW(url);
		//WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/Database.db");
		float timer = 0;
		timeOut = 5f;
		bool failed = false;
		while(!loadIP.isDone) {
			//Debug.Log("trying to loadIP");
			if (timer > timeOut && loadIP.size == 0) {
				failed = true;
				break;
			}
			timer += Time.deltaTime;
		}
		if (failed)
			loadIP.Dispose ();
		else if (!failed) {
			if (loadIP.size != 0) {
				File.WriteAllBytes (Application.dataPath + "/serverip.cfg", loadIP.bytes);
				//Debug.Log("wrote file to loadIP from server");
			}
		}
	}
	private bool LoadIPfromFile(string fileName)
	{

		string line;
		// Create a new StreamReader, tell it which file to read and what encoding the file
		// was saved as
		StreamReader theReader = new StreamReader(fileName, Encoding.Default);
		// Immediately clean up the reader after this block of code is done.
		// You generally use the "using" statement for potentially memory-intensive objects
		// instead of relying on garbage collection.
		// (Do not confuse this with the using directive for namespace at the 
		// beginning of a class!)
		using (theReader)
		{
			// While there's lines left in the text file, do this:
			do
			{
				line = theReader.ReadLine();

				if (line != null)
				{
					chatIPaddr = line;
				}
			}
			while (line != null);
			// Done reading, close the reader and return true to broadcast success    
			theReader.Close();
			return true;
		}
	}

	public void changeIp(string ip)
	{
		chatIPaddr = ip;
	}


	public void toggleServerButton()
	{ 
		serverButtonActive = !serverButtonActive;
		serverButton.SetActive(serverButtonActive);
	}

	public void joinServer(){
		cManager.QuitLocalGame ();

		cManager.UpdateJoinAddress (chatIPaddr);
		//		//cManager.InitNetworkTransport ();

		cManager.JoinLocalGame ();
	}
	// Update is called once per frame
	void Update () {
		//quits app if esc is pressed
		if (Input.GetKeyUp ("escape"))
			Application.Quit();
		//bring chat from side/hidden to middle of screen
		//it steps instantly so if u know how to fix this try to fix
//		switch (cbState) {
//		case chatBoxState.closed:
//			break;
//		case chatBoxState.opened:
//			break;
//		case chatBoxState.closing:
//			float dstep = chatEnterSpeed* Time.deltaTime;
//			chatBox.localPosition = Vector3.Lerp(dChatPos, chatBox.localPosition, dstep);
//			if (chatBox.localPosition == dChatPos)
//				cbState = chatBoxState.closed;
//			break;
//		case chatBoxState.opening:
//			float estep = chatEnterSpeed * Time.deltaTime;
//			chatBox.localPosition = Vector3.Lerp(eChatPos, chatBox.localPosition, estep);
//			if (chatBox.localPosition == eChatPos)
//				cbState = chatBoxState.opened;
//			break;
//		}
	}

	//button function for closing/opening chat
//	public void openOrCloseChat(){
//		if(cbState == chatBoxState.closed)
//			cbState = chatBoxState.opening;
//		else if(cbState == chatBoxState.opened)
//			cbState = chatBoxState.closing;
//	}

	public void quitApp()
	{
		Application.Quit();
	}
}
