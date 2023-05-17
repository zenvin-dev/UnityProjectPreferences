# UnityProjectPreferences
Like Unity's built-in EditorPrefs, but relative to a single project.

Project preferences are stored as binary data in the project directory. Changes will be saved automatically on domain reload and on quitting the editor.
The `EditorPrefs` class provides ways to interface with the stored data:
| Method Signature | Description |
|:-|:-|
| GetBool(string, bool) |	Returns the `bool` value corresponding to the key, if it exists. |
| GetFloat(string, float) | Returns the `float` value corresponding to the key, if it exists. |
| GetInt(string, int) | Returns the `int` value corresponding to the key, if it exists. |
| GetString(string, string) | Returns the `string` value corresponding to the key, if it exists. |
| HasKey(string) | Returns all value types for which the key exists. |
| SetBool(string, bool) | Sets the value of the `bool` preference identified by key. |
| SetFloat(string, float) | Sets the value of the `float` preference identified by key. |
| SetInt(string, float) | Sets the value of the `int` preference identified by key. |
| SetString(string, string) | Sets the value of the `string` preference identified by key. |
| Save() | Forces writing the preferences to the preference file. |
