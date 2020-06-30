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

public class ProtoMoveRequest : ProtoBase
{
	public ProtoMoveRequest(Vector3 pos, string token){
		Debug.Log("ProtoMoveRequest");
		short header = EncodeShort(ProtoBase.ProtocolNumbers["CHARACTER_MOVE_REQ"]);
        var bytes_nxny = new Byte[8];
        byte[] bytes_nx = BitConverter.GetBytes(pos.x);
        byte[] bytes_ny = BitConverter.GetBytes(pos.y);
        bytes_nx.CopyTo(bytes_nxny,0);
        bytes_ny.CopyTo(bytes_nxny,4);
        byte[] b64encoded_bytes_nxny = CryptoHelper.EncryptBase64(bytes_nxny);
        Debug.Log("b64encoded nxny : " + Encoding.ASCII.GetString(b64encoded_bytes_nxny));
        var str_b64encoded= System.Text.Encoding.ASCII.GetString(b64encoded_bytes_nxny);
        var encrypted_nxny = CryptoHelper.Encrypt(str_b64encoded, Encoding.ASCII.GetBytes(CryptoHelper.PublicKey));
        Debug.Log("encrypted nxny : " + Encoding.ASCII.GetString(encrypted_nxny));
#if ENAN

		var encrypted_username = CryptoHelper.Encrypt(username, Encoding.ASCII.GetBytes(CryptoHelper.PublicKey));
		var encrypted_password = CryptoHelper.Encrypt(password, Encoding.ASCII.GetBytes(CryptoHelper.PublicKey));
		Debug.Log("encrypted username : " + Encoding.ASCII.GetString(encrypted_username));
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
#endif

	}
}
