// ATick for .NET — example runner.
//   cd examples
//   dotnet run -- SignPfx          (or PadesLevels, Appearance, DeferredEsign, …)
// Each example is a class ATickExamples.<Name> with a public static void Run().
using System;
using System.Reflection;

namespace ATickExamples
{
    static class Program
    {
        static int Main(string[] args)
        {
            string name = args.Length > 0 ? args[0] : "SignPfx";
            var type = Type.GetType("ATickExamples." + name);
            var run = type?.GetMethod("Run", BindingFlags.Public | BindingFlags.Static);
            if (run == null)
            {
                Console.WriteLine("Unknown example: " + name);
                Console.WriteLine("Try: SignPfx, PadesLevels, Appearance, MarkColor, HashAlgorithms, Invisible,");
                Console.WriteLine("     TickVariations, DeferredEsign, CertifyLock, MultiPlacement, Encrypted,");
                Console.WriteLine("     MultiRevision, DateFormats, MakeContainer, FastSigning, DocumentTimestamp,");
                Console.WriteLine("     Metadata, FieldApi, Pem, Verify, SignAlreadySigned");
                return 1;
            }
            run.Invoke(null, null);
            return 0;
        }
    }
}
