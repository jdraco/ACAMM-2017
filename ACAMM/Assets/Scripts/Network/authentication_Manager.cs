using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
//this is the net worokclass
public class authentication_Manager : NetworkManager {

	public List<NetworkConnection> Clients;
	public bool b_StartServer = false;
	public string HostIP = "";
	public string joinAddress, selfAddress, serverLog;
	public string inputMessage, userName;
	public NetworkClient thisClient;
	public string myExtIP = "";
	bool loadFinish = false;
	public Image onlineStatus;
	public openThirdParty otp;
	public Signing signLocal;

	List<GameObject> palmDetector;// = new List<GameObject>();

	void Start () {
		Clients = new List<NetworkConnection> ();
		RefreshIP ();
		//get the user name from save and set it as name
		userName = PlayerPrefs.GetString ("name");
		GlobalValues.authManager = this;
	}

	void Update() {
		if (Input.GetKeyUp ("enter"))
			LogMessage ();
	}

	//init network transport, dont really need honestly
	public void InitNetworkTransport(){
		NetworkTransport.Init ();
		ConnectionConfig config = new ConnectionConfig();
		//reliableFragmentedChannel = 
		config.AddChannel(QosType.ReliableFragmented);
		HostTopology topology = new HostTopology(config, maxConnections);
		//clientSocket = 
		NetworkTransport.AddHost(topology);
	}

	//this gets the ipaddress list and returns value to a string
	public string GetIP()
	{
		string strHostName = "";
		strHostName = System.Net.Dns.GetHostName();

		IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

		IPAddress[] addr = ipEntry.AddressList;

		string rturn = "";
		int j = 0;
		for (int i = 0; i < addr.Length; i++) {
			j = i + 1;
			rturn = rturn + j + ":" + addr [i].ToString() + "\n";
		}

		j++;

		if(myExtIP == "")
			rturn = rturn + j + ":" + "waiting for reply...";	
		else
			rturn = rturn + j + ":" + myExtIP;
		return rturn;

	}

	public IEnumerator GetExtIP()
	{
		WWW myExtIPWWW = new WWW("http://checkip.dyndns.org");
		yield return myExtIPWWW;
		myExtIP=myExtIPWWW.text;
		myExtIP=myExtIP.Substring(myExtIP.IndexOf(":")+1);
		myExtIP=myExtIP.Substring(0,myExtIP.IndexOf("<"));
		HostIP = GetIP ();
		//selfAddress.text = HostIP;
	}

	public void RefreshIP(){
		StartCoroutine (GetExtIP ());
		HostIP = GetIP ();
		//selfAddress.text = HostIP;
	}

	public void UpdateJoinAddress(){
		s_NetworkManager.singleton.networkAddress = joinAddress;
	}

	public void UpdateJoinAddress(string addr){
		s_NetworkManager.singleton.networkAddress = addr;
	}

	//server receives message from client and relays message to other clients
	void OnClientMessage(NetworkMessage netMsg)
	{
		var nmsg = netMsg.ReadMessage<MasterMsgTypes.UCMsg> ();
		NetworkServer.SendToAll (MasterMsgTypes.vcMsg, (MessageBase)nmsg);
		sysMessage (nmsg.sender + " : " + nmsg.msg);
	}

	public IEnumerator LoadPalm() {
		AsyncOperation scanner = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync ("NewScanner");
		while (scanner.isDone == false)
			yield return null;
		if (palmDetector != null) {
			palmDetector.RemoveRange (0, palmDetector.Count);
			palmDetector = null;
		}
		if (palmDetector == null) {
			palmDetector = new List<GameObject> ();
			palmDetector.AddRange (GameObject.Find ("ScanManager").GetComponent<NewScanManager> ().ListOfFingerPoints);
		}
		if (palmDetector != null) {
			foreach (GameObject finger in palmDetector) {
				finger.GetComponent<Collider> ().enabled = false;
			}
		}
	}

