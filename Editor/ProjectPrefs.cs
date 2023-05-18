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


		static ProjectPrefs () {
			RegisterCallbacks ();
		}


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


		public static bool SetValue (PrefKey key, PrefValue value) {
			return SetValue (key, value, ValueOverrideOption.AlwaysOverride);
		}

		public static bool SetValue (PrefKey key, PrefValue value, ValueOverrideOption option) {
			if (value == null || !key.Valid) {
				return false;
			}
			if (option == ValueOverrideOption.AlwaysOverride || !values.TryGetValue (key, out PrefValue existing)) {
				values[key] = value;
				return true;
			}
			if (option == ValueOverrideOption.OverrideMatchingType && existing.Type != value.Type) {
				return false;
			}
			values[key] = value;
			return true;
		}


		public static bool GetBool (string key, bool fallback) {
			return TryGetBool (key, out bool value) ? value : fallback;
		}

		public static int GetInt (string key, int fallback) {
			return TryGetInt (key, out int value) ? value : fallback;
		}

		public static string GetString (string key, string fallback) {
			return TryGetString (key, out string value) ? value : fallback;
		}

		public static float GetFloat (string key, float fallback) {
			return TryGetFloat (key, out float value) ? value : fallback;
		}

		public static bool TryGetBool (string key, out bool value) {
			Load ();
			if (values.TryGetValue (key, out PrefValue val)) {
				value = val.BoolValue;
				return val.Type == KeyType.Bool;
			}
			value = default;
			return false;
		}

		public static bool TryGetInt (string key, out int value) {
			Load ();
			if (values.TryGetValue (key, out PrefValue val)) {
				value = val.IntValue;
				return val.Type == KeyType.Bool;
			}
			value = default;
			return false;
		}

		public static bool TryGetString (string key, out string value) {
			Load ();
			if (values.TryGetValue (key, out PrefValue val)) {
				value = val.StringValue;
				return val.Type == KeyType.Bool;
			}
			value = default;
			return false;
		}

		public static bool TryGetFloat (string key, out float value) {
			Load ();
			if (values.TryGetValue (key, out PrefValue val)) {
				value = val.FloatValue;
				return val.Type == KeyType.Bool;
			}
			value = default;
			return false;
		}


		public static bool HasKey (PrefKey key) {
			return values.ContainsKey (key);
		}

		public static bool HasKey (Func<PrefKey, bool> filter) {
			foreach (var pref in values) {
				if (filter.Invoke (pref.Key)) {
					return true;
				}
			}
			return false;
		}

		public static bool DeleteKey (PrefKey key) {
			return values.Remove (key);
		}

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

		public static void DeleteAll () {
			values.Clear ();
			Save ();
		}


		private static void RegisterCallbacks () {
			AssemblyReloadEvents.beforeAssemblyReload += OnAssemblyReload;
			EditorApplication.quitting += OnEditorQuit;
		}

		private static void OnAssemblyReload () {
			Save ();
			loaded = false;
			Debug.Log ("Saving Project Prefs...");
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