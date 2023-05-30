using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace Zenvin.ProjectPreferences {
	[InitializeOnLoad]
	public sealed class ProjectPrefs {

		public enum ValueOverrideOption {
			DontOverride,
			OverrideMatchingType,
			AlwaysOverride,
		}

		private static bool loaded = false;
		private static readonly Dictionary<PrefKey, PrefValue> values = new Dictionary<PrefKey, PrefValue> ();

		/// <summary> The number of preference keys. </summary>
		public static int Count => values.Count;


		static ProjectPrefs () {
			RegisterCallbacks ();
		}

		internal static void Reload () {
			if (loaded) {
				values.Clear ();
				loaded = false;
			}
			Load ();
		}

		internal static IEnumerator<KeyValuePair<PrefKey, PrefValue>> GetPreferenceEnumerator () {
			foreach (var kvp in values) {
				yield return kvp;
			}
		}


		/// <summary>
		/// Write all current keys and their values to the preferences file.
		/// </summary>
		public static void Save () {
			if (values.Count == 0) {
				return;
			}

			var path = GetPrefFilePath ();

			using (var stream = File.Create (path)) {
				using (var writer = new BinaryWriter (stream)) {
					writer.Write (values.Count);
					foreach (var val in values) {
						val.Key.Serialize (writer);
						PrefValue.Serialize (writer, val.Value);
					}
				}
			}
		}

		/// <summary>
		/// Attempts to set the value associated with the given key. <br></br>
		/// Will always replace existing values.
		/// </summary>
		public static bool SetValue (PrefKey key, PrefValue value) {
			return SetValue (key, value, ValueOverrideOption.AlwaysOverride);
		}

		/// <summary>
		/// Attempts to set the value associated with the given key. <br></br>
		/// Whether existing values are replaced, depends on the given <see cref="ValueOverrideOption"/>.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the value was set, otherwise <see langword="false"/>.
		/// </returns>
		public static bool SetValue (PrefKey key, PrefValue value, ValueOverrideOption overrideOption) {
			if (value == null || !key.Valid) {
				return false;
			}
			if (overrideOption == ValueOverrideOption.AlwaysOverride || !values.TryGetValue (key, out PrefValue existing)) {
				values[key] = value;
				return true;
			}
			if (overrideOption == ValueOverrideOption.OverrideMatchingType && existing.Type != value.Type) {
				return false;
			}
			values[key] = value;
			return true;
		}

		/// <summary>
		/// Gets the value associated with the given key as an <see cref="object"/>. <br></br>
		/// </summary>
		/// <param name="key"></param>
		/// <returns>
		/// The value associated with the given key as an <see cref="object"/>. Otherwise <see langword="null"/>.
		/// </returns>
		public static object GetValue (PrefKey key) {
			return TryGetValue (key, out object value) ? value : null;
		}

		/// <summary>
		/// Attempts to get the value associated with the given key.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the key exists in the preferences, otherwise <see langword="false"/>.
		/// </returns>
		public static bool TryGetValue (PrefKey key, out object value) {
			value = null;
			Load ();
			if (values.TryGetValue (key, out PrefValue val)) {
				switch (val.Type) {
					case KeyType.Bool:
						value = val.BoolValue;
						break;
					case KeyType.Int:
						value = val.IntValue;
						break;
					case KeyType.String:
						value = val.StringValue;
						break;
					case KeyType.Float:
						value = val.FloatValue;
						break;
				}
				return true;
			}
			return false;
		}

		public static bool GetBool (PrefKey key, bool fallback) {
			return TryGetBool (key, out bool value) ? value : fallback;
		}

		public static int GetInt (PrefKey key, int fallback) {
			return TryGetInt (key, out int value) ? value : fallback;
		}

		public static string GetString (PrefKey key, string fallback) {
			return TryGetString (key, out string value) ? value : fallback;
		}

		public static float GetFloat (PrefKey key, float fallback) {
			return TryGetFloat (key, out float value) ? value : fallback;
		}

		public static bool TryGetBool (PrefKey key, out bool value) {
			Load ();
			if (values.TryGetValue (key, out PrefValue val)) {
				value = val.BoolValue;
				return val.Type == KeyType.Bool;
			}
			value = default;
			return false;
		}

		public static bool TryGetInt (PrefKey key, out int value) {
			Load ();
			if (values.TryGetValue (key, out PrefValue val)) {
				value = val.IntValue;
				return val.Type == KeyType.Bool;
			}
			value = default;
			return false;
		}

		public static bool TryGetString (PrefKey key, out string value) {
			Load ();
			if (values.TryGetValue (key, out PrefValue val)) {
				value = val.StringValue;
				return val.Type == KeyType.Bool;
			}
			value = default;
			return false;
		}

		public static bool TryGetFloat (PrefKey key, out float value) {
			Load ();
			if (values.TryGetValue (key, out PrefValue val)) {
				value = val.FloatValue;
				return val.Type == KeyType.Bool;
			}
			value = default;
			return false;
		}


		/// <summary>
		/// Checks whether a given key exists in the preferences.
		/// </summary>
		public static bool HasKey (PrefKey key) {
			return values.ContainsKey (key);
		}

		/// <summary>
		/// Checks whether any key exists in the preferences, that matches a given filter.
		/// </summary>
		public static bool HasKey (Func<PrefKey, bool> filter) {
			foreach (var pref in values) {
				if (filter.Invoke (pref.Key)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Deletes a given key from the preferences.
		/// </summary>
		public static bool DeleteKey (PrefKey key) {
			return values.Remove (key);
		}

		/// <summary>
		/// Deletes all keys from the preferences, that match a given filter.
		/// </summary>
		public static int DeleteKeys (Func<PrefKey, bool> filter) {
			if (filter == null) {
				return 0;
			}
			var list = GetKeys (filter);

			for (int i = 0; i < list.Count; i++) {
				values.Remove (list[i]);
			}

			return list.Count;
		}

		/// <summary>
		/// Returns a collection of all keys in the preferences, that match a given filter.
		/// </summary>
		public static List<PrefKey> GetKeys (Func<PrefKey, bool> filter) {
			if (filter == null) {
				return null;
			}

			var list = new List<PrefKey> ();
			foreach (var pref in values) {
				if (filter.Invoke (pref.Key)) {
					list.Add (pref.Key);
				}
			}
			return list;
		}

		/// <summary>
		/// Clears out all keys in the preferences.
		/// </summary>
		public static void DeleteAll () {
			values.Clear ();
		}


		private static void RegisterCallbacks () {
			AssemblyReloadEvents.beforeAssemblyReload += OnAssemblyReload;
			EditorApplication.quitting += OnEditorQuit;
		}

		private static void OnAssemblyReload () {
			Save ();
			loaded = false;
		}

		private static void OnEditorQuit () {
			Save ();
		}

		private static void Load () {
			if (loaded) {
				return;
			}
			loaded = true;

			var path = GetPrefFilePath ();
			if (!File.Exists (path)) {
				return;
			}

			int count;
			using (var stream = File.OpenRead (path)) {
				using (var reader = new BinaryReader (stream)) {
					count = reader.ReadInt32 ();
					for (int i = 0; i < count; i++) {
						if (PrefKey.TryDeserialize (reader, out PrefKey? key) && PrefValue.TryDeserialize (reader, out PrefValue value) && key.HasValue) {
							values[key.Value] = value;
						}
					}
				}
			}
		}

		private static string GetPrefFilePath () {
			return Path.Combine (Application.dataPath, "../ProjectPrefs.dat");
		}
	}
}