
namespace KSoft.Wwise.SoundBank
{
	partial class AkSoundBankObjectBase
	{
		static readonly Values.GroupTagData32 kMediaIndexSignature = new(
			"DIDX", "audiokinetic_sound_bank_data_index"); // BankDataIndexChunkID

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
		static AkSoundBankObjectBase NewDIDX(uint generatorVersion)
		{
			return generatorVersion switch
			{
				_ => new AkSoundBankDataIndex(),
			};
		}
	};

	sealed class AkSoundBankDataIndex
		: AkSoundBankObjectBase
	{
		public AkMediaHeader[] LoadedMedia;

		public override void Serialize(IO.EndianStream s, AkSubchunkHeader header)
		{
			if (s.IsReading)
			{
				LoadedMedia = new AkMediaHeader[header.ChunkSize / AkMediaHeader.kSizeOf];
			}

			s.StreamArray(LoadedMedia);
		}
	};
}
