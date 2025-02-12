﻿// ==================================================
// 
//   Created by Atqa Munzir
// 
// ==================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Komutil.JsonPlayerPrefs
{
	/// <summary>
	///     A replacement for Unity's PlayerPrefs that stores data in a JSON file.
	/// </summary>
	internal class JsonPlayerPrefsImpl
	{
		private List<PlayerPref> playerPrefs = new List<PlayerPref>();
		private string encryptSalt;
		private string savePath;

		// Constructor
		public JsonPlayerPrefsImpl(string savePath, string encryptSalt = "salt")
		{
			this.savePath = savePath;
			this.encryptSalt = encryptSalt;
			// try to load existing data
			if (File.Exists(savePath))
			{
				using (StreamReader reader = new StreamReader(savePath))
				{
					string json = reader.ReadToEnd();
					JsonPlayerPrefsImpl data = JsonUtility.FromJson<JsonPlayerPrefsImpl>(json);
					playerPrefs = data.playerPrefs;
				}
			}
		}

		/// <summary>
		///     Removes all keys and values from the preferences. Use with caution.
		/// </summary>
		public void DeleteAll()
		{
			playerPrefs.Clear();
		}

		/// <summary>
		///     Removes key and its corresponding value from the preferences.
		/// </summary>
		public void DeleteKey(string key)
		{
			for (int i = playerPrefs.Count - 1; i >= 0; i--)
			{
				if (playerPrefs[i].Key == key)
				{
					playerPrefs.RemoveAt(i);
				}
			}
		}

		/// <summary>
		///     Returns the value corresponding to key in the preference file if it exists.
		/// </summary>
		public float GetFloat(string key, float defaultValue = 0f)
		{
			if (TryGetPlayerPref(key, out var playerPref))
			{
				if (float.TryParse(playerPref.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
				{
					return playerPref.Encrypt
						? float.Parse(AescbcEncryption.Decrypt(playerPref.Value, encryptSalt), NumberStyles.Any,
							CultureInfo.InvariantCulture)
						: value;
				}
			}

			return defaultValue;
		}

		/// <summary>
		///     Returns the value corresponding to key in the preference file if it exists.
		/// </summary>
		public int GetInt(string key, int defaultValue = 0)
		{
			if (TryGetPlayerPref(key, out var playerPref))
			{
				if (int.TryParse(playerPref.Value, out var value))
				{
					return playerPref.Encrypt
						? int.Parse(AescbcEncryption.Decrypt(playerPref.Value, encryptSalt))
						: value;
				}
			}

			return defaultValue;
		}

		/// <summary>
		///     Returns the value corresponding to key in the preference file if it exists.
		/// </summary>
		public string GetString(string key, string defaultValue = "")
		{
			if (TryGetPlayerPref(key, out var playerPref))
			{
				return playerPref.Encrypt ? AescbcEncryption.Decrypt(playerPref.Value, encryptSalt) : playerPref.Value;
			}

			return defaultValue;
		}

		/// <summary>
		///     Returns true if key exists in the preferences.
		/// </summary>
		public bool HasKey(string key)
		{
			for (int i = 0; i < playerPrefs.Count; i++)
			{
				if (playerPrefs[i].Key == key)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		///     Writes all modified preferences to disk.
		/// </summary>
		public void Save()
		{
			// create directory if it doesn't already exist
			string directory = Path.GetDirectoryName(savePath);
			Directory.CreateDirectory(directory);
			
			// serialize and save file
			string json = JsonUtility.ToJson(this);
			using (StreamWriter writer = new StreamWriter(savePath))
			{
				writer.WriteLine(json);
			}
		}

		/// <summary>
		///     Sets the value of the preference identified by key.
		/// </summary>
		public void SetFloat(string key, float value, bool encrypt = false)
		{
			SetString(key, value.ToString(CultureInfo.InvariantCulture), encrypt);
		}

		/// <summary>
		///     Sets the value of the preference identified by key.
		/// </summary>
		public void SetInt(string key, int value, bool encrypt = false)
		{
			SetString(key, value.ToString(), encrypt);
		}

		/// <summary>
		///     Sets the value of the preference identified by key.
		/// </summary>
		public void SetString(string key, string value, bool encrypt = false)
		{
			PlayerPref playerPref;
			value = encrypt ? AescbcEncryption.EncryptJson(value, encryptSalt).CipherText : value;

			if (TryGetPlayerPref(key, out playerPref))
			{
				playerPref.Value = value;
				playerPref.Encrypt = encrypt;
			}
			else
			{
				playerPrefs.Add(new PlayerPref(key, value, encrypt));
			}
		}

		private bool TryGetPlayerPref(string key, out PlayerPref playerPref)
		{
			playerPref = null;
			for (int i = 0; i < playerPrefs.Count; i++)
			{
				if (playerPrefs[i].Key == key)
				{
					playerPref = playerPrefs[i];
					return true;
				}
			}
			return false;
		}

		[Serializable]
		private class PlayerPref
		{
			public string Key;
			public string Value;
			public bool Encrypt;

			public PlayerPref(string key, string value, bool encrypt = false)
			{
				Key = key;
				Value = value;
				Encrypt = encrypt;
			}
		}
	}
}