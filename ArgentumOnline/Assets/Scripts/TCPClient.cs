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
using UnityEngine.UI;

using System.IO;
using System.Security.Cryptography;
using System.Linq;



public class CryptoHelper
{
		public static string PublicKey = null;
		public static string Token = null;

		public static byte[] Base64DecodeString(byte[] b64encoded)
		{
			var d= System.Text.Encoding.ASCII.GetString(b64encoded).ToCharArray();
    		byte[] decodedByteArray = Convert.FromBase64CharArray(d, 0, d.Length);
    		return decodedByteArray;
		}

		public static byte[] Base64EncodeBytes(byte[] bytes)
		{
			char[] encodedArray = new char[1024];
			int size = Convert.ToBase64CharArray (bytes, 0, bytes.Length,encodedArray , 0,Base64FormattingOptions.None);
			var d = System.Text.Encoding.ASCII.GetBytes(encodedArray);
			byte[] outArray = new byte[size];
			Array.Copy(d,0, outArray, 0, size);
			return outArray;
		}

		static public string Decrypt(byte[] input, byte[] key)
		{
			// Check arguments.
			if (input == null || input.Length <= 0)
				throw new ArgumentNullException("input");
			if (key == null || key.Length <= 0)
				throw new ArgumentNullException("Key");

			string plaintext = null;
			byte[] decodedByteArray =  Base64DecodeString(input);
			//Debug.Log("decodedByteArray: " + Encoding.UTF8.GetString (decodedByteArray));
		  	// Create an Aes object
		  	// with the specified key and IV.
		  	using (Aes aesAlg = Aes.Create())
		  	{
				aesAlg.Key  = key;
				aesAlg.Mode = CipherMode.CFB;
				aesAlg.Padding = PaddingMode.Zeros;
				aesAlg.IV =  key;

				// Create a decryptor to perform the stream transform.
				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
				// Create the streams used for decryption.
				using (MemoryStream msDecrypt = new MemoryStream(decodedByteArray)){
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)){
						using (StreamReader srDecrypt = new StreamReader(csDecrypt)){
  							// Read the decrypted bytes from the decrypting stream
							// and place them in a string.
							plaintext = srDecrypt.ReadToEnd();
						}
					}
				}
			}
			return plaintext;
		}

		public static byte[] Encrypt(string plainText, byte[] Key)
		{
		            // Check arguments.
		            if (plainText == null || plainText.Length <= 0)
		                throw new ArgumentNullException("plainText");
		            if (Key == null || Key.Length <= 0)
		                throw new ArgumentNullException("Key");
		            byte[] encrypted;
		            // Create an Aes object
		            // with the specified key and IV.
		            using (Aes aesAlg = Aes.Create())
		            {
		                aesAlg.Key = Key;
		                aesAlg.Mode = CipherMode.CFB;
						aesAlg.Padding = PaddingMode.Zeros;
						aesAlg.IV =  Key;
		                // Create an encryptor to perform the stream transform.
		                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

		                // Create the streams used for encryption.
		                using (MemoryStream msEncrypt = new MemoryStream())
		                {
		                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
		                    {
		                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
		                        {
		                            //Write all data to the stream.
		                            swEncrypt.Write(plainText);
		                        }
		                        encrypted = msEncrypt.ToArray();
		                    }
		                }
		            }

		            // Return the encrypted bytes from the memory stream.
		            return Base64EncodeBytes(encrypted);
		}
}

//Base class for all protocol messages
public class ProtoBase
{


