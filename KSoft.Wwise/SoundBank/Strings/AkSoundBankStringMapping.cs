#if CONTRACTS_FULL_SHIM
using Contract = System.Diagnostics.ContractsShim.Contract;
#else
using Contract = System.Diagnostics.Contracts.Contract; // SHIM'D
#endif

namespace KSoft.Wwise.SoundBank
{
	sealed class AkSoundBankStringMapping
		: AkSoundBankStringMappingBase
	{
		static readonly Memory.Strings.StringStorage kStringStorage = new(
			Memory.Strings.StringStorageWidthType.Ascii, Memory.Strings.StringStorageLengthPrefix.Int8);
		static readonly Text.StringStorageEncoding kStringEncoding = new(kStringStorage);

		void SerializeStringType(IO.EndianStream s, AKBKHashHeader hdr, AkSoundBank bank)
		{
			Contract.Assert(hdr.Type == AkSoundBankStringMappingBase.StringType.Bank);

			uint bank_id = uint.MaxValue;
			string str = null;

			s.Stream(ref bank_id);
			s.Stream(ref str, kStringEncoding);

			bank.MapIdToName(bank_id, str);
		}
		public override void Serialize(IO.EndianStream s, AkSubchunkHeader header)
		{
			Contract.Assert(s.IsReading);

			var bank = KSoft.Debug.TypeCheck.CastReference<AkSoundBank>(s.Owner);

			long eos = EndOfStream(s, header);

			while (s.BaseStream.Position != eos)
			{
				AKBKHashHeader hdr = new();
				hdr.Serialize(s);

				for (int x = 0; x < hdr.Size; x++)
				{
					SerializeStringType(s, hdr, bank);
				}
			}
		}
	};
}
