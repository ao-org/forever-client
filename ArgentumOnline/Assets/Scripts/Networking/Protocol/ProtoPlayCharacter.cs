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

public class ProtoPlayCharacter : ProtoBase
{
	public ProtoPlayCharacter(string charname, string token){
		Debug.Log("ProtoPlayCharacter: " + charname + " " + token );
		Debug.Assert(charname.Length>0);
		short header = EncodeShort(ProtoBase.ProtocolNumbers["PLAY_CHARACTER"]);
		var encrypted_charname = CryptoHelper.Encrypt(charname, Encoding.ASCII.GetBytes(CryptoHelper.PublicKey));
		var encrypted_token = CryptoHelper.Encrypt(token, Encoding.ASCII.GetBytes(ProtoBase.PrivateKey));
        Debug.Log("encrypted_token " + encrypted_token);
		int buffer_size = /* header */ 4 + /* len(encrypted_charname) */ 2 +
						   /* actual size of encrypted_charname*/ encrypted_charname.Length +
						   /* actual size of encrypted_token*/ encrypted_token.Length;

		short encoded_size = (short)EncodeShort((short)buffer_size);
		short encoded_len_token = (short)EncodeShort((short)encrypted_token.Length);
		mBytes = new Byte[buffer_size];
		ProtoBase.WriteShortToArray(mBytes,0,header);
		ProtoBase.WriteShortToArray(mBytes,2,encoded_size);

		byte[] tmp = new byte[2+ encrypted_charname.Length + encrypted_token.Length];
		// Write username and size to the tmp buffer
		ProtoBase.WriteShortToArray(tmp,0,encoded_len_token);
		Array.Copy(encrypted_token, 0, tmp, 2, encrypted_token.Length);
        // write encrypted charname
        Array.Copy(encrypted_charname, 0, tmp, 2 + encrypted_token.Length, encrypted_charname.Length);
		Array.Copy(tmp,0, mBytes,4,tmp.Length);
	}
}
