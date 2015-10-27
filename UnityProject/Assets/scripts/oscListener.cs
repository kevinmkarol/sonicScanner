using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityOSC;


public class oscListener : MonoBehaviour {
	public String OSCHost = "127.0.0.1";
	public int ListenerPort = 3200;
	private Dictionary<string, ServerLog> servers;


	//Variables for smoothing/averaging orientation
	private int smoothingSize = 300;
	private int smoothingIndex;
	private float[] depth, roll, pitch, yaw;


	public GameObject sensedObject;
	public GameObject sensorObject;

	// Use this for initialization
	void Start () {
		OSCHandler.Instance.Init ();
		servers = new Dictionary<string, ServerLog>();

		//Grab the game objects
		sensorObject = GameObject.FindGameObjectWithTag ("sensorObject");
		sensedObject = GameObject.FindGameObjectWithTag ("sensedObject");

		//Initialize Values
		depth = new float[smoothingSize];
		roll = new float[smoothingSize];
		pitch = new float[smoothingSize];
		yaw = new float[smoothingSize];
		smoothingIndex = 0;

		//Setup the view and initial object locations
		Camera.main.transform.position = new Vector3 (0, 0, -500);

		sensorObject.transform.position = new Vector3(0, 0, 0);                     
		sensedObject.transform.position = new Vector3(0, 0, 0);

		sensorObject.transform.eulerAngles = new Vector3 (0, 0, 0);
		sensedObject.transform.eulerAngles = new Vector3 (0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		OSCHandler.Instance.UpdateLogs ();
		servers = OSCHandler.Instance.Servers;

		//Read in all OSC values recieved from processing
		foreach (KeyValuePair<string, ServerLog> item in servers) 
		{
			if(item.Value.log.Count > 0)
			{
				int lastPacketIndex = item.Value.packets.Count - 1;

				depth[smoothingIndex] = (float)item.Value.packets[lastPacketIndex].Data[0];
				roll[smoothingIndex] = (float)item.Value.packets[lastPacketIndex].Data[1];
				pitch[smoothingIndex] = (float)item.Value.packets[lastPacketIndex].Data[2];
				yaw[smoothingIndex] = (float)item.Value.packets[lastPacketIndex].Data[3];
				smoothingIndex = (smoothingIndex + 1) % smoothingSize;
			}
		}

		//Calculate normalized values
		int averageDepth = 0;
		int averageRoll = 0;
		int averagePitch = 0;
		int averageYaw = 0;

		foreach (int val in depth) {
			averageDepth += val;
		}

		foreach (int val in roll) {
			averageRoll += val;
		}

		foreach(int val in pitch){
			averagePitch += val;
		}

		foreach(int val in yaw){
			averageYaw += val;
		}

		//Divide by 100 for world scale
		averageDepth = (averageDepth / smoothingSize)/100;
		averageRoll = averageRoll / smoothingSize;
		averagePitch = averagePitch / smoothingSize;
		averageYaw = averageYaw / smoothingSize;


		Vector3 calculatedAngle = new Vector3(averageRoll, averagePitch, averageYaw);

		sensorObject.transform.eulerAngles = calculatedAngle;
		sensedObject.transform.eulerAngles = calculatedAngle;

		Quaternion rot = Quaternion.Euler (calculatedAngle);
		sensedObject.transform.position = new Vector3 (0, 0, 0);
		sensedObject.transform.position += calculatedAngle * averageDepth;



	}
}
