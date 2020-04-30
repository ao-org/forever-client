/*
		Copyright 2020, Pablo Ignacio Marquez Tello aka Morgolock, All rights reserved.
		Argentum Online Clasico
		noland.studios@gmail.com
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient : MonoBehaviour {
	#region private members
	private TcpClient mSocket;
	private Thread mReceiveThread;
	private string mServerIP;
	private string mServerPort;
	#endregion

	void Start () {
		Debug.Log("Initializing TCPClient");
	}
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			Debug.Log("SendMessage");
			SendMessage();
		}
	}
	/// <summary>
	/// This method attempt to establish a TCP connection with the remote host
	/// passed in as remote_ip and remote_port
	/// </summary>
	public void ConnectToTcpServer (string remote_ip, string remote_port) {
		if( mSocket!=null && mSocket.Connected )
		{
				Debug.Log("Already connected to the server!.");
				return;
		}
		Debug.Log("Trying ConnectToTcpServer " + remote_ip + ":" + remote_port);
		mServerIP = remote_ip;
		mServerPort = remote_port;
		try {
			mReceiveThread = new Thread (new ThreadStart(ListenForData));
			mReceiveThread.IsBackground = true;
			mReceiveThread.Start();
		}
		catch (Exception e) {
			Debug.Log("On client connect exception " + e);
		}
	}
	/// <summary>
	/// Runs in background mReceiveThread; Listens for incomming data.
	/// </summary>
	private void ListenForData() {
		try {
			mSocket = new TcpClient(mServerIP, Convert.ToInt32(mServerPort));
			Byte[] bytes = new Byte[1024];
			while (true) {
				// Get a stream object for reading
				using (NetworkStream stream = mSocket.GetStream()) {
					int length;
					// Read incomming stream into byte arrary.
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message.
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						Debug.Log("server message received as: " + serverMessage);
					}
				}
			}
		}
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
		}
	}
	/// <summary>
	/// Send message to server using socket connection.
	/// </summary>
	private void SendMessage() {
		if (mSocket == null) {
			return;
		}
		try {
			// Get a stream object for writing.
			NetworkStream stream = mSocket.GetStream();
			if (stream.CanWrite) {
				string clientMessage = "This is a message from one of your clients.";
				// Convert string message to byte array.
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
				// Write byte array to mSocket stream.
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
				Debug.Log("Client sent his message - should be received by server");
			}
		}
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
		}
	}
}
