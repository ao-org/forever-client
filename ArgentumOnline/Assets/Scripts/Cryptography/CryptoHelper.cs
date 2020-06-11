/*
		Copyright 2020, Pablo Ignacio Marquez Tello aka Morgolock, All rights reserved.
		Argentum Online Clasico
		noland.studios@gmail.com
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Security.Cryptography;



public class CryptoHelper
{
		public static string PublicKey = null;
		public static string Token = null;
		public static int BufferSize = 1024*2;

		public static byte[] Base64DecodeString(byte[] b64encoded)
		{
			var d= System.Text.Encoding.ASCII.GetString(b64encoded).ToCharArray();
    		byte[] decodedByteArray = Convert.FromBase64CharArray(d, 0, d.Length);
			if(decodedByteArray.Length>500)
				Array.Resize(ref decodedByteArray, decodedByteArray.Length + 100);
    		return decodedByteArray;
		}

		public static byte[] Base64EncodeBytes(byte[] bytes)
		{
			Debug.Assert(bytes.Length>0);
			char[] encodedArray = new char[BufferSize*4];
			int size = Convert.ToBase64CharArray (bytes, 0, bytes.Length,encodedArray , 0,Base64FormattingOptions.None);
			Debug.Assert(size<encodedArray.Length);
			var d = System.Text.Encoding.ASCII.GetBytes(encodedArray);
			byte[] outArray = new byte[size];
			Array.Copy(d,0, outArray, 0, size);
			return outArray;
		}

		public static byte[] DecryptBase64(byte[] data)
		{
     		FromBase64Transform tBase64 = new FromBase64Transform();
     		MemoryStream streamDecrypted = new MemoryStream();
     		CryptoStream stream = new CryptoStream(streamDecrypted, tBase64, CryptoStreamMode.Write);
     		stream.Write(data, 0, data.Length);
     		stream.FlushFinalBlock();
     		stream.Close();
     		return streamDecrypted.ToArray();
		}


		public static byte[] EncryptBase64(byte[] data)
		{
			ToBase64Transform tBase64 = new ToBase64Transform();
			MemoryStream streamDecrypted = new MemoryStream();
			CryptoStream stream = new CryptoStream(streamDecrypted, tBase64, CryptoStreamMode.Write);
			stream.Write(data, 0, data.Length);
			stream.FlushFinalBlock();
			stream.Close();
			return streamDecrypted.ToArray();
		}

		public static string Decrypt(byte[] input, byte[] key)
		{
			Debug.Assert(input.Length>0);
			Debug.Assert(key.Length>0);
			// Check arguments.
			if (input == null || input.Length <= 0)
				throw new ArgumentNullException("input");
			if (key == null || key.Length <= 0)
				throw new ArgumentNullException("Key");

			string plaintext = null;
			byte[] decodedByteArray =  Base64DecodeString(input);
			byte[] dummy=  DecryptBase64(input);

			Debug.Log("decodedByteArray len " + decodedByteArray.Length);
			//Debug.Log("decodedByteArray  " + System.Text.Encoding.ASCII.GetString(decodedByteArray));
			byte[] buffer = new byte[decodedByteArray.Length];
			int offset =0;
			int num_read =0;
		  	using (Aes aesAlg = Aes.Create())
		  	{
				aesAlg.KeySize = 128;
				aesAlg.BlockSize = 128;
				aesAlg.FeedbackSize = 8;
				aesAlg.Mode = CipherMode.CFB;
				aesAlg.Padding = PaddingMode.Zeros;
				aesAlg.IV =  key;
				aesAlg.Key  = key;
				Debug.Log("buffer len " + buffer.Length);
				// Create a decryptor to perform the stream transform.
				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
				// Create the streams used for decryption.
				using (MemoryStream memoryStream = new MemoryStream(decodedByteArray)){
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)){

						do {
							num_read = cryptoStream.Read(buffer,offset, 1);
							Debug.Log("num_read " + num_read + " offset " + offset);
							offset+=num_read;
						} while(offset<=dummy.Length-1);

//						num_read = cryptoStream.Read(buffer,0, buffer.Length-1);
/*
						using (StreamReader streamReader = new StreamReader(cryptoStream)){
  							// Read the decrypted bytes from the decrypting stream
							// and place them in a string.

							plaintext = streamReader.ReadToEnd();
						}
*/
						cryptoStream.Close();
					}

				}
			}
			/*
			Debug.Log("decrypt offset: " + offset);
			byte[] fbuffer = new byte[offset];
			Array.Copy(buffer, 0, fbuffer, 0, offset);
			*/
			plaintext = System.Text.Encoding.ASCII.GetString(buffer);
			Debug.Log("plaintext " + plaintext);
			return plaintext;

		}






		public static void print_byte_array(byte[] byteArray){
			for (int x = 0; x < byteArray.Length; x++)
    		{
    			Debug.Log("{0:X2} " + byteArray[x]);
    			//if (((x+1)%20) == 0) Console.WriteLine();
			}
		}

		public static byte[] Encrypt(string plainText, byte[] Key)
		{
					Debug.Assert(plainText.Length>0);
					Debug.Assert(Key.Length>0);
					Debug.Log("Encrypt plainText= " +plainText + " len " + plainText.Length);
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
						aesAlg.KeySize = 128;
						aesAlg.BlockSize = 128;
						aesAlg.FeedbackSize = 8;
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
					Debug.Log("encrypted.length = " + encrypted.Length);
		            return EncryptBase64(encrypted);
		}
}
