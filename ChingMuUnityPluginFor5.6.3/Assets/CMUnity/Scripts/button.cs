using UnityEngine;
using System.Collections;

public class button : MonoBehaviour {

	// define the button server name, IP, ID.
	// Be sure they are keeping accord with server.
	public string serverName = "MC300Server";
	public string serverIP = "SH1dt006";
	public int buttonID = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// get button state
		CMVrpn.CMButton(serverName + "@" + serverIP, buttonID);
		//bool state = CMVrpn.CMButton(serverName + "@" + serverIP, buttonID);

	}
}
