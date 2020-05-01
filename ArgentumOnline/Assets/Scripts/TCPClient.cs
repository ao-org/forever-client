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


//Base class for all protocol messages
public class ProtoBase
{
	private Byte[] mBytes;
	public ProtoBase(uint size)
	{
		mBytes = new Byte[size];
	}
	public Byte[] Data() { return mBytes; }
	public int Size() { return mBytes.Length; }
}

public class ProtoOpenSession : ProtoBase
{
	public ProtoOpenSession() : base(4u) {}

}


public class TCPClient : MonoBehaviour {
	#region private members
	private TcpClient 	mSocket;
	private Thread 		mReceiveThread;
	private Thread 		mSendThread;
	private string 		mServerIP;
	private string 		mServerPort;

	private Queue<ProtoBase> mQueue = new Queue<ProtoBase>();
	#endregion

	void Start () {
		mQueue.Clear();
		Debug.Log("Initializing TCPClient");
	}
	void Update () {
		/*
		if (Input.GetKeyDown(KeyCode.Space)) {
			//Debug.Log("SendMessage");
			SendMessage();
		}
		*/
	}

   private void OnConnectionError(Exception e)
   {
	   Debug.Log("OnConnectionError " + e.Message);

   }

   private void OnConnectionEstablished()
   {
	   //Upon connection, send the OpenSession msg
	   Debug.Log("OnConnectionEstablished!!!");
	   ProtoOpenSession open_session = new ProtoOpenSession();
	   SendMessage(open_session);
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
			OnConnectionError(e);
		}
	}
	/// <summary>
	/// Runs in background mReceiveThread; Listens for incomming data.
	/// </summary>
	private void ListenForData() {
		try {
			mSocket = new TcpClient(mServerIP, Convert.ToInt32(mServerPort));
			Byte[] bytes = new Byte[1024];
			if(mSocket.Connected){
				OnConnectionEstablished();
			}
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
		catch (SocketException socketException){
			Debug.Log("Socket exception: " + socketException);
			OnConnectionError(socketException);
		}
		catch(Exception e){
			Debug.Log("Socket exception: " + e);
			OnConnectionError(e);
		}
	}

	private void WaitAndSendMessageWorkload() {
		if (mSocket == null) {
			return;
		}
		while (true) {
			try {
				// Get a stream object for writing.
				NetworkStream stream = mSocket.GetStream();
				while (mQueue.Count > 0)
				{
					if (stream.CanWrite) {
						ProtoBase msg = mQueue.Peek();
						//string clientMessage = "This is a message from one of your clients.";
						// Convert string message to byte array.
						//byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
						// Write byte array to mSocket stream.
						stream.Write(msg.Data(), 0, msg.Size());
						Debug.Log("Client sent his message - should be received by server");
						mQueue.Dequeue();
					}
				}
			}
			catch (SocketException socketException) {
				Debug.Log("Socket exception: " + socketException);
			}
		}
	}

	/// <summary>
	/// Send message to server using socket connection.
	/// </summary>
	private void SendMessage(ProtoBase msg) {
		if (mSocket == null) {
			return;
		}
		try {
			// Get a stream object for writing.
			NetworkStream stream = mSocket.GetStream();
			if (stream.CanWrite) {
				stream.Write(msg.Data(), 0, msg.Size());
				Debug.Log("Client sent his message - should be received by server");
			}
		}
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
		}
	}
}
