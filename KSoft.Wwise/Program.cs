using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSoft.Wwise
{
    public static class Program
    {
        public static void Initialize()
            => KSoft.Program.RegisterTraceSources(DebugTraceClass);

        public static void Dispose()
        {
        }

        public static Type DebugTraceClass => typeof(Debug.Trace);
    };
}