	//client receives message from server/message from clients that is relayed to server
	void OnServerMessage(NetworkMessage netMsg)
	{
		var nmsg = netMsg.ReadMessage<MasterMsgTypes.UCMsg> ();
		#if UNITY_ANDROID
		#else
		switch (nmsg.msg) {
		case "palm":
			StartCoroutine ("LoadPalm");
//			AsyncOperation scanner = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync ("NewScanner");
//
//			if(palmDetector == null)
//				palmDetector = GameObject.Find ("WhiteDots").GetComponent<Collider> ();
//			if(palmDetector != null)
//				palmDetector.enabled = false;
			break;
		case "palml":
			if (palmDetector == null) {
				palmDetector = new List<GameObject> ();
				palmDetector.AddRange (GameObject.Find ("ScanManager").GetComponent<NewScanManager> ().ListOfFingerPoints);
			}
			if (palmDetector != null) {
				foreach (GameObject finger in palmDetector) {
					finger.GetComponent<Collider> ().enabled = false;
				}
			}
			break;
		case "palmul":
			if (palmDetector == null) {
				palmDetector = new List<GameObject> ();
				palmDetector.AddRange (GameObject.Find ("ScanManager").GetComponent<NewScanManager> ().ListOfFingerPoints);
			}
			if (palmDetector != null) {
				foreach (GameObject finger in palmDetector) {
					finger.GetComponent<Collider> ().enabled = true;
				}
			}
			break;
		case "sign":
			otp.openBatFile ();
			break;
		case "close":
			Application.Quit();
			break;
		case "signlocal":
			UnityEngine.SceneManagement.SceneManager.LoadSceneAsync ("Signing");
			break;
		case "signliftup":
			if(nmsg.coor == true && signLocal != null && nmsg.sender != userName)
			{
				signLocal.SignLiftupNetwork(nmsg.sender);
			}
			break;
		default :
			if(nmsg.coor == true && signLocal != null && nmsg.sender != userName)
			{
				nmsg.msg = nmsg.msg.Substring(1, nmsg.msg.Length - 2);
				string[] vecstring = nmsg.msg.Split(',');
				Vector3 position = new Vector3(float.Parse(vecstring[0]),float.Parse(vecstring[1]),float.Parse(vecstring[2]));
				position.x = position.x;
				signLocal.SigningUpdateNetwork( position, nmsg.sender);
			}

			break;
		}
		#endif
		//var nmsg = netMsg.ReadMessage<MasterMsgTypes.UCMsg> ();
		//sysMessage (nmsg.sender + " : " + nmsg.msg);
	}

	public override void OnServerConnect (NetworkConnection conn)
	{
		//DebugLog (conn.address + " Has connected!");
		//DebugLog ("Connection ID: " + conn.connectionId);
		Clients.Add (conn);
	}

	//removes said client from list of connection
	public override void OnServerDisconnect (NetworkConnection conn)
	{
		//DebugLog (conn.address + " Has disconnected!");
		//DebugLog ("Connection ID: " + conn.connectionId);
		Clients.Remove (conn);
	}

	//update onlinestatus to online
	public override void OnClientConnect (NetworkConnection conn)
	{
		DebugLog ("Successfully connected!");
		onlineStatus.color = Color.green;
	}

	//update onlinestatus to offline and start attempting to reconnect
	public override void OnClientDisconnect (NetworkConnection conn)
	{
		DebugLog ("Disconnected! reconnecting to server");
		onlineStatus.color = Color.red;
		//authentication_Manager.singleton.StartClient();
		QuitLocalGame ();

		UpdateJoinAddress (s_NetworkManager.singleton.networkAddress);
		//		//cManager.InitNetworkTransport ();

		JoinLocalGame ();
	}

	//join specified server
	public void JoinLocalGame()//join server
	{
		thisClient = authentication_Manager.singleton.StartClient();
		DebugLog ("Establishing connection to chat server : " + s_NetworkManager.singleton.networkAddress);
		onlineStatus.color = Color.yellow;
		//NetworkServer.Listen(c_NetworkManager.singleton.networkPort);
		thisClient.RegisterHandler(MasterMsgTypes.vcMsg, OnServerMessage);
		thisClient.RegisterHandler(MasterMsgTypes.ucMsg, OnServerMessage);
	}

	//quit server
	public void QuitLocalGame()//quit server
	{
		authentication_Manager.singleton.StopClient();
	}

	//sends message to connected server
	public void LogMessage(){
		var msg = new MasterMsgTypes.UCMsg ();
		msg.msg = inputMessage;
		msg.sender = userName;
		thisClient.Send (MasterMsgTypes.ucMsg, msg);
		inputMessage = "";
	}

	public void SendSigningCoordinates(Vector3 input){
		var msg = new MasterMsgTypes.UCMsg ();
		msg.msg = input.ToString();
		msg.sender = userName;
		msg.coor = true;
		thisClient.Send (MasterMsgTypes.ucMsg, msg);
	}

	public void SendPenLiftup(){
		var msg = new MasterMsgTypes.UCMsg ();
		msg.msg = "signliftup";
		msg.sender = userName;
		msg.coor = true;
		thisClient.Send (MasterMsgTypes.ucMsg, msg);
	}

	public void sysMessage(string msg) {
		DebugLog(msg);
	}

	//use this function to send debuglogs to chat message locally
	public void DebugLog(object log)
	{
		Debug.Log (log);
		serverLog = serverLog + log.ToString () + " \n";
	}

}
