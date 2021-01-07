using UnityEngine;
using System.Collections;

public class analog : MonoBehaviour {

	// define the analog server name, IP, ID.
	// Be sure they are keeping accord with server.
	public string serverName = "MC300Server";
	public string serverIP = "SH1dt006";
	public int analogID = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// get analog value
		CMVrpn.CMAnalog(serverName + "@" + serverIP, analogID);

	}
}
