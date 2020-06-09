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
