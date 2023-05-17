using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zenvin.ProjectPreferences {
	public static class ProjectPrefs {

		private static bool loaded = false;
		private static readonly Dictionary<string, bool> boolValues = new Dictionary<string, bool> ();
		private static readonly Dictionary<string, int> intValues = new Dictionary<string, int> ();
		private static readonly Dictionary<string, string> stringValues = new Dictionary<string, string> ();
		private static readonly Dictionary<string, float> floatValues = new Dictionary<string, float> ();


		public static void Save () {
			if (boolValues.Count == 0 && intValues.Count == 0 && stringValues.Count == 0 && floatValues.Count == 0) {
				return;
			}

			var path = GetPrefFilePath ();

			using (var stream = File.Create (path)) {
				using (var writer = new BinaryWriter (stream)) {
					writer.Write (boolValues.Count);
					foreach (var val in boolValues) {
						writer.Write (val.Key);
						writer.Write (val.Value);
					}

					writer.Write (intValues.Count);
					foreach (var val in intValues) {
						writer.Write (val.Key);
						writer.Write (val.Value);
					}

					writer.Write (stringValues.Count);
					foreach (var val in stringValues) {
						writer.Write (val.Key);
						writer.Write (val.Value);
					}

					writer.Write (floatValues.Count);
					foreach (var val in floatValues) {
						writer.Write (val.Key);
						writer.Write (val.Value);
					}
				}
			}
		}


		public static bool GetBool (string key, bool fallback) {
			return TryGetBool (key, out bool value) ? value : fallback;
		}

		public static bool TryGetBool (string key, out bool value) {
			Load ();
			return boolValues.TryGetValue (key, out value);
		}

		public static void SetBool (string key, bool value) {
			Load ();
			boolValues[key] = value;
		}


		public static int GetInt (string key, int fallback) {
			return TryGetInt (key, out int value) ? value : fallback;
		}

		public static bool TryGetInt (string key, out int value) {
			Load ();
			return intValues.TryGetValue (key, out value);
		}

		public static void SetInt (string key, int value) {
			Load ();
			intValues[key] = value;
		}

		public static string GetString (string key, string fallback) {
			return TryGetString (key, out string value) ? value : fallback;
		}

		public static bool TryGetString (string key, out string value) {
			Load ();
			return stringValues.TryGetValue (key, out value);
		}

		public static void SetString (string key, string value) {
			Load ();
			stringValues[key] = value;
		}

		public static float GetFloat (string key, float fallback) {
			return TryGetFloat (key, out float value) ? value : fallback;
		}

		public static bool TryGetFloat (string key, out float value) {
			Load ();
			return floatValues.TryGetValue (key, out value);
		}

		public static void SetFloat (string key, float value) {
			Load ();
			floatValues[key] = value;
		}


		[InitializeOnLoadMethod]
		private static void RegisterReloadCallback () {
			AssemblyReloadEvents.beforeAssemblyReload += OnAssemblyReload;
		}

		private static void OnAssemblyReload () {
			Save ();
			loaded = false;
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
						boolValues[reader.ReadString ()] = reader.ReadBoolean ();
					}

					count = reader.ReadInt32 ();
					for (int i = 0; i < count; i++) {
						intValues[reader.ReadString ()] = reader.ReadInt32 ();
					}

					count = reader.ReadInt32 ();
					for (int i = 0; i < count; i++) {
						stringValues[reader.ReadString ()] = reader.ReadString ();
					}

					count = reader.ReadInt32 ();
					for (int i = 0; i < count; i++) {
						floatValues[reader.ReadString ()] = reader.ReadSingle ();
					}
				}
			}
		}

		private static string GetPrefFilePath () {
			return Path.Combine (Application.dataPath, "../ProjectPrefs.dat");
		}

	}
}