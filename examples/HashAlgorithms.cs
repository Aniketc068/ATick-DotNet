// ATick for .NET example — SHA-256 / SHA-384 / SHA-512.
using System;
using System.IO;
using System.Globalization;
using Aniketc068.ATick;

namespace ATickExamples
{
    public static class HashAlgorithms
    {
        public static void Run()
        {
            byte[] pfx = File.ReadAllBytes(Path.Combine("samples", "ABC12.pfx"));
            byte[] pdf1 = File.ReadAllBytes(Path.Combine("samples", "blank.pdf"));
            string now = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            string baseOpts = "\"password\":\"ABC12\",\"cn\":\"DS TEST CERTIFICATE 06\",\"reason\":\"Approved\",\"date\":\"" + now
                + "\",\"page\":1,\"rect\":[300,55,575,175],\"pades\":true";
            foreach (string h in new[] { "sha256", "sha384", "sha512" })
                Save("13_hash_" + h + ".pdf", Atick.SignPfx(pdf1, pfx, "{" + baseOpts + ",\"hash_algo\":\"" + h + "\"}"));
        }
        static void Save(string name, byte[] data)
        {
            Directory.CreateDirectory("signed");
            File.WriteAllBytes(Path.Combine("signed", name), data);
            Console.WriteLine("  " + name + " (" + data.Length + " bytes)");
        }
    }
}
