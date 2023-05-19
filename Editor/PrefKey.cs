using System.IO;

namespace Zenvin.ProjectPreferences {
	public struct PrefKey {
		public string Namespace;
		public string Block;
		public string Key;

		public bool Valid => !string.IsNullOrWhiteSpace (Key);

		public PrefKey (string @namespace, string block, string key) : this (key) {
			Namespace = @namespace;
			Block = block;
		}

		public PrefKey (string @namespace, string key) : this (@namespace, "", key) {

		}

		public PrefKey (string key) : this () {
			Key = key;
		}


		internal void Serialize (BinaryWriter writer) {
			writer.Write (Valid);
			if (!Valid) {
				return;
			}
			writer.Write (Namespace ?? "");
			writer.Write (Block ?? "");
			writer.Write (Key ?? "");
		}

		internal static bool TryDeserialize (BinaryReader reader, out PrefKey? key) {
			key = null;
			if (!reader.ReadBoolean ()) {
				return false;
			}
			key = new PrefKey (reader.ReadString (), reader.ReadString (), reader.ReadString ());
			return true;
		}


		public override string ToString () {
			return $"{Namespace}#{Block}#{Key}";
		}

		public static implicit operator PrefKey (string key) {
			return new PrefKey (key);
		}
	}
}