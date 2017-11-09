using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class s_NetworkManager : NetworkManager {

	public List<NetworkConnection> Clients;
	public bool b_StartServer = false;
	public string HostIP = "";
	public UnityEngine.UI.Text hostAddress, joinAddress, selfAddress, serverLog;
	public UnityEngine.UI.InputField inputMessage, userName;
	public NetworkClient thisClient;
	public string myExtIP = "";
	bool loadFinish = false;
	public int MaxConnections = 20;

//	void Awake() {
//		Application.targetFrameRate = 30;
//	}

	void Start () {
		Application.targetFrameRate = 30;
		Clients = new List<NetworkConnection> ();

		RefreshIP ();
	}

	void Update(){
		if (Input.GetKeyDown("enter"))
		{
			LogMessage ();
		}
	}

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
		selfAddress.text = HostIP;
	}
		
	public void RefreshIP(){
		StartCoroutine (GetExtIP ());
		HostIP = GetIP ();
		selfAddress.text = HostIP;
	}

	public void UpdateJoinAddress(){
		s_NetworkManager.singleton.networkAddress = joinAddress.text;
	}

	public void UpdateHostAddress(){
		s_NetworkManager.singleton.networkAddress = hostAddress.text;
	}
	public void CreateServer()//start server
	{
		if (!b_StartServer) {
			ConnectionConfig config = new ConnectionConfig();
			config.AddChannel(QosType.ReliableSequenced);
			config.AddChannel(QosType.Unreliable);
			NetworkServer.Configure(config, MaxConnections);
			s_NetworkManager.singleton.StartServer ();
			//NetworkServer.Listen(s_NetworkManager.singleton.networkPort);
			NetworkServer.RegisterHandler(MasterMsgTypes.ucMsg, OnClientMessage);
			NetworkServer.RegisterHandler(MasterMsgTypes.vcMsg, OnServerMessage);
			b_StartServer = true;
		}
	}

	public void StopServer()//stop server
	{
		if (b_StartServer) {
			s_NetworkManager.singleton.StopServer ();
			b_StartServer = false;
		}
	}

	public void CreateLocalGame()//host and join server
	{
		thisClient = s_NetworkManager.singleton.StartHost();
		//NetworkServer.Listen(s_NetworkManager.singleton.networkPort);
		NetworkServer.RegisterHandler(MasterMsgTypes.ucMsg, OnClientMessage);
		thisClient.RegisterHandler(MasterMsgTypes.ucMsg, OnClientMessage);
		NetworkServer.RegisterHandler(MasterMsgTypes.vcMsg, OnServerMessage);
		thisClient.RegisterHandler(MasterMsgTypes.vcMsg, OnServerMessage);
		networkAddress = hostAddress.text;
	}

	public void StopLocalGame()//quit and stop server
	{
		s_NetworkManager.singleton.StopHost ();
	}

	void OnClientMessage(NetworkMessage netMsg)
	{
		var nmsg = netMsg.ReadMessage<MasterMsgTypes.UCMsg> ();
		NetworkServer.SendToAll (MasterMsgTypes.vcMsg, (MessageBase)nmsg);
		//sysMessage (nmsg.sender + " : " + nmsg.msg);
	}

	void OnServerMessage(NetworkMessage netMsg)
	{
		var nmsg = netMsg.ReadMessage<MasterMsgTypes.UCMsg> ();
		switch (nmsg.msg) {
		case "palm":
		case "palml":
		case "palmul":
		case "sign":
		case "close":
		case "signlocal":
		case "signliftup":
		case "deletepen":
			sysMessage (nmsg.sender + " : " + nmsg.msg);
			break;
		default :
			break;
		}

	}

	public virtual void OnServerEvent(MasterMsgTypes.NetworkMasterServerEvent evt)
	{
		if (evt == MasterMsgTypes.NetworkMasterServerEvent.MessageSent)
		{
		}
	}

	public override void OnServerConnect (NetworkConnection conn)
	{
		DebugLog (conn.address + " Has connected!");
		DebugLog ("Connection ID: " + conn.connectionId);
		Clients.Add (conn);
	}
		
	public override void OnServerDisconnect (NetworkConnection conn)
	{
		DebugLog (conn.address + " Has disconnected!");
		DebugLog ("Connection ID: " + conn.connectionId);
		Clients.Remove (conn);
	}

	public override void OnClientConnect (NetworkConnection conn)
	{
		DebugLog ("Successfully connected!");
	}

	public override void OnClientDisconnect (NetworkConnection conn)
	{
		DebugLog ("Disconnected!");
	}
		
	public override void OnStartHost ()
	{
		
	}


	public override void OnStopHost ()
	{
	}

	public void JoinLocalGame()//join server
	{
		thisClient = s_NetworkManager.singleton.StartClient();
		//NetworkServer.Listen(s_NetworkManager.singleton.networkPort);
		thisClient.RegisterHandler(MasterMsgTypes.ucMsg, OnClientMessage);
		thisClient.RegisterHandler(MasterMsgTypes.vcMsg, OnServerMessage);
	}

	public void QuitLocalGame()//quit server
	{
		s_NetworkManager.singleton.StopClient();
	}

	public void GetServerInfo()
	{
	}

	public void LogMessage(){
		var msg = new MasterMsgTypes.UCMsg ();
		msg.msg = inputMessage.text;
		msg.sender = userName.text;
		thisClient.Send (MasterMsgTypes.ucMsg, msg);
		inputMessage.text = "";
	}

	public void SendCommand(string command){
		var msg = new MasterMsgTypes.UCMsg ();
		msg.msg = command;
		msg.sender = "Server Command";
		thisClient.Send (MasterMsgTypes.ucMsg, msg);
	}

	public void SendCountryDelete(string country){
		var msg = new MasterMsgTypes.UCMsg ();
		msg.msg = "deletepen";
		msg.sender = country;
		msg.coor = 0;
		thisClient.Send (MasterMsgTypes.ucMsg, msg);
	}

	void sysMessage(string msg) {
		DebugLog(msg);
	}

	void DebugLog(object log)
	{
		Debug.Log (log);
		serverLog.text = serverLog.text + log.ToString () + " \n";
	}
	/*public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId)
    {
        Vector3 spawPos = Vector3.right * conn.connectionId;
 
        GameObject player = Instantiate(base.playerPrefab, spawPos, Quaternion.identity) as GameObject;
 
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }*/
}
