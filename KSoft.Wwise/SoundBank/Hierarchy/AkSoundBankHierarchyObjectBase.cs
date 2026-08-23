
namespace KSoft.Wwise.SoundBank
{
	abstract partial class AkSoundBankHierarchyObjectBase
		: IO.IEndianStreamSerializable
	{
		public uint ID;

		#region Factory
		sealed class AkSoundBankHierarchyDefaultImpl
			: AkSoundBankHierarchyObjectBase
		{
			readonly HircType mType;

			public AkSoundBankHierarchyDefaultImpl(HircType type)
			{
				mType = type;
			}

			public override string ToString()
			{
				return mType.ToString();
			}
		};

		public static AkSoundBankHierarchyObjectBase New(HircType type)
		{
			return type switch
			{
				HircType.Sound =>		new AkSoundBankHierarchySound(),
				HircType.Action =>		new AkSoundBankHierarchyAction(),
				HircType.Event =>		new AkSoundBankHierarchyEvent(),
				HircType.RanSeqCntr =>	new AkSoundBankHierarchyRanSeqCntr(),
				_ =>					new AkSoundBankHierarchyDefaultImpl(type),
			};
		}
		#endregion

		#region IEndianStreamSerializable Members
		public virtual void Serialize(IO.EndianStream s)
		{
			s.Stream(ref ID);
		}
		#endregion
	};
}
