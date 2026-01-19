using Diag = System.Diagnostics;

namespace KSoft.Wwise.Debug
{
	/// <summary>Utility class for tracing assembly logic (or lack thereof)</summary>
	internal static class Trace
	{
		/// <summary>Tracer for the <see cref="KSoft.Wwise"/> namespace</summary>
		public static Diag.TraceSource Wwise { get; }		= new ("KSoft.Wwise",				Diag.SourceLevels.All);
		/// <summary>Tracer for the <see cref="KSoft.Wwise.FilePackage"/> namespace</summary>
		public static Diag.TraceSource FilePackage { get; } = new("KSoft.Wwise.FilePackage",	Diag.SourceLevels.All);
	};
}
