using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
//this is the net worokclass
public class c_NetworkManager : NetworkManager {

	public List<NetworkConnection> Clients;
	public bool b_StartServer = false;
	public string HostIP = "";
	public UnityEngine.UI.Text joinAddress, selfAddress, serverLog;
	public UnityEngine.UI.InputField inputMessage, userName;
	public NetworkClient thisClient;
	public string myExtIP = "";
	bool loadFinish = false;

	SphereCollider palmDetector;
	//public Image onlineStatus;

	void Start () {
		Clients = new List<NetworkConnection> ();
		RefreshIP ();
		//get the user name from save and set it as name
		userName.text = PlayerPrefs.GetString ("name");
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
		s_NetworkManager.singleton.networkAddress = joinAddress.text;
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

	//client receives message from server/message from clients that is relayed to server
	void OnServerMessage(NetworkMessage netMsg)
	{
		var nmsg = netMsg.ReadMessage<MasterMsgTypes.UCMsg> ();

//		if (nmsg.msg == "palm") {
//			UnityEngine.SceneManagement.SceneManager.LoadSceneAsync ("NewScanner");
//		}
		//var nmsg = netMsg.ReadMessage<MasterMsgTypes.UCMsg> ();
		sysMessage (nmsg.sender + " : " + nmsg.msg);
	}

	public override void OnServerConnect (NetworkConnection conn)
	{
		DebugLog (conn.address + " Has connected!");
		DebugLog ("Connection ID: " + conn.connectionId);
		Clients.Add (conn);
	}

	//removes said client from list of connection
	public override void OnServerDisconnect (NetworkConnection conn)
	{
		DebugLog (conn.address + " Has disconnected!");
		DebugLog ("Connection ID: " + conn.connectionId);
		Clients.Remove (conn);
	}

	//update onlinestatus to online
	public override void OnClientConnect (NetworkConnection conn)
	{
		DebugLog ("Successfully connected!");
		//onlineStatus.color = Color.green;
	}

	//update onlinestatus to offline and start attempting to reconnect
	public override void OnClientDisconnect (NetworkConnection conn)
	{
		DebugLog ("Disconnected! reconnecting to server");
		//onlineStatus.color = Color.red;
		c_NetworkManager.singleton.StartClient();
	}

	//join specified server
	public void JoinLocalGame()//join server
	{
		thisClient = c_NetworkManager.singleton.StartClient();
		DebugLog ("Establishing connection to chat server : " + s_NetworkManager.singleton.networkAddress);
		//onlineStatus.color = Color.yellow;
		//NetworkServer.Listen(c_NetworkManager.singleton.networkPort);
		thisClient.RegisterHandler(MasterMsgTypes.vcMsg, OnServerMessage);
		thisClient.RegisterHandler(MasterMsgTypes.ucMsg, OnServerMessage);
	}

	//quit server
	public void QuitLocalGame()//quit server
	{
		c_NetworkManager.singleton.StopClient();
	}

	//sends message to connected server
	public void LogMessage(){
		var msg = new MasterMsgTypes.UCMsg ();
		msg.msg = inputMessage.text;
		msg.sender = userName.text;
		thisClient.Send (MasterMsgTypes.ucMsg, msg);
		inputMessage.text = "";
	}
		
	public void sysMessage(string msg) {
		DebugLog(msg);
	}

	//use this function to send debuglogs to chat message locally
	public void DebugLog(object log)
	{
		Debug.Log (log);
		serverLog.text = serverLog.text + log.ToString () + " \n";
	}

}
