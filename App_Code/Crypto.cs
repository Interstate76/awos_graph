using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace krypto
{
	public class Crypto
	{
		static string passPhraseX = "%a%5fr@se";
		static string saltValueX = "s@ t$r#uy";
		static string hashAlgorithmX = "SHA1";
		static int passwordIterationsX = 2;
		static string initVectorX = "@1B$c3%4e5_6g(H+";

		static int keySizeX = 256;
		public object passPhrase {
			get { return passPhraseX; }
		}
		public object saltValue {
			get { return saltValueX; }
		}
		public object hashAlgorithm {
			get { return hashAlgorithmX; }
		}
		public object passwordIterations {
			get { return passwordIterationsX; }
		}
		public object initVector {
			get { return initVectorX; }
		}
		public object keySize {
			get { return keySizeX; }
		}

		//Public Sub New()
		//    passPhraseX
		//    saltValueX
		//    hashAlgorithmX
		//    passwordIterationsX()
		//    initVectorX
		//    keySizeX()
		//End Sub

		/// <summary>
		/// szyfrowanie tekstu
		/// </summary>
		/// <param name="strIN">string wejściowy</param>
		/// <returns>ala ma kota</returns>
		/// <remarks>a kot ma AIDS</remarks>
		public static string Encrypt(string strIN)
		{

			byte[] initVectorBytes = null;
			initVectorBytes = Encoding.ASCII.GetBytes(initVectorX);

			byte[] saltValueBytes = null;
			saltValueBytes = Encoding.ASCII.GetBytes(saltValueX);

			byte[] plainTextBytes = null;
			plainTextBytes = Encoding.UTF8.GetBytes(strIN);

			PasswordDeriveBytes password = null;
			password = new PasswordDeriveBytes(passPhraseX, saltValueBytes, hashAlgorithmX, passwordIterationsX);

			byte[] keyBytes = null;
			keyBytes = password.GetBytes(keySizeX / 8);

			RijndaelManaged symmetricKey = null;
			symmetricKey = new RijndaelManaged();

			symmetricKey.Mode = CipherMode.CBC;

			ICryptoTransform encryptor = null;
			encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

			MemoryStream memoryStream = null;
			memoryStream = new MemoryStream();

			CryptoStream cryptoStream = null;
			cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
			cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

			cryptoStream.FlushFinalBlock();

			byte[] cipherTextBytes = null;
			cipherTextBytes = memoryStream.ToArray();

			memoryStream.Close();
			cryptoStream.Close();

			string cipherText = null;
			cipherText = Convert.ToBase64String(cipherTextBytes);

			return cipherText;
		}

		/// <summary>
		/// deszyfrowanie tekstu
		/// </summary>
		/// <param name="strIN">zaszyfrowany string</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static string Decrypt(string strIN)
		{

			byte[] initVectorBytes = null;
			initVectorBytes = Encoding.ASCII.GetBytes(initVectorX);

			byte[] saltValueBytes = null;
			saltValueBytes = Encoding.ASCII.GetBytes(saltValueX);


			byte[] cipherTextBytes = null;
			cipherTextBytes = Convert.FromBase64String(strIN);


			PasswordDeriveBytes password = null;
			password = new PasswordDeriveBytes(passPhraseX, saltValueBytes, hashAlgorithmX, passwordIterationsX);

			byte[] keyBytes = null;
			keyBytes = password.GetBytes(keySizeX / 8);

			RijndaelManaged symmetricKey = null;
			symmetricKey = new RijndaelManaged();

			symmetricKey.Mode = CipherMode.CBC;

			ICryptoTransform decryptor = null;
			decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

			MemoryStream memoryStream = null;
			memoryStream = new MemoryStream(cipherTextBytes);

			CryptoStream cryptoStream = null;
			cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

			byte[] plainTextBytes = null;
			plainTextBytes = new byte[cipherTextBytes.Length + 1];

			int decryptedByteCount = 0;
			decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

			memoryStream.Close();
			cryptoStream.Close();

			string plainText = null;
			plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

			return plainText;
		}
	}
}

