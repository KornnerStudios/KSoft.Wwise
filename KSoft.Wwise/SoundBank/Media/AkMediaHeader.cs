
namespace KSoft.Wwise.SoundBank
{
	public struct AkMediaHeader
		: IO.IEndianStreamSerializable
		, System.IEquatable<AkMediaHeader>
	{
		/// <summary>Size of this struct on disk</summary>
		public const int kSizeOf = sizeof(uint) * 3;

		public uint ID, Offset, Size;

		#region Overrides
		public override readonly bool Equals(object? obj) => obj is AkMediaHeader other && Equals(other);
		public readonly bool Equals(AkMediaHeader other) => ID == other.ID && Offset == other.Offset && Size == other.Size;
		public static bool operator ==(AkMediaHeader left, AkMediaHeader right) => left.Equals(right);
		public static bool operator !=(AkMediaHeader left, AkMediaHeader right) => !left.Equals(right);
		public override readonly int GetHashCode() => System.HashCode.Combine(ID, Offset, Size);
		#endregion

		#region IEndianStreamSerializable Members
		public void Serialize(IO.EndianStream s)
		{
			s.Stream(ref ID);
			s.Stream(ref Offset);
			s.Stream(ref Size);
		}
		#endregion
	};
}