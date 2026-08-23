
namespace KSoft.Wwise.SoundBank
{
	public struct AkPlaylistItem
		: IO.IEndianStreamSerializable
		, System.IEquatable<AkPlaylistItem>
	{
		/// <summary>Size of this struct on disk</summary>
		internal const uint kSizeOf = 5;

		public uint ID;
		public sbyte Weight;

		#region Overrides
		public override readonly bool Equals(object? obj) => obj is AkPlaylistItem other && Equals(other);
		public readonly bool Equals(AkPlaylistItem other) => ID == other.ID && Weight == other.Weight;
		public static bool operator ==(AkPlaylistItem left, AkPlaylistItem right) => left.Equals(right);
		public static bool operator !=(AkPlaylistItem left, AkPlaylistItem right) => !left.Equals(right);
		public override readonly int GetHashCode() => System.HashCode.Combine(ID, Weight);
		#endregion

		#region IEndianStreamSerializable Members
		public void Serialize(IO.EndianStream s)
		{
			s.Stream(ref ID);
			s.Stream(ref Weight);
		}
		#endregion
	};
}