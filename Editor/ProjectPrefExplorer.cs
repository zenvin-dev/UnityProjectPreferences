using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zenvin.ProjectPreferences {
	public class ProjectPrefExplorer : EditorWindow {

		private const float ColumnSpacing = 10f;

		private Vector2 scroll;
		private List<KeyValuePair<PrefKey, PrefValue>> prefCache;
		private List<PrefKey> removeKeys = new List<PrefKey> ();
		private string search;


		[MenuItem ("Window/Zenvin/Project Preference Explorer")]
		private static void Init () {
			var win = GetWindow<ProjectPrefExplorer> ();
			win.titleContent = new GUIContent ("Project Prefs");
			win.Show ();
		}

		private void OnGUI () {
			UpdatePrefCache (false);

			float width = (position.width - 18f) * 0.175f;

			DrawListHeader (width);
			scroll = GUILayout.BeginScrollView (scroll, false, true);
			DrawPrefList (width);
			GUILayout.EndScrollView ();
			GUILayout.BeginHorizontal ();
			EditorGUI.BeginDisabledGroup (removeKeys.Count == 0);
			if (GUILayout.Button ("Apply")) {
				ProjectPrefs.DeleteKeys (removeKeys.Contains);
				ProjectPrefs.Save ();
				removeKeys.Clear ();
			}
			EditorGUI.EndDisabledGroup ();
			if (GUILayout.Button ("Reload")) {
				removeKeys.Clear ();
				ProjectPrefs.Reload ();
				UpdatePrefCache (true);
			}
			GUILayout.EndHorizontal ();
		}

		private void DrawListHeader (float width) {
			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Namespace", EditorStyles.boldLabel, GUILayout.Width (width));
			GUILayout.Space (ColumnSpacing);
			EditorGUILayout.LabelField ("Block", EditorStyles.boldLabel, GUILayout.Width (width));
			GUILayout.Space (ColumnSpacing);
			EditorGUILayout.LabelField ("Key", EditorStyles.boldLabel, GUILayout.Width (width));
			GUILayout.Space (ColumnSpacing);
			EditorGUILayout.LabelField ("Value (read-only)", EditorStyles.boldLabel);

			search = EditorGUILayout.TextField (search);
			GUILayout.EndHorizontal ();
		}

		private void DrawPrefList (float columnWidth) {
			for (int i = 0; i < prefCache.Count; i++) {
				DrawPreference (prefCache[i], columnWidth);
			}

			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
		}

		private void DrawPreference (KeyValuePair<PrefKey, PrefValue> current, float width) {
			var key = current.Key;
			var rem = removeKeys.Contains (key);
			var val = current.Value;
			var txt = val.ToString ();

			var hasQuery = !string.IsNullOrEmpty (search);
			var query = search.ToUpper ();
			var keyNamespaceMatch = !hasQuery || (key.Namespace?.ToUpper()?.Contains (query) ?? false);
			var keyBlockMatch = !hasQuery || (key.Block?.ToUpper()?.Contains (query) ?? false);
			var keyKeyMatch = !hasQuery || (key.Key?.ToUpper()?.Contains (query) ?? false);
			var valMatch = !hasQuery || txt.ToUpper().Contains (query);

			if (!keyNamespaceMatch && !keyBlockMatch && !keyKeyMatch && !valMatch) {
				return;
			}

			GUI.backgroundColor = rem ? Color.red : Color.white;
			GUI.color = rem ? Color.red : Color.white;

			GUILayout.BeginHorizontal ();
			DrawPreferenceCell (new GUIContent (key.Namespace, current.Key.Namespace), width, keyNamespaceMatch && hasQuery, rem, EditorStyles.label);
			GUILayout.Space (ColumnSpacing);
			DrawPreferenceCell (new GUIContent (key.Block, current.Key.Block), width, keyBlockMatch && hasQuery, rem, EditorStyles.label);
			GUILayout.Space (ColumnSpacing);
			DrawPreferenceCell (new GUIContent (key.Key, current.Key.Key), width, keyKeyMatch && hasQuery, rem, EditorStyles.label);
			GUILayout.Space (ColumnSpacing);
			DrawPreferenceCell (new GUIContent(val.Type.ToString ()), 75f, false, rem, EditorStyles.whiteLabel);
			DrawPreferenceCell (new GUIContent (txt), null, valMatch && hasQuery, rem, EditorStyles.textArea);


			if (GUILayout.Button (EditorGUIUtility.IconContent ("d_Toolbar Minus"), EditorStyles.label, GUILayout.Width (30f)) && !rem) {
				removeKeys.Add (current.Key);
			}
			GUILayout.EndHorizontal ();
		}

		private void DrawPreferenceCell (GUIContent content, float? columnWidth, bool highlight, bool delete, GUIStyle style) {
			GUI.color = highlight ? Color.green : (delete ? Color.red : Color.white);
			GUI.backgroundColor = delete ? Color.red : Color.white;
			if (columnWidth.HasValue) {
				EditorGUILayout.LabelField (content, style, GUILayout.Width (columnWidth.Value));
			} else {
				EditorGUILayout.LabelField (content, style);
			}
		}

		private void UpdatePrefCache (bool force = false) {
			if (!force && prefCache != null && prefCache.Count == ProjectPrefs.Count) {
				return;
			}

			if (prefCache == null) {
				prefCache = new List<KeyValuePair<PrefKey, PrefValue>> (ProjectPrefs.Count);
			} else {
				prefCache.Clear ();
			}

			var iEnum = ProjectPrefs.GetPreferenceEnumerator ();
			while (iEnum.MoveNext ()) {
				prefCache.Add (iEnum.Current);
			}
		}
	}
}