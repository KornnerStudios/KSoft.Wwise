
namespace KSoft.Wwise.SoundBank
{
	abstract partial class AkSoundBankObjectBase
	{
		protected static long EndOfStream(IO.EndianStream s, AkSubchunkHeader header)
		{
			return s.BaseStream.Position + header.ChunkSize;
		}

		public abstract void Serialize(IO.EndianStream s, AkSubchunkHeader header);

		#region Factory
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Retains sound-bank protocol signature reference data.")]
		static readonly Values.GroupTagData32 kGlobalSettingsSignature = new(
			"STMG", "audiokinetic_global_settings"); // BankStateMgrChunkID

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Retains sound-bank protocol signature reference data.")]
		static readonly Values.GroupTagData32 kFxParamsSignature = new(
			"FXPR", "audiokinetic_fx_params"); // BankFXParamsChunkID
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Retains sound-bank protocol signature reference data.")]
		static readonly Values.GroupTagData32 kEnvSettingsSignature = new(
			"ENVS", "audiokinetic_env_settings"); // BankEnvSettingChunkID

		public static AkSoundBankObjectBase? New(uint chunkId, uint generatorVersion)
		{
				 if (chunkId == kHierarchySignature.ID)		{ return NewHIRC(generatorVersion); }
			else if (chunkId == kStringMappingSignature.ID)	{ return NewSTID(generatorVersion); }
			else if (chunkId == kDataSignature.ID)			{ return NewDATA(generatorVersion); }
			else if (chunkId == kMediaIndexSignature.ID)	{ return NewDIDX(generatorVersion); }

			return null;
		}
		#endregion
	};
}
