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

public class ProtoCharacterSays : ProtoBase
{

	public ProtoCharacterSays(Character character, string words, string token){
		Debug.Log("ProtoCharacterSays: " + character.Name() + " " + words);
		short header = EncodeShort(ProtoBase.ProtocolNumbers["CHARACTER_SAYS"]);

		string xml_chat = CreateXMLChatString(character,words);
		Debug.Log("Account :" + xml_chat);
		var encrypted_data = CryptoHelper.Encrypt(xml_chat, Encoding.ASCII.GetBytes(CryptoHelper.PublicKey));
		Debug.Log("encrypted account : " + Encoding.ASCII.GetString(encrypted_data));

		int buffer_size = /* header */ 4 + encrypted_data.Length;
		short encoded_size = (short)EncodeShort((short)buffer_size);
		mBytes = new Byte[buffer_size];
		ProtoBase.WriteShortToArray(mBytes,0,header);
		ProtoBase.WriteShortToArray(mBytes,2,encoded_size);
		Array.Copy(encrypted_data, 0, mBytes, 4, encrypted_data.Length);
	}

    private string CreateXMLChatString(Character c, string words){
        var char_pos = c.Position();
        string xml_string = @"<?xml version=""1.0""?>
                <Chat>
                    <Sentence>
                            <words> " + words + @"</words>
                            <uuid>  "  + c.UUID() + @"</uuid>
                    </Sentence>
                    <position>
                            <map>  " + char_pos.Item1 + @"</map>
                            <x> " + char_pos.Item2.ToString()  + @"</x>
                            <y> " + char_pos.Item3.ToString()  + @"</y>
                    </position>
                </Chat>";
        return xml_string;

    }


}
