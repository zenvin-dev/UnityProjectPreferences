using System.IO;

namespace Zenvin.ProjectPreferences {
	public class PrefValue {
		internal KeyType Type { get; private set; }
		internal bool BoolValue { get; private set; }
		internal int IntValue { get; private set; }
		internal string StringValue { get; private set; }
		internal float FloatValue { get; private set; }


		private PrefValue () { }

		public PrefValue (bool value) {
			Type = KeyType.Bool;
			BoolValue = value;
		}

		public PrefValue (int value) {
			Type = KeyType.Int;
			IntValue = value;
		}

		public PrefValue (string value) {
			Type = KeyType.String;
			StringValue = value;
		}

		public PrefValue (float value) {
			Type = KeyType.Float;
			FloatValue = value;
		}


		internal static void Serialize (BinaryWriter writer, PrefValue value) {
			writer.Write (value != null);
			if (value == null) {
				return;
			}

			writer.Write ((int)value.Type);
			switch (value.Type) {
				case KeyType.None:
					break;
				case KeyType.Bool:
					writer.Write (value.BoolValue);
					break;
				case KeyType.Int:
					writer.Write (value.IntValue);
					break;
				case KeyType.String:
					writer.Write (value.StringValue != null);
					if (value.StringValue != null) {
						writer.Write (value.StringValue);
					}
					break;
				case KeyType.Float:
					writer.Write (value.FloatValue);
					break;
			}
		}

		internal static bool TryDeserialize (BinaryReader reader, out PrefValue value) {
			value = null;
			if (!reader.ReadBoolean ()) {
				return false;
			}

			value = new PrefValue {
				Type = (KeyType)reader.ReadInt32 ()
			};
			switch (value.Type) {
				case KeyType.Bool:
					value.BoolValue = reader.ReadBoolean ();
					break;
				case KeyType.Int:
					value.IntValue = reader.ReadInt32 ();
					break;
				case KeyType.String:
					value.StringValue = reader.ReadBoolean () ? reader.ReadString () : null;
					break;
				case KeyType.Float:
					value.FloatValue = reader.ReadSingle ();
					break;
			}

			return true;
		}


		public static implicit operator PrefValue (bool value) {
			return new PrefValue (value);
		}

		public static implicit operator PrefValue (int value) {
			return new PrefValue (value);
		}

		public static implicit operator PrefValue (string value) {
			return new PrefValue (value);
		}

		public static implicit operator PrefValue (float value) {
			return new PrefValue (value);
		}
	}
}