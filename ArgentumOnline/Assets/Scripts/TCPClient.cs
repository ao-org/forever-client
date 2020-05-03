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
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;


//Base class for all protocol messages
public class ProtoBase
{
	static public IDictionary<string,short> ProtocolNumbers = new Dictionary<string,short>()
                        {
                        	{"OPEN_SESSION"			, unchecked((short)0x00AA)},
							{"SESSION_OPENED"		, unchecked((short)0xBBBB)},
							{"SESSION_ERROR"		, unchecked((short)0xBBB1)},
							{"LOGIN_REQUEST"		, unchecked((short)0xDEAD)},
							{"LOGIN_OKAY"       	, unchecked((short)0xAFA1)},
							{"LOGIN_ERROR"			, unchecked((short)0xAFA0)},
							{"SIGNUP_REQUEST"   	, unchecked((short)0xBEEF)},
							{"SIGNUP_OKAY"			, unchecked((short)0xBFB1)},
							{"SIGNUP_ERROR"			, unchecked((short)0xBFB0)},
							{"ACTIVATE_REQUEST"		, unchecked((short)0xBAAD)},
							{"ACTIVATE_OKAY"		, unchecked((short)0x7777)},
							{"ACTIVATE_ERROR"		, unchecked((short)0x8888)},
							{"CODE_REQUEST"			, unchecked((short)0xDAAB)},
						    {"CODE_REQ_OKAY"		, unchecked((short)0x1111)},
							{"CODE_REQ_ERROR"		, unchecked((short)0x2222)},
							{"NEW_PASSWORD"			, unchecked((short)0xFAAA)},
							{"NEW_PASSWORD_OKAY"	, unchecked((short)0x3333)},
							{"NEW_PASSWORD_ERROR"   , unchecked((short)0x4444)},
							{"FORGOT_PASSWORD"		, unchecked((short)0xCBCB)},
							{"FORGOT_PASSWORD_OKAY"	, unchecked((short)0x2014)},
							{"FORGOT_PASSWORD_ERROR", unchecked((short)0x2015)},
							{"RESET_PASSWORD"		, unchecked((short)0xFBFB)},
							{"RESET_PASSWORD_OKAY"	, unchecked((short)0x2016)}
						};

	public ProtoBase(uint size)
	{
		mBytes = new Byte[size];
	}
	protected byte[] mBytes;
	public byte[] Data() { return mBytes; }
	public int Size() { return mBytes.Length; }

	static public byte GetHighByte(short s)
	{
		byte ret = (byte)((s>>8)&0xFF);
		return ret;
	}
	static public byte GetLowByte(short s)
	{
		byte ret = (byte)(s&0xFF);
		return ret;
	}
	static public short EncodeShort(short s)
	{
		 /*
		 	Different computers use different conventions for ordering the bytes within multibyte integer values. Some computers put
			the most significant byte first (known as big-endian order) and others put the least-significant byte first (known as little-endian order).
			To work with computers that use different byte ordering, all integer values that are sent over the network are sent
			in network byte order which has the most significant byte first.

			The HostToNetworkOrder method converts multibyte integer values that are stored on the host system from the byte order
			used by the host to the byte order used by the network
		 */
		 short i = System.Net.IPAddress.HostToNetworkOrder(s);
		 Debug.Log("System.Net.IPAddress.HostToNetworkOrder(" + s + ")" + " i " + i);
		 return i;
	}

}

public class ProtoOpenSession : ProtoBase
{
	public ProtoOpenSession() : base(4)
	{
		short header = EncodeShort(ProtoBase.ProtocolNumbers["OPEN_SESSION"]);
		mBytes = new byte[] { GetLowByte(header), GetHighByte(header), 0x00, 0x04 };
		Debug.Log("Ecoded " + mBytes);
	}
}


public class TCPClient : MonoBehaviour {
	#region private members
	private TcpClient 	mSocket;
	/*
		NetworkStream stream = mSocket.GetStream() will be accesed from two different threads: Receive and Send workloads.
		According to MS Documentation there is no need for a mutex as this is supposed to be thread safe when using just 2 threads:
		Read and write operations can be performed simultaneously on an instance of the NetworkStream class without the need
		for synchronization. As long as there is one unique thread for the write operations and one unique thread for the read
		operations, there will be no cross-interference between read and write threads and no synchronization is required.
	*/
	private Thread 		mReceiveThread;
	private Thread 		mSendThread;
	private string 		mServerIP;
	private string 		mServerPort;

	//private Queue<ProtoBase> mSendQueue = new Queue<ProtoBase>();
	// Construct a ConcurrentQueue.
    private ConcurrentQueue<ProtoBase> mSendQueue = new ConcurrentQueue<ProtoBase>();
	#endregion

	void Start () {
		//mSendQueue.Clear();
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

	private void CreateSendWorkload()
	{
		try {
 			mReceiveThread = new Thread (new ThreadStart(WaitAndSendMessageWorkload));
			mReceiveThread.IsBackground = true;
			mReceiveThread.Start();
	    }
		catch (Exception e) {
			Debug.Log("On client connect exception " + e);
			OnConnectionError(e);
		}
	}

   	private void OnConnectionEstablished()
   	{

	   Debug.Log("OnConnectionEstablished!!!");
	   //Upon connection we create the send workload which will be responsible for
	   //sending messages to the server through the tpc connection.
	   CreateSendWorkload();
	   //Now the workload is running we push the message to the send queue to be
	   //consumed by the workload
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
			mReceiveThread = new Thread (new ThreadStart(ListenForDataWorkload));
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
	private void ListenForDataWorkload() {
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
						Debug.Log("Read " + length + " bytes from server. " + incommingData + "{" + incommingData + "}");

						Debug.Log(String.Format("{0,10:X}", incommingData[0]) + " " + incommingData[1]);
						//TODO HANDLE MESSAGES FROM SERVER

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
		while (true) {
			try {
				// Get a stream object for writing.
				NetworkStream stream = mSocket.GetStream();
				while (mSendQueue.Count > 0)
				{
					if (stream.CanWrite) {
						ProtoBase msg;
						if (mSendQueue.TryDequeue(out msg))
      					{
         					Debug.Log("msg {" + msg.Data() + "}");
							stream.Write(msg.Data(), 0, msg.Size());
						}
					}
				}
			}
			catch (SocketException socketException) {
				Debug.Log("Socket exception: " + socketException);
				OnConnectionError(socketException);
			}
		}
	}

	/// <summary>
	/// Send message to server using socket connection.
	/// </summary>
	private void SendMessage(ProtoBase msg) {
		mSendQueue.Enqueue(msg);
	}
}
