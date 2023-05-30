# UnityProjectPreferences
Similar to Unity's built-in EditorPrefs, but relative to a single project. And more versatile.

Project preferences are stored as binary data in the project directory. Changes will be saved automatically on domain reload and on quitting the editor.
The `ProjectPrefs` class provides ways to interface with the stored data:

| Method Signature | Description |
|-|-|
| `public static void Save ()` | Write all current keys and their values to the preferences file. |
| `public static bool SetValue (PrefKey key, PrefValue value)` | Attempts to set the value associated with the given key. <br> Will always replace existing values. |
| `public static bool SetValue (PrefKey key, PrefValue value, ValueOverrideOption overrideOption)` | Attempts to set the value associated with the given key. <br> Whether existing values are replaced, depends on the given <see cref="ValueOverrideOption"/>. <br> Returns `true` if the value was set, otherwise `false`. |
| `public static object GetValue (PrefKey key)` | Gets the value associated with the given key as an <see cref="object"/>. <br> The value associated with the given key as an `object`. Otherwise `null`. |
| `public static bool TryGetValue (PrefKey key, out object value)` | Attempts to get the value associated with the given key. <br> Returns `true` if the key exists in the preferences, otherwise `false`. |
| `public static bool GetBool (PrefKey key, bool fallback)` |  |
| `public static int GetInt (PrefKey key, int fallback)` |  |
| `public static string GetString (PrefKey key, string fallback)` |  |
| `public static float GetFloat (PrefKey key, float fallback)` |  |
| `public static bool TryGetBool (PrefKey key, out bool value)` |  |
| `public static bool TryGetInt (PrefKey key, out int value)` |  |
| `public static bool TryGetString (PrefKey key, out string value)` |  |
| `public static bool TryGetFloat (PrefKey key, out float value)` |  |
| `public static bool HasKey (PrefKey key)` | Checks whether a given key exists in the preferences. |
| `public static bool HasKey (Func<PrefKey, bool> filter)` | Checks whether any key exists in the preferences, that matches a given filter. |
| `public static bool DeleteKey (PrefKey key)` | Deletes a given key from the preferences. |
| `public static int DeleteKeys (Func<PrefKey, bool> filter)` | Deletes all keys from the preferences, that match a given filter. |
| `public static List<PrefKey> GetKeys (Func<PrefKey, bool> filter)` | Returns a collection of all keys in the preferences, that match a given filter. |
| `public static void DeleteAll ()` | Clears out all keys in the preferences. |
