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

using System.IO;
using System.Security.Cryptography;
using System.Linq;

public class CryptoHelper
{
		public static string PublicKey = null;
		public static string Token = null;

		public static byte[] Base64DecodeString(byte[] b64encoded)
		{
			var d= System.Text.Encoding.UTF8.GetString(b64encoded).ToCharArray();
    		byte[] decodedByteArray = Convert.FromBase64CharArray(d, 0, d.Length);
    		return decodedByteArray;
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
				aesAlg.Padding = PaddingMode.None;
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

		static public byte[] Encrypt(byte[] input, byte[] key){
			// Check arguments.
            if (input == null || input.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
            	throw new ArgumentNullException("Key");
		    //if (IV == null || IV.Length <= 0)
		    //     throw new ArgumentNullException("IV");
		    byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
				aesAlg.Mode = CipherMode.CBC;
				/*
        aesAlg.KeySize = 128;
        aesAlg.BlockSize = 128;
        aesAlg.FeedbackSize = 128;
        aesAlg.Padding = PaddingMode.Zeros;
        aesAlg.Key = key;
        aesAlg.IV = iv;
		*/
                aesAlg.Key = key;
				//Debug.Log("AES KEY " + key + " " + Encoding.UTF8.GetBytes(key));
                //aesAlg.IV = IV;
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
                            swEncrypt.Write(input);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
		}
}

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

	static public string PrivateKey = "pablomarquezARG1";


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

	static public void print_bytes(byte[] array)
	{
		//for()
		//Debug.Log(String.Format("{0,10:X}", incommingData[0]) + " " + incommingData[1]);
	}

}

// We send this OPEN_SESSION msg to the server to get the session TOKEN
public class ProtoOpenSession : ProtoBase
{
	public ProtoOpenSession() : base(4)
	{
		short header = EncodeShort(ProtoBase.ProtocolNumbers["OPEN_SESSION"]);
		mBytes = new byte[] { GetLowByte(header), GetHighByte(header), 0x00, 0x04 };
		Debug.Log("Ecoded " + mBytes);
	}
}

class ProtoSessionOpened : ProtoBase
{
	private byte[] mToken = new byte[64];
	public  byte[] GetToken() { return mToken; }
	public ProtoSessionOpened(byte[] data) : base(64)
	{
		//mToken = decryption(data, ServerGlobals.aes_key)
		//mToken = CryptoHelper.Decrypt(data,Encoding.ASCII.GetBytes(ProtoBase.PrivateKey));
	}

}
/*
176         def __init__(self, token):
177                 self.token = token
178
179         def getEncodedData(self):
180                 encrypted_token = encryption(self.token, ServerGlobals.aes_key )
181                 data = ncd.encode_short( LoginProtocolMessages.protocolNumbers['SESSION_OPENED'] )
182                 data += ncd.encode_short( 4 + len(encrypted_token) )
183                 data += encrypted_token
184                 return data
185
186         @staticmethod
187         def createFromEncodedData(data):
188                 token = decryption(data, ServerGlobals.aes_key)
189                 print(" ServerGlobals.aes_key " , ServerGlobals.aes_key)
190                 print("TOKENNNNN: " , token)
191                 return SessionOpenedMessage( token )
*/

public class ProtoLoginRequest : ProtoBase
{
	public ProtoLoginRequest(byte[] data) : base((uint)data.Length)
	{
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

	//private Queue<ProtoBase> mSendQueue = new Queue<ProtoBase>();
	// Construct a ConcurrentQueue.
    private ConcurrentQueue<ProtoBase> mSendQueue = new ConcurrentQueue<ProtoBase>();
	#endregion

	static Dictionary<short, Func<TCPClient, byte[], int>> PocessFunctions
        = new Dictionary<short, Func<TCPClient, byte[], int>>
    {
        { ProtoBase.ProtocolNumbers["SESSION_OPENED"], (@this, x) => @this.ProcessSessionOpened(x) },
    };

	public int ProcessSessionOpened(byte[] encrypted_token)
	{
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
		 return 1;
	}

	void Start () {
		Debug.Log("Initializing TCPClient");
		mIncommingData = new List<byte>();
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

	private int ProcessPacket(short id, byte[] data)
	{
		return PocessFunctions[id](this,data);
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
						// Copy the bytes received from the network to the array incommingData
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						Debug.Log("Read " + length + " bytes from server. " + incommingData + "{" + incommingData + "}");
						// Apprend the bytes to any excisting data previously received
						mIncommingData.AddRange(incommingData);
						//Attempt to build as many packets and process them
						bool failed_to_build_packet = false;
						// We consume the packets
						while( mIncommingData.Count>4 && !failed_to_build_packet)
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
