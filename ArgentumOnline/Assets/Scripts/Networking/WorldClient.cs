/*
		Argentum Forever - Copyright 2020, Pablo Ignacio Marquez Tello aka Morgolock, All rights reserved.
		gulfas@gmail.com
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class WorldClient : MonoBehaviour {
	#region private members
	private TcpClient 	mSocket;
	/*
		NetworkStream stream = mSocket.GetStream() will be accesed from two different threads: Receive and Send workloads.
		According to MS Documentation there is no need for a mutex as this is supposed to be thread safe when using just 2 threads:
		Read and write operations can be performed simultaneously on an instance of the NetworkStream class without the need
		for synchronization. As long as there is one unique thread for the write operations and one unique thread for the read
		operations, there will be no cross-interference between read and write threads and no synchronization is required.
	*/
	private List<byte>	mIncommingData;
	private Thread 		mReceiveThread;
	private Thread 		mSendThread;
	private string 		mServerIP;
	private string 		mServerPort;
	private string		mUsername;
	private string		mPassword;
	private MainMenu	mMainMenu;
	private bool		mAppQuit;

	public bool IsSessionOpen(){
		return CryptoHelper.PublicKey.Length > 0;
	}

	public bool IsConnected(){
		if(mSocket == null){
			return false;
		}
		else {
			return mSocket.Connected;
		}
	}
	// Construct a ConcurrentQueue for Sending messages to the server
    private ConcurrentQueue<ProtoBase> mSendQueue = new ConcurrentQueue<ProtoBase>();
	// Connection events queue
	private ConcurrentQueue<Tuple<string, string>> mEventsQueue = new ConcurrentQueue<Tuple<string, string>>();
	#endregion

	private string mOperationUponSessionOpened = "NOOP";

	static Dictionary<short, Func<WorldClient, byte[], int>> ProcessFunctions
        = new Dictionary<short, Func<WorldClient, byte[], int>>
    {
		{ ProtoBase.ProtocolNumbers["PLAY_CHARACTER_OKAY"], (@this, x) => @this.ProcessPlayCharacterOkay(x) },
		{ ProtoBase.ProtocolNumbers["PLAY_CHARACTER_ERROR"], (@this, x) => @this.ProcessPlayCharacterError(x) }
    };
	public void SetUsernameAndPassword(string u, string p){
		mUsername = u;
		mPassword = p;
	}
	public int ProcessPlayCharacterOkay(byte[] data){
		Debug.Log("ProcessPlayCharacterOkay");
		mEventsQueue.Enqueue(Tuple.Create("PLAY_CHARACTER_OKAY",""));
		return 1;
	}
	public int ProcessPlayCharacterError(byte[] data){
		Debug.Log("ProcessPlayCharacterError");
		short error_code = ProtoBase.DecodeShort(data);
		var error_string = ProtoBase.LoginErrorCodeToString(error_code);
		mEventsQueue.Enqueue(Tuple.Create("LOGIN_ERROR_MSG_BOX_TITLE",error_string));
		return 1;
	}

	void Start (){
		Debug.Log("Initializing WorldClient");
		mIncommingData = new List<byte>();
		mAppQuit = false;
	}
	public void SetMainMenu(MainMenu m){
		Debug.Assert(m!=null);
		mMainMenu = m;
	}
	void ShowMessageBox(string title, string message){
		Debug.Log("ShowMessageBox " + title + " " + message);
		mMainMenu.ShowMessageBox(title,message,true);
	}
	void Update(){
		try {
			if (mEventsQueue.Count > 0){
				Tuple<string, string> e;
				if (mEventsQueue.TryDequeue(out e)){

					if(e.Item1 == "SIGNUP_OKAY"){
						mMainMenu.OnAccountCreated();
					}
					else if(e.Item1 == "ACTIVATE_OKAY") {
						mMainMenu.OnAccountActivated();
					}
					else if(e.Item1 == "LOGIN_OKAY"){
						mMainMenu.OnLoginOkay();
					}
					else { // normal message box
						if(e.Item2 !=null){
							Debug.Log("Event {" + e.Item2 + "}");
							ShowMessageBox(e.Item1,e.Item2);
						}
						else{
							ShowMessageBox(e.Item1,"Whatever");
						}
					}
				}
			}
		}
		catch (Exception e) {
			Debug.Log("Failed to read events" + e.Message);
		}
	}

    private void OnConnectionError(string title, string msg){
	   Debug.Log("OnConnectionError " + msg);
	   mEventsQueue.Enqueue(Tuple.Create(title,msg));
    }

	private void CreateSendWorkload()
	{
		Debug.Log("CreateSendWorkload");
		if(mSendThread!=null){
			Debug.Log("Destroying thread " + mSendThread.Name);
			mSendThread.Abort();
			mSendThread = null;
		}
		try {
 			mSendThread = new Thread (new ThreadStart(WaitAndSendMessageWorkload));
			mSendThread.IsBackground = true;
			mSendThread.Start();
	    }
		catch (Exception e) {
			Debug.Log("On client connect exception " + e);
			OnConnectionError("Error", "CreateSendWorkload");
		}
	}

	private void CreateListenWorkload()
	{
		Debug.Log("CreateListenWorkload");
		if(mReceiveThread!=null){
			Debug.Log("Destroying thread " + mReceiveThread.Name);
			mReceiveThread.Abort();
			mReceiveThread = null;
		}
		try {
			mReceiveThread = new Thread (new ThreadStart(ListenForDataWorkload));
			mReceiveThread.IsBackground = true;
			mReceiveThread.Name = "ListenForDataWorkload" + DateTime.Now;
			Debug.Log("Creating thread " + " ListenForDataWorkload" + DateTime.Now);
			mReceiveThread.Start();
		}
		catch (Exception e) {
			Debug.Log("On client connect exception " + e);
			OnConnectionError("Error", "CreateListenWorkload");
		}
	}

	public void AttemptToLogin()
	{
		mOperationUponSessionOpened = "LOGIN_REQUEST";
		ProtoOpenSession open_session = new ProtoOpenSession();
 	  	SendMessage(open_session);
	}

	public void AttemptToActivate()
	{
		mOperationUponSessionOpened = "ACTIVATE_REQUEST";
		ProtoOpenSession open_session = new ProtoOpenSession();
 	  	SendMessage(open_session);
	}

	public void AttemptToSignup()
	{
		mOperationUponSessionOpened = "SIGNUP_REQUEST";
		ProtoOpenSession open_session = new ProtoOpenSession();
		SendMessage(open_session);
	}

   	private void OnConnectionEstablished()
   	{
	   Debug.Log("OnConnectionEstablished!!!");
	   //Upon connection we create the send workload which will be responsible for
	   //sending messages to the server through the tpc connection.
	   //CreateSendWorkload();
	   //Now the workload is running we push the message to the send queue to be
	   //consumed by the workload
	   ProtoOpenSession open_session = new ProtoOpenSession();
	   SendMessage(open_session);
   }
	public void ConnectToTcpServer (string remote_ip, string remote_port, string operation="NOOP") {
		mOperationUponSessionOpened = operation;
		mAppQuit = false;
		if( mSocket!=null && mSocket.Connected )
		{
				Debug.Log("Already connected to the server!.");
				return;
		}
		Debug.Log("Trying ConnectToTcpServer " + remote_ip + ":" + remote_port);
		mServerIP = remote_ip;
		mServerPort = remote_port;
		CreateSendWorkload();
		CreateListenWorkload();
	}

	private int ProcessPacket(short id, byte[] data){
		Debug.Assert(ProcessFunctions.ContainsKey(id));
		return ProcessFunctions[id](this,data);
	}

	private void StopNetworkWorkloads(){
		if(mSocket!=null){
			mSocket.Close();
			mSocket = null;
		}
		if(mReceiveThread!=null){
			mReceiveThread.Abort();
			mReceiveThread.Join();
			mReceiveThread = null;
		}
		if(mSendThread!=null){
			mSendThread.Abort();
			mSendThread.Join();
			mSendThread =null;
		}

	}
	public void OnApplicationQuit(){
            Debug.Log("WorldClient Application ending after " + Time.time + " seconds");
			mAppQuit = true;
			StopNetworkWorkloads();
    }
	private void ListenForDataWorkload() {
		try {
			mSocket = new TcpClient();
			mSocket.LingerState = new LingerOption(true,0);
			mSocket.ReceiveTimeout = 1000;
			mSocket.SendTimeout = 1000;
			mSocket.NoDelay = true;
			mSocket.Connect(mServerIP, Convert.ToInt32(mServerPort));
			if(mSocket.Connected){
				OnConnectionEstablished();
			}
			//mSocket.GetStream().ReadTimeout = 1000;
			//mSocket.GetStream().WriteTimeout = 1000;
			Byte[] bytes = new Byte[1024];
			while (!mAppQuit) {
				// Get a stream object for reading
				using (NetworkStream stream = mSocket.GetStream()){
					int length;
					if(stream.CanRead){
							try {
									while ((length = stream.Read(bytes, 0, bytes.Length)) != 0){
										// Copy the bytes received from the network to the array incommingData
										var incommingData = new byte[length];
										Array.Copy(bytes, 0, incommingData, 0, length);
										Debug.Log("Read " + length + " bytes from server. " + incommingData + "{" + incommingData + "}");
										// Apprend the bytes to any excisting data previously received
										mIncommingData.AddRange(incommingData);
										//Attempt to build as many packets and process them
										bool failed_to_build_packet = false;
										// We consume the packets
										while( mIncommingData.Count>=4 && !failed_to_build_packet){
											var msg_size 	= mIncommingData.GetRange(2, 2).ToArray();
											Debug.Log(" msg_size len " + msg_size.Length);
											var header	 	= mIncommingData.GetRange(0, 2).ToArray();
											short decoded_size = ProtoBase.DecodeShort(msg_size);
											Debug.Log(" Msg_size: " + decoded_size);
											short message_id = ProtoBase.DecodeShort(header);
											Debug.Log(String.Format("{0,10:X}", header[0]) + " " + String.Format("{0,10:X}", header[1]));
											failed_to_build_packet = (decoded_size > 1024);
											//Drop the heade and size fields
											var message_data	 	= mIncommingData.GetRange(4,decoded_size-4).ToArray();
											mIncommingData.RemoveRange(0,decoded_size);
											ProcessPacket(message_id, message_data);
										}
									}
							}
							catch(IOException e){
								Debug.Log("Timeout? IOException (" + e.Message  + ") " + e);
							}
					}
				}
			}
			Debug.Log("ListenForDataWorkload thread finished due to OnApplicationQuit event!");
		}
		catch (SocketException socketException){
			Debug.Log("Socket exception (" + socketException.ErrorCode  + ") " + socketException);
			switch(socketException.ErrorCode){
				case 10061:
					OnConnectionError("CONNECTION_ERROR_MSGBOX_TITLE","CONNECTION_ERROR_CANNOT_REACH_SERVER");
					break;
				default:
					break;
			}
			//OnConnectionError(socketException);
		}
		catch(ThreadAbortException e) {
            Debug.Log("Thread - caught ThreadAbortException - resetting.");
            Debug.Log("Exception message: {0}" + e.Message);
            //Thread.ResetAbort();
        }
		catch(Exception e){
			Debug.Log("Socket exception: " + e);
			//OnConnectionError(e);
		}

		Debug.Log("WorldClient::ListenForDataWorkload finished");
	}

	private void WaitAndSendMessageWorkload() {
		while (!mAppQuit) {
			try {
				if( mSocket!=null && mSocket.Connected )
				{
					// Get a stream object for writing.
					NetworkStream stream = mSocket.GetStream();
					while (mSendQueue.Count > 0)
					{
						if (stream.CanWrite) {
							ProtoBase msg;
							if (mSendQueue.TryDequeue(out msg))
	      					{
								Debug.Assert(msg.Data()!=null);
								stream.Write(msg.Data(), 0, msg.Size());
							}
						}
					}
				}
			}
			catch (SocketException socketException) {
				Debug.Log("Socket exception: " + socketException);
				//OnConnectionError(socketException);
			}
			catch(ThreadAbortException e) {
	            Debug.Log("Thread - caught ThreadAbortException - resetting.");
	            Debug.Log("Exception message: {0}" + e.Message);
	        }
			catch(Exception e){
				Debug.Log("Socket exception: " + e);
			}
		}
		Debug.Log("WorldClient::WaitAndSendMessageWorkload finished.");
	}

	/// <summary>
	/// Send message to server using socket connection.
	/// </summary>
	private void SendMessage(ProtoBase msg) {
		mSendQueue.Enqueue(msg);
	}
}
