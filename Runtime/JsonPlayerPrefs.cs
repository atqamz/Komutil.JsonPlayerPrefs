// ==================================================
// 
//   Created by Atqa Munzir
// 
// ==================================================
using UnityEngine;

namespace Komutil.JsonPlayerPrefs
{
	public static class JsonPlayerPrefs
	{
		// EXAMPLE USAGE:
		// JsonPlayerPrefs.SetInt("testKey", 18);
		// JsonPlayerPrefs.Save();
		// int i = JsonPlayerPrefs.GetInt("testKey");
		private static JsonPlayerPrefsImpl _playerPrefs;

		private static JsonPlayerPrefsImpl PlayerPrefs
		{
			get
			{
				if (_playerPrefs == null)
				{
					_playerPrefs = new JsonPlayerPrefsImpl(
						$"{Application.persistentDataPath}/PlayerPrefs.json",
						"KhalishGantengAbies"
					);
					Debug.Log($"Initializing JsonPlayerPrefsImpl with save path: {"{Application.persistentDataPath}/PlayerPrefs.json"}");
				}
				return _playerPrefs;
			}
		}
		
		/// <summary>
		///     Removes all keys and values from the preferences. Use with caution.
		/// </summary>
		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		/// <summary>
		///     Removes key and its corresponding value from the preferences.
		/// </summary>
		public static void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(key);
		}
		
		/// <summary>
		///     Returns the value corresponding to key in the preferences if it exists.
		/// </summary>
		public static float GetFloat(string key, float defaultValue = 0f)
		{
			return PlayerPrefs.GetFloat(key, defaultValue);
		}

		/// <summary>
		///     Returns the value corresponding to key in the preferences if it exists.
		/// </summary>
		public static int GetInt(string key, int defaultValue = 0)
		{
			return PlayerPrefs.GetInt(key, defaultValue);
		}
		
		/// <summary>
		///     Returns the value corresponding to key in the preferences if it exists.
		/// </summary>
		public static string GetString(string key, string defaultValue = "")
		{
			return PlayerPrefs.GetString(key, defaultValue);
		}

		/// <summary>
		///		Returns true if the key exists in the preferences.
		/// </summary>
		public static bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(key);
		}
		
		/// <summary>
		///		Writes all modified preferences to disk.
		/// </summary>
		public static void Save()
		{
			PlayerPrefs.Save();
		}
		
		/// <summary>
		///     Sets the value of the preference identified by key.
		/// </summary>
		public static void SetFloat(string key, float value, bool encrypt = false)
		{
			PlayerPrefs.SetFloat(key, value, encrypt);
		}
		
		/// <summary>
		///		Sets the value of the preference identified by key.
		/// </summary>
		public static void SetInt(string key, int value, bool encrypt = false)
		{
			PlayerPrefs.SetInt(key, value, encrypt);
		}
		
		/// <summary>
		///		Sets the value of the preference identified by key.
		/// </summary>
		public static void SetString(string key, string value, bool encrypt = false)
		{
			PlayerPrefs.SetString(key, value, encrypt);
		}
	}
}