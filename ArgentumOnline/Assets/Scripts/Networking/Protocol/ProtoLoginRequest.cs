/*
		Argentum Forever - Copyright 2020, Pablo Ignacio Marquez Tello aka Morgolock, All rights reserved.
		gulfas@gmail.com
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

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