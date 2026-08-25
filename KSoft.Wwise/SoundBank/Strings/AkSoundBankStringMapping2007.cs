namespace KSoft.Wwise.SoundBank
{
	sealed class AkSoundBankStringMapping2007
		: AkSoundBankStringMappingBase
	{
		public struct StringHashEntry
			: IO.IEndianStreamSerializable
		{
			public uint Offset; // if this is -1, we have to skip it. TODO: serialize entries into memory stream?
			public uint Key, ID;

			public string Value;

			#region IEndianStreamSerializable Members
			public void Serialize(IO.EndianStream s)
			{
				s.Stream(ref Offset);
			}
			#endregion
		};
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Retained as intentional 2007 sound-bank format serialization scaffolding.")]
		public class StringGroup
			: IO.IEndianStreamSerializable
		{
			AKBKHashHeader mHeader;
			public StringHashEntry[] Entries = null!;

			public uint ID => mHeader.Hash;

			#region IEndianStreamSerializable Members
			void SerializeGroupEntries(IO.EndianStream s)
			{
				s.StreamArrayInt32(ref Entries);
				for (int x = 0; x < Entries.Length; x++)
				{
					s.Stream(ref Entries[x].ID);
					s.Stream(ref Entries[x].Value, Memory.Strings.StringStorage.CStringAscii);
					s.Stream(ref Entries[x].Key);
				}
			}
			public void Serialize(IO.EndianStream s)
			{
				s.Stream(ref mHeader);

				long eos = EndOfStream(s, mHeader);
				SerializeGroupEntries(s);
				if (s.BaseStream.Position != eos)
				{
					throw new System.IO.InvalidDataException(string.Create(System.Globalization.CultureInfo.InvariantCulture,
						$"String group ended at position {s.BaseStream.Position}, expected {eos}."));
				}
			}
			#endregion
		};

		public StringHashEntry[] Events = null!;

		static void SerializeEntries(IO.EndianStream s, ref StringHashEntry[] entries)
		{
			s.StreamArrayInt32(ref entries);
			for (int x = 0; x < entries.Length; x++)
			{
				s.Stream(ref entries[x].Value, Memory.Strings.StringStorage.CStringAscii);
				s.Stream(ref entries[x].Key);
			}
		}
		static void SerializeGroups(IO.EndianStream s, ref StringHashEntry[] entries)
		{
			s.StreamArrayInt32(ref entries);
			for (int x = 0; x < entries.Length; x++)
			{
				s.Stream(ref entries[x].ID);
				s.Stream(ref entries[x].Value, Memory.Strings.StringStorage.CStringAscii);
				s.Stream(ref entries[x].Key);
			}
		}
		void SerializeStringType(IO.EndianStream s, AKBKHashHeader hdr, AkSoundBank bank)
		{
			Util.MarkUnusedVariable(ref bank);

			switch (hdr.Type)
			{
				case AkSoundBankStringMappingBase.StringType.OldEvents:
					SerializeEntries(s, ref Events);
					break;
			}
		}

		public override void Serialize(IO.EndianStream s, AkSubchunkHeader header)
		{
			if (!s.IsReading)
			{
				throw new System.InvalidOperationException(string.Create(System.Globalization.CultureInfo.InvariantCulture,
					$"String mapping 2007 serialization requires a readable stream; stream mode is {s.StreamMode}."));
			}

			var bank = KSoft.Debug.TypeCheck.CastReference<AkSoundBank>(s.Owner!);

			long eos = EndOfStream(s, header);

			while (s.BaseStream.Position != eos)
			{
				AKBKHashHeader hdr = new();
				hdr.Serialize(s);

				SerializeStringType(s, hdr, bank);
			}
		}
	};
}
