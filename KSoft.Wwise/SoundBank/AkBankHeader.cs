namespace KSoft.Wwise.SoundBank
{
	partial class AkSoundBank
	{
		struct AkBankHeader : IO.IEndianStreamSerializable
		{
			/// <summary>Size of this struct on disk</summary>
			public const int kSizeOf = sizeof(uint) * 4;

			public uint BankGeneratorVersion, SoundBankID, LanguageID, FeedbackSupported;

			#region IEndianStreamSerializable Members
			void SerializeOld(IO.EndianStream s)
			{
				if (!s.IsReading)
				{
					throw new System.InvalidOperationException(string.Format(
						"Old bank header serialization requires a readable stream; stream mode is {0}.",
						s.StreamMode));
				}

				s.Pad32(); // Type; 0 or 1 (Init.bk)
				s.Pad32(); // LanguageID?
				s.Stream(ref BankGeneratorVersion);
				s.Pad32(); // seen as '0', '12'
				s.Pad32(); // some kind of ID
				s.Stream(ref SoundBankID);
			}
			public void Serialize(IO.EndianStream s)
			{
				uint sdk_ver = (KSoft.Debug.TypeCheck.CastReference<AkSoundBank>(s.Owner)).SdkVersion;

				if (AkVersion.HasOldBankHeader(sdk_ver))
				{
					SerializeOld(s);
				}
				else
				{
					s.Stream(ref BankGeneratorVersion);
					s.Stream(ref SoundBankID);
					s.Stream(ref LanguageID);
					s.Stream(ref FeedbackSupported);
				}
			}
			#endregion
		};
	};
}