	static public IDictionary<string,short> LoginProtocolErrors = new Dictionary<string,short>()
						{
							{"ACCOUNT_ALREADY_EXISTS"				, 0x00},
							{"ACCOUNT_DOESNT_EXIST"					, 0x01},
							{"CANNOT_OPEN_ACCOUNT_FILE"				, 0x02},
							{"CORRUPT_ACCOUNT_FILE"					, 0x03},
							{"USER_ALREADY_HOLDS_ACTIVE_SESSION"    , 0x04},
							{"WRONG_PASSWORD"						, 0x05},
							{"USER_IS_BANNED"						, 0x06},
							{"REACHED_MAX_USER_COUNT"				, 0x07},
							{"VALIDATION_METHOD_FAILED"				, 0x08},
							{"MUST_ACTIVATE_ACCOUNT"				, 0x09},
							{"INVALID_ACTIVATION_CODE"				, 0x0A},
							{"ACCOUNT_ALREADY_ACTIVE"				, 0x0B},
							{"INVALID_EMAIL"						, 0x0C},
							{"INVALID_PUBLIC_KEY"					, 0x0D},
							{"PASSWORD_TOO_SHORT"					, 0x0E},
							{"PASSWORD_TOO_LONG"					, 0x0F},
							{"PASSWORD_IS_NOT_ALNUM"				, 0x10},
							{"OLD_PASSWORD_IS_NOT_VALID"			, 0x11},
							{"USERNAME_TOO_SHORT"					, 0x12},
							{"USERNAME_TOO_LONG"					, 0x13},
							{"USERNAME_IS_NOT_ALNUM"				, 0x14},
							{"INVALID_PASSWORD_RESET_CODE"			, 0x15},
							{"INVALID_PASSWORD_RESET_HOST"			, 0x16},
							{"TRY_LATER"							, 0x17},
							{"PASSWORD_CANNOT_CONTAIN_USERNAME"		, 0x18},
							{"INVALID_DELETE_CODE"					, 0x19},
							{"USERNAME_CANNOT_START_WITH_NUMBER"	, 0x20},
							{"PASSWORD_MUST_HAVE_ONE_UPPERCASE"		, 0x21},
							{"PASSWORD_MUST_HAVE_ONE_LOWERCASE"		, 0x22},
							{"PASSWORD_MUST_HAVE_TWO_NUMBERS"		, 0x23},
							{"CLOSED_MAINTENANCE"					, 0x24},
							{"CLOSED_BETA_TESTING"					, 0x25},
							{"NO_ERROR"								, 0xFF}
						};

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

	static public string LoginErrorCodeToString(short code){
		foreach(var pair in LoginProtocolErrors)
		{
    		if(pair.Value == code) return pair.Key;
		}
		Debug.Assert(false);
		return "TRY_LATER";
	}

	static public string PrivateKey = "pablomarquezARG1";
	public ProtoBase(){}
	public ProtoBase(uint size) {
		mBytes = new Byte[size];
	}
	protected byte[] mBytes;
	public byte[] Data() { return mBytes; }
	public int Size() { return mBytes.Length; }
	static public byte GetHighByte(short s){
		byte ret = (byte)((s>>8)&0xFF);
		return ret;
	}
	static public byte GetLowByte(short s){
		byte ret = (byte)(s&0xFF);
		return ret;
	}
	static public void WriteShortToArray(byte[] dst, int index, short s)
	{
		Debug.Assert(dst!=null);
		dst[index] = GetLowByte(s);
		dst[1+index] = GetHighByte(s);
	}

