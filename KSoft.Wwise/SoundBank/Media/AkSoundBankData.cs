
namespace KSoft.Wwise.SoundBank
{
	partial class AkSoundBankObjectBase
	{
		static readonly Values.GroupTagData32 kDataSignature = new(
			"DATA", "audiokinetic_sound_bank_data"); // BankDataChunkID

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
		static AkSoundBankObjectBase NewDATA(uint generatorVersion)
		{
			return generatorVersion switch
			{
				_ => new AkSoundBankData(),
			};
		}
	};

	sealed class AkSoundBankData
		: AkSoundBankObjectBase
	{
		public byte[] Buffer;

		public override void Serialize(IO.EndianStream s, AkSubchunkHeader header)
		{
			if (s.IsReading)
			{
				Buffer = new byte[header.ChunkSize];
			}

			s.Stream(Buffer);
		}
	};
}
