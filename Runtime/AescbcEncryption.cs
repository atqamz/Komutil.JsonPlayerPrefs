// ==================================================
// 
//   Created by Atqa Munzir
// 
// ==================================================
using System;
using System.Security.Cryptography;
using System.Text;
namespace Komutil.JsonPlayerPrefs
{
	public static class AescbcEncryption
	{
		public static (string CipherText, string Key, string IV) EncryptJson(string json, string password)
		{
			// Generate a random 32-byte key from the password
			using (Rfc2898DeriveBytes keyDerivationFunction = new Rfc2898DeriveBytes(password, 16, 100000))
			{
				byte[] key = keyDerivationFunction.GetBytes(32); // 256-bit key
				byte[] salt = keyDerivationFunction.Salt;

				// Generate a random 16-byte IV
				byte[] iv = new byte[16];
				using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
				{
					rng.GetBytes(iv);
				}

				// Encrypt the JSON string
				using (Aes aes = Aes.Create())
				{
					aes.Key = key;
					aes.IV = iv;
					aes.Mode = CipherMode.CBC;
					aes.Padding = PaddingMode.PKCS7;

					using (ICryptoTransform encryptor = aes.CreateEncryptor())
					{
						byte[] plainTextBytes = Encoding.UTF8.GetBytes(json);
						byte[] cipherText = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);

						// Combine salt, IV, and ciphertext for storage/transmission
						byte[] result = new byte[salt.Length + iv.Length + cipherText.Length];
						Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
						Buffer.BlockCopy(iv, 0, result, salt.Length, iv.Length);
						Buffer.BlockCopy(cipherText, 0, result, salt.Length + iv.Length, cipherText.Length);

						// Return as Base64
						return (Convert.ToBase64String(result), Convert.ToBase64String(key),
							Convert.ToBase64String(iv));
					}
				}
			}
		}

		public static string Decrypt(string encryptedData, string password)
		{
			byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

			// Extract salt, IV, and ciphertext
			byte[] salt = new byte[16];
			byte[] iv = new byte[16];
			byte[] cipherText = new byte[encryptedBytes.Length - salt.Length - iv.Length];

			Buffer.BlockCopy(encryptedBytes, 0, salt, 0, salt.Length);
			Buffer.BlockCopy(encryptedBytes, salt.Length, iv, 0, iv.Length);
			Buffer.BlockCopy(encryptedBytes, salt.Length + iv.Length, cipherText, 0, cipherText.Length);

			// Derive key from password
			using (Rfc2898DeriveBytes keyDerivationFunction = new Rfc2898DeriveBytes(password, salt, 100000))
			{
				byte[] key = keyDerivationFunction.GetBytes(32); // 256-bit key

				// Decrypt
				using (Aes aes = Aes.Create())
				{
					aes.Key = key;
					aes.IV = iv;
					aes.Mode = CipherMode.CBC;
					aes.Padding = PaddingMode.PKCS7;

					using (ICryptoTransform decryptor = aes.CreateDecryptor())
					{
						byte[] plainTextBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
						return Encoding.UTF8.GetString(plainTextBytes);
					}
				}
			}
		}
	}
}