	static public short EncodeShort(short s){
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
	static public short DecodeShort(byte[] bytes){
		Debug.Assert(bytes.Length==2);
		short in_as_short = BitConverter.ToInt16(bytes, 0);
		short i = System.Net.IPAddress.NetworkToHostOrder(in_as_short);
		return i;
	}
	static public void print_bytes(byte[] array){}
}

// We send this OPEN_SESSION msg to the server to get the session TOKEN
public class ProtoOpenSession : ProtoBase
{
	public ProtoOpenSession() : base(4) {
		short header = EncodeShort(ProtoBase.ProtocolNumbers["OPEN_SESSION"]);
		mBytes = new byte[] { GetLowByte(header), GetHighByte(header), 0x00, 0x04 };
		Debug.Log("Ecoded " + mBytes);
	}
}

public class ProtoLoginRequest : ProtoBase
{
	public ProtoLoginRequest(string username, string password, string token){
		Debug.Log("ProtoLoginRequest: " + username + " " + password);
		Debug.Assert(username.Length>0);
		Debug.Assert(password.Length>0);
		short header = EncodeShort(ProtoBase.ProtocolNumbers["LOGIN_REQUEST"]);
		var encrypted_username = CryptoHelper.Encrypt(username, Encoding.ASCII.GetBytes(CryptoHelper.PublicKey));
		var encrypted_password = CryptoHelper.Encrypt(password, Encoding.ASCII.GetBytes(CryptoHelper.PublicKey));
		Debug.Log("encrypted username : " + Encoding.ASCII.GetString(encrypted_username));
		//var sdf =CryptoHelper.Decrypt(Encoding.ASCII.GetBytes(encrypted_username), Encoding.ASCII.GetBytes(CryptoHelper.PublicKey))


		//Debug.Log("encrypted_token(" + encrypted_token.Length + ") " + Encoding.ASCII.GetString(encrypted_token));
		//var du	= CryptoHelper.Decrypt(n,Encoding.ASCII.GetBytes(ProtoBase.PrivateKey));
		string  asd = CryptoHelper.Decrypt(encrypted_username, Encoding.ASCII.GetBytes(ProtoBase.PrivateKey));
		Debug.Log("DEC encrypted username : [" +  asd + "]");
		///Debug.Assert(username == asd);

		Debug.Log("Decrypted Token : " + CryptoHelper.Token);


		Debug.Log("encrypted password : " + Encoding.ASCII.GetString(encrypted_password));

		int buffer_size = /* header */ 4 + /* len(encrypted_username) */ 2 +
						   /* actual size of encrypted_username*/ encrypted_username.Length +
						   /* len(encrypted_password) */ 2 +
						   /* actual size of encrypted_password*/ encrypted_password.Length;

		short encoded_size = (short)EncodeShort((short)buffer_size);
		short encoded_len_username = (short)EncodeShort((short)encrypted_username.Length);
		short encoded_len_password = (short)EncodeShort((short)encrypted_password.Length);
		mBytes = new Byte[buffer_size];
		ProtoBase.WriteShortToArray(mBytes,0,header);
		ProtoBase.WriteShortToArray(mBytes,2,encoded_size);

		byte[] tmp = new byte[4+ encrypted_username.Length + encrypted_password.Length];
		// Write username and size to the tmp buffer
		ProtoBase.WriteShortToArray(tmp,0,encoded_len_username);
		Array.Copy(encrypted_username, 0, tmp, 2, encrypted_username.Length);
		// Write password and size to the tmp buffer
		ProtoBase.WriteShortToArray(tmp,2+encrypted_username.Length,encoded_len_password);
		Array.Copy(encrypted_password, 0, tmp, 4 + encrypted_username.Length, encrypted_password.Length);
		Array.Copy(tmp,0, mBytes,4,tmp.Length);
	}
}

public class ProtoSignupRequest : ProtoBase
{
	public ProtoSignupRequest(string username, string password, string token){
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

	static Dictionary<short, Func<TCPClient, byte[], int>> PocessFunctions
        = new Dictionary<short, Func<TCPClient, byte[], int>>
    {
        { ProtoBase.ProtocolNumbers["SESSION_OPENED"], (@this, x) => @this.ProcessSessionOpened(x) },
		{ ProtoBase.ProtocolNumbers["SESSION_ERROR"], (@this, x) => @this.ProcessSessionError(x) },
		{ ProtoBase.ProtocolNumbers["LOGIN_OKAY"], (@this, x) => @this.ProcessLoginOkay(x) },
		{ ProtoBase.ProtocolNumbers["LOGIN_ERROR"], (@this, x) => @this.ProcessLoginError(x) }
    };
	public void SetUsernameAndPassword(string u, string p){
		mUsername = u;
		mPassword = p;
	}
	public int ProcessSessionOpened(byte[] encrypted_token){
		Debug.Log("ProcessOpenSession");
		/*
			We got the ENCRYPTED_SESSION_TOKEN.
			ENCRYPTED_SESSION_TOKEN looks like 9gGYkcl6LsVbz2NfdJBJzKJQWHEZmEj4wY6RuWyDBTiNOrwia4X5gyTzCZsGQc4ds5rO/SU637+hNyKphm6vaFB0NdKLPfBuIt3Qc1L65msjWdYwuVuUuqmeuIHrIQtl
			The ENCRYPTED_SESSION_TOKEN must be decrypted with the 'private key' and decoded as shown below:

			cipher = AES.new( "pablomarquezARG1" )
			DECRYPTED_SESSION_TOKEN = cipher.decrypt(base64.b64decode(ENCRYPTED_SESSION_TOKEN)).rstrip(PADDING)
			DECRYPTED_SESSION_TOKEN will be a 64 chars string like A84XWygJIoH8bAiaiRn9N/S2DObSpZvMuXxE5A0opGY5dzkjrjCRBTmoh7/PnUTmsO4gh9nLouzEiQQsIZS68g==

			The 'public key' is the first 16 chars of the DECRYPTED_SESSION_TOKEN. The 'public key' will be used to encrypt the username and password in the next and last step of the
		 */
		Debug.Assert(encrypted_token.Length>0);
		//var encrypted_token = datadata.ToList().GetRange(4, data.Length -4).ToArray();
		Debug.Log("ProcessSessionOpened data.len " + encrypted_token.Length + " " + encrypted_token);
		//Decrypt TOKEN, store it and get PublicKey needed for the LOGIN_REQUEST message
		Debug.Log("encrypted_token(" + encrypted_token.Length + ") " + Encoding.ASCII.GetString(encrypted_token));
		CryptoHelper.Token	= CryptoHelper.Decrypt(encrypted_token,Encoding.ASCII.GetBytes(ProtoBase.PrivateKey));
		Debug.Log("Decrypted Token : " + CryptoHelper.Token);
		CryptoHelper.PublicKey = CryptoHelper.Token.Substring(0,16);
		switch(mOperationUponSessionOpened)
		{
			 case "LOGIN_REQUEST":
			 	Debug.Log("Session opened, attempting to login into account.");
			 	var login_request = new ProtoLoginRequest(mUsername, mPassword, CryptoHelper.PublicKey);
			 	SendMessage(login_request);
				break;
		 	 case "SIGNUP_REQUEST":
			 	Debug.Log("Session opened, attempting to signup.");
			   //var login_request = new ProtoLoginRequest(mUsername, mPassword, CryptoHelper.PublicKey);
			   //SendMessage(login_request);
			   	break;
			 default:
			  	break;
		}
		mOperationUponSessionOpened = "NOOP";
		return 1;
	}
	public int ProcessSessionError(byte[] data){
		Debug.Log("ProcessSessionError");
		var error_string = ProtoBase.LoginErrorCodeToString(data[0]);
		mEventsQueue.Enqueue(Tuple.Create("Cannot open session",error_string));
		return 1;
	}
	public int ProcessLoginOkay(byte[] data){
		Debug.Log("ProcessLoginOkay");
		mEventsQueue.Enqueue(Tuple.Create("LOGIN_OKAY",""));
		return 1;
	}
	public int ProcessLoginError(byte[] data){
		Debug.Log("ProcessLoginError");
		short error_code = ProtoBase.DecodeShort(data);
		var error_string = ProtoBase.LoginErrorCodeToString(error_code);
		mEventsQueue.Enqueue(Tuple.Create("LOGIN_ERROR_MSG_BOX_TITLE",error_string));
		return 1;
	}
	void Start (){
		Debug.Log("Initializing TCPClient");
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
		ProtoOpenSession open_session = new ProtoOpenSession();
 	  	SendMessage(open_session);
	}

	public void AttemptToSignup()
	{
		Debug.Log("AttemptToSignup");
		//ProtoOpenSession open_session = new ProtoOpenSession();
 	  	//SendMessage(open_session);
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
		return PocessFunctions[id](this,data);
	}

	public void OnApplicationQuit(){
            Debug.Log("TCPCLIENT Application ending after " + Time.time + " seconds");
			mAppQuit = true;
			if(mReceiveThread!=null){
				mReceiveThread.Abort();
				mReceiveThread.Join();
			}
			if(mSendThread!=null){
				mSendThread.Abort();
				mSendThread.Join();
			}
    }
	private void ListenForDataWorkload() {
		try {
			mSocket = new TcpClient(mServerIP, Convert.ToInt32(mServerPort));
			Byte[] bytes = new Byte[1024];
			if(mSocket.Connected){
				OnConnectionEstablished();
			}
			while (!mAppQuit) {
				// Get a stream object for reading
				using (NetworkStream stream = mSocket.GetStream()){
					int length;
					if(stream.CanRead)
					{
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
								while( mIncommingData.Count>=4 && !failed_to_build_packet)
								{
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
		catch(Exception e){
			Debug.Log("Socket exception: " + e);
			//OnConnectionError(e);
		}
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
         					Debug.Log("msg {" + msg.Data() + "}");
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
			catch(Exception e){
				Debug.Log("Socket exception: " + e);
				//OnConnectionError(e);
			}
		}
		Debug.Log("WaitAndSendMessageWorkload thread finished due to OnApplicationQuit event!");
	}

	/// <summary>
	/// Send message to server using socket connection.
	/// </summary>
	private void SendMessage(ProtoBase msg) {
		mSendQueue.Enqueue(msg);
	}
